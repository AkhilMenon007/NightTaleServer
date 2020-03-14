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
        public void RegisterInputHandler(IServerWritable outputHandler)
        {
            writables.Add(outputHandler);
        }

        public void WriteDataToWriter(DarkRiftWriter writer) 
        {
            foreach (var writable in writables)
            {
                writable.WriteUpdateDataToWriter(writer);
            }
        }

        public void UnregisterInputHandler(IServerWritable outputHandler)
        {
            writables.Remove(outputHandler);
        }
    }
}