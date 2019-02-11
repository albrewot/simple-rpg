using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class LivingCreature
    {
        public int CurrentHitPoints { get; set; }
        public int MaximumHitPoints { get; set; }

        //Constructor
        public LivingCreature(int current_hp, int max_hp)
        {
            CurrentHitPoints = current_hp;
            MaximumHitPoints = max_hp;
        }
    }
}
