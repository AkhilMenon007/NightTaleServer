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
        public readonly HashSet<ServerNetworkEntity> objects = new HashSet<ServerNetworkEntity>();
        public readonly List<LocalityOfRelevance> adjacentLoRs = new List<LocalityOfRelevance>();

        public readonly Bounds bounds;

        public readonly int[] index;
        public readonly Room room;

        public HashSet<IClient> GetPlayers() 
        {
            return players;
        }

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
        public void AddPlayer(PlayerEntity player) 
        {
            players.Add(player.player.client);
            AddObject(player);
        }
        public void RemovePlayer(PlayerEntity player) 
        {
            RemoveObject(player);
            players.Remove(player.player.client);
        }
        public void AddObject(ServerNetworkEntity obj)
        {
            objects.Add(obj);
            obj.lor = this;
        }
        public void RemoveObject(ServerNetworkEntity obj) 
        {
            objects.Remove(obj);
            if(obj.lor == this) 
            {
                obj.lor = null;
            }
        }
        private void TransferPlayer(PlayerEntity player,LocalityOfRelevance target) 
        {
            target?.AddPlayer(player);
            RemovePlayer(player);
        }
        public void TransferObject(ServerNetworkEntity obj,LocalityOfRelevance target) 
        {
            if(obj is PlayerEntity player) 
            {
                TransferPlayer(player, target);
            }
            else 
            {
                target?.AddObject(obj);
                RemoveObject(obj);
            }
        }

        public void SendMessageToClientsInLOR(Message message,SendMode sendMode) 
        {
            foreach (var playerClient in players)
            {
                playerClient.SendMessage(message, sendMode);
            }
        }
    }
}