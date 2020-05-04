using FYP.Shared;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FYP.Shared
{
    [CreateAssetMenu(menuName ="FYP/SkillSystem/ItemLookup")]
    public class ItemLookup : ScriptableObject
    {

        [SerializeField]
        [ReorderableList]
        private List<ItemDataItemPair> itemList = new List<ItemDataItemPair>();

        [SerializeField]
        [ReadOnly]
        private int itemDataCount = 0;


        public Item GetItem(uint id)
        {
            int index = (int)id - ItemData.RESERVED_ITEM_COUNT;
            if (index >= 0 && index < itemList.Count)
            {
                return itemList[index].item;
            }
            return null;
        }
        private void OnValidate()
        {
            foreach (var obj in itemList)
            {
                if(obj!=null && obj.item!=null && obj.itemData != null) 
                {
                    obj.item.itemData = obj.itemData;
                }
            }
        }

#if UNITY_EDITOR

        [ContextMenu("RefreshItemList")]
        public void RefreshItemList() 
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(Item).FullName);  
            var itemGuids = AssetDatabase.FindAssets("t:" + typeof(ItemData).FullName);
            itemDataCount = itemGuids.Length;
            itemList = new List<ItemDataItemPair>(itemDataCount);

            for (int i = 0; i < itemDataCount; i++)
            {
                itemList.Add(new ItemDataItemPair());
            }
            foreach (var item in itemGuids)
            {
                var wData = AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(item));
                itemList[wData.GetIndex()].itemData = wData;
            }

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var data = AssetDatabase.LoadAssetAtPath<Item>(path);
                if(data!=null && data.itemData != null) 
                {
                    itemList[data.itemData.GetIndex()].item = data;
                }
            }
        }
#endif


    }
    [Serializable]
    public class ItemDataItemPair 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2235:Mark all non-serializable fields", Justification = "<Pending>")]
        public Item item = null;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2235:Mark all non-serializable fields", Justification = "<Pending>")]
        public ItemData itemData = null;
        public ItemDataItemPair()
        {
            item = null;
            itemData = null;
        }
    }
}