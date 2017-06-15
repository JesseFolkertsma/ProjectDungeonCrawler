﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinding_Visualizer))]
public class PathFinding : MonoBehaviour {

    //make a 3d grid (visually in the editor) from the object this stands on

    //get all static objects in scene

    //reference
    public static PathFinding self;
    public static Pathfinding_Visualizer visualizer;

    public int widthSize, heightSize;
    public float widthSizeNode, heightSizeNode;

    public Node[,,] grid; //moet dit wel omzetten in custom class met lists van list, sinds geen serializing
    public static List<GameObject> bakeable;

    public bool visualize = true;
    public bool visualizeNodes = false;
    [SerializeField]
    private bool visualizeRaycasts = false;

    public delegate void _Repaint();
    public static _Repaint repaint;
    [SerializeField]
    private int edgeBakeAmount;
    [SerializeField]
    private int calculationsPerFrame = 100, initializesPerFrame = 20000;
    public static bool pathfindable = false;
    [HideInInspector]
    public Transform center; //priorites baking of areas around this object
    [SerializeField]
    private float positionUpdateFrequency = 3;

    private void Awake()
    {
        //initialize reference to self
        self = this;
        visualizer = GetComponent<Pathfinding_Visualizer>();
        grid = new Node[widthSize, heightSize, widthSize];
    }

    private void Start()
    {
        StartCoroutine(UpdatePosition());
    }

    private void Update()
    {
        if (repaint != null)
            repaint();
    }

    //current room
    public GameObject curRoom = null;
    public IEnumerator UpdatePosition()
    {
        PDC.Generating.MapVisualizer mV = PDC.Generating.MapVisualizer.self;

        if(!(mV != null))
        {
            Debug.Log("Map Visualizer does not excist in this scene. Aborting.");
            yield break;
        }

        while (!(center != null))
            yield return null;

        while (true)
        {
            bool changed = false;

            //check all room positions
            foreach (GameObject room in mV.spawnedRooms) {

                //check if not null
                if(!(curRoom != null))
                {
                    curRoom = room;
                    changed = true;
                    continue;
                }

                //check if same room
                if (room == curRoom)
                    continue;

                //check if further than other room
                if (Vector3.Distance(curRoom.transform.position, center.position) >
                    Vector3.Distance(room.transform.position, center.position))
                {
                    curRoom = room;
                    changed = true;
                }
            }

            if(curRoom != null)
            {
                //rebake
                if(changed)
                    StartBake();
            }

            yield return new WaitForSeconds(positionUpdateFrequency);
        }
    }

    public void StartBake()
    {
        StartCoroutine(Bake());
    }

    private Coroutine bake;
    public IEnumerator Bake() //1 grote fout: de bake bugt de fuck out als de objecten aan de bovenkant komen
    {
        if(bake != null)
            StopCoroutine(bake);
        #region Prepare Bake
        int calc = 0;
        //initialize grid
        for (int x = 0; x < widthSize; x++)
            for (int y = 0; y < heightSize; y++)
                for (int z = 0; z < widthSize; z++)
                {
                    calc++;
                    grid[x, y, z] = new Node(x, y, z);
                    if(calc > initializesPerFrame)
                    {
                        calc = 0;
                        yield return null;
                    }
                }

        //move to closest
        transform.position = curRoom.transform.position;

        //reset pathfindable
        bakeable = new List<GameObject>();
        pathfindable = true;

        //get all gameobjects
        GameObject[] all = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject possibleBakeObj in all)
        {
            if (possibleBakeObj.isStatic)
                bakeable.Add(possibleBakeObj);
        }

        SetMidsAndBoundary(); //this is the node where this transform now is, neccessity for getnodefromvector
        #endregion

        bake = StartCoroutine(BakePreparedScene(center)); //now bake all objects in the 3d array
    }

    private bool currentlyBakingObject = false;
    private List<List<Node>> bakedObjects;
    private IEnumerator BakePreparedScene(Transform center)
    {
        List<GameObject> toBake = bakeable; //contantly remove from this list

        while(toBake.Count > 0)
        {
            //get cheapest
            GameObject closest = null;
            float dis = 0;
            foreach (GameObject obj in toBake)
            {
                float distance = Vector3.Distance(obj.transform.position, center.position);
                //remove from tobake
                if(!(closest != null))
                {
                    closest = obj;
                    dis = distance;
                    continue;
                }

                if (distance > dis)
                    continue;

                dis = distance;
                closest = obj;
            }  

            StartCoroutine(BakeObject(closest, BakeType.Object));
            toBake.Remove(closest);

            while (currentlyBakingObject)
                yield return null;
        }
    }

    public enum BakeType {Object, Enemy, Movable, Walkable }
    private IEnumerator BakeObject(GameObject bakeable, BakeType type) //hier zit HET PROBLEEM
    {
        if (type == BakeType.Object)
            currentlyBakingObject = true;

        List<Node> ret = new List<Node>();
        //get size collider
        Collider c = bakeable.GetComponent<Collider>();
        if (!(c != null))
        {
            currentlyBakingObject = false;
            yield return null;
            if (type != BakeType.Object)
                EndRealtimeBakeFrame(bakeable, ret);
            yield break;
        }

        Vector3 size = c.bounds.size;

        //get mid size bakeable
        Node midNode = GetNodeFromVector(c.bounds.center);

        int _x = (int)(size.x / widthSizeNode);
        int _y = (int)(size.y / heightSizeNode);
        int _z = (int)(size.z / widthSizeNode);
        int halfX = _x / 2;
        int halfY = _y / 2;
        int halfZ = _z / 2;
        
        Collider[] hits;
        Node node;

        if (!(midNode != null))
        {
            //Debug.Log(bakeable.name + " not scanned, the center has to be in the bakeable area.");
            currentlyBakingObject = false;
            yield return null;
            if (type != BakeType.Object)
                EndRealtimeBakeFrame(bakeable, ret);
            yield break;
        }

        int calc = 0;
        #region Old Code
        for (int x = midNode.x - halfX; x <= halfX + midNode.x; x++)
            for (int y = midNode.y - halfY; y <= halfY + midNode.y + 1; y++)
                for (int z = midNode.z - halfZ; z <= halfZ + midNode.z; z++)
                {                 
                    //check if in bounds
                    if (x < 0 || x >= grid.GetLength(0))
                        continue;
                    if (y < 0 || y >= grid.GetLength(1))
                        continue;
                    if (z < 0 || z >= grid.GetLength(2))
                        continue;

                    node = grid[x, y, z];
                    if (node.filled && node.bakeType == BakeType.Object)
                        continue;

                    calc++;
                    if (calc > calculationsPerFrame)
                    {
                        calc = 0;
                        yield return null;
                    }

                    Vector3 midPos = GetVectorFromNode(grid[x, y, z]);
                    hits = Physics.OverlapSphere(midPos, heightSizeNode / 2); //in het midden schieten, niet in de hoek
                    if (visualizeRaycasts)
                    {
                        Color v = hits.Length > 0 ? Color.red : Color.grey;
                        Debug.DrawRay(midPos, Vector3.down, v, 1);
                    }
                    foreach (Collider hit in hits)
                    {
                        if (bakeable != hit.transform.gameObject)
                            continue;
                        node.bakeType = type;
                        node.filled = true;
                        ret.Add(node);
                        break;
                    }
                }

        #endregion
        currentlyBakingObject = false;
        if(type != BakeType.Object)
            EndRealtimeBakeFrame(bakeable, ret);
    }

    private void EndRealtimeBakeFrame(GameObject bakeable, List<Node> nodes)
    {
        foreach (BakeProcess bP in realtimeBake)
            if (bP.obj == bakeable)
            {
                bP.busy = false;
                bP.bakeable.myNodes = nodes;
                break;
            }
    }

    private bool CheckOutOfBounds(int x, int y, int z)
    {
        if (x <= 0 || x >= grid.GetLength(0) - 1)
            return true;
        if (x <= 0 || y >= grid.GetLength(1) - 1)
            return true;
        if (z <= 0 || z >= grid.GetLength(2) - 1)
            return true;
        return false;
    }

    private List<BakeProcess> realtimeBake = new List<BakeProcess>();
    private class BakeProcess
    {
        public GameObject obj;
        public Pathfinding_Bakeable bakeable;
        public bool busy;

        public BakeProcess(Pathfinding_Bakeable bakeable)
        {
            this.bakeable = bakeable;
            obj = bakeable.gameObject;
        }
    }
    public void BakeObjectRealTime(Pathfinding_Bakeable bakeable)
    {
        BakeProcess bP = new BakeProcess(bakeable);
        realtimeBake.Add(bP);
        StartCoroutine(_BakeObjectRealTime(bP));
    }

    private IEnumerator _BakeObjectRealTime(BakeProcess process)
    {
        while (!pathfindable)
            yield return null;
        
        GameObject g = process.obj;

        while (true)
        {
            //bake again
            process.busy = true;

            foreach (Node node in process.bakeable.myNodes)
                node.filled = false;

            StartCoroutine(BakeObject(g, process.bakeable.bakeType));

            while (process.busy)
                yield return null;

            yield return new WaitForSeconds(process.bakeable.refreshSpeed);
        }     
    }

    //set mids to calculate from
    private int midW;
    private int midH;
    private Vector3 boundary;
    private void SetMidsAndBoundary()
    {
        midW = grid.GetLength(0) / 2;
        midH = grid.GetLength(1) / 2;
        
        GetBoundary();
    }

    public Vector3 GetBoundary()
    {
        boundary = transform.position;

        //boundary is left bottom
        boundary.x -= widthSize * widthSizeNode / 2;
        boundary.y -= heightSize * heightSizeNode / 2;
        boundary.z -= widthSize * widthSizeNode / 2;
        return boundary;
    }
    //<3
    public Vector3 GetVectorFromNode(Node node)
    {
        //calc from boundary
        Vector3 pos = boundary;
        pos.x += widthSizeNode * node.x;
        pos.y += heightSizeNode * node.y;
        pos.z += widthSizeNode * node.z;
        return pos;
    }

    public Vector3 GetSizeSquire()
    {
        return new Vector3(
            GetFullX(),
            GetFullY(),
            GetFullX());
    }

    public float GetFullX()
    {
        return widthSize * widthSizeNode;
    }

    public float GetFullY()
    {
        return heightSize * heightSizeNode;
    }

    //this is what you publicly access
    public Node GetNodeFromVector(Vector3 pos)
    {
        Vector3 difference = pos - transform.position;

        int x = midW + Mathf.FloorToInt(difference.x / widthSizeNode);
        int y = midH + Mathf.FloorToInt(difference.y / heightSizeNode);
        int z = midW + Mathf.FloorToInt(difference.z / widthSizeNode);

        //debug checks, it makes sure to return null when outside the grid
        if (x >= grid.GetLength(0) || x < 0)
            return null;
        if (y >= grid.GetLength(1) || y < 0)
            return null;
        if (z >= grid.GetLength(2) || z < 0)
            return null;
        return grid[x, y, z];
    }

    [Serializable]
    public class Node
    {
        public bool filled; //true when object is in node
        public float height; //different to center height of average object in space, create max height difference, so something like -2.5f or something for instance
        public int x, y, z; //pos
        public BakeType bakeType;

        public Node(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    }
}
