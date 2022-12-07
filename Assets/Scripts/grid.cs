using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public static Grid instance; 
    public Tile[,] gridArray = new Tile[16, 16]; //2d array of tiles that stores each tile of the grid

    public Grid()
    {
        if (instance != null)
        {
            Debug.LogError("More than one instance");
            return;
        }
        instance = this;
    }
    public void AddToGrid(Tile tile, int x, int y)
    {
        gridArray[x, y] = tile; //adds a new tile to the grid at a certain position 
    }
    public Tile GetTileAt(int x, int y)
    {
        return gridArray[x, y];
    }

    public void DestroyGrid()
    {
        instance = null;
    }
    public void InitConnections() //adds all the connections to the tiles
    {
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                gridArray[i, j].AddConnection(gridArray[i + 1, j]);
                gridArray[i, j].AddConnection(gridArray[i, j + 1]);
            }
        }
    }
}
