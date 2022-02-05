#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// The base class of both <see cref="InventoryItemDefinition"/> and <see cref="ItemGeneratorDefinition"/>. 
    /// This object defines the most basic funcitonality of any Steamworks Inventory item regardless of type.
    /// </summary>
    public abstract class InventoryItemPointer : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        /// <summary>
        /// This must match the definition ID set in Steamworks for this item
        /// </summary>
        public SteamItemDef_t definitionID;

        public abstract InventoryItemType ItemType { get; }

        /// <summary>
        /// This is simply an in game representation of the confugraiton you should have created in your Steamworks Partner site for this item.
        /// </summary>
        public List<CraftingRecipe> Recipes;
        [HideInInspector]
        public List<ValveItemDefAttribute> ValveItemDefAttributes;

#if !UNITY_SERVER
        public CraftingRecipe this[string name]
        {
            get { return Recipes?.FirstOrDefault(p => p.name == name); }
        }

        /// <summary>
        /// Converts a Crafting Recipe into a ItemExchangeRecipe suitable for use in the Steamworks API
        /// </summary>
        /// <param name="recipe">The recipe to create</param>
        /// <param name="Edits">This will contain the resulting edits to in memory item instances assuming the exchange is accepted by Steamworks.</param>
        /// <returns></returns>
        public ItemExchangeRecipe PrepareItemExchange(CraftingRecipe recipe, out Dictionary<InventoryItemDefinition, List<SteamItemDetails_t>> Edits)
        {
            //Build ItemExchangeRecipe from available instances to match this recipe
            ItemExchangeRecipe itemRecipe = new ItemExchangeRecipe();
            itemRecipe.itemToGenerate = definitionID;
            itemRecipe.ItemsToConsume = new List<ExchangeItemCount>();

            //Verify quantity
            foreach (var reagent in recipe.items)
            {
                if (reagent.item.Sum < reagent.count)
                {
                    Debug.LogError("InventoryItemPointer.Craft - Failed to fetch the required items for the recipe, insufficent supply of '" + reagent.item.name + "'.");
                    Edits = null;
                    return null;
                }
            }

            Edits = new Dictionary<InventoryItemDefinition, List<SteamItemDetails_t>>();

            //Extract required amounts
            foreach (var reagent in recipe.items)
            {
                if (reagent.item.Sum >= reagent.count)
                {
                    var ConsumedSoFar = 0;
                    List<ExchangeItemCount> resultCounts = new List<ExchangeItemCount>();

                    List<SteamItemDetails_t> ItemEdits = new List<SteamItemDetails_t>();

                    foreach (var instance in reagent.item)
                    {
                        if (reagent.count - ConsumedSoFar >= instance.m_unQuantity)
                        {
                            //We need to consume all of these
                            ConsumedSoFar += instance.m_unQuantity;

                            resultCounts.Add(new ExchangeItemCount() { instanceId = instance.m_itemId, quantity = instance.m_unQuantity });

                            var edit = instance;
                            edit.m_unQuantity = 0;
                            ItemEdits.Add(edit);
                        }
                        else
                        {
                            //We only need some of these
                            int need = Convert.ToInt32(reagent.count - ConsumedSoFar);
                            ConsumedSoFar += need;

                            resultCounts.Add(new ExchangeItemCount() { instanceId = instance.m_itemId, quantity = Convert.ToUInt32(need) });

                            var edit = instance;
                            edit.m_unQuantity -= Convert.ToUInt16(need);
                            ItemEdits.Add(edit);

                            break;
                        }
                    }

                    Edits.Add(reagent.item, ItemEdits);

                    itemRecipe.ItemsToConsume.AddRange(resultCounts);
                }
                else
                {
                    Debug.LogWarning("Crafting request was unable to complete due to insuffient resources.");
                    return null;
                }
            }

            return itemRecipe;
        }

        /// <summary>
        /// Attempts to exchange the required items for a new copy of this item
        /// This is subject to checks by valve as to rather or not this is a legitimate recipie and that the use has sufficent items available for exchange
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns>True if the request is successfuly sent to Steamworks for processing</returns>
        public void Craft(CraftingRecipe recipe)
        {
            var itemRecipe = PrepareItemExchange(recipe, out Dictionary<InventoryItemDefinition, List<SteamItemDetails_t>> edits);

            if(itemRecipe.ItemsToConsume == null || itemRecipe.ItemsToConsume.Count < 1)
            {
                Debug.LogWarning("Attempted to craft item [" + name + "] with no items to consume selected!\nThis will be refused by Steamworks so will not be sent!");
                return;
            }

            if (itemRecipe != null)
            {
                var result = SteamworksInventoryTools.ExchangeItems(itemRecipe, (status, results) =>
                {
                    if (status)
                    {
                        //Remove the counts for the consumed items
                        foreach (var kvp in edits)
                        {
                            foreach (var item in kvp.Value)
                            {
                                kvp.Key.RemoveAll(p => p.m_itemId == item.m_itemId);
                                kvp.Key.Add(item);
                            }
                        }

                        if(SteamSettings.current.isDebugging)
                        {
                            StringBuilder sb = new StringBuilder("Inventory Item [" + name + "] Crafted,\nItems Consumed:\n");
                            foreach(var item in recipe.items)
                            {
                                sb.Append("\t" + item.count + " [" + item.item.name + "]");
                            }
                        }
                    }
                    else
                    {
                        if (SteamSettings.current.isDebugging)
                        {
                            Debug.LogWarning("Request to craft item [" + name + "] failed, confirm the item and recipie configurations are correct in the app settings.");
                        }
                    }

                    if(SteamSettings.Client.inventory != null)
                    {
                        SteamSettings.Client.inventory.evtItemsExchanged.Invoke(status, results);
                    }
                });

                if(!result)
                {
                    if (SteamSettings.Client.inventory != null)
                    {
                        SteamSettings.Client.inventory.evtItemsExchanged.Invoke(false, new SteamItemDetails_t[] { });
                        if (SteamSettings.current.isDebugging)
                            Debug.LogWarning("Request to craft item [" + name + "] was refused by Steamworks.");
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to exchange the required items for a new copy of this item
        /// This is subject to checks by valve as to rather or not this is a legitimate recipie and that the use has sufficent items available for exchange
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns>True if the request is successfuly sent to Steamworks for processing</returns>
        public void Craft(int recipeIndex)
        {
            var recipe = Recipes[recipeIndex];
            Craft(recipe);
        }

        /// <summary>
        /// Grants a copy of this item if available in the items definition on the Steamworks backend.
        /// </summary>
        public void GrantPromoItem()
        {
            SteamworksInventoryTools.AddPromoItem(definitionID, (status, results) =>
            {
                if (SteamSettings.Client.inventory != null)
                {
                    SteamSettings.Client.inventory.evtItemsGranted.Invoke(status, results);
                }
            });
        }
#endif
    }
}
#endif