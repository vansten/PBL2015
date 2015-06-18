﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Weapons
{
    public class Pipe : Weapon
    {
        public Pipe(GameObject obj) : base(obj)
        {
            Durability = 30;
            Damage = 20;
            OffsetPosition = new Vector3(0.1f, 0, 0);
            OffsetRotation = new Vector3(0, -10, 0);
            Type = WeaponType.MEDIUM;
            IsCraftable = true;
            CraftingCost = 40;
            Name = "Pipe";
        }
    }
}
