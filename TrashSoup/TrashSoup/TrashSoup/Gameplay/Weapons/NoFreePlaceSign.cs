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
