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

        private Dictionary<uint, ClientControlledMover> movers = new Dictionary<uint, ClientControlledMover>();
        private void Awake()
        {
            networkEntity.OnOwnerAssigned += AddListeners;
            networkEntity.OnOwnerRemoved += RemoveListener;
        }

        private void AddListeners()
        {
            inputController = networkEntity.owner.inputController;
            inputController.RegisterInputHandler(this, ClientDataTags.MoveObject);
            inputController.RegisterInputHandler(this, ClientDataTags.SetObjectPosition);
        }

        public void UnregisterMover(uint entityID)
        {
            movers.Remove(entityID);
        }

        public void RegisterMover(uint entityID, ClientControlledMover mover) 
        {
            if (mover != null) 
            {
                movers[entityID] = mover;
            }
        }

        public override void HandlePlayerInputFromReader(DarkRiftReader reader,ClientDataTags tag)
        {
            var data = reader.ReadSerializable<EntityID>();
            if(movers.TryGetValue(data.entID,out var mover)) 
            {
                mover.ReadTranslationDataFromReader(reader,tag);
            }
            else 
            {
                Debug.LogError($"{data.entID} is not registered for MovementInputHandler of Player {inputController.player.player.client.ID}");
                reader.Position = reader.Length;
            }
        }



        private void RemoveListener(ServerPlayer obj)
        {
            inputController.UnregisterInputHandler(this,ClientDataTags.MoveObject);
            inputController.UnregisterInputHandler(this, ClientDataTags.SetObjectPosition);
            inputController = null;
        }


        private void OnDestroy()
        {
            networkEntity.OnOwnerAssigned -= AddListeners;
            networkEntity.OnOwnerRemoved += RemoveListener;
        }
    }
}