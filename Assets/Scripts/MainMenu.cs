using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class MainMenu : MonoBehaviour
{
    private string inputfieldText = "";
    public Text leaderboardText;
    public Slider difficultySlider;
    private static float difficulty;
    public static float GetDifficulty() { return difficulty;  }
    public Toggle endlessToggle;
    private static bool endlessMode;
    public static bool IsEndlessMode() { return endlessMode;  }
    public GameObject fileButton;

    public static string playerName;

    //messagae display
    public GameObject messageGO;
    public Text messageText;
    bool continuerunning = false;
    bool returnfromstartgame = false;

    void Start()
    {
        //populate leaderboard
        try
        {
            StreamReader sr = new StreamReader("users.txt");
            string? line;
            string[] splitString;
            string textToAdd = "";
            List<string[]> playersAndScores = new List<string[]>();
            while ((line = sr.ReadLine()) != null)
            {
                splitString = line.Split(",");
                playersAndScores.Add(splitString);
            }
            List<string[]> sortedScores = mergeSort(playersAndScores);
            int numberOfPlayersOnScoreBoard = 0;
            for (int i = sortedScores.Count - 1; (i >= 0 && numberOfPlayersOnScoreBoard < 8); i--)
            {
                textToAdd += $"{sortedScores[i][0]}: {sortedScores[i][1]} \n";
                numberOfPlayersOnScoreBoard++;
            }
            leaderboardText.text = textToAdd;
        }
        catch (Exception)
        {
            leaderboardText.text = "Users file is missing!";
            fileButton.SetActive(true);
        }
        
    }

    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
    public void CreateUsersFile()
    {
        StreamWriter sw = new StreamWriter("users.txt", false);
        fileButton.SetActive(false);
        sw.Close();
        Start();
    }

    public void StartGame()
    {
        if (inputfieldText.Length > 12)
        {
            if (!continuerunning)
            {
                NewMessage("The name you entered is too long! It will be cut down to 12 characters.", 1);
                return;
            }

            inputfieldText = inputfieldText.Substring(0, 12);
        }
        searchFile();
        if (returnfromstartgame)
        {
            return;
        }
        SceneManager.LoadScene("Game");
        difficulty = difficultySlider.value; //0 = easy, 1 = medium, 2 = hard
        endlessMode = endlessToggle.isOn;
    }

    private void searchFile()
    {
        playerName = inputfieldText;
        if (inputfieldText == "")
        {
            return;
        }
        StreamReader sr = new StreamReader("users.txt");
        string? line;
        string[] splitString;
        while ((line = sr.ReadLine()) != null)
        {
            splitString = line.Split(",");
            if (splitString[0] == inputfieldText) //if name exists in file
            {
                returnfromstartgame = false;
                if (!continuerunning)
                {
                    NewMessage($"There is already a user called {inputfieldText}. Would you like to play as them?", 0);
                    returnfromstartgame = true;
                }
                sr.Close();
                return;
            }
        } //if name doesnt exist in file
        sr.Close();
        StreamWriter sw = new StreamWriter("users.txt", true); //setting append mode to true as to not overwrite the file
        sw.WriteLine($"{inputfieldText},0");
        Debug.Log("new user created");
        sw.Close();
    }

    public void readInputField(string s)
    {
        inputfieldText = s;
    }

    public static List<string[]> mergeSort(List<string[]> array)
    {
        List<string[]> left = new List<string[]>();
        List<string[]> right = new List<string[]>();
        List<string[]> result = new List<string[]>();
        if (array.Count <= 1)
        {
            return array;
        }
        int midPoint = array.Count / 2;;
        for (int i = 0; i < midPoint; i++)
        {
            left.Add(array[i]);
        }
        int x = 0;
        for (int i = midPoint; i < array.Count; i++)
        {
            right.Add(array[i]);
            x++;
        }
        left = mergeSort(left);
        right = mergeSort(right);
        result = merge(left, right);
        return result;
    }
    public static List<string[]> merge(List<string[]> left, List<string[]> right)
    {
        List<string[]> result = new List<string[]>();
        int indexLeft = 0, indexRight = 0;
        while (indexLeft < left.Count || indexRight < right.Count)
        {  
            if (indexLeft < left.Count && indexRight < right.Count)
            {
                if (Convert.ToInt32(left[indexLeft][1]) <= Convert.ToInt32(right[indexRight][1]))
                {
                    result.Add(left[indexLeft]);
                    indexLeft++;
                }
                else
                {
                    result.Add(right[indexRight]);
                    indexRight++;
                }
            }
            else if (indexLeft < left.Count)
            {
                result.Add(left[indexLeft]);
                indexLeft++;
            }
            else if (indexRight < right.Count)
            {
                result.Add(right[indexRight]);
                indexRight++;
            }
        }
        return result;
    }

    private void NewMessage(string text, int function)
    {
        continuerunning = false;
        messageGO.SetActive(true);
        messageText.text = text;
    }
    public void MessageBack()
    {
        messageGO.SetActive(false);
    }
    public void MessageContinue()
    {
        continuerunning = true;
        messageGO.SetActive(false);
        StartGame();
    }

}
