using FYP.Server.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FYP.Server.RoomManagement
{
    public class Room :MonoBehaviour
    {
        [SerializeField]
        private Transform origin = null;
        private Scene roomScene;
        private PhysicsScene physicsScene;
        private readonly HashSet<NetworkTransform> networkObjects = new HashSet<NetworkTransform>();
        private readonly HashSet<ServerPlayer> playersInRoom = new HashSet<ServerPlayer>();
        private LocalityOfRelevance[,,] localityOfRelevances;
        public uint roomID { get; private set; } = 0;
        public RoomTemplate roomTemplate { get; private set; }

        public int roomCount => playersInRoom.Count;
        public int maxCapacity => roomTemplate.playerLimit;

        public bool canEnter => maxCapacity == 0 || roomCount < maxCapacity;

        public void Initialize(RoomTemplate roomTemplate,uint id)
        {
            this.roomTemplate = roomTemplate;
            PopulateLOR(roomTemplate);
            roomID = id;

            CreateSceneParameters csp = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
            roomScene = SceneManager.CreateScene($"Room_{roomID.ToString()}",csp);
            physicsScene = roomScene.GetPhysicsScene();
            SceneManager.MoveGameObjectToScene(gameObject, roomScene);
        }


        public bool AddPlayer(ServerPlayer player) 
        {
            if (playersInRoom.Contains(player)) 
            {
                Debug.LogWarning("Tried adding same player to room");
                return false;
            }
            if(canEnter) 
            {
                var res = SpawnObject(player.playerTransform);
                if (res) 
                {
                    playersInRoom.Add(player);
                    player.transform.parent = transform;
                    player.playerTransform.position = origin.position;
                    player.playerTransform.rotation = origin.rotation;
                    return true;
                }
                else 
                {
                    Debug.LogWarning("Failed adding player to room removing the object");
                    RemoveObject(player.playerTransform);
                }
            }
            return false;
        }
        public void RemovePlayer(ServerPlayer player) 
        {
            RemoveObject(player.playerTransform);
            playersInRoom.Remove(player);
        }

        public bool SpawnObject(NetworkTransform objectToSpawn) 
        {
            objectToSpawn.room = this;
            var lor = GetLOR(objectToSpawn.position);


            lor.AddPlayer(objectToSpawn);
            return networkObjects.Add(objectToSpawn);
        }



        public bool RemoveObject(NetworkTransform objectToRemove) 
        {
            if(objectToRemove.room == this) 
            {
                objectToRemove.room = null;
            }
            objectToRemove.lor?.RemoveObject(objectToRemove);

            return networkObjects.Remove(objectToRemove);
        }


        public LocalityOfRelevance GetLOR(Vector3 position) 
        {
            position += roomTemplate.sceneSize / 2f;

            float boundX = roomTemplate.sceneSize.x / roomTemplate.lorCountX;
            float boundY = roomTemplate.sceneSize.y / roomTemplate.lorCountY;
            float boundZ = roomTemplate.sceneSize.z / roomTemplate.lorCountZ;

            int x = Mathf.Clamp(Mathf.FloorToInt(position.x / boundX), 0, roomTemplate.lorCountX - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt(position.y / boundY), 0, roomTemplate.lorCountY - 1);
            int z = Mathf.Clamp(Mathf.FloorToInt(position.z / boundZ), 0, roomTemplate.lorCountZ - 1);

            return localityOfRelevances[x,y,z];
        }
        private void OnDrawGizmos()
        {
            foreach (var item in localityOfRelevances)
            {
                Gizmos.DrawWireCube(item.bounds.center, item.bounds.size);
            }
        }

        #region Populate LOR

        private void PopulateLOR(RoomTemplate roomTemplate)
        {
            localityOfRelevances = new LocalityOfRelevance[roomTemplate.lorCountX, roomTemplate.lorCountY, roomTemplate.lorCountZ];
            Vector3 lorIncrement = roomTemplate.sceneSize;
            lorIncrement.x /= roomTemplate.lorCountX;
            lorIncrement.y /= roomTemplate.lorCountY;
            lorIncrement.z /= roomTemplate.lorCountZ;

            Vector3 lorBufferedSize = lorIncrement + roomTemplate.padding;


            Vector3 start = roomTemplate.centre + lorIncrement / 2 - new Vector3(roomTemplate.sceneSize.x / 2, roomTemplate.sceneSize.y / 2, roomTemplate.sceneSize.z / 2);
            Vector3 lorPosition = start;
            for (int x = 0; x < roomTemplate.lorCountX; x++)
            {
                lorPosition.y = start.y;
                for (int y = 0; y < roomTemplate.lorCountY; y++)
                {
                    lorPosition.z = start.z;
                    for (int z = 0; z < roomTemplate.lorCountZ; z++)
                    {

                        localityOfRelevances[x, y, z] = new LocalityOfRelevance(new Bounds(lorPosition, lorBufferedSize));
                        lorPosition.z += lorIncrement.z;
                    }
                    lorPosition.y += lorIncrement.y;
                }
                lorPosition.x += lorIncrement.x;
            }

            for (int x = 0; x < roomTemplate.lorCountX; x++)
            {
                for (int y = 0; y < roomTemplate.lorCountY; y++)
                {
                    for (int z = 0; z < roomTemplate.lorCountZ; z++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    if (x + i >= 0 && x + i < roomTemplate.lorCountX
                                        && y + i >= 0 && y + i < roomTemplate.lorCountY
                                        && z + i >= 0 && z + i < roomTemplate.lorCountZ)
                                    {
                                        localityOfRelevances[x, y, z].adjacentLoRs.Add(localityOfRelevances[x + i, y + i, z + i]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}