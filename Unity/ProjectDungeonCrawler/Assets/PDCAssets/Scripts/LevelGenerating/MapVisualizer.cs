using System;
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
            private float roomWidth;
            [SerializeField]
            private float roomHeight;
            [SerializeField, Range(0,20)]
            private int densityInteriorMin, densityInteriorMax;
            [SerializeField, Range(0, 20)]
            private int densityEnemyMin, densityEnemyMax;
            [SerializeField]
            private GameObject player;
            [SerializeField]
            private List<Enemy> enemies = new List<Enemy>();

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

                float minX = -(float)sizeX / 2 * roomWidth;
                float minY = -(float)sizeY / 2 * roomHeight;
                float minZ = -(float)sizeZ / 2 * roomWidth;
                MapGenerator mG = GetComponent<MapGenerator>();

                for (int x = 0; x < sizeX; x++)
                    for(int y = 0; y < sizeY; y++)
                        for(int z = 0; z < sizeZ; z++)
                        {
                            //spawn room
                            MapGenerator.Node n = level[x, y, z];
                            Vector3 pos = new Vector3();
                            pos.x = minX + roomWidth * x + roomWidth / 2;
                            pos.y = minY + roomHeight * y + roomHeight / 2;
                            pos.z = minZ + roomWidth * z + roomWidth / 2;
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
                                points -= iI.cost;
                                iI.obj.SetActive(true);
                            }

                            //spawn player
                            if(n == entrance)
                            {
                                int p = mG.random.Next(0, rI.spawnPositions.Count - 1);
                                Transform t = rI.spawnPositions[p].spawnPosition;
                                Instantiate(player, t.position, t.rotation);
                                yield return null;
                                continue;
                            }

                            //spawn enemies
                            if(enemies.Count == 0)
                            {
                                Debug.Log("There are no enemies to be spawned!");
                                yield break;
                            }

                            int e = mG.random.Next(densityEnemyMin, densityEnemyMax);
                            int _e = 0;
                            List<int> positions = new List<int>();
                            while(_e < e)
                            {
                                int newEnemy = mG.random.Next(0, enemies.Count - 1);
                                Enemy enemy = enemies[newEnemy];
                                _e += enemy.cost;
                                int enemySpawnpos = 0;
                                bool fit = false;
                                while (!fit)
                                {
                                    enemySpawnpos = mG.random.Next(0, rI.spawnPositions.Count - 1);
                                    if (!positions.Contains(enemySpawnpos))
                                        fit = true;
                                    if (positions.Count >= rI.spawnPositions.Count)
                                        break;
                                }

                                if (!fit)
                                    break;

                                //spawn enemy
                                Transform t = rI.spawnPositions[enemySpawnpos].spawnPosition;
                                Instantiate(enemy.obj, t.position, t.rotation);
                            }

                            yield return null;
                        }
            }

            [Serializable]
            public class Enemy
            {
                public GameObject obj;
                public int cost;
            }
        }
    }
}
