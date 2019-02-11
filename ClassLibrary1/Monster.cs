using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Monster : LivingCreature
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MaximumDamage { get; set; }
        public int RewardExperiencePoints { get; set; }
        public int RewardGold { get; set; }
        public List<LootItem> LootTable { get; set; }

        //Constructor
        public Monster(int id, string name, int max_dmg, int reward_exp, int reward_gold, int current_hp, int max_hp) : base(current_hp, max_hp)
        {
            ID = id;
            Name = name;
            MaximumDamage = max_dmg;
            RewardExperiencePoints = reward_exp;
            RewardGold = reward_gold;
            LootTable = new List<LootItem>();
        }
    }
}
