using DarkRift;
using FYP.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    public abstract class PlayerInputHandler : MonoBehaviour,IServerReadable
    {
        [SerializeField]
        private PlayerInputController playerInputController = null;
        public abstract void HandlePlayerInputFromReader(DarkRiftReader reader,ClientDataTags tag);

        protected void RegisterHandlerForTag(ClientDataTags tag) 
        {
            playerInputController.RegisterInputHandler(this, tag);
        }
        protected void UnregisterHandlerForTag(ClientDataTags tag) 
        {
            playerInputController.UnregisterInputHandler(this, tag);
        }
    }
}