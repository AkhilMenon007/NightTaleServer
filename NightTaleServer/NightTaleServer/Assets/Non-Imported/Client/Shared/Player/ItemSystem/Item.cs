using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FYP.Shared {
    public class Item : ScriptableObject
    {
        public ItemData itemData = null;
        public virtual EquipSlot equippableSlots { get; } = 0;
        public bool isEquippable => equippableSlots != 0;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(ItemLookup).FullName);
            if (guids.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var lookup = AssetDatabase.LoadAssetAtPath<ItemLookup>(path);
                if (itemData != null)
                {
                    var oldItem = itemData;
                    itemData = null;
                    var currItem = lookup.GetItem(oldItem.id);
                    //Value in lookup is set to this and hasnt changed
                    if (currItem != this)
                    {
                        if (currItem != null)
                        {
                            Debug.LogError($"Another item already uses the item data with name {currItem.name}", currItem);
                            itemData = null;
                        }
                        else
                        {
                            itemData = oldItem;
                        }
                    }
                    else
                    {
                        itemData = oldItem;
                    }
                    lookup.RefreshItemList();
                }
            }
            else if (guids.Length == 0)
            {
                Debug.LogError("ItemLookup not set");
            }
            else
            {
                Debug.LogError("Multiple ItemLookups found!!");
                foreach (var item in guids)
                {
                    Debug.Log(item);
                }
            }
        }
#endif
    }
}