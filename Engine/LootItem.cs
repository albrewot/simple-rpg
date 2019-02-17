using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class LootItem
    {
        public Item Details { get; set; }
        public int DropPercentage { get; set; }
        public bool isDefaultItem { get; set; }

        //Constructor
        public LootItem(Item details, int drop_perctentage, bool default_item)
        {
            Details = details;
            DropPercentage = drop_perctentage;
            isDefaultItem = default_item;
        }
    }
}
