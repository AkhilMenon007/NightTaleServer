using FYP.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    public class ArmorSlot : ItemSlot
    {
        public ServerArmor equippedArmor => equippedItem as ServerArmor;

    }
}