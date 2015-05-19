using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Weapons
{
    public class Machete : Weapon
    {
        public Machete(GameObject obj) : base(obj)
        {
            Durability = 50;
            Damage = 25;
            Type = WeaponType.MEDIUM;
            IsCraftable = true;
            CraftingCost = 100;
            Name = "Machete";
        }
    }
}
