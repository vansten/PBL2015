using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay.Weapons
{
    public class Newspaper : Weapon
    {
        public Newspaper(GameObject obj) : base(obj)
        {
            Durability = 20;
            Damage = 7;
            Type = WeaponType.LIGHT;
            IsCraftable = false;
            Name = "Newspaper";

            ParticleTexturePaths = new string[] 
            {
                "Textures/Particles/Particle_wood01"
            };
            DestroyCueName = "stoneHit";
        }
    }
}
