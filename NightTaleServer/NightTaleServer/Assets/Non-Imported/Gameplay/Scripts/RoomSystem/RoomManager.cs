using DarkRift.Server;
using FYP.Server.Player;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.RoomManagement
{
    public class RoomManager : MonoBehaviour
    {
        [Tooltip("The default room people will be logged into. Should be global or should allow for random join.")]
        [SerializeField]
        private RoomTemplate _defaultRoomTemplate = null;
        public RoomTemplate defaultRoomTemplate => _defaultRoomTemplate;

        [SerializeField]
        private TemplateList templateList = null;

        [ReorderableList]
        [SerializeField]
        private List<RoomTemplate> persistentInstances = new List<RoomTemplate>();
        public static RoomManager instance = null;

        private Dictionary<ushort, Dictionary<uint, Room>> roomInstances = new Dictionary<ushort, Dictionary<uint, Room>>();
        private bool defaultRoomCreated;

        public uint defaultRoomID { get; private set; } = 0;

        private uint roomIDCounter = 1;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
                return;
            }

            InitializeDeafultInstances();
        }

        private uint GetNextRoomID() 
        {
            var old = roomIDCounter;
            roomIDCounter++;
            if(roomIDCounter == uint.MaxValue) 
            {
                roomIDCounter = 1;
            }
            return old;
        }

        private void InitializeDeafultInstances()
        {
            foreach (var template in persistentInstances)
            {
                if (template == _defaultRoomTemplate)
                {
                    CreateDefaultRoom();
                }
                else if (!CreateRoom(template,GetNextRoomID()))
                {
                    Debug.LogError($"Error creating room for template {template.name}");
                }
            }
            if (!defaultRoomCreated)
            {
                CreateDefaultRoom();
            }
        }

        private void CreateDefaultRoom()
        {
            if (!CreateRoom(_defaultRoomTemplate, defaultRoomID))
            {
                Debug.LogError($"Error creating default room");
            }
            else
            {
                defaultRoomCreated = true;
            }
        }

        public RoomTemplate GetTemplate(ushort tempID) 
        {
            return templateList.GetTemplateID(tempID);
        }

        public bool CreateRoom(RoomTemplate template) 
        {
            return CreateRoom(template, GetNextRoomID());
        }

        private bool CreateRoom(RoomTemplate template,uint id)
        {
            var room = template.CreateRoom(id);
            if (!roomInstances.ContainsKey(template.templateID)) 
            {
                roomInstances.Add(template.templateID, new Dictionary<uint, Room>());
            }
            if (roomInstances[template.templateID].ContainsKey(room.roomID)) 
            {
                room.Close();
                Debug.LogError("Duplicate instance ID");
                return false;
            }
            roomInstances[template.templateID][room.roomID] = room;
            return true;
        }

        public Room EnterRoom(ServerPlayer player,ushort templateID,uint roomID) 
        {
            var template = GetTemplate(templateID);
            var res = AddPlayerToRoom(player,template, roomID);
            if(res == null) 
            {
                res = AddPlayerToRoom(player, template.fallbackRoom, 0);
                if(res == null) 
                {
                    res = AddPlayerToRoom(player, defaultRoomTemplate, 0);
                }
            }
            return res;
        }

        public bool ChangeRoom(ServerPlayer player,ushort templateID,uint roomID) 
        {
            var targetRoom = GetRoom(templateID, roomID);
            if(targetRoom == null || !targetRoom.canEnter) 
            {
                return false;
            }
            player.playerTransform.room.RemovePlayer(player);
            
            return targetRoom.AddPlayer(player);
        }

        private Room AddPlayerToRoom(ServerPlayer player,RoomTemplate room,uint roomID) 
        {
            if(room != null) 
            {
                var instRoom = GetRoom(room.templateID, roomID);
                if (instRoom != null && instRoom.AddPlayer(player)) 
                {
                    return instRoom;
                }
            }
            return null;
        }


        public Room GetRoom(ushort templateID,uint roomID) 
        {
            if (roomInstances.ContainsKey(templateID)) 
            {
                if (roomInstances[templateID].ContainsKey(roomID)) 
                {
                    return roomInstances[templateID][roomID];
                }
            }
            return null;
        }
    }
}