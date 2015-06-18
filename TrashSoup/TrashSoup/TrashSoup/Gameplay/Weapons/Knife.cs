using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Weapons
{
    public class Knife : Weapon
    {
        public Knife(GameObject obj):base(obj)
        {
            Durability = 35;
            Damage = 13;
            Type = WeaponType.LIGHT;
            IsCraftable = true;
            CraftingCost = 50;
            Name = "Knife";

            ParticleTexturePaths = new string[] 
            {
                "Textures/Particles/Particle_metal01",
                "Textures/Particles/Particle_metal02"
            };
            DestroyCueName = "metalHit";
        }
    }
}
