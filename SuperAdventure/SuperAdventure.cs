using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Engine;


namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {

        private Player _player;
        private Monster _currentMonster;

        public SuperAdventure()
        {
            InitializeComponent();

            _player = new Player(10, 10, 20, 0);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));

            UpdatePlayerStats();

        }

        private void SuperAdventure_Load(object sender, EventArgs e)
        {
        }
        //
        //Movement Click Listeners
        //
        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            //Get weapon form the select dropdown
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            //Determine the amount of damage to deal
            int damageToMonster = RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);

            //apply the damage to the monster
            _currentMonster.CurrentHitPoints -= damageToMonster;

            //display the message
            rtbMessages.Text += "You hit " + _currentMonster.Name + " for " + damageToMonster.ToString() + " points" + Environment.NewLine;
            ScrollToBottomOfMessages();

            //Check if monster is dead
            if (_currentMonster.CurrentHitPoints <= 0)
            {
                //monster is dead
                rtbMessages.Text += Environment.NewLine;
                rtbMessages.Text += "You defeated the " + _currentMonster.Name + Environment.NewLine;

                //Give player reward exp
                _player.ExperiencePoints += _currentMonster.RewardExperiencePoints;
                rtbMessages.Text += "You received " + _currentMonster.RewardExperiencePoints.ToString() + " experience points." + Environment.NewLine;

                //Give Player reward gold
                _player.Gold += _currentMonster.RewardGold;
                rtbMessages.Text += "You received " + _currentMonster.RewardGold.ToString() + " gold." + Environment.NewLine;
                ScrollToBottomOfMessages();

                //Generated items to loot
                List<InventoryItem> LootedItems = new List<InventoryItem>();

                foreach (LootItem lootItem in _currentMonster.LootTable)
                {
                    if (RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage)
                    {
                        LootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }
                //if there is no rng item selected, add default item
                if (LootedItems.Count == 0)
                {
                    foreach (LootItem lootItem in _currentMonster.LootTable)
                    {
                        if (lootItem.isDefaultItem)
                        {
                            LootedItems.Add(new InventoryItem(lootItem.Details, 1));
                        }
                    }
                }

                //Add Looted items to player inventory
                foreach (InventoryItem inventoryItem in LootedItems)
                {
                    _player.AddRewardItem(inventoryItem.Details);
                    //Display Message
                    if (inventoryItem.Quantity == 1)
                    {
                        rtbMessages.Text += "You looted " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.Name + Environment.NewLine;
                        ScrollToBottomOfMessages();
                    }
                    else
                    {
                        rtbMessages.Text += "You looted " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.NamePlural + Environment.NewLine;
                        ScrollToBottomOfMessages();
                    }
                }

                //refresh player info and inventory
                UpdatePlayerStats();

                UpdateInventory();
                UpdateWeaponCB();
                UpdatePotionCB();

                rtbMessages.Text += Environment.NewLine;
                ScrollToBottomOfMessages();

                MoveTo(_player.CurrentLocation);
            }
            else
            {
                //Monster is still alive
                //Monster will Attack
                int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);

                //Display Message
                rtbMessages.Text += "The " + _currentMonster.Name.ToString() + " did " + damageToPlayer.ToString() + " points of damage" + Environment.NewLine;
                ScrollToBottomOfMessages();

                //Substract damage from player
                _player.CurrentHitPoints -= damageToPlayer;

                //refresh player health in UI
                lblHitPoints.Text = _player.CurrentHitPoints.ToString();

                //check if player is dead
                if (_player.CurrentHitPoints <= 0)
                {
                    //Display Message
                    rtbMessages.Text += "The " + _currentMonster.Name.ToString() + " killed you." + Environment.NewLine;
                    ScrollToBottomOfMessages();

                    MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                }
            }
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            //Get selected potion from the select
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

            //Add healing amount to player hp
            _player.CurrentHitPoints += potion.AmountToHeal;

            //Current hitpoints cannot exceed max hitpoints
            if (_player.CurrentHitPoints > _player.MaximumHitPoints)
            {
                _player.CurrentHitPoints = _player.MaximumHitPoints;
            }

            //Remove the potion from player inventory
            foreach (InventoryItem ii in _player.Inventory)
            {
                ii.Quantity--;
                break;
            }

            //Update player hitpoints in UI
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            //Display message
            rtbMessages.Text = "You drank a " + potion.Name + " and restored " + potion.AmountToHeal.ToString() + " hit points" + Environment.NewLine;
            ScrollToBottomOfMessages();

            //Monster get Their tun to attack
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);

            //Display Message
            rtbMessages.Text += "The " + _currentMonster.Name.ToString() + " did " + damageToPlayer.ToString() + " points of damage" + Environment.NewLine;
            ScrollToBottomOfMessages();

            //Substract damage from player
            _player.CurrentHitPoints -= damageToPlayer;

            //refresh player health in UI
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            //check if player is dead
            if (_player.CurrentHitPoints <= 0)
            {
                //Display Message
                rtbMessages.Text += "The " + _currentMonster.Name.ToString() + " killed you." + Environment.NewLine;
                ScrollToBottomOfMessages();

                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }

        }
        //
        //Movement Method
        //
        private void MoveTo(Location newLocation)
        {
            //Confirm if the player has such item
            if (!_player.HasItemToEnterLocation(newLocation))
            {
                //Display item needed in the message box

                rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                ScrollToBottomOfMessages();
                return;

            }

            //Update the player current location
            _player.CurrentLocation = newLocation;

            //SHow available movement buttons
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);
            btnEast.Visible = (newLocation.LocationToEast != null);

            //Display current location name and description
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;
            ScrollToBottomOfMessages();

            //Completely Heal the player
            _player.CurrentHitPoints = _player.MaximumHitPoints;

            //Update Hitpoints in the UI
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            //Does the location havea quest?
            if (newLocation.QuestAvailableHere != null)
            {
                //See if the player has the quest and if the player already completed it
                bool playerAlreadyHaveQuest = _player.HasQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompletedQuest = _player.CompletedQuest(newLocation.QuestAvailableHere);

                if (playerAlreadyHaveQuest)
                {
                    if (!playerAlreadyCompletedQuest)
                    {
                        //Checks if player has all quest items
                        bool playerHasAllQuestItems = _player.HasItemsNeeded(newLocation.QuestAvailableHere);

                        //the player has all quest items
                        if (playerHasAllQuestItems)
                        {
                            //Display Message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You completed the " + newLocation.QuestAvailableHere.Name + " quest" + Environment.NewLine;

                            _player.RemoveQuestItems(newLocation.QuestAvailableHere);

                            //Give quest rewards
                            rtbMessages.Text += "You received: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;
                            ScrollToBottomOfMessages();

                            _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;
                            _player.AddRewardItem(newLocation.QuestAvailableHere.RewardItem);

                            _player.MarkQuestCompleted(newLocation.QuestAvailableHere);
                        }
                    }
                }
                //player doesnt have the quest
                else
                {
                    //Display message that a new quest has been received
                    rtbMessages.Text += "You received the " + newLocation.QuestAvailableHere.Name + "quest" + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To completed it, return with: " + Environment.NewLine;
                    ScrollToBottomOfMessages();

                    foreach (QuestCompletionItem questCompletionItem in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if (questCompletionItem.Quantity == 1)
                        {
                            rtbMessages.Text += questCompletionItem.Quantity.ToString() + " " +
                            questCompletionItem.Details.Name + Environment.NewLine;
                            ScrollToBottomOfMessages();
                        }
                        else
                        {
                            rtbMessages.Text += questCompletionItem.Quantity.ToString() + " " +
                            questCompletionItem.Details.NamePlural + Environment.NewLine;
                            ScrollToBottomOfMessages();
                        }
                    }
                    rtbMessages.Text += Environment.NewLine;
                    ScrollToBottomOfMessages();

                    //Add quest to player questlog
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            // Does the location have a monster?
            if (newLocation.MonsterLivingHere != null)
            {
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;
                ScrollToBottomOfMessages();

                // Make a new monster, using the values from the standard monster in the World.Monster list
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(
                    standardMonster.ID, standardMonster.Name,
                    standardMonster.MaximumDamage, standardMonster.RewardExperiencePoints,
                    standardMonster.RewardGold, standardMonster.CurrentHitPoints,
                    standardMonster.MaximumHitPoints
                );

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
            }
            else
            {
                _currentMonster = null;
                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }

            //UpdatePlayer Stats
            UpdatePlayerStats();

            // Refresh player's inventory list
            UpdateInventory();

            // Refresh player's quest list
            UpdateQuestLog();

            // Refresh player's weapons combobox
            UpdateWeaponCB();

            // Refresh player's potions combobox
            UpdatePotionCB();
        }

        private void UpdateInventory()
        {
            dgvInventory.RowHeadersVisible = false;
            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";
            dgvInventory.Rows.Clear();
            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] {
                         inventoryItem.Details.Name,
                         inventoryItem.Quantity.ToString()
                     });
                }
            }
        }

        private void UpdateQuestLog()
        {
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Done?";

            dgvQuests.Rows.Clear();

            foreach (PlayerQuest playerQuest in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] {
                    playerQuest.Details.Name,playerQuest.IsCompleted.ToString()
                });
            }
        }

        private void UpdateWeaponCB()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is Weapon)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon)inventoryItem.Details);
                    }
                }
            }

            if (weapons.Count == 0)
            {
                // The player doesn't have any weapons, so hide the weapon combobox and the "Use" button
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";

                cboWeapons.SelectedIndex = 0;
            }
        }

        private void UpdatePotionCB()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is HealingPotion)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)inventoryItem.Details);
                    }
                }
            }

            if (healingPotions.Count == 0)
            {
                // The player doesn't have any potions, so hide the potion combobox and the "Use" button
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }

        private void ScrollToBottomOfMessages()
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }

        private void UpdatePlayerStats()
        {
            // Refresh player information and inventory controls
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }
    }
}

