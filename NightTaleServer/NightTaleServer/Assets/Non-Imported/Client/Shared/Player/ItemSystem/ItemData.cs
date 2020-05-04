using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FYP.Shared
{
    [CreateAssetMenu(menuName = "FYP/GameData/Item")]
    public class ItemData : ScriptableObject
    {
        [NaughtyAttributes.ReadOnly]
        public ushort id = 0;
        public const ushort RESERVED_ITEM_COUNT = 1;
        /// <summary>
        /// Used to get the index in the lookup.Do not use this for sending data to server.Use ID instead.
        /// </summary>
        public int GetIndex()
        {
            return id - RESERVED_ITEM_COUNT;
        }
#if UNITY_EDITOR
        [ContextMenu("Reset IDs")]
        public void ResetID()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(ItemData).Name);  //FindAssets uses tags check documentation for more info
            ItemData[] a = new ItemData[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            }

            for (ushort i = 0; i < a.Length; i++)
            {
                a[i].id =(ushort)( i + RESERVED_ITEM_COUNT);
            }
        }
#endif
    }
}