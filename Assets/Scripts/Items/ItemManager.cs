/* Script Info - Script Name: ItemManager.cs, Created by: Brock Barlow, This script is used to handle items. */

namespace Items
{
    using System.Collections.Generic;
    using Combat;

    public class ItemManager
    {
        private List<BaseItem> m_ActiveList; //list of item(s) being used
        public List<BaseItem> activeList { get { return m_ActiveList; } }
        public List<BaseItem> inventory;     //list of item(s)
        public int sizeOfList;               //used to find size of list(s)
        public TurnBased item;

        public void Update() { activeList.RemoveAll(i => i.Alive == false); }
        //needs to be called after using an item.

        public void AddToInventory() { inventory.Add(item); }
        //simply adds the item(s) to the inventory list.

        public void SetItemActive()
        {
            //3) Determine the item type (attack or defense?)
            //4) Add listeners.

            if (inventory.Contains(item)) { activeList.Add(item); inventory.Remove(item); } else { return; }

            item.UseItem();

            var itemType = item.GetType();

            if (itemType == typeof(InstantItem))
            {
                item.UseItem();
                Update();
            }
            else if (itemType == typeof(TurnBased))
            {
                CombatManager.self.onPlayerTurn.AddListener(item.UpdateSelf);
                item.UseItem();
                Update();
            }
            else if (itemType == typeof(TimeBased))
            {
                CombatManager.self.onCombatUpdate.AddListener(item.UpdateSelf);
                item.UseItem();
                Update();
            }
        }

        public void SetItemNonActive() { if (activeList.Contains(item)) { inventory.Add(item); activeList.Remove(item); } }
        //if active list has any item(s) still after battle, remove them and add them back to the inventory list.
    }
}


//need to know if inventory has limit (yes, but how many?).
//need to know if active inventory has limit (yes, but how many?).
//
//we have two item slots for active: one for heal and one for other.
//need to limit # of heals and others.
//slot 1 only has heal, slot 2 only has turn/time buff.