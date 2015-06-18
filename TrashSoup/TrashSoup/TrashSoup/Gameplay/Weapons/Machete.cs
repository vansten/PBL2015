using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Weapons
{
    public class Machete : Weapon
    {
        public Machete(GameObject obj) : base(obj)
        {
            Durability = 50;
            Damage = 25;
            OffsetPosition = new Vector3(0.04f, 0, 0.03f);
            OffsetRotation = new Vector3(0, -60, 0);
            Type = WeaponType.MEDIUM;
            IsCraftable = true;
            CraftingCost = 100;
            Name = "Machete";
        }
    }
}
