using DarkRift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    public abstract class PlayerInputHandler : MonoBehaviour,IServerReadable
    {
        [SerializeField]
        private PlayerInputController playerInputController = null;
        public abstract void HandlePlayerInputFromReader(DarkRiftReader reader);

        protected void RegisterHandlerForTag(ushort tag) 
        {
            playerInputController.RegisterInputHandler(this, tag);
        }
        protected void UnregisterHandlerForTag(ushort tag) 
        {
            playerInputController.UnregisterInputHandler(this, tag);
        }
    }
}