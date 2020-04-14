using DarkRift;
using FYP.Client;
using FYP.Server.Player;
using FYP.Shared;
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
        private Transform _origin = null;
        [SerializeField]
        private ClientScenes.SceneList _clientSceneIndex = ClientScenes.SceneList.Gameplay;

        public ushort clientSceneIndex => (ushort)_clientSceneIndex;
        public int playerCount => players.Count;
        public Transform origin => _origin;

        private Scene roomScene;
        private PhysicsScene physicsScene;
        public Dictionary<uint,ServerNetworkEntity> networkEntities { get; } = new Dictionary<uint, ServerNetworkEntity>();
        public HashSet<PlayerEntity> players { get; } = new HashSet<PlayerEntity>();

        private LocalityOfRelevance[,,] localityOfRelevances;
        public uint instanceID { get; private set; } = 0;
        public RoomTemplate roomTemplate { get; private set; }

        public int roomCount => players.Count;
        public int maxCapacity => roomTemplate.playerLimit;

        public bool canEnter => maxCapacity == 0 || roomCount < maxCapacity;


        private uint nextEntityID = 0;


        public LocalityOfRelevance[,,] GetAllLORs() 
        {
            return localityOfRelevances;
        }

        private uint GetNextEntityID() 
        {
            return nextEntityID++;
        }
        public void Initialize(RoomTemplate roomTemplate,uint id)
        {
            this.roomTemplate = roomTemplate;
            PopulateLOR(roomTemplate);
            instanceID = id;

            CreateSceneParameters csp = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
            roomScene = SceneManager.CreateScene($"Room_{instanceID.ToString()}",csp);
            physicsScene = roomScene.GetPhysicsScene();
            SceneManager.MoveGameObjectToScene(gameObject, roomScene);
        }

        public bool AddPlayer(PlayerEntity player) 
        {
            if (players.Contains(player)) 
            {
                Debug.LogWarning("Tried adding same player to room");
                return false;
            }
            if(canEnter) 
            {
                players.Add(player);
                SpawnObject(player);
                player.position = _origin.position;
                player.rotation = _origin.rotation;
                return true;
            }
            return false;
        }

        public void RemovePlayer(PlayerEntity player) 
        {
            DestroyObject(player);
            players.Remove(player);
        }


        public void SpawnObject(ServerNetworkEntity objectToSpawn) 
        {
            SceneManager.MoveGameObjectToScene(objectToSpawn.gameObject, roomScene);
            var entID = GetNextEntityID();
            objectToSpawn.entityID = entID;
            objectToSpawn.transform.parent = transform;
            objectToSpawn.room = this;
            var lor = GetLOR(objectToSpawn.position);
            if(objectToSpawn is PlayerEntity player) 
            {
                lor.AddPlayer(player);
            }
            else 
            {
                lor.AddObject(objectToSpawn);
            }
            networkEntities.Add(entID,objectToSpawn);
            objectToSpawn.OnEnteredRoom?.Invoke(this);
        }
        public void DestroyObject(ServerNetworkEntity objectToRemove)
        {
            objectToRemove.OnLeftRoom?.Invoke(this);
            if(objectToRemove is PlayerEntity player) 
            {
                objectToRemove.lor?.RemovePlayer(player);
            }
            else 
            {
                objectToRemove.lor?.RemoveObject(objectToRemove);
            }
            networkEntities.Remove(objectToRemove.entityID);
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
            if (localityOfRelevances != null) 
            {
                foreach (var item in localityOfRelevances)
                {
                    Gizmos.DrawWireCube(item.bounds.center, item.bounds.size);
                }
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

                        localityOfRelevances[x, y, z] = new LocalityOfRelevance(this,new Bounds(lorPosition, lorBufferedSize),x,y,z);
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
                                        && y + j >= 0 && y + j < roomTemplate.lorCountY
                                        && z + k >= 0 && z + k < roomTemplate.lorCountZ)
                                    {
                                        localityOfRelevances[x, y, z].adjacentLoRs.Add(localityOfRelevances[x + i, y + j, z + k]);
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
            var stack = new Stack<PlayerEntity>();
            foreach (var item in players)
            {
                stack.Push(item);
            }
            while (stack.Count > 0) 
            {
                var obj = stack.Pop();
                RemovePlayer(obj);
            }
            foreach (var obj in networkEntities.Values)
            {
                obj.OnLeftRoom?.Invoke(this);
            }
            networkEntities.Clear();
            Destroy(gameObject);
        }
    }
}