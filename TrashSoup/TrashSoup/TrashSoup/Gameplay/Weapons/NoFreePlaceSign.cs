using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Weapons
{
    public class NoFreePlaceSign : Weapon
    {
        public NoFreePlaceSign(GameObject obj):base(obj)
        {
            Durability = 40;
            Damage = 35;
            Type = WeaponType.HEAVY;
            IsCraftable = true;
            CraftingCost = 150;
            Name = "NoFreePlace Sign";
        }
    }
}
