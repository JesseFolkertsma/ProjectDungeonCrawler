﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{

    #region Normal Functions

    [SerializeField]
    public Transform player;

    [SerializeField]
    private int resolutionX = 256, resolutionY = 144;

    private void Awake()
    {
        InitializeMap();
    }

    public void PressMap()
    {
        //convert mousepos to pixel position
        GetMapMovement(ConvertMapToNode(Input.mousePosition));
    }

    private Node ConvertMapToNode(Vector2 vec) //dit werkt niet
    {
        float x = resolutionX / vec.x;
        float y = resolutionY / vec.y;

        //calculate percentage into grid
        x = Mathf.Lerp(0, grid.GetLength(0), x) - 1;
        y = Mathf.Lerp(0, grid.GetLength(1), y) - 1;

        return grid[(int)x, (int)y];
    }

    private Vector2 ConvertNodeToMap(Node node) //onbekend of dit werkt
    {
        float x = (float)Screen.width / resolutionX;
        float y = (float)Screen.height / resolutionY;

        return new Vector2(x * node.x, y * node.y);
    }

    #endregion

    #region Map Baking

    //map node system
    private Node[,] grid;

    public enum TerrainType { Road = 1, Walkable = 3, Difficult = 8, Unwalkable }
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

    private class NodeCom : IComparable<NodeCom>
    {
        public Node node;
        public NodeCom parentNode;
        public int value;

        public int CompareTo(NodeCom other)
        {
            if (other == null)
                return 1;

            return value - other.value;
        }

        public NodeCom(Node node, NodeCom parentNode)
        {
            this.node = node;
            this.parentNode = parentNode;
            value = (int)node.terrain;
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

    #region Map Movement

    private void GetMapMovement(Node goal)
    {
        if (getMapMovement != null)
            StopCoroutine(getMapMovement);

        getMapMovement = StartCoroutine(_GetMapMovement(goal));
    }

    private Coroutine getMapMovement;
    public int calculationsPerFrame = 35;
    private List<NodeCom> open;
    private List<Node> closed;
    NodeCom curNode, startNode;
    private Vector3 endPos;
    private IEnumerator _GetMapMovement(Node goal)
    {
        open = new List<NodeCom>();
        closed = new List<Node>();

        //add start node
        startNode = new NodeCom(ConvertMapToNode(player.position), null);
        open.Add(startNode);
        endPos = ConvertNodeToMap(goal);

        int calc = 0;
        curNode = null;
        int x, y;
        while (open.Count > 0)
        {
            calc++;
            open.Sort();

            //remove from open and add to closed
            curNode = open[0];
            //if goal has been reached
            if (curNode.node == goal)
                break;

            open.RemoveAt(0);
            closed.Add(curNode.node);

            //shortcuts
            x = curNode.node.x;
            y = curNode.node.y;

            //check adjecent
            CheckNode(x + 1, y);
            CheckNode(x - 1, y);
            CheckNode(x, y + 1);
            CheckNode(x, y - 1);

            //optimization
            if (calc > calculationsPerFrame)
            {
                calc = 0;
                yield return null;
            }
        }

        if (curNode.node != goal)
            yield break;

        List<Vector2> path = new List<Vector2>();

        while (curNode != null)
        {
            path.Add(ConvertNodeToMap(curNode.node));
            curNode = curNode.parentNode;
        }

        player.GetComponent<MapPlayer>().Move(path);
    }

    private void CheckNode(int x, int y)
    {
        //check if out of bounds
        if (x < 0 || x > resolutionX - 1)
            return;
        if (y < 0 || y > resolutionY - 1)
            return;

        Node node = grid[x, y];

        //check if open and closed contains this
        if (closed.Contains(node)) //no work eh?
            return;
        foreach (NodeCom nC in open) //es tu no work
            if (nC.node == node)
                return;

        //add to open
        NodeCom _n = new NodeCom(node, curNode);
        Vector2 myPos = ConvertNodeToMap(_n.node);
        _n.value += (int)Vector2.Distance(myPos, player.position);
        _n.value += (int)Vector2.Distance(myPos, endPos);

        open.Add(_n);
    }

    #endregion
}
