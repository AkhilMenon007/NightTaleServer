using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace FYP.Shared
{
    /// <summary>
    /// This is the class of all the items which can Perform actions based on user Input
    /// </summary>
    public class Weapon : Item
    {
        public WeaponData weaponData => itemData as WeaponData;
        public override EquipSlot equippableSlots { get => EquipSlot.PrimaryHand | EquipSlot.SecondaryHand; }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (itemData != null && !(itemData is WeaponData))
            {
                itemData = null;
            }
            base.OnValidate();
        }
#endif
    }
}