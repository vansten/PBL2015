using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Weapons
{
    public class Hammer : Weapon
    {
        public Hammer(GameObject obj) : base(obj)
        {
            Durability = 35;
            Damage = 18;
            OffsetPosition = new Vector3(0.1f, 0, 0.05f);
            OffsetRotation = new Vector3(0, 0, -30);
            Type = WeaponType.MEDIUM;
            IsCraftable = true;
            CraftingCost = 50;
            Name = "Hammer";
        }
    }
}
