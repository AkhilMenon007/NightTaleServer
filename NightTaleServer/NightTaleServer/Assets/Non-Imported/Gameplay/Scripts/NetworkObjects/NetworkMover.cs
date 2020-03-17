using DarkRift;
using FYP.Server.Player;
using FYP.Shared;
using System;
using UnityEngine;

namespace FYP.Server
{
    [RequireComponent(typeof(ServerNetworkEntity))]
    public class NetworkMover : MonoBehaviour, IServerWritable
    {
        [Tooltip("The minimum distance to be moved for server to send data to clients")]
        [SerializeField]
        private float positionalThreshold = 0.02f;

        [Tooltip("The minimum rotation angle for server to send data to clients")]
        [SerializeField]
        private float rotationalThreshold = 2f;


        private ServerNetworkEntity networkEntity = null;


        private bool _isDirty = false;
        public bool isDirty 
        { 
            get => _isDirty;
            private set 
            {
                if(_isDirty != value) 
                {
                    if (value)
                    {
                        networkEntity.outputWriter.OnReset += ResetUpdateData;
                        networkEntity.outputWriter.RegisterOutputHandler(this);
                    }
                    else
                    {
                        networkEntity.outputWriter.UnregisterOutputHandler(this);
                        networkEntity.outputWriter.OnReset -= ResetUpdateData;
                    }
                    _isDirty = value;
                }
            } 
        }

        private Vector3 lastSentPos;
        private Quaternion lastSentRot;


        private void Awake()
        {
            networkEntity = GetComponent<ServerNetworkEntity>();
        }

        //Input Listener
        public void WriteUpdateDataToWriter(DarkRiftWriter writer)
        {
            writer.Write(new ServerUpdateTag(ServerUpdateTags.PositionalData));
            writer.Write(new TransformData() {position = networkEntity.position,rotation = networkEntity.rotation });
        }


        private void FixedUpdate()
        {
            var dPos = (networkEntity.position - lastSentPos).magnitude;
            var dRot = Quaternion.Angle(lastSentRot, networkEntity.rotation);
            if(dPos > positionalThreshold || dRot > rotationalThreshold) 
            {
                isDirty = true;
            }
        }

        public void WriteStateDataToWriter(DarkRiftWriter writer)
        {
            //Update synchronizes state no need to write again
        }

        public void ResetUpdateData()
        {
            lastSentPos = networkEntity.position;
            lastSentRot = networkEntity.rotation;
            isDirty = false;
        }

        public void OnDestroy()
        {
            networkEntity.outputWriter.UnregisterOutputHandler(this);
        }
    }
}