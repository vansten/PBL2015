using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Weapons
{
    public class Hammer : Weapon
    {
        public Hammer(GameObject obj) : base(obj)
        {
            Durability = 2;
            Damage = 18;
            Type = WeaponType.MEDIUM;
            IsCraftable = true;
            CraftingCost = 50;
            Name = "Hammer";
            ParticleTexturePaths = new string[] 
            {
                "Textures/Particles/Particle_metal01",
                "Textures/Particles/Particle_metal02"
            };
            DestroyCueName = "metalHit";
        }
    }
}
