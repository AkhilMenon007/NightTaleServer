using DarkRift.Server;
using FYP.Server.Player;
using FYP.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server
{
    [RequireComponent(typeof(PlayerEntity))]
    public class PlayerInputController : MonoBehaviour
    {
        public PlayerEntity player { get; private set; }

        private readonly Dictionary<ushort, IServerReadable> inputHandlerLookup = new Dictionary<ushort, IServerReadable>();

        public T GetMessageHandler<T>(ushort tag) where T:MonoBehaviour,IServerReadable
        {
            if (inputHandlerLookup.ContainsKey(tag)) 
            {
                return inputHandlerLookup[tag] as T;
            }
            return null;
        }

        private void Awake()
        {
            player = GetComponent<PlayerEntity>();
            player.OnOwnerAssigned += OwnerAssignedCallback;
            player.OnOwnerRemoved += OwnerRemovedCallback;
        }

        private void OwnerAssignedCallback()
        {
            player.owner.client.MessageReceived += HandleClientInput;
        }

        private void HandleClientInput(object sender, MessageReceivedEventArgs e)
        {
            if (e.Tag == (ushort)ClientTags.ClientUpdate) 
            {
                using(var message = e.GetMessage()) 
                {
                    using(var reader = message.GetReader()) 
                    {
                        while (reader.Position <= reader.Length) 
                        {
                            var updateData = reader.ReadSerializable<UpdateData>();
                            if(inputHandlerLookup.TryGetValue(updateData.tag,out var handler)) 
                            {
                                handler.HandlePlayerInputFromReader(reader);
                            }
                            else 
                            {
                                Debug.LogError($"No Handler registered for tag {updateData.tag} on Player {player.player.client.ID}");
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void RegisterInputHandler(IServerReadable inputHandler,ushort tag) 
        {
            if (inputHandler != null) 
            {
                inputHandlerLookup[tag] = inputHandler;
            }
        }

        public void UnregisterInputHandler(IServerReadable inputHandler,ushort tag) 
        {
            if(inputHandlerLookup.TryGetValue(tag,out var handler)) 
            {
                if(handler == inputHandler) 
                {
                    inputHandlerLookup.Remove(tag);
                }
            }
        }

        private void OwnerRemovedCallback(ServerPlayer obj)
        {
            obj.client.MessageReceived -= HandleClientInput;
        }
    }
}