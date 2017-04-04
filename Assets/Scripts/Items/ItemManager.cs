/* Script Info - Script Name: ItemManager.cs, Created by: Brock Barlow, This script is used to handle items. */

namespace Items
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using Combat;

    public class ItemManager
    {
        [Serializable]
        public class SaveLists
        {
            // All different Types of Items
            public List<InstantItem> instantItems = new List<InstantItem>();
            public List<TimeBuff> timeBuffs = new List<TimeBuff>();
            public List<TurnBuff> turnBuffs = new List<TurnBuff>();
        }

        private List<BaseItem> m_Inventory = new List<BaseItem>();   //list of item(s)
        private List<BaseItem> m_ActiveList = new List<BaseItem>();  //list of item(s) being used
        private List<BaseItem> m_CombatInventory = new List<BaseItem>(); //list of item(s) being taken in to combat

        private SaveLists m_Lists = new SaveLists(); 

        private string m_SavePath = Environment.CurrentDirectory + "/Inventory.xml";

        public List<BaseItem> inventory { get { return m_Inventory; } }
        [XmlIgnore] public List<BaseItem> activeList { get { return m_ActiveList; } }
        [XmlIgnore] public List<BaseItem> combatInventory { get { return m_CombatInventory; } }

        public void ItemUpdate() { activeList.RemoveAll(i => i.Alive == false); } //needs to be called after using an item.

        public void AddCombatItem(BaseItem item)
        {
            if (!inventory.Contains(item))
                return;
            combatInventory.Add(item); inventory.Remove(item);
        }

        public void AddInventoryItem(BaseItem item)
        {
            if (item == null || inventory.Contains(item))
                return;
            inventory.Add(item);
        }

        public void SetItemActive(BaseItem item)
        {
            if (combatInventory.Contains(item)) { activeList.Add(item); combatInventory.Remove(item); } else { return; }

            item.UseItem();

            var itemType = item.GetType();

            if (itemType == typeof(TurnBased)) { CombatManager.self.onPlayerTurn.AddListener(item.UpdateSelf); }
            else if (itemType == typeof(TimeBased)) { CombatManager.self.onCombatUpdate.AddListener(item.UpdateSelf); }
        }

        public void SetItemNonActive(BaseItem item)
        {
            if (activeList.Contains(item)) { combatInventory.Add(item); activeList.Remove(item); }
            if (combatInventory.Contains(item)) { inventory.Add(item); combatInventory.Remove(item); }
        }

        public void SaveItems()
        {
            m_Lists.instantItems = new List<InstantItem>();
            m_Lists.timeBuffs = new List<TimeBuff>();
            m_Lists.turnBuffs = new List<TurnBuff>();

            foreach (var bi in inventory)
            {
                var t = bi.GetType();
                if(t == typeof(InstantItem))
                    m_Lists.instantItems.Add(bi as InstantItem);
                if (t == typeof(TimeBuff))
                    m_Lists.timeBuffs.Add(bi as TimeBuff);
                if (t == typeof(TurnBuff))
                    m_Lists.turnBuffs.Add(bi as TurnBuff);
            }

            var itemsStream = File.Create(m_SavePath);

            var serializer = new XmlSerializer(typeof(SaveLists));
            serializer.Serialize(itemsStream, m_Lists);
            itemsStream.Close();
        }

        public void LoadItems()
        {
            if(!File.Exists(m_SavePath))
                SaveItems();

            var stream = new StreamReader(m_SavePath);

            var reader = new XmlSerializer(typeof(SaveLists));
            m_Lists = (SaveLists)reader.Deserialize(stream);

            m_Inventory = new List<BaseItem>();

            m_Lists.instantItems.ForEach(i => m_Inventory.Add(i));
            m_Lists.turnBuffs.ForEach(i => m_Inventory.Add(i));
            m_Lists.timeBuffs.ForEach(i => m_Inventory.Add(i));

            m_Lists.instantItems = new List<InstantItem>();
            m_Lists.timeBuffs = new List<TimeBuff>();
            m_Lists.turnBuffs = new List<TurnBuff>();

            stream.Close();
        }
    }
}