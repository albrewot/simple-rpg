using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Player : LivingCreature
    {
        public int Gold { get; set; }
        public int ExperiencePoints { get; set; }
        public int Level { get { return ((ExperiencePoints / 100) + 1); } }
        public Location CurrentLocation { get; set; }
        public List<InventoryItem> Inventory { get; set; }
        public List<PlayerQuest> Quests { get; set; }

        //Constructor
        public Player(int current_hp, int max_hp, int gold, int exp) : base(current_hp, max_hp)
        {
            Gold = gold;
            ExperiencePoints = exp;
            Inventory = new List<InventoryItem>();
            Quests = new List<PlayerQuest>();
        }

        //Check if player has item to enter a location
        public bool HasItemToEnterLocation(Location location)
        {
            if(location.ItemRequiredToEnter == null)
            {
                return true;
            }
            //check player inventory for such item
            foreach (InventoryItem inventoryItem in Inventory)
            {
                if (inventoryItem.Details.ID == location.ItemRequiredToEnter.ID)
                {
                    return true;
                }
            }

            return false;
        }

        //Check if Player has a location's quest
        public bool HasQuest(Quest quest)
        {
            foreach(PlayerQuest playerQuest in Quests)
            {
                if(quest.ID == playerQuest.Details.ID)
                {
                    return true;
                }
            }
            return false;
        }

        //check if player completed a quest
        public bool CompletedQuest(Quest quest)
        {
            foreach(PlayerQuest playerQuest in Quests)
            {
                if(quest.ID == playerQuest.Details.ID)
                {
                    return playerQuest.IsCompleted;
                }
            }

            return false;
        }

        //check if player has all quest items needed
        public bool HasItemsNeeded(Quest quest)
        {
            foreach(QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                bool foundInInventory = false;

                //check items in inventory to see if player has quest items
                foreach(InventoryItem ii in Inventory)
                {
                    if(qci.Details.ID == ii.Details.ID)
                    {
                        foundInInventory = true;

                        //check if player has enough quest items
                        if(ii.Quantity < qci.Quantity)
                        {
                            return false;
                        }
                    }
                }

                if (!foundInInventory)
                {
                    return false;
                }

            }

            //if script gets here, means that player have all quest items needed
            return true;
        }

        //remove quest items from inventory
        public void RemoveQuestItems(Quest quest)
        {
            foreach(QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                foreach(InventoryItem ii in Inventory)
                {
                    if(qci.Details.ID == ii.Details.ID)
                    {
                        ii.Quantity -= qci.Quantity;
                        break;
                    }
                }
            }
        }

        //Add reward items to player inventory
        public void AddRewardItem(Item reward_item)
        {
            foreach(InventoryItem ii in Inventory)
            {
                //search if player already have that item
                if(ii.Details.ID == reward_item.ID)
                {
                    ii.Quantity++;
                    return;
                }

            }
            //if not add a new item to inventory
            Inventory.Add(new InventoryItem(reward_item, 1));
        }

        //mark quest as completed
        public void MarkQuestCompleted(Quest quest)
        {
            //find the quest to mark
            foreach(PlayerQuest pq in Quests)
            {
                if(pq.Details.ID == quest.ID)
                {
                    pq.IsCompleted = true;
                    return;
                }
            }
        }
    }
}
