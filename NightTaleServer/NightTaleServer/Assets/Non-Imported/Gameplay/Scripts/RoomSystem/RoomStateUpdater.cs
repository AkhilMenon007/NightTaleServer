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
                using (var relWriter = DarkRiftWriter.Create())
                {
                    using (var writer = DarkRiftWriter.Create())
                    {

                        foreach (var adjLor in lor.adjacentLoRs)
                        {
                            foreach (var item in adjLor.objects)
                            {
                                item.unreliableOutputWriter.WriteUpdateDataToWriter(writer);
                                item.reliableOutputWriter.WriteUpdateDataToWriter(relWriter);
                            }
                        }
                        if (writer.Length != 0)
                        {
                            using (var message = Message.Create((ushort)ServerTags.UpdateData, writer))
                            {
                                foreach (var player in lor.GetPlayerClients())
                                {
                                    player.SendMessage(message, SendMode.Unreliable);
                                }
                            }
                        }
                        if (relWriter.Length != 0) 
                        {
                            using (var message = Message.Create((ushort)ServerTags.ReliableData, relWriter))
                            {
                                foreach (var player in lor.GetPlayerClients())
                                {
                                    player.SendMessage(message, SendMode.Reliable);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var item in room.networkEntities.Values)
            {
                item.unreliableOutputWriter.ResetUpdateData();
                item.reliableOutputWriter.ResetUpdateData();
            }
        }
    }
}