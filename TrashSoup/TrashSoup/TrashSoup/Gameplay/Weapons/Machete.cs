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

            ParticleTexturePaths = new string[] 
            {
                "Textures/Particles/Particle_metal01",
                "Textures/Particles/Particle_metal02",
                "Textures/Particles/Particle_wood01",
                "Textures/Particles/Particle_wood02",
                "Textures/Particles/Particle_wood03"
            };
            DestroyCueName = "metalHit";
        }
    }
}
