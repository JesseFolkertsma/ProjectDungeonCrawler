using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC
{
    namespace Generating
    {
        public class MapVisualizer : MonoBehaviour
        {
            [SerializeField]
            private float roomSize;
            [SerializeField, Range(0,20)]
            private int densityInteriorMin, densityInteriorMax;

            public void SpawnRooms(MapGenerator.Node[,,] level, MapGenerator.Node entrance)
            {
                if(densityInteriorMax < densityInteriorMin)
                {
                    Debug.Log("The max Interior density is lower than it's minimal counterpart!");
                    return;
                }

                StartCoroutine(_SpawnRooms(level, entrance));
            }

            private IEnumerator _SpawnRooms(MapGenerator.Node[,,] level, MapGenerator.Node entrance)
            {
                //for loop length
                int sizeX = level.GetLength(0);
                int sizeY = level.GetLength(1);
                int sizeZ = level.GetLength(2);

                float minX = -(float)sizeX / 2;
                float minY = -(float)sizeY / 2;
                float minZ = -(float)sizeZ / 2;
                MapGenerator mG = GetComponent<MapGenerator>();

                for (int x = 0; x < sizeX; x++)
                    for(int y = 0; y < sizeY; y++)
                        for(int z = 0; z < sizeZ; z++)
                        {
                            //spawn room
                            MapGenerator.Node n = level[x, y, z];
                            Vector3 pos = new Vector3();
                            pos.x = sizeX + roomSize * x;
                            pos.y = sizeY + roomSize * y;
                            pos.z = sizeZ + roomSize * z;
                            GameObject room = Instantiate(n.room.room, pos, new Quaternion(0, (float)n.room.rotation, 0, 0));

                            //set interior
                            RoomInterior rI = room.GetComponent<RoomInterior>();
                            int points = mG.random.Next(densityInteriorMin, densityInteriorMax);
                            int _points = 0;

                            //if the amount is more than this room is able to give
                            foreach (RoomInterior.InteriorItem item in rI.objectsInRoom)
                                _points += item.cost;
                            if (points > _points)
                                points = _points;
                            int sizeInteriorInRoom = rI.objectsInRoom.Count;
                            while (points > 0)
                            {
                                //enable another interior item
                                RoomInterior.InteriorItem iI = rI.objectsInRoom[mG.random.Next(0, sizeInteriorInRoom)];
                                if (iI.cost > points)
                                    continue;
                                points -= iI.cost;
                                iI.obj.SetActive(true);
                            }
                            yield return null;
                        }

                //spawn the player
            }
        }
    }
}
