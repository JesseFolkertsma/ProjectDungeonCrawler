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

            public void SpawnRooms(MapGenerator.Node[,,] level, MapGenerator.Node entrance)
            {
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

                for (int x = 0; x < sizeX; x++)
                    for(int y = 0; y < sizeY; y++)
                        for(int z = 0; z < sizeZ; z++)
                        {
                            MapGenerator.Node n = level[x, y, z];
                            Vector3 pos = new Vector3();
                            pos.x = sizeX + roomSize * x;
                            pos.y = sizeY + roomSize * y;
                            pos.z = sizeZ + roomSize * z;
                            Instantiate(n.room.room, pos, new Quaternion(0, (float)n.room.rotation, 0, 0));
                            yield return null;
                        }

                //spawn the player

                //set interior
            }
        }
    }
}
