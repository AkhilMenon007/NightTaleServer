using DarkRift;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server
{
    public class EntityOutputWriter : MonoBehaviour
    {
        public ServerNetworkEntity entity { get; private set; } = null;
        private readonly HashSet<IServerWritable> writables = new HashSet<IServerWritable>();

        public Action OnReset { get; set; }
        private void Awake()
        {
            entity = GetComponent<ServerNetworkEntity>();
        }
        public void RegisterOutputHandler(IServerWritable outputHandler)
        {
            writables.Add(outputHandler);
        }

        public void WriteUpdateDataToWriter(DarkRiftWriter writer) 
        {
            if (writables.Count >0) 
            {
                //writer.Write(new ServerUpdateData() { entityID = entity.entityID, dataCount = (ushort)writables.Count });
                writer.Write(entity.entityID);
                var pos = writer.Reserve(sizeof(ushort));

                foreach (var writable in writables)
                {
                    writable.WriteUpdateDataToWriter(writer);
                }
                var curPos = writer.Position;
                writer.Position = pos;
                writer.Write((ushort)curPos);
                writer.Position = curPos;
            }
        }
        public void WriteStateDataToWriter(DarkRiftWriter writer) 
        {
            foreach (var writable in writables)
            {
                writable.WriteStateDataToWriter(writer);
            }
        }

        public void ResetUpdateData() 
        {
            OnReset?.Invoke();
        }

        public void UnregisterOutputHandler(IServerWritable outputHandler)
        {
            writables.Remove(outputHandler);
        }
    }
}