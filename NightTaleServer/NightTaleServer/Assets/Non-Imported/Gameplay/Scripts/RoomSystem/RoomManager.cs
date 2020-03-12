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

        public Room defaultRoom { get; private set; } = null;

        [SerializeField]
        private TemplateList templateList = null;

        [ReorderableList]
        [SerializeField]
        private List<RoomTemplate> persistentInstances = new List<RoomTemplate>();
        public static RoomManager instance = null;
        private bool defaultRoomCreated;

        public uint defaultRoomID { get; private set; } = 0;

        public Dictionary<ushort, Dictionary<uint, Room>> RoomInstancesByTemplateID { get; } = new Dictionary<ushort, Dictionary<uint, Room>>();
        public Dictionary<uint, Room> RoomInstances { get; } = new Dictionary<uint, Room>();
        public Dictionary<RoomTemplate, uint> persistentRoomIDs { get; } = new Dictionary<RoomTemplate, uint>();
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
            foreach (var room in RoomInstances)
            {
                persistentRoomIDs[room.Value.roomTemplate] = room.Key;
            }
        }

        private void CreateDefaultRoom()
        {
            var room = CreateRoom(_defaultRoomTemplate, defaultRoomID);
            if (room == null)
            {
                Debug.LogError($"Error creating default room");
            }
            else
            {
                defaultRoom = room;
                defaultRoomCreated = true;
            }
        }

        public RoomTemplate GetTemplate(ushort tempID) 
        {
            return templateList.GetTemplateID(tempID);
        }

        public Room CreateRoom(RoomTemplate template) 
        {
            return CreateRoom(template, GetNextRoomID());
        }

        private Room CreateRoom(RoomTemplate template,uint id)
        {
            var room = template.CreateRoom(id);
            if (!RoomInstancesByTemplateID.ContainsKey(template.templateID)) 
            {
                RoomInstancesByTemplateID.Add(template.templateID, new Dictionary<uint, Room>());
            }
            if (RoomInstancesByTemplateID[template.templateID].ContainsKey(room.instanceID)) 
            {
                room.Close();
                Debug.LogError("Duplicate instance ID");
                return null;
            }
            RoomInstancesByTemplateID[template.templateID][room.instanceID] = room;
            RoomInstances[room.instanceID] = room;
            return room;
        }

        public Room EnterRoom(PlayerEntity player,ushort templateID,uint roomID) 
        {
            var template = GetTemplate(templateID);
            var res = AddPlayerToRoom(player,template, roomID);
            if(res == null) 
            {
                res = AddPlayerToRoom(player, template.fallbackRoom, persistentRoomIDs[template]);
                if(res == null) 
                {
                    res = AddPlayerToRoom(player, defaultRoomTemplate, defaultRoomID);
                }
            }

            return res;
        }

        public Room EnterDefaultRoom(PlayerEntity player) 
        {
            return EnterRoom(player, defaultRoomTemplate.templateID, defaultRoomID);
        }

        public bool ChangeRoom(PlayerEntity player,ushort templateID,uint roomID) 
        {
            var targetRoom = GetRoom(templateID, roomID);
            if(targetRoom == null || !targetRoom.canEnter) 
            {
                return false;
            }
            player.room.RemovePlayer(player);
            
            return targetRoom.AddPlayer(player);
        }

        public void LeaveRoom(PlayerEntity player,Room room)
        {
            player.room.RemovePlayer(player);
        }

        private Room AddPlayerToRoom(PlayerEntity player,RoomTemplate room,uint roomID) 
        {
            if(room != null) 
            {
                var roomInst = GetRoom(room.templateID, roomID);
                if (roomInst != null && roomInst.AddPlayer(player))
                {
                    return roomInst;
                }
            }
            return null;
        }


        public Room GetRoom(ushort templateID,uint roomID) 
        {
            if (RoomInstancesByTemplateID.ContainsKey(templateID)) 
            {
                if (RoomInstancesByTemplateID[templateID].ContainsKey(roomID)) 
                {
                    return RoomInstancesByTemplateID[templateID][roomID];
                }
            }
            return null;
        }
        public Room GetRoom(uint roomID) 
        {
            if(RoomInstances.TryGetValue(roomID,out var room)) 
            {
                return room;
            }
            return null;
        }

        public void DeleteRoom(uint roomID) 
        {
            if(RoomInstances.TryGetValue(roomID,out var room)) 
            {
                if (RoomInstancesByTemplateID.TryGetValue(room.roomTemplate.templateID,out var tempDict)) 
                {
                    tempDict.Remove(roomID);
                }
                RoomInstances.Remove(roomID);
                room.Close();
            }
        }
    }
}