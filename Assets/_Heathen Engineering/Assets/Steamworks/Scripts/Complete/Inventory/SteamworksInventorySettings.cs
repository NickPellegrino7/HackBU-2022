#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    [Serializable] 
    public class SteamworksInventorySettings
    {
        public List<InventoryItemDefinition> itemDefinitions = new List<InventoryItemDefinition>();

        public List<ItemGeneratorDefinition> itemGenerators = new List<ItemGeneratorDefinition>();

        public List<TagGeneratorDefinition> tagGenerators = new List<TagGeneratorDefinition>();

        public List<InventoryItemBundleDefinition> itemBundles = new List<InventoryItemBundleDefinition>();

        #region Client Only Code
#if !UNITY_SERVER

        [HideInInspector]
        public UnityEvent evtItemInstancesUpdated = new UnityEvent();
        [HideInInspector]
        public UnityItemDetailEvent evtItemsGranted = new UnityItemDetailEvent();
        [HideInInspector]
        public UnityItemDetailEvent evtItemsConsumed = new UnityItemDetailEvent();
        [HideInInspector]
        public UnityItemDetailEvent evtItemsExchanged = new UnityItemDetailEvent();
        [HideInInspector]
        public UnityItemDetailEvent evtItemsDropped = new UnityItemDetailEvent();

        private Dictionary<SteamItemDef_t, InventoryItemPointer> itemPointerIndex = new Dictionary<SteamItemDef_t, InventoryItemPointer>();
        private Dictionary<SteamItemDef_t, InventoryItemDefinition> itemDefinitionIndex = new Dictionary<SteamItemDef_t, InventoryItemDefinition>();
        private Dictionary<SteamItemDef_t, ItemGeneratorDefinition> itemGeneratorIndex = new Dictionary<SteamItemDef_t, ItemGeneratorDefinition>();

        

        #region Utilities
        /// <summary>
        /// Indexes the items and generators for fast look up by ID
        /// </summary>
        public void BuildIndex()
        {
            if (itemPointerIndex == null)
                itemPointerIndex = new Dictionary<SteamItemDef_t, InventoryItemPointer>();

            if (itemDefinitionIndex == null)
                itemDefinitionIndex = new Dictionary<SteamItemDef_t, InventoryItemDefinition>();

            if (itemGeneratorIndex == null)
                itemGeneratorIndex = new Dictionary<SteamItemDef_t, ItemGeneratorDefinition>();

            if (SteamSettings.current.isDebugging)
            {
                var items = itemDefinitions != null ? itemDefinitions.Count : 0;
                var generators = itemGenerators != null ? itemGenerators.Count : 0;
                Debug.Log("Building internal indices for " + items + " items and " + generators + " generators.");
            }

            foreach (var item in itemDefinitions)
            {
                if (itemDefinitionIndex.ContainsKey(item.definitionID))
                    itemDefinitionIndex[item.definitionID] = item;
                else
                    itemDefinitionIndex.Add(item.definitionID, item);

                if(itemPointerIndex.ContainsKey(item.definitionID))
                    itemPointerIndex[item.definitionID] = item;
                else
                    itemPointerIndex.Add(item.definitionID, item);
            }

            foreach (var item in itemGenerators)
            {
                if (itemGeneratorIndex.ContainsKey(item.definitionID))
                    itemGeneratorIndex[item.definitionID] = item;
                else
                    itemGeneratorIndex.Add(item.definitionID, item);

                if (itemPointerIndex.ContainsKey(item.definitionID))
                    itemPointerIndex[item.definitionID] = item;
                else
                    itemPointerIndex.Add(item.definitionID, item);
            }
        }
        /// <summary>
        /// Clears all instances for all registered items
        /// </summary>
        public void ClearItemCounts()
        {
            foreach (var item in itemDefinitions)
            {
                item.Clear();
            }
        }

        /// <summary>
        /// Handles detail item updates typically from Steamworks
        /// </summary>
        /// <param name="details"></param>
        public void HandleItemDetailUpdate(IEnumerable<SteamItemDetails_t> details)
        {
            var hasUpdate = false;
            //Batch the details by ID
            var batches = new Dictionary<SteamItemDef_t, List<SteamItemDetails_t>>();
            foreach(var detail in details)
            {
                if(!batches.ContainsKey(detail.m_iDefinition))
                {
                    batches.Add(detail.m_iDefinition, new List<SteamItemDetails_t>());
                }

                batches[detail.m_iDefinition].Add(detail);

                if(itemDefinitionIndex.ContainsKey(detail.m_iDefinition))
                {
                    hasUpdate = true;
                    var target = itemDefinitionIndex[detail.m_iDefinition];
                    target.Add(detail);
                }
                else if (SteamSettings.current.isDebugging)
                {
                    Debug.LogWarning("No item definition found for item " + detail.m_iDefinition.m_SteamItemDef + " but an item instance " + detail.m_itemId.m_SteamItemInstanceID + " exists in the player's inventory with a unit count of " + detail.m_unQuantity + "\nConsider adding an item definition for this to your Steamworks Inventory Settings.");
                }
            }

            foreach(var id in batches.Keys)
            {
                if(itemDefinitionIndex.ContainsKey(id))
                {
                    hasUpdate = true;
                    var target = itemDefinitionIndex[id];
                    target.AddRange(batches[id]);
                }
                else
                {
                    Debug.LogWarning("No item definition found for item " + id + " but recieved instance update information for it\nConsider adding an item definition for this to your Steamworks Inventory Settings.");
                }
            }

            if (hasUpdate)
            {
                if(SteamSettings.current.isDebugging)
                {
                    StringBuilder sb = new StringBuilder("Inventory Item Detail Update:\n");
                    foreach(var item in itemDefinitions)
                    {
                        sb.Append("\t[" + item.name + "] has " + item.Count + " instances for a sum of " + item.Sum + " units.\n");
                    }
                    Debug.Log(sb.ToString());
                }

                evtItemInstancesUpdated.Invoke();
            }
        }

        /// <summary>
        /// Registeres this instance of settings as the active Steamworks Inventory Settings
        /// </summary>
        public void Register()
        {
            BuildIndex();
        }
        /// <summary>
        /// Gets the Item Definition for the specified detail object
        /// </summary>
        /// <typeparam name="T">The InventoryItemDefinition derived type that the definition should be casted to</typeparam>
        /// <param name="steamDetail"></param>
        /// <returns></returns>
        public T GetDefinition<T>(SteamItemDetails_t steamDetail) where T: InventoryItemDefinition
        {
            return this[steamDetail] as T;
        }
        /// <summary>
        /// Gets the Item Definition for the specified steam definition id
        /// </summary>
        /// <typeparam name="T">The InventoryItemDefinition derived type that the definition should be casted to</typeparam>
        /// <param name="steamDefinition"></param>
        /// <returns></returns>
        public T GetDefinition<T>(SteamItemDef_t steamDefinition) where T : InventoryItemDefinition
        {
            return this[steamDefinition] as T;
        }
        /// <summary>
        /// Gets the Item Definition for the specified detail object
        /// </summary>
        /// <param name="steamDetail"></param>
        /// <returns></returns>
        public InventoryItemDefinition GetDefinition(SteamItemDetails_t steamDetail)
        {
            return this[steamDetail] as InventoryItemDefinition;
        }
        /// <summary>
        /// Gets the Item Definition for the specified steam definition id
        /// </summary>
        /// <param name="steamDefinition"></param>
        /// <returns></returns>
        public InventoryItemDefinition GetDefinition(SteamItemDef_t steamDefinition)
        {
            return this[steamDefinition] as InventoryItemDefinition;
        }

        public InventoryItemDefinition GetDefinition(int steamDefinition)
        {
            return this[steamDefinition] as InventoryItemDefinition;
        }

        public InventoryItemPointer this[SteamItemDef_t id]
        {
            get
            {
                if (itemPointerIndex == null)
                    BuildIndex();

                if (itemPointerIndex.ContainsKey(id))
                    return itemPointerIndex[id];
                else
                    return null;
            }
        }

        public InventoryItemPointer this[SteamItemDetails_t item]
        {
            get
            {
                return this[item.m_iDefinition];
            }
        }

        public InventoryItemPointer this[int itemId]
        {
            get
            {
                return this[new SteamItemDef_t(itemId)];
            }
        }
        #endregion

        /// <summary>
        /// Used internally to update registered items when new details are recieved from Steamworks
        /// </summary>
        /// <param name="details"></param>
        public static void InternalItemDetailUpdate(IEnumerable<SteamItemDetails_t> details)
        {
            SteamSettings.Client.inventory.HandleItemDetailUpdate(details);
        }

        /// <summary>
        /// <para>Updates the InventoryItemDefinition.Instances list of each of the referenced Item Definitions with the results of a 'Get All Items' query against the current user's Steamworks Inventory.</para>
        /// <para>This will cause the Instances member of each item to reflect the current state of the users inventory.</para>
        /// <para> <para>
        /// <para>This will trigger the Item Instances Updated event after steam responds with the users inventory items and the items have been updated to reflect the correct instances.</para>
        /// </summary>
        public void RefreshInventory()
        {
            if (itemDefinitions.Count == 0)
                return;

            foreach (var item in itemDefinitions)
            {
                item.Clear();
            }

            var result = SteamworksInventoryTools.GetAllItems(null);
            if (!result)
                Debug.LogWarning("[SteamworksInventorySettings.RefreshInventory] - Call failed");
        }

        /// <summary>
        /// <para>Grants the user all available promotional items</para>
        /// <para>This will trigger the Item Instances Updated event after steam responds with the users inventory items and the items have been updated to reflect the correct instances.</para>
        /// <para> <para>
        /// <para>NOTE: this additivly updates the Instance list for effected items and is not a clean refresh!
        /// Consider a call to Refresh Inventory to resolve a complete and accurate refresh of all player items.</para>
        /// </summary>
        public void GrantAllPromotionalItems()
        {
            var result = SteamworksInventoryTools.GrantPromoItems((status, results) =>
            {
                evtItemsGranted.Invoke(status, results);
            });
            if (!result)
                Debug.LogWarning("[SteamworksInventorySettings.GrantAllPromotionalItems] - Call failed");
        }

        /// <summary>
        /// <para>Grants the user a promotional item</para>
        /// <para>This will trigger the Item Instances Updated event after steam responds with the users inventory items and the items have been updated to reflect the correct instances.</para>
        /// <para> <para>
        /// <para>NOTE: this additivly updates the Instance list for effected items and is not a clean refresh!
        /// Consider a call to Refresh Inventory to resolve a complete and accurate refresh of all player items.</para>
        /// </summary>
        /// <paramref name="itemDefinition">The item type to grant to the user.</paramref>
        public void GrantPromotionalItem(InventoryItemDefinition itemDefinition)
        {
            var result = SteamworksInventoryTools.AddPromoItem(itemDefinition.definitionID, (status, results) =>
            {
                evtItemsGranted.Invoke(status, results);
            });
            if (!result)
                Debug.LogWarning("[SteamworksInventorySettings.GrantPromotionalItem] - Call failed");
        }

        /// <summary>
        /// <para>Grants the user the promotional items indicated</para>
        /// <para>This will trigger the Item Instances Updated event after steam responds with the users inventory items and the items have been updated to reflect the correct instances.</para>
        /// <para>NOTE: this additivly updates the Instance list for effected items and is not a clean refresh!
        /// Consider a call to Refresh Inventory to resolve a complete and accurate refresh of all player items.</para>
        /// </summary>
        /// <param name="itemDefinitions">The list of items to be granted if available</param>
        public void GrantPromotionalItems(IEnumerable<InventoryItemDefinition> itemDefinitions)
        {
            List<SteamItemDef_t> items = new List<SteamItemDef_t>();
            foreach(var itemDef in itemDefinitions)
            {
                items.Add(itemDef.definitionID);
            }

            var result = SteamworksInventoryTools.AddPromoItems(items, (status, results) =>
            {
                evtItemsGranted.Invoke(status, results);
            });
            if (!result)
                Debug.LogWarning("[SteamworksInventorySettings.GrantPromotionalItems] - Call failed");
        }

        /// <summary>
        /// <para>Determins if the result handle belongs to the user</para>
        /// </summary>
        /// <param name="resultHandle">The inventory result handle to check the user on</param>
        /// <param name="user">The user to check against</param>
        public bool CheckUserResult(SteamInventoryResult_t resultHandle, ulong user)
        {
            return SteamworksInventoryTools.CheckResultSteamID(resultHandle, user);
        }

        /// <summary>
        /// <para>Determins if the result handle belongs to the user</para>
        /// </summary>
        /// <param name="resultHandle">The inventory result handle to check the user on</param>
        /// <param name="user">The user to check against</param>
        public bool CheckUserResult(SteamInventoryResult_t resultHandle, CSteamID user)
        {
            return SteamworksInventoryTools.CheckResultSteamID(resultHandle, user);
        }

        /// <summary>
        /// <para>Determins if the result handle belongs to the user</para>
        /// </summary>
        /// <param name="resultHandle">The inventory result handle to check the user on</param>
        /// <param name="user">The user to check against</param>
        public bool CheckUserResult(SteamInventoryResult_t resultHandle, UserData user)
        {
            return SteamworksInventoryTools.CheckResultSteamID(resultHandle, user);
        }

        /// <summary>
        /// Consumes a single unit of a single instnace of this item if available.
        /// </summary>
        /// <param name="itemDefinition"></param>
        public void ConsumeItem(InventoryItemDefinition itemDefinition)
        {
            itemDefinition.Consume(1);
        }

        /// <summary>
        /// <para>Attempts to consume the requested units of the indicated item</para>
        /// <para>NOTE: this may need to iterate over multiple instances and may need to send multiple consume requests any of which may fail and each of which will trigger an Item Instance Update event call.</para>
        /// <para>You are recomended to use the the SteamItemInstance_t overload of this method when consuming more than 1 unit of an item.</para>
        /// </summary>
        /// <param name="itemDefinition">The item to consume for</param>
        /// <param name="count">The number of item units to try and consume</param>
        public void ConsumeItem(InventoryItemDefinition itemDefinition, int count)
        {
            itemDefinition.Consume(count);
        }
                
        /// <summary>
        /// <para>Exchange items for the indicated recipe</para>
        /// <para>NOTE: this method will trigger the Items Exchanged event and can optionally trigger a full inventory refresh on completion of the exchange.</para>
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="postExchangeRefresh"></param>
        public void ExchangeItems(InventoryItemDefinition itemToCraft, CraftingRecipe recipe)
        {
            itemToCraft.Craft(recipe);
        }

        public void ExchangeItems(InventoryItemDefinition itemToCraft, int recipeIndex)
        {
            itemToCraft.Craft(recipeIndex);
        }

        /// <summary>
        /// Calls start purchase ... if successful this will open the Steam Overlay to the shoping cart with these items in it.
        /// Note this requires that each of these items be configured properly to be sold on the Steam store. If any of them have any issue with there configuration such as missing price information or similar then the request will simply be ignored by Valve.
        /// </summary>
        /// <param name="itemsToPurchase">This can be a collection of <see cref="InventoryItemDefinition"/> and or <see cref="InventoryItemBundleDefinition"/> objects both of which derive from <see cref="InventoryItemPointer"/></param>
        public void StartPurchase(IEnumerable<InventoryItemDefinitionCount> itemsToPurchase)
        {
            SteamworksInventoryTools.StartPurchase(itemsToPurchase);
        }

        /// <summary>
        /// <para>Triggers the indicated generator to drop and item if available.</para>
        /// <para>NOTE: This will trigger an Items Droped event and optionally a Refresh of the player's inventory</para>
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="postDropRefresh"></param>
        public void TriggerItemDrop(ItemGeneratorDefinition generator, bool postDropRefresh)
        {
            generator.TriggerDrop((status, results) =>
            {
                if(status)
                {
                    if(SteamSettings.current.isDebugging)
                    {
                        Debug.Log("Item Drop for [" + generator.name + "] completed with status " + status + " and " + results.Count() + " instances effected.");
                    }
                    if (postDropRefresh)
                        RefreshInventory();
                }

                evtItemsDropped.Invoke(status, results);
            });
        }

        public void TriggerItemDrop(ItemGeneratorDefinition generator)
        {
            TriggerItemDrop(generator, false);
        }

        public void TriggerItemDropAndRefresh(ItemGeneratorDefinition generator)
        {
            TriggerItemDrop(generator, true);
        }

        /// <summary>
        /// Splits an instance quantity, if the destination instance is -1 this will create a new stack of the defined quantity.
        /// </summary>
        /// <param name="source">The instance to split</param>
        /// <param name="quantity">The number of items to remove from the source stack</param>
        /// <param name="destination">The target to move the quanity to</param>
        /// <returns></returns>
        public bool TransferQuantity(InventoryItemDefinition item, SteamItemDetails_t source, uint quantity, SteamItemInstanceID_t destination)
        {
            return item.TransferQuantity(source, quantity, destination);
        }

        /// <summary>
        /// Moves the quantity from the source into a new stack
        /// </summary>
        /// <param name="source">Source instance to move units from</param>
        /// <param name="quantity">The number of units to move</param>
        /// <returns></returns>
        public bool SplitInstance(InventoryItemDefinition item, SteamItemDetails_t source, uint quantity)
        {
            return item.SplitInstance(source, quantity);
        }

        /// <summary>
        /// Moves the source instance in its entirety to the destination. 
        /// </summary>
        /// <param name="source">The source to move</param>
        /// <param name="destination">The target destination</param>
        /// <returns></returns>
        public bool StackInstance(InventoryItemDefinition item, SteamItemDetails_t source, SteamItemInstanceID_t destination)
        {
            return item.StackInstance(source, destination);
        }

        /// <summary>
        /// Consolodate all stacks of this into a single stack
        /// </summary>
        /// <returns></returns>
        public void Consolidate(InventoryItemDefinition item)
        {
            item.Consolidate();
        }

#endif
        #endregion
    }
}
#endif