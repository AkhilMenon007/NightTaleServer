using DarkRift;
using FYP.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.RoomManagement
{
    [RequireComponent(typeof(Room))]
    public class RoomStateUpdater : MonoBehaviour
    {
        private Room room = null;
        private void Awake()
        {
            room = GetComponent<Room>();
        }
        private void FixedUpdate()
        {
            foreach (var lor in room.GetAllLORs())
            {
                using (var writer = DarkRiftWriter.Create())
                {
                    foreach (var adjLor in lor.adjacentLoRs)
                    {
                        foreach (var item in adjLor.objects)
                        {
                            item.outputWriter.WriteUpdateDataToWriter(writer);
                        }
                    }
                    using (var message = Message.Create((ushort)ServerTags.UpdateData, writer))
                    {
                        lor.SendMessageToClientsInLOR(message, SendMode.Unreliable);
                    }
                }
            }
            foreach (var item in room.networkEntities.Values)
            {
                item.outputWriter.ResetUpdateData();
            }
        }
    }
}