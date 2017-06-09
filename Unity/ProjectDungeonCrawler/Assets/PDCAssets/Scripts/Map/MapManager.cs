using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour {

    #region Normal Functions

    public int resolutionX = 256, resolutionY = 144;

    private void Awake()
    {
        InitializeMap();
    }

    public void PressMap()
    {
        //convert mousepos to pixel position
        Vector2 mousePos = Input.mousePosition;
        float x = mousePos.x / resolutionX;
        float y = mousePos.y / resolutionY;

        //calculate percentage into grid
        x = Mathf.Lerp(0, grid.GetLength(0), x);
        y = Mathf.Lerp(0, grid.GetLength(1), y);

        print(grid[(int)x,(int)y].terrain);
    }

    #endregion

    #region Map Baking

    //map node system
    private Node[,] grid;

    public enum TerrainType {Road = 1, Walkable = 3, Difficult = 8, Unwalkable }
    [Serializable]
    private class Node
    {
        public int x, y;
        public TerrainType terrain;

        public Node(int _x, int _y, TerrainType _terrain)
        {
            x = _x;
            y = _y;
            terrain = _terrain;
        }
    }

    [SerializeField, Tooltip("Green: Road, White: Walkable, Grey: Difficult, Black: Unwalkable."), 
        Header("Also enable Read/Write in the Texture's import settings.")]
    private Texture2D mapSkeleton;
    [Tooltip("Size of texture grid"), SerializeField]
    private int texX, texY;
    public void InitializeMap() //make this a customeditor button
    {
        float x = mapSkeleton.width / texX; //percentage of place where to place pos
        float y = mapSkeleton.height / texY;

        grid = new Node[texX, texY];
        Vector2 pos;

        for (int _x = 0; _x < texX; _x++)
            for (int _y = 0; _y < texY; _y++)
            {
                pos = new Vector2(x * _x, y * _y);
                //calculate terrain type
                TerrainType terrain = TerrainType.Walkable;
                Color col;

                //get color from terrain
                col = mapSkeleton.GetPixel((int)pos.x, (int)pos.y);

                //I cannot use a switch with a color, ugly ugly
                if (col == Color.green)
                    terrain = TerrainType.Road;
                else if (col == Color.white)
                    terrain = TerrainType.Walkable;
                else if (col == Color.red)
                    terrain = TerrainType.Difficult;
                else if (col == Color.black)
                    terrain = TerrainType.Unwalkable;
                else print("Combination of colors encountered! unable to clearly see which color it is so will make it walkable. Color: " + col);

                grid[_x, _y] = new Node(_x, _y, terrain);
            }
    }

    #endregion
}
