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
            [SerializeField, Range(0, 100)]
            private int difficulty;
            [SerializeField]
            private int difficultyBuffer;
            [SerializeField]
            private GameObject player;
            [SerializeField]
            private List<Enemy> enemies = new List<Enemy>();

            private MapLoader loader;

            #region Quest Info

            public int enemyValue;

            #endregion

            public void SpawnRooms(MapGenerator.Node[,,] level, MapGenerator.Node entrance, MapGenerator.Node exit, List<TagManager.TagType> tags)
            {
                loader = GetComponent<MapLoader>();

                if(densityInteriorMax < densityInteriorMin)
                {
                    Debug.Log("The max Interior density is lower than it's minimal counterpart!");
                    return;
                }
                if (densityEnemyMax < densityEnemyMin)
                {
                    Debug.Log("The max enemy density is lower than it's minimal counterpart!");
                    return;
                }

                StartCoroutine(_SpawnRooms(level, entrance, exit, tags));
            }

            private IEnumerator _SpawnRooms(MapGenerator.Node[,,] level, MapGenerator.Node entrance, MapGenerator.Node exit, List<TagManager.TagType> tags)
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

                            //check if room is require
                            if (!n.initialized)
                                continue;
                            
                            Vector3 pos = new Vector3();
                            pos.x = minX + roomWidth * x + roomWidth / 2;
                            pos.y = minY + roomHeight * y + roomHeight / 2;
                            pos.z = minZ + roomWidth * z + roomWidth / 2;
                            GameObject room = Instantiate(n.room.room, pos, Quaternion.identity);
                            room.transform.eulerAngles = new Vector3(0, (float)n.room.rotation, 0);

                            //set interior
                            RoomInterior rI = room.GetComponent<RoomInterior>();
                            if(!(rI != null))
                            {
                                Debug.Log("There isnt a script on this room. Location: " + x + " " + y + " " + z + " Name: " + n.room.room.name);
                                continue;
                            }

                            int points = mG.random.Next(densityInteriorMin, densityInteriorMax);
                            int _points = 0;

                            //if the amount is more than this room is able to give
                            foreach (RoomInterior.InteriorItem item in rI.objectsInRoom)
                                _points += item.cost;
                            if (points > _points)
                                points = _points;
                            List<RoomInterior.InteriorItem> interior = rI.objectsInRoom;
                            bool stillFit = true;
                            while (points > 0 && interior.Count > 0 && stillFit)
                            {
                                //check if hasItemWithFitTag
                                stillFit = false;
                                foreach (RoomInterior.InteriorItem _iI in interior)
                                    if (tags.Contains(_iI.obj.GetComponent<TagManager>()._tag))
                                    {
                                        stillFit = true;
                                        break;
                                    }

                                //enable another interior item
                                RoomInterior.InteriorItem iI = interior[mG.random.Next(0, interior.Count)]; //also remove
                                TagManager t = iI.obj.GetComponent<TagManager>();
                                if (!tags.Contains(t._tag))
                                    continue;

                                interior.Remove(iI);
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
                                continue;
                            }

                            List<Enemy> fitEnemies = new List<Enemy>();
                            foreach(Enemy en in enemies)
                                if (tags.Contains(en.obj.GetComponent<TagManager>()._tag))
                                {
                                    //check if right difficulty
                                    if (en.difficulty < difficulty - difficultyBuffer || 
                                        en.difficulty > difficulty + difficultyBuffer)
                                        continue;
                                    fitEnemies.Add(en);
                                    break;
                                }

                            if(fitEnemies.Count == 0)
                            {
                                Debug.Log("There are no normal enemies! / the difficulty doesn't match the available enemies!");
                                continue;
                            }

                            if(rI.spawnPositions.Count == 0)
                            {
                                Debug.Log("There are no spawn positions in this room");
                                yield break;
                            }

                            int e = mG.random.Next(densityEnemyMin, densityEnemyMax);
                            int _e = 0;
                            List<int> positions = new List<int>();
                            while(_e < e)
                            {
                                int newEnemy = mG.random.Next(0, fitEnemies.Count - 1);
                                Enemy enemy = fitEnemies[newEnemy];
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
                                enemyValue += enemy.cost;
                            }

                            yield return null;
                        }

                loader.SetProgress(MapLoader.Progress.Placing_Rooms);
            }

            [Serializable]
            public class Enemy
            {
                public GameObject obj;
                public int cost;
                [Range(0,100)]
                public int difficulty;
            }
        }
    }
}
