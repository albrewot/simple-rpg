using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Weapon : Item
    {
        public int MinimumDamage { get; set; }
        public int MaximumDamage { get; set; }

        //Constructor
        public Weapon(int id, string name, string name_plural, int min_dmg, int max_dmg) : base(id, name, name_plural)
        {
            MinimumDamage = min_dmg;
            MaximumDamage = max_dmg;
        }
    }
}
