using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC
{
    namespace Generating
    {
        [RequireComponent(typeof(MapVisualizer))]
        public class MapGenerator : MonoBehaviour
        {
            #region Inspector Settings

            [SerializeField]
            private int widthMin, widthMax;
            #endregion

            public Node[,,] level;

            //random generating
            public System.Random random;
            [SerializeField]
            private string seed;
            [HideInInspector]
            public int _seed;
            [HideInInspector]
            private bool randomSeed;
            public Node entrance;
            private MapVisualizer visualizer;

            #region Shortcuts for data
            [HideInInspector]
            public int size;
            #endregion

            #region Room Management
            [SerializeField, Tooltip(
                "All the different types of rooms (for instance: Top (Entrance), Left(Entrance), Right(Entrance), Back(Closed), Front(Closed), Down(Closed)) are required to make a full dungeon.")]
            private List<Room> rooms;
            private List<Room> _rooms; //all rooms + rotated rooms

            private void ConvertRooms()
            {
                _rooms = new List<Room>();
                foreach (Room rc in rooms)
                {
                    Room temp = rc;
                    if (!temp.needsConverting)
                        continue;
                    for (int i = 0; i < 4; i++)
                    {
                        Room _temp = TurnRoom(temp);
                        _rooms.Add(_temp);
                        temp = _temp;
                    }
                }
            }

            private Room TurnRoom(Room r)
            {
                Room ret = new Room();
                ret.front = r.left;
                ret.right = r.front;
                ret.back = r.right;
                ret.left = r.back;

                //stabilize rotation
                int rot = (int)r.rotation + 90;
                if (rot > 270)
                    rot = 0;
                ret.rotation = (Rotation)rot;

                ret.room = r.room;
                return ret;
            }
            #endregion

            private void Awake()
            {
                if (widthMin < 2)
                {
                    Debug.Log("Try setting the minimal width to at least 2.");
                    return;
                }
                if (widthMax < widthMin)
                {
                    Debug.Log("The max width is lower than the min width!");
                    return;
                }

                visualizer = GetComponent<MapVisualizer>();

                //convert room to all directions
                ConvertRooms();

                //set seed
                SetSeed();

                //initialize nodes and nodelevels
                StartCoroutine(InitializeLevel());
            }

            #region STEPS

            //STEP #1
            private void SetSeed()
            {
                if (randomSeed)
                {
                    int i = UnityEngine.Random.Range(0, 1000).GetHashCode();
                    _seed = i;
                }
                else
                    _seed = seed.GetHashCode();
                random = new System.Random(_seed);
            }

            //STEP #2     
            private IEnumerator InitializeLevel()
            {
                //data
                size = random.Next(widthMin, widthMax);
                level = new Node[size, size, size];
                for (int x = 0; x < size; x++)
                {
                    yield return null;
                    for (int y = 0; y < size; y++)
                    {
                        for (int z = 0; z < size; z++)
                        {
                            Node n = level[x, y, z] = new Node();
                            n.posX = x;
                            n.posY = y;
                            n.posZ = z;
                        }
                    }
                }

                //set entrance
                SetEntrance();

                //make path using seed spiraling down
                StartCoroutine(CreateRootPath());

                //spawn all _rooms
            }

            //STEP #3
            private void SetEntrance()
            {
                int x = random.Next(0, size);
                int z = random.Next(0, size);
                entrance = level[x, size - 1, z];
            }

            //STEP #4
            private enum RootDirection { Down, Left, Right, Front, Back, Count }
            private IEnumerator CreateRootPath()
            {
                //list of the path, get last when in while, first in branches
                List<Node> path = new List<Node>();
                path.Add(entrance);
                while (path[path.Count - 1].posY != 0) //maak hier miss een stack van
                {
                    yield return null;
                    //check which direction to go (left right forward back bottom), and check if you CAN
                    int dir = random.Next(0, (int)RootDirection.Count - 1);
                    Node newN = null;
                    Node oldN = path[path.Count - 1];
                    oldN.initialized = true;
                    oldN.room = new Room();
                    switch (dir) //the direction of the next part of the path
                    {
                        //check if direction can be taken
                        //else continue
                        case (int)RootDirection.Down:
                            newN = level[oldN.posX, oldN.posY - 1, oldN.posZ]; //no additional checks required
                            break;
                        case (int)RootDirection.Left:
                            if (oldN.posX <= 0) //check if adjecent excist
                                continue;
                            //set new node, if a check fails it doesnt matter because it will set again when it works
                            newN = level[oldN.posX - 1, oldN.posY, oldN.posZ];
                            if (newN.initialized) //check if initialized
                                continue;
                            //init old node room and set direction
                            oldN.room.left = Connection.Required;
                            break;
                        case (int)RootDirection.Right:
                            if (oldN.posX >= size - 1)
                                continue;
                            newN = level[oldN.posX + 1, oldN.posY, oldN.posZ];
                            if (newN.initialized)
                                continue;
                            oldN.room.right = Connection.Required;
                            break;
                        case (int)RootDirection.Front:
                            if (oldN.posZ >= size - 1)
                                continue;
                            newN = level[oldN.posX, oldN.posY, oldN.posZ + 1];
                            if (newN.initialized)
                                continue;
                            oldN.room.front = Connection.Required;
                            break;
                        case (int)RootDirection.Back:
                            if (oldN.posZ <= 0)
                                continue;
                            newN = level[oldN.posX, oldN.posY, oldN.posZ - 1];
                            if (newN.initialized)
                                continue;
                            oldN.room.back = Connection.Required;
                            break;
                    }

                    InitializeNode(oldN);

                    //save that new node in list
                    path.Add(newN);

                    //repeat until the ground floor has been reached
                }

                //function: create branches
                while (path.Count > 0) //use same list in a different way, always get bottom of the list (0)
                {
                    //fill nodes until none are left / accessible
                    //check foreach adjecents, if accessible but empty, fill and add to list

                    Node n = path[0];

                    //check adjecent

                    //top
                    if (n.room.top != Connection.Closed) //check out of bounds
                        if (n.posY < size)
                        {
                            //check !initialized
                            Node _n = level[n.posX, n.posY + 1, n.posZ];
                            if (!_n.initialized)
                            {
                                //pick room
                                InitializeNode(_n);
                                //add to list
                                path.Add(_n);
                            }
                        }

                    //bottom
                    if (n.room.down != Connection.Closed)
                        if (n.posY > 0)
                        {
                            Node _n = level[n.posX, n.posY - 1, n.posZ];
                            if (!_n.initialized)
                            {
                                InitializeNode(_n);
                                path.Add(_n);
                            }
                        }

                    //right
                    if (n.room.right != Connection.Closed)
                        if (n.posX < size)
                        {
                            Node _n = level[n.posX + 1, n.posY, n.posZ];
                            if (!_n.initialized)
                            {
                                InitializeNode(_n);
                                path.Add(_n);
                            }
                        }

                    //left
                    if (n.room.left != Connection.Closed)
                        if (n.posX > 0)
                        {
                            Node _n = level[n.posX - 1, n.posY, n.posZ];
                            if (!_n.initialized)
                            {
                                InitializeNode(_n);
                                path.Add(_n);
                            }
                        }

                    //front
                    if (n.room.front != Connection.Closed)
                        if (n.posZ < size)
                        {
                            Node _n = level[n.posX, n.posY, n.posZ + 1];
                            if (!_n.initialized)
                            {
                                InitializeNode(_n);
                                path.Add(_n);
                            }
                        }

                    //back
                    if (n.room.back != Connection.Closed)
                        if (n.posZ > 0)
                        {
                            Node _n = level[n.posX, n.posY, n.posZ - 1];
                            if (!_n.initialized)
                            {
                                InitializeNode(_n);
                                path.Add(_n);
                            }
                        }

                    //repeat until no _rooms are left to fill
                    yield return null;
                }

                visualizer.SpawnRooms(level, entrance);
            }

            #region Tools

            private void InitializeNode(Node n)
            {
                //get all _rooms with set directions (builds from getfit_rooms)
                List<Room> fitRooms = GetFitRooms(n);
                if (fitRooms.Count == 0)
                {
                    Debug.Log("There arent any fit _rooms for this Node!");
                    return;
                }

                //pick random GetFitRooms to put in old node node
                int chosen = random.Next(0, fitRooms.Count - 1);
                n.room = fitRooms[chosen];
            }

            private List<Room> GetFitRooms(Node n)
            {
                List<Room> ret = new List<Room>();
                foreach (Room r in _rooms)
                    if (CheckBorders(n, r))
                        ret.Add(r);
                return ret;
            }

            #endregion

            //check borders
            public bool CheckBorders(Node node, Room room) //checks both borders and (if initialized) if they meet set required passages
            {
                //check top
                if (room.top != Connection.Closed)
                {
                    //check out of bounds
                    if (size <= node.posY)
                        return false;

                    //check own requirements
                    if (node.initialized)
                        if (node.room.top != room.top && node.room.top != Connection.Required)
                            return false;

                    //check adjecent requirements
                    Node adj = level[node.posX, node.posY + 1, node.posZ];
                    if (adj.initialized)
                        if (adj.room.down != room.top)
                            return false;
                }

                //check bottom
                if (room.down != Connection.Closed)
                {
                    //check out of bounds
                    if (0 == node.posY)
                        return false;

                    //check own requirements
                    if (node.initialized)
                        if (node.room.down != room.down && node.room.down != Connection.Required)
                            return false;

                    //check adjecent requirements
                    Node adj = level[node.posX, node.posY - 1, node.posZ];
                    if (adj.initialized)
                        if (adj.room.top != room.down)
                            return false;
                }

                //check left
                if (room.left != Connection.Closed)
                {
                    //check out of bounds
                    if (0 == node.posX)
                        return false;

                    //check own requirements
                    if (node.initialized)
                        if (node.room.left != room.left && node.room.left != Connection.Required)
                            return false;

                    //check adjecent requirements
                    Node adj = level[node.posX - 1, node.posY, node.posZ];
                    if (adj.initialized)
                        if (adj.room.right != room.left)
                            return false;
                }

                //check right
                if (room.right != Connection.Closed)
                {
                    //check out of bounds
                    if (size <= node.posX)
                        return false;

                    //check own requirements
                    if (node.initialized)
                        if (node.room.right != room.right && node.room.right != Connection.Required)
                            return false;

                    //check adjecent requirements
                    Node adj = level[node.posX + 1, node.posY, node.posZ];
                    if (adj.initialized)
                        if (adj.room.left != room.right)
                            return false;
                }

                //check front
                if (room.front != Connection.Closed)
                {
                    //check out of bounds
                    if (size <= node.posZ)
                        return false;

                    //check own requirements
                    if (node.initialized)
                        if (node.room.front != room.front && node.room.front != Connection.Required)
                            return false;

                    //check adjecent requirements
                    Node adj = level[node.posX, node.posY, node.posZ + 1];
                    if (adj.initialized)
                        if (adj.room.back != room.front)
                            return false;
                }


                //check back
                if (room.back != Connection.Closed)
                {
                    //check out of bounds
                    if (0 == node.posZ)
                        return false;

                    //check own requirements
                    if (node.initialized)
                        if (node.room.back != room.back && node.room.back != Connection.Required)
                            return false;

                    //check adjecent requirements
                    Node adj = level[node.posX, node.posY, node.posZ - 1];
                    if (adj.initialized)
                        if (adj.room.front != room.back)
                            return false;
                }

                return true;
            }

            #endregion

            //return corresponding (random) room

            #region Classes

            public class Node
            {
                public bool initialized;
                public Room room;
                public int posX, posY, posZ;
            }

            public enum Connection { Closed, Entrance, Full, Required }
            public enum Rotation { Default, Right = 90, Turned = 180, Left = 270 }

            [Serializable]
            public class Room //create array in room for props to set active at will
            {
                public GameObject room;
                //entrances
                [Tooltip("Whether or not you need rotated versions of this room.")]
                public bool needsConverting;
                public Connection top, down,
                    left, right,
                    front, back;
                [HideInInspector]
                public Rotation rotation;
            }

            #endregion
        }
    }
}
