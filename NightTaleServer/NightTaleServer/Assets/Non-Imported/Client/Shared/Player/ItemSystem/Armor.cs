using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Shared
{
    public class Armor : Item
    {
        [SerializeField]
        private EquipSlot equipSlot = EquipSlot.Chest;

        public override EquipSlot equippableSlots => equipSlot;
    }
}