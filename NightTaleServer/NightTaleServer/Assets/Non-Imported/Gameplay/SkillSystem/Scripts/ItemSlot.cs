using DarkRift;
using FYP.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    public abstract class ItemSlot : MonoBehaviour,IServerWritable
    {
        [SerializeField]
        protected ServerNetworkEntity entity = null;
        public EquipSlot slot;

        protected EquipMessage syncMessage;
        
        public Item equippedItem { get; set; } = null;

        private Item lastSentItem = null;

        private bool itemDirty = false;
        protected virtual bool isDirty 
        {
            get => itemDirty;
        }

        private bool listenerRegistered = false;

        protected virtual void Awake()
        {
            syncMessage.slotAndID.equipSlot = slot;
        }

        protected void RegisterListener()
        {
            if (!listenerRegistered) 
            {
                listenerRegistered = true;
                entity.reliableOutputWriter.RegisterOutputHandler(this);
            }
        }
        protected void UnregisterListener()
        {
            if (!isDirty) 
            {
                entity.reliableOutputWriter.UnregisterOutputHandler(this);
                listenerRegistered = false;
            }
        }

        public virtual void EquipItem(Item item) 
        {
            if((item.equippableSlots|slot) == item.equippableSlots) 
            {
                if (equippedItem != item) 
                {
                    equippedItem = item;
                    if (item != null) 
                    {
                        syncMessage.slotAndID.itemID = item.itemData.id;
                    }
                    else 
                    {
                        syncMessage.slotAndID.itemID = ItemData.NO_ITEM_ID;
                    }
                    if (lastSentItem != equippedItem) 
                    {
                        itemDirty = true;
                        RegisterListener();
                    }
                }
            }
        }

        public virtual void ResetUpdateData()
        {
            lastSentItem = equippedItem;
            itemDirty = false;
            UnregisterListener();
        }

        public virtual void WriteStateDataToWriter(DarkRiftWriter writer)
        {
            writer.Write(new ServerUpdateTag(ServerUpdateTags.ItemChange));
            writer.Write(syncMessage);
        }

        public virtual void WriteUpdateDataToWriter(DarkRiftWriter writer)
        {
            if (lastSentItem != equippedItem)
            {
                writer.Write(new ServerUpdateTag(ServerUpdateTags.ItemChange));
                writer.Write(syncMessage);
            }
        }
    }
}