using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Weapons
{
    public class Bottle : Weapon
    {
        public Bottle(GameObject obj):base(obj)
        {
            Durability = 1;
            Damage = 35;
            Type = WeaponType.LIGHT;
            IsCraftable = false;
            Name = "Bottle";
        }
    }
}
