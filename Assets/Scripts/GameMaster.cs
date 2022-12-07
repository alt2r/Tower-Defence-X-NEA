using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameMaster : MonoBehaviour
{
    //towers
    public GameObject ballistaTower;
    public GameObject gunpowderTower;
    public GameObject iceTower;
    public GameObject fireTower;
    public GameObject lightningTower;
    public GameObject powerTower;

    //map stuff
    public GameObject tileGO;
    private const float TILESIZE = 4;
    public GameObject gameMasterGO;
    public GameObject pathGO;
    public GameObject startGO, endGO;
    public GameObject mapGO;
    public GameObject wayPointGO;
    private static float totalPathLength = 0;
    public static float GetTotalPathLength() { return totalPathLength; }
    public GameObject castle;
    public static List<Waypoint> waypointList = new List<Waypoint>();
    private int pathcount = 0;
    private Grid grid = new Grid();

    //enemy game objects and other wave spawning related things
    public GameObject enemyGO;
    public GameObject flyingEnemyGO;
    public GameObject bossEnemyGO;

    public static List<Enemy> enemyList;
    const float TIMEBETWEEENWAVES = 5;
    private float countdown;
    public Text waveCountdownText, waveCounterText, playerBalanceDisplay;
    static int waveNumber;
    public static int GetWaveNumber() { return waveNumber;  }
    private bool waveFinishedSpawning;
    bool flyingwave = false;


    //UI

    public Image endHealthBar;

    public GameObject sp;
    public static GameObject shopPanel;

    public GameObject turretMenuPrefab; //unity cant assign values to static variables in the inspector
    public static GameObject turretMenu; //so whenever we want something static assigned from the inspector we have to have a transition variable
    public static bool activeTurretMenu;

    //player stuff
    public static int playerBalance;
    public static float playerHealth;
    public static bool gameWon;

    //counters to display on end screen
    private static int activeTowers;
    public static void IncrementActiveTowers() { activeTowers++; }
    public static void DeIncrementActiveTowers() { activeTowers--; }
    public static int GetActiveTowers() { return activeTowers; }
    public static int enemiesKilled;

    float hp;

    public GameObject popUpp;
    public static GameObject popUpBase;

    Dictionary<string, int> firstSpawns = new Dictionary<string, int>(); //displaying a pop up when the first enemy of each type comes
    //int 0 = not spawned 1 = spawned but no pop up displayed 2 = spawned and pop up displayed

    void Awake()
    {
        //resetting everything that needs to be reset in cas ethis is not the first game the user has played 
        hp = 100;
        enemiesKilled = 0;
        activeTowers = 0;
        gameWon = false;
        playerBalance = 30000; //300
        playerHealth = 100;
        activeTurretMenu = false;
        waveFinishedSpawning = true;
        waveNumber = 0; //0
        countdown = 2;
        enemyList = new List<Enemy>();

        GenerateLevel();
        shopPanel = sp; //unity and static stuff dont get along
        turretMenu = turretMenuPrefab;
        popUpBase = popUpp;
        BuildManager bm = new BuildManager(ballistaTower, gunpowderTower, iceTower, fireTower, lightningTower, powerTower);
        waveCounterText.text = $"Curret Wave: {waveNumber}"; //setting the wave counter text
        InvokeRepeating("RefreshBalanceDisplay", 0f, 0.25f); //repeats the RefreshBalanceDisplay() function every 0.25 seconds
        InvokeRepeating("CheckPopUps", 0f, 1f);

        //setting up pop up dict
        firstSpawns.Add("blue", 0);
        firstSpawns.Add("yellow", 0);
        firstSpawns.Add("green", 0);
        firstSpawns.Add("red", 0);
        firstSpawns.Add("orange", 0);
        firstSpawns.Add("pink", 0);
        firstSpawns.Add("white", 0);
        firstSpawns.Add("black", 0);
        firstSpawns.Add("flying", 0);
    }
    private void RefreshBalanceDisplay() //updates the player balance counter on the top right of the screen
    {
        playerBalanceDisplay.text = $"{playerBalance}g";
    }

   private void CheckPopUps()
    {
        string change = ""; //each pop up should only display once. Change changes the value ascosiated with the pop up being displaeyed to 2, meaning it wont be displayed again.
        foreach (KeyValuePair<string, int> x in firstSpawns)
        {
            if (x.Value == 1)
            {
                switch (x.Key)
                {
                    case "blue":
                        NewPopUp("Blue", "Blue is the standard enemy. It is slow and doesn't have much health. Ballistas and gunpowder towers are effective against blues.", popUpBase);
                        change = "blue";
                        break;
                    case "yellow":
                        NewPopUp("Yellow", "Yellow enemies are faster than blues, but still don.t have much health. Ballistas and gunpowder towers are effective against yellows.", popUpBase);
                        change = "yellow";
                        break;
                    case "green":
                        NewPopUp("Green", "Green enemies are stronger and faster then yellows. Its best to slow them down with an ice tower and then use gunpowders to mow them down.", popUpBase);
                        change = "green";
                        break;
                    case "red":
                        NewPopUp("Red", "Red is the strongest of the standard enemies. While slower than greens, they have double the health. Fire and lightning towers are effective.", popUpBase);
                        change = "red";
                        break;
                    case "orange":
                        NewPopUp("Orange", "Orange enemies accelerate as they move, best to slow them down with ice towers and take them out before they get too fast.", popUpBase);
                        change = "orange";
                        break;
                    case "pink":
                        NewPopUp("Pink", "Pink enemies slowly regenerate their health over time. The best way to take them down is to concentrate fire on one part of the path. Setting towers to taregt the strongest enemy in range also helps.", popUpBase);
                        change = "pink";
                        break;
                    case "white":
                        NewPopUp("White (boss)", "White is the weaker of the two boss enemies. It spawns minions to help it reach the end. Fire towers are effective", popUpBase);
                        change = "white";
                        break;
                    case "black":
                        NewPopUp("Black (boss)", "Black is the strongest of the two boss enemies, boasting an enourmous healthpool. Plenty of lightning towers are reccomended. Seguns dad", popUpBase);
                        change = "black";
                        break;
                    case "flying":
                        NewPopUp("Flying Enemies", "All Blue, Yellow, Green and Red enemies have a chance to spawn as a flying enemy, which fly above the map and their own way to the end. All turret types can damage flying enemies", popUpBase);
                        change = "flying";
                        break;
                    default:
                        break;
                }
            }
        }
        if (change != "")
        {
            firstSpawns[change] = 2;
        }
    }

    public void RefreshEndHealthBar()
    {
        endHealthBar.fillAmount = playerHealth / 100;
        endHealthBar.color = new Color(1 - (playerHealth / 100), playerHealth / 100, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth != hp) //if the health has changed since last frame
        {
           // Debug.Log($"{playerHealth}/100, took {hp - playerHealth} damage");
            RefreshEndHealthBar(); //refresh health bar
            hp = playerHealth;
        }

        WaveSpawner();
        if (playerHealth <= 0)
        {
            Lose();
        }
    }

    private void GenerateLevel()
    {
        Vector3 position = new Vector3(1f, 0f, 1f);
        Vector3 endPos = new Vector3(14f, 0f, 14f); //the end of the path, players base

        GameObject map = null;

        while (position != endPos || pathcount < 13)//will keep trying generation until a level where the path reaches the end is generated
        {
            pathcount = 0;
            totalPathLength = 0; //the total length of the paths. will determine the speed of the enemies to keep the game fair
            waypointList.Clear(); //clearing the waypoint list every time it runs GenerateLevel()

            if (map != null)
            {
                Destroy(map);
            }
            map = Instantiate(mapGO); //the parent class of the entire map

            float tilesize2 = TILESIZE + 0.5f; //0.5 is the gap between tiles 
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    position = new Vector3((i * tilesize2), 0f, (j * tilesize2));
                    grid.AddToGrid(new Tile(tileGO, position, map.transform), i, j);
                }
            }
            //16x16 grid is now generated
            grid.InitConnections(); //initialise the connections of the tiles. for flying enemy pathfinding
            position = new Vector3(1f, 0f, 1f); //the current position of the gird that the algorithm is looking at
            Instantiate(startGO, (position * tilesize2) + new Vector3(0f, 2.5f, 0f), new Quaternion(0, 0, 0, 0), map.transform); //gotta add 2.5 to the y so it gets instaitiated on top of the grid and not clipping through it

            int genTimeOut = 0; //stops the while loop iterating forever
            position = new Vector3(1f, 0f, 1f);

            while (position != endPos && genTimeOut < 150) //stop when current position is the position of the paths end
            {
                position = AddPath(position, GeneratePathLength(), GeneratePathDirection(), map.transform); //returns an updated value for position
                genTimeOut++;
            }

        }
        foreach (Waypoint w in waypointList)
        {
            w.Activate();
        }
        Quaternion castlerot = new Quaternion();
        castlerot.eulerAngles = new Vector3(0, 180, 0);
        GameObject.Instantiate(castle, new Vector3(35, 0, 24), castlerot);

    }
    private Vector3 AddPath(Vector3 currentPos, float length, string dir, Transform parent)
    {
        if (IsPathValid(currentPos, length, dir))
        {
            DeleteTilesUnderPath(currentPos, length, dir);
            path p = new path(pathGO, currentPos, parent, length, dir, TILESIZE);
            pathcount++;

            switch (dir) //update value of position
            {
                case "posX":
                    currentPos.x += length;
                    break;
                case "negX":
                    currentPos.x -= length;
                    break;
                case "posZ":
                    currentPos.z += length;
                    break;
                case "negZ":
                    currentPos.z -= length;
                    break;

            }
            waypointList.Add(new Waypoint(wayPointGO, currentPos * (TILESIZE + 0.5f) + new Vector3(0f, 2f, 0f)));
            totalPathLength += length;
        }
        return currentPos;
    }
    private float GeneratePathLength() //function returns a random float value between 2 and 9, but is more likely to return a numbers closer to the midle value (5/6)
    {
        System.Random rand = new System.Random(); //have to specify that i want the System random as opposed to the Unity random
        int randNum = rand.Next(1, 101);
        if (randNum <= 5)
        {
            return 2f;
        }
        else if (5 < randNum && randNum <= 12) //using else if because you cant put arithmatic logic in a switch case statement
        {
            return 3f;
        }
        else if (12 < randNum && randNum <= 28) //numbers chance of being returned is determined by how close they are to the centre number (5.5). 
        {
            return 4f;
        }
        else if (28 < randNum && randNum <= 50)//for example, 3 and 8 both have a 7% chance of being returned
        {
            return 5f;
        }
        else if (50 < randNum && randNum <= 72)
        {
            return 6f;
        }
        else if (72 < randNum && randNum <= 88)
        {
            return 7f;
        }
        else if (88 < randNum && randNum <= 95)
        {
            return 8f;
        }
        else
        {
            return 9f;
        }

    }
    private string GeneratePathDirection()
    {
        System.Random rand = new System.Random();
        int randNum = rand.Next(1, 9); //lower limit is inclusive, upper limit is exclusive

        if (randNum <= 3)
        {
            return "posZ";
        }
        else if (3 < randNum && randNum <= 6)
        {
            return "posX";
        }
        else if (randNum == 7)
        {
            return "negX";
        }
        else
        {
            return "negZ";
        }

    }
    private bool IsPathValid(Vector3 position, float length, string direction)
    {
        switch (direction)
        {

            case ("posX"):
                if (position.x + length < 15) //if it goes out of bounds, its not valid
                {
                    for (int i = 0; i <= length; i++) //loping over all tiles it will cover
                    {
                        if (grid.GetTileAt(Convert.ToInt32(position.x) + i, Convert.ToInt32(position.z)).IsPath()) //checking if each tile is a path
                        {
                            return false; //if any tile is a path then its invalid as it cant cross over another path
                        }
                    }
                    return true; //otherwise it is valid
                }
                return false;
            case ("negX"): 
                if (position.x - length > 0)
                {
                    for (int i = 0; i >= -length; i--)
                    {
                        if (grid.GetTileAt(Convert.ToInt32(position.x) + i, Convert.ToInt32(position.z)).IsPath())
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            case ("posZ"):
                if (position.z + length < 15)
                {
                    for (int i = 0; i <= length; i++)
                    {
                        if (grid.GetTileAt(Convert.ToInt32(position.x), Convert.ToInt32(position.z) + i).IsPath())
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            case ("negZ"):
                if (position.z - length > 0)
                {
                    for (int i = 0; i >= -length; i--)
                    {
                        if (grid.GetTileAt(Convert.ToInt32(position.x), Convert.ToInt32(position.z) + i).IsPath())
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            default:
                return true;

        }
    }
    private void DeleteTilesUnderPath(Vector3 position, float length, string direction)
    {
        for (int i = 0; i < length; i++)
        {
            grid.GetTileAt(Convert.ToInt32(position.x), Convert.ToInt32(position.z)).SetAsPath();
            switch (direction)
            {
                case "posX":
                    position.x += 1f;
                    break;

                case "negX":
                    position.x -= 1f;
                    break;

                case "posZ":
                    position.z += 1f;
                    break;

                case "negZ":
                    position.z -= 1f;
                    break;

                default:
                    break;
            }

        }
    }
    //ENEMY SPAWNING

    public void WaveSpawner()
    {
        if (waveFinishedSpawning && enemyList.Count == 0)
        {
            if (countdown <= 0f)
            {
                waveNumber++;
                if (waveNumber > 25 && (waveNumber + 2) % 5 == 0) //33, 38, 43, 48 etc  endless mode post 30 boss waves. also handles wave 28 because easier
                {
                    Enemy x = new Enemy(bossEnemyGO, 4.5f, 4.5f, "Black", null);
                    x.SetBossStats((waveNumber * 50) + 5000, 50f / waveNumber);
                    string spawnRate = Convert.ToString(50f / waveNumber);
                    float bosshealth = (waveNumber * 50) + 5000;
                    switch (MainMenu.GetDifficulty())
                    {
                        case 0:
                            bosshealth *= 0.8f;
                            break;
                        case 2:
                            bosshealth *= 1.2f;
                            break;
                        default:
                            break;
                    }
                    NewPopUp("Boss Wave", $"Health: {bosshealth}, Minion spawn rate: {spawnRate.Substring(0, 4)} seconds", popUpBase);
                    enemyList.Add(x);
                }
                else
                {
                    switch (waveNumber)
                    {
                        case (8):
                            Enemy a = new Enemy(bossEnemyGO, 4.5f, 4.5f, "White", null);
                            enemyList.Add(a);
                            NewPopUp("Boss Wave", "White is the weaker of the two boss enemies. This white enemy has come alone, but they have been known to show up in regular waves too!", popUpBase);
                            break;
                        case (15):
                            Enemy b = new Enemy(bossEnemyGO, 4.5f, 4.5f, "Black", null);
                            b.SetBossStats(2000, 3); //black enemies need to change in higher waves, they were too weak. BALANCE CHANGE
                            NewPopUp("Boss Wave", "Black is the stronger of the two enemies, and always comes alone. Its stats can vary, as some black enemies are stronger than others", popUpBase);
                            enemyList.Add(b);
                            break;
                        case (21):
                            Enemy c = new Enemy(bossEnemyGO, 4.5f, 4.5f, "Black", null);
                            c.SetBossStats(5000, 2);
                            enemyList.Add(c);
                            NewPopUp("Boss Wave", "Health: 5000, Minion spawn rate: 2 seconds", popUpBase);

                            break;
                        default:
                            if (waveNumber % 10 == 0)
                            {
                                flyingwave = true;
                                NewPopUp("Flying Wave", "All enemies this wave will be flying, and will attack from all angles!", popUpBase);
                            }
                            else
                            {
                                flyingwave = false;
                            }
                            StartCoroutine(SpawnWave()); //normal wave, spawn enemies 
                            break;
                    }
                }
                countdown = TIMEBETWEEENWAVES;
            }
            countdown -= Time.deltaTime;
            if (Mathf.Floor(countdown) == 0 || Mathf.Floor(countdown) >= TIMEBETWEEENWAVES - 1)
            {
                waveCountdownText.text = "    WAVE SPAWNING";
            }
            else if (Mathf.Floor(countdown) > 0)
            {
                waveCountdownText.text = $"NEXT WAVE COMING IN: {Mathf.Floor(countdown)}";
                if (!MainMenu.IsEndlessMode() && waveNumber >= 29)
                {
                    Win();
                }
            }
        }
    }
    IEnumerator SpawnWave()
    {
        waveCounterText.text = $"Curret Wave: {waveNumber}";
        waveFinishedSpawning = false;
        System.Random r = new System.Random();
        if (flyingwave)
        {
            for (int i = 0; i < (waveNumber * 2); i++) //changed * 4 to * 2 because it bodied every time balance change
            {
                Enemy x;
                string c = DecideColour(waveNumber);
                if (c != "White" && c != "Orange" && c != "Pink") //no orange, pink or white
                {
                    int f = r.Next(1, 3);
                    if (f == 1)
                    {
                        x = new Enemy(flyingEnemyGO, r.Next(1, 9) * 4.5f, 0, c, null);
                        enemyList.Add(x);
                    }
                    else
                    {
                        x = new Enemy(flyingEnemyGO, 0, r.Next(1, 9) * 4.5f, c, null);
                        enemyList.Add(x);
                    }                   
                    
                }
                float denom = ((waveNumber / 10f) * (totalPathLength / 80f)) + 1.5f;
                yield return new WaitForSeconds(0.8f / denom); //so they are the same distance apart regardless of the speed modifier
            }
        }
        else
        {
            for (int i = 0; i < (waveNumber * 4) + 5; i++)
            {
                Enemy x;
                int rand = r.Next(1, 11);
                string c = DecideColour(waveNumber);
                if (rand == 2 && waveNumber >= 5 && c != "White" && c != "Orange" && c != "Pink") //10% of enemies past wave 5 should be flying but not white, pink or orange
                {
                    x = new Enemy(flyingEnemyGO, 4.5f, 4.5f, c, null);
                    if (firstSpawns["flying"] == 0)
                    {
                        firstSpawns["flying"] = 1;
                    }
                }
                else
                {
                    if (c == "White")
                    {
                        x = new Enemy(bossEnemyGO, 4.5f, 4.5f, c, null);
                    }
                    else
                    {
                        x = new Enemy(enemyGO, 4.5f, 4.5f, c, null);
                    }
                }
                enemyList.Add(x);
                float denom = ((waveNumber / 10f) * (totalPathLength / 80f)) + 1.5f;
                yield return new WaitForSeconds(0.8f / denom); //so they are the same distance apart regardless of the speed modifier
            }
        }
        waveFinishedSpawning = true;

    }

    string DecideColour(float wavenum)
    {
        Dictionary<string, float> colourChances = new Dictionary<string, float>();
        colourChances.Add("Blue", 1);
        colourChances.Add("Yellow", 0.11f * (wavenum - 2));
        colourChances.Add("Green", 0.08f * (wavenum - 5));
        colourChances.Add("Red", 0.06f * (wavenum - 10));
        if (wavenum > 7)
        {
            colourChances["Pink"] = 0.02f;
            colourChances["Orange"] = 0.04f;
        }
        else
        {
            colourChances.Add("Pink", 0);
            colourChances.Add("Orange", 0);
        }
        if (wavenum > 17)
        {
            colourChances["White"] = 0.01f; //boss enemies among normal ones
        }
        else
        {
            colourChances.Add("White", 0);
        }
        System.Random r = new System.Random();
        float randValue = r.Next(1, 101) * 0.01f;

        if (colourChances["Red"] > 0.75)
        {
            colourChances["Red"] = 0.75f; //so the endgame isnt just red spam
        }

        if (colourChances["White"] >= randValue)
        {
            return "White";
        }
        randValue -= colourChances["White"];

        if (colourChances["Orange"] >= randValue)
        {
            if (firstSpawns["orange"] == 0)
            {
                firstSpawns["orange"] = 1;
            }
            return "Orange";
        }
        randValue -= colourChances["Orange"];

        if (colourChances["Pink"] >= randValue)
        {
            if (firstSpawns["pink"] == 0)
            {
                firstSpawns["pink"] = 1;
            }
            return "Pink";
        }
        randValue -= colourChances["Pink"];

        if (colourChances["Red"] >= randValue)
        {
            if (firstSpawns["red"] == 0)
            {
                firstSpawns["red"] = 1;
            }
            return "Red";
        }
        if (colourChances["Red"] < 0)
        {
            colourChances["Red"] = 0;
        }
        randValue -= colourChances["Red"];

        if (colourChances["Green"] >= randValue)
        {
            if (firstSpawns["green"] == 0)
            {
                firstSpawns["green"] = 1;
            }
            return "Green";
        }
        if (colourChances["Green"] < 0)
        {
            colourChances["Green"] = 0;
        }
        randValue -= colourChances["Green"];

        if (colourChances["Yellow"] >= randValue)
        {
            if (firstSpawns["yellow"] == 0)
            {
                firstSpawns["yellow"] = 1;
            }
            return "Yellow";
        }
        if (firstSpawns["blue"] == 0)
        {
            firstSpawns["blue"] = 1;
        }
        return "Blue";

    }

    //win lose

    private void Win()
    {
        gameWon = true;
        StopAllCoroutines(); //so stuff doesnt keep on looping despite the scene not being loaded, which caused the game to crash
        SceneManager.LoadScene("EndScreen");       
    }
    private void Lose()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("EndScreen");
    }
    //pop ups

    public static void NewPopUp(string title, string desc, GameObject popUp)
    {
        GameObject x = Instantiate(popUp);
        x.GetComponent<PopUp>().Setup(title, desc, x);
        Destroy(x, 10); //destroys pop up after 10sec bc clicking them was getting annoying

    }
}

