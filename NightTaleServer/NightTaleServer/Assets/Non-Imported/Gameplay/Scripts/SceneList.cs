using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FYP.Server
{
    [System.Serializable]
    public class SharedScene 
    {
        public int serverIndex = 0;
        public int clientIndex = 0;
    }
    [CreateAssetMenu(menuName = "FYP/SceneList")]
    public class SceneList : ScriptableObject
    {
        [ReorderableList]
        [SerializeField]
        private List<SharedScene> scenes = new List<SharedScene>();

        public int GetClientIndex(int serverIndex) 
        {
            var res = scenes.Find(x => x.serverIndex == serverIndex);
            if (res != null) 
            {
                return res.clientIndex;
            }
            return -1;
        }

    }
}