using DarkRift;
using DarkRift.Server;
using FYP.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player 
{
    public class MovementInputHandler : PlayerInputHandler
    {
        [SerializeField]
        private ServerNetworkEntity networkEntity = null;

        private PlayerInputController inputController = null;

        private Dictionary<uint, NetworkMover> movers = new Dictionary<uint, NetworkMover>();
        private void Awake()
        {
            networkEntity.OnOwnerAssigned += AddListeners;
            networkEntity.OnOwnerRemoved += RemoveListener;
        }

        private void AddListeners()
        {
            inputController = networkEntity.owner.inputController;
            inputController.RegisterInputHandler(this, (ushort)ClientDataTags.MoveObject);
        }

        public void UnregisterMover(uint entityID)
        {
            movers.Remove(entityID);
        }

        public void RegisterMover(uint entityID,NetworkMover mover) 
        {
            if (mover != null) 
            {
                movers[entityID] = mover;
            }
        }

        public override void HandlePlayerInputFromReader(DarkRiftReader reader)
        {
            var data = reader.ReadSerializable<MovementMessage>();
            if(movers.TryGetValue(data.entID,out var mover)) 
            {
                mover.ReadDataFromReader(reader);
            }
            else 
            {
                Debug.LogError($"{data.entID} is not registered for MovementInputHandler of Player {inputController.player.player.client.ID}");
                reader.Position = reader.Length;
            }
        }



        private void RemoveListener(ServerPlayer obj)
        {
            inputController.UnregisterInputHandler(this, (ushort)ClientDataTags.MoveObject);
            inputController = null;
        }


        private void OnDestroy()
        {
            networkEntity.OnOwnerAssigned -= AddListeners;
            networkEntity.OnOwnerRemoved += RemoveListener;
        }
    }
}