using DarkRift;
using DarkRift.Server;
using FYP.Server.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FYP.Server.RoomManagement
{
    public class LocalityOfRelevance
    {
        private readonly HashSet<IClient> players = new HashSet<IClient>();
        private readonly HashSet<NetworkTransform> objects = new HashSet<NetworkTransform>();
        public readonly List<LocalityOfRelevance> adjacentLoRs = new List<LocalityOfRelevance>();

        public readonly Bounds bounds;

        public readonly int[] index;
        public readonly Room room;

        public LocalityOfRelevance(Room room,Bounds bounds,int x,int y,int z)
        {
            this.room = room;
            this.bounds = bounds;
            index = new int[3] { x, y, z };
        }

        public bool IsInLOR(Vector3 position) 
        {
            return bounds.Contains(position);
        }
        public IEnumerable<IClient> GetPlayerClients() 
        {
            return players;
        }
        public void AddPlayer(PlayerTransform player) 
        {
            players.Add(player.player.client);
            AddObject(player);
        }
        public void RemovePlayer(PlayerTransform player) 
        {
            RemoveObject(player);
            players.Remove(player.player.client);
        }
        public void AddObject(NetworkTransform obj)
        {
            objects.Add(obj);
            obj.lor = this;
        }
        public void RemoveObject(NetworkTransform obj) 
        {
            objects.Remove(obj);
            if(obj.lor == this) 
            {
                obj.lor = null;
            }
        }
        public void TransferPlayer(PlayerTransform player,LocalityOfRelevance target) 
        {
            target?.AddPlayer(player);
            RemovePlayer(player);
        }
        public void TransferObject(NetworkTransform obj,LocalityOfRelevance target) 
        {
            if(obj is PlayerTransform player) 
            {
                TransferPlayer(player, target);
                return;
            }
            else 
            {
                target?.AddObject(obj);
                RemoveObject(obj);
            }
        }

        public void SendMessageToVisibleClients(Message message,SendMode sendMode) 
        {
            foreach (var lor in adjacentLoRs)
            {
                foreach (var playerClient in lor.GetPlayerClients())
                {
                    playerClient.SendMessage(message, sendMode);
                }
            }
        }
    }
}