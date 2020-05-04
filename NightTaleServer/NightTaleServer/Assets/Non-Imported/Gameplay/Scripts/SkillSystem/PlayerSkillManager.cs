using DarkRift.Server;
using FYP.Server.RoomManagement;
using FYP.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    public class PlayerSkillManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerEntity playerEntity = null;
        private ServerPlayer serverPlayer = null;

        private void Awake()
        {
            serverPlayer = playerEntity.player;
            playerEntity.OnEnteredRoom += AddListeners;
            playerEntity.OnLeftRoom += RemoveListeners;
        }

        private void AddListeners(Room obj)
        {
            serverPlayer.client.MessageReceived += HandleSkillMessage;
        }

        private void HandleSkillMessage(object sender, MessageReceivedEventArgs e)
        {
            switch ((ClientTags)e.Tag) 
            {
                case ClientTags.ChangeWeapon:
                    {
                        break;
                    }
                case ClientTags.ChangeSkill:
                    {
                        break;
                    }
            }
        }

        private void RemoveListeners(Room obj)
        {
            serverPlayer.client.MessageReceived -= HandleSkillMessage;
        }

        private void OnDestroy()
        {
            playerEntity.OnEnteredRoom -= AddListeners;
            playerEntity.OnLeftRoom -= RemoveListeners;
        }
    }
}