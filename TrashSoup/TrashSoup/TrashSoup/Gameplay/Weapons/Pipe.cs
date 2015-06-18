using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Weapons
{
    public class Pipe : Weapon
    {
        public Pipe(GameObject obj) : base(obj)
        {
            Durability = 30;
            Damage = 20;
            Type = WeaponType.MEDIUM;
            IsCraftable = true;
            CraftingCost = 40;
            Name = "Pipe";

            ParticleTexturePaths = new string[] 
            {
                "Textures/Particles/Particle_metal01",
                "Textures/Particles/Particle_metal02",
            };
            DestroyCueName = "metalHit";
        }
    }
}
