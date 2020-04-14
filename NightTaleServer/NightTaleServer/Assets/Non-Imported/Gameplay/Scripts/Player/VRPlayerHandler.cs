using DarkRift;
using FYP.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    public class VRPlayerHandler : MonoBehaviour, IServerReadable, IServerWritable
    {
        [SerializeField]
        private Transform head = null;
        [SerializeField]
        private Transform left = null;
        [SerializeField]
        private Transform right = null;

        [SerializeField]
        private PlayerControllableEntity controllableEntity = null;
        //[SerializeField]
        //private ServerNetworkEntity networkEntity = null;

        //private bool _isDirty = false;
        //public bool isDirty
        //{
        //    get => _isDirty;
        //    private set
        //    {
        //        if (_isDirty != value)
        //        {
        //            if (value)
        //            {
        //                networkEntity.outputWriter.OnReset += ResetUpdateData;
        //                networkEntity.outputWriter.RegisterOutputHandler(this);
        //            }
        //            else
        //            {
        //                networkEntity.outputWriter.UnregisterOutputHandler(this);
        //                networkEntity.outputWriter.OnReset -= ResetUpdateData;
        //            }
        //            _isDirty = value;
        //        }
        //    }
        //}
       
        private void OnEnable()
        {
            controllableEntity.RegisterReadable(this, ClientDataTags.VRTransform);
            head.gameObject.SetActive(true);
            left.gameObject.SetActive(true);
            right.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            controllableEntity.UnregisterReadable(ClientDataTags.VRTransform);
            head.gameObject.SetActive(false);
            left.gameObject.SetActive(false);
            right.gameObject.SetActive(false);
        }

        public void HandlePlayerInputFromReader(DarkRiftReader reader, ClientDataTags tag)
        {
            var data = reader.ReadSerializable<VRTransformData>();
            head.transform.localPosition = data.vrTransforms[0].position;
            left.transform.localPosition = data.vrTransforms[1].position;
            right.transform.localPosition = data.vrTransforms[2].position;

            head.transform.localRotation = data.vrTransforms[0].rotation;
            left.transform.localRotation = data.vrTransforms[1].rotation;
            right.transform.localRotation = data.vrTransforms[2].rotation;
        }

        public void ResetUpdateData()
        {
            //Update Synchronizes data no need to send
        }

        public void WriteStateDataToWriter(DarkRiftWriter writer)
        {
            var transforms = new TransformData[3];
            transforms[0].position = head.localPosition;
            transforms[1].position = left.localPosition;
            transforms[2].position = right.localPosition;

            transforms[0].rotation = head.localRotation;
            transforms[1].rotation = left.localRotation;
            transforms[2].rotation = right.localRotation;
            writer.Write(new ServerUpdateTag(ServerUpdateTags.VRData));
            writer.Write(new VRTransformData { vrTransforms = transforms });
        }

        public void WriteUpdateDataToWriter(DarkRiftWriter writer)
        {
            WriteStateDataToWriter(writer);
        }
    }
}