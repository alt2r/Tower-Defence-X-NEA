using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    // Start is called before the first frame update

    public Text waveReached, difficulty, activeTowers, enemiesKilled, bigText;
    void Start()
    {
        SetUpStats();
        if (GameMaster.gameWon)
        {
            bigText.text = "YOU WIN!";
            waveReached.text = $"Wave Reached: 30"; //it was displaying 29 on a win bc this would be called before wavenum incrementation. not an issue for losing as you lose mid wave anyway
        }
        else
        {
            bigText.text = "YOU LOSE!";
            return;
        }

        //incrementing the players wins in the file
        if (MainMenu.playerName == "")
        {
            return;
        }
        StreamReader sr = new StreamReader("users.txt");
        string? line;
        string[] splitString;
        List<string[]> list = new List<string[]>();
        while ((line = sr.ReadLine()) != null)
        {
            splitString = line.Split(",");
            list.Add(splitString);
        }
        sr.Close();
        foreach (string[] x in list) //list now has all the data of the file
        {
            if (x[0] == MainMenu.playerName)
            {
                x[1] = Convert.ToString(Convert.ToInt32(x[1]) + 1);
            }
        }
        StreamWriter sw = new StreamWriter("users.txt");
        foreach (string[] x in list)
        {
            sw.WriteLine($"{x[0]},{x[1]}"); //rewrite the file with updated data
        }
        sw.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MenuButtonPressed()
    {
        SceneManager.LoadScene("Main Menu");
        BuildManager.instance.DestroyBM();
        Grid.instance.DestroyGrid();
    }

    private void SetUpStats()
    {
        waveReached.text = $"Wave Reached: {GameMaster.GetWaveNumber()}";
        string d = "";
        switch (MainMenu.GetDifficulty())
        {
            case 0:
                d = "Easy";
                break;
            case 1:
                d = "Medium";
                break;
            case 2:
                d = "Hard";
                break;
            default:
                break;
        }
        difficulty.text = "Difficulty: " + d;
        activeTowers.text = $"Active Towers: {GameMaster.GetActiveTowers()}";
        enemiesKilled.text = $"Enemies Killed: {GameMaster.enemiesKilled}";
    }
}
