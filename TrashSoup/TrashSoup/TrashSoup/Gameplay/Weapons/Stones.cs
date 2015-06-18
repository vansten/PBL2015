using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Weapons
{
    public class Stones : Weapon
    {
        public Stones(GameObject obj):base(obj)
        {
            Durability = 10;
            Damage = 10;
            Type = WeaponType.LIGHT;
            IsCraftable = true;
            CraftingCost = 15;
            Name = "Stone";

            ParticleTexturePaths = new string[] 
            {
                "Textures/Particles/Particle_stone01"
            };
            DestroyCueName = "stoneHit";
        }
    }
}
