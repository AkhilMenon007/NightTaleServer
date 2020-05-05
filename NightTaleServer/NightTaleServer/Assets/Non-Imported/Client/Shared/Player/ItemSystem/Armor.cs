using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Shared
{
    public class Armor : Item
    {
        [SerializeField]
        private EquipSlot slot;

        public override EquipSlot equippableSlots => slot;
    }
}