using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class HealingPotion :Item
    {
        public int AmountToHeal { get; set; }

        //Constructor
        public HealingPotion(int id, string name, string name_plural, int amount_to_heal) : base(id, name, name_plural)
        {
            AmountToHeal = amount_to_heal;
        }
    }
}
