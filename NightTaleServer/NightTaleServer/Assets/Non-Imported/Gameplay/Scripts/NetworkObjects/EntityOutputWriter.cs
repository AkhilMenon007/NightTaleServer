using DarkRift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server
{
    public class EntityOutputWriter : MonoBehaviour
    {
        public ServerNetworkEntity entity { get; private set; } = null;
        private readonly HashSet<IServerWritable> writables = new HashSet<IServerWritable>();

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
                writer.Write(new ServerUpdateData() { entityID = entity.entityID, dataCount = (ushort)writables.Count });
                foreach (var writable in writables)
                {
                    writable.WriteUpdateDataToWriter(writer);
                }
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
            foreach (var writable in writables)
            {
                writable.ResetUpdateData();
            }
        }

        public void UnregisterOutputHandler(IServerWritable outputHandler)
        {
            writables.Remove(outputHandler);
        }
    }
}