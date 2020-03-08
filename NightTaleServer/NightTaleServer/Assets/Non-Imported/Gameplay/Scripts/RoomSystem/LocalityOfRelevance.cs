using FYP.Server.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FYP.Server.RoomManagement
{
    public class LocalityOfRelevance
    {
        private readonly HashSet<NetworkTransform> players = new HashSet<NetworkTransform>();
        public readonly List<LocalityOfRelevance> adjacentLoRs = new List<LocalityOfRelevance>();

        public readonly Bounds bounds;

        public LocalityOfRelevance(Bounds bounds)
        {
            this.bounds = bounds;
        }

        public bool IsInLOR(Vector3 position) 
        {
            return bounds.Contains(position);
        }
        public IEnumerable<NetworkTransform> GetPlayers() 
        {
            return players;
        }
        public bool AddPlayer(NetworkTransform player) 
        {
            var res = players.Add(player);
            player.lor = this;
            player.OnLOREntered?.Invoke(this);
            return res;
        }
        public bool RemoveObject(NetworkTransform player) 
        {
            player.OnLORExited?.Invoke(this);
            player.lor = null;
            return players.Remove(player);
        }
    }
}