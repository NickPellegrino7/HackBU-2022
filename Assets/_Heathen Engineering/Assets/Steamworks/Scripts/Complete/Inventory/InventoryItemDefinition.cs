#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{

    /// <summary>
    /// <para>The base of your in game inventory item definitions</para>
    /// <para>This can be derived from to create custom Item Definitions. It defines all the required fields for the wider system to funciton correctly but can be expanded with images, models and other data that might be useful for your game.</para>
    /// </summary>
    /// /// <example>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <para>An example of expanding the InventoryItemDefinition structure to add a Avatar image for use in UI rendering.</para>
    /// <para>Note that in this case we have added a asset menu field such that right clicking in your project folder and selecting
    /// Create > Steamworks > My Game > Item Definition with Avatar
    /// will give you a new scriptable avatar with all the required fields and an extra Texture field to relate a given image to as your avatar.</para>
    /// </description>
    /// <code>
    /// [CreateAssetMenu(menuName = "Steamworks/My Game/Item Definition with Avatar")]
    /// public class ItemDefintionWithAvatar : InventoryItemDefinition
    /// {
    ///         public Texture avatarImage;
    /// }
    /// </code>
    /// </item>
    /// </list>
    /// </example>
    public abstract class InventoryItemDefinition : InventoryItemPointer, IEnumerable<SteamItemDetails_t>, ICollection<SteamItemDetails_t>
    {
        public override InventoryItemType ItemType { get { return InventoryItemType.ItemDefinition; } }

        public UnityEvent inventoryChanged;

        /// <summary>
        /// A list of Steamworks Item Details related to this item definition
        /// This is refreshed from Steamworks by calling SteamworksInventoryManager.RefreshInventory
        /// </summary>
        private List<SteamItemDetails_t> instances = new List<SteamItemDetails_t>();

        /// <summary>
        /// The total quantity of all instances
        /// </summary>
        public int Sum
        {
            get
            {
                if (instances != null)
                    return instances.Sum(p => p.m_unQuantity);
                else
                    return 0;
            }
        }

        /// <summary>
        /// The number of instances of this item.
        /// Remimber each instance can have 0 to many quantity use <see cref="Sum"/> for the sum of all instance quantities
        /// </summary>
        public int Count => instances != null ? instances.Count : 0;

        public bool IsReadOnly => throw new NotImplementedException();

#if !UNITY_SERVER

        /// <summary>
        /// This cannot be undone and will remove items from the palyer's inventory
        /// Be very sure the player wants to do this!
        /// </summary>
        /// <param name="count"></param>
        public void Consume(int count)
        {
            if (Sum > count)
            {
                var ConsumedSoFar = 0;

                List<SteamItemDetails_t> Edits = new List<SteamItemDetails_t>();

                foreach (var instance in instances)
                {
                    if (count - ConsumedSoFar >= instance.m_unQuantity)
                    {
                        //We need to consume all of these
                        ConsumedSoFar += instance.m_unQuantity;
                        SteamworksInventoryTools.ConsumeItem(instance.m_itemId, instance.m_unQuantity,
                            (status, results) =>
                            {
                                if (!status)
                                {
                                    Debug.LogWarning("Failed to consume (" + instance.m_unQuantity.ToString() + ") units of item [" + instance.m_iDefinition.m_SteamItemDef.ToString() + "]");
                                    SteamSettings.Client.inventory.evtItemsConsumed.Invoke(status, results);
                                }
                            });

                        var edit = instance;
                        edit.m_unQuantity = 0;
                        Edits.Add(edit);
                    }
                    else
                    {
                        //We only need some of these
                        var need = count - ConsumedSoFar;
                        ConsumedSoFar += need;
                        SteamworksInventoryTools.ConsumeItem(instance.m_itemId, Convert.ToUInt32(need),
                            (status, results) =>
                            {
                                if (!status)
                                    Debug.LogWarning("Failed to consume (" + need.ToString() + ") units of item [" + instance.m_iDefinition.m_SteamItemDef.ToString() + "]");

                                if (SteamSettings.Client.inventory != null)
                                {
                                    SteamSettings.Client.inventory.evtItemsConsumed.Invoke(status, results);
                                }
                            });

                        var edit = instance;
                        edit.m_unQuantity -= Convert.ToUInt16(need);
                        Edits.Add(edit);

                        break;
                    }
                }

                //Manually update our instances to account for the quantity changes we expect to see
                foreach (var edit in Edits)
                {
                    instances.RemoveAll(p => p.m_itemId == edit.m_itemId);
                    instances.Add(edit);
                }

                if (Edits != null && Edits.Count > 0)
                    inventoryChanged.Invoke();
            }
        }

        /// <summary>
        /// Consumes from a specific instance of an item
        /// </summary>
        /// <remarks>
        /// Note its not possible to consume more than the number of items in this instance stack
        /// </remarks>
        /// <param name="instance">The instance of an item to consume from</param>
        /// <param name="count">The number to consume from the instances quantity</param>
        public void Consume(SteamItemDetails_t instance, int count)
        {
            var ConsumedSoFar = 0;

            List<SteamItemDetails_t> Edits = new List<SteamItemDetails_t>();

            if (count - ConsumedSoFar >= instance.m_unQuantity)
            {
                //We need to consume all of these
                ConsumedSoFar += instance.m_unQuantity;
                SteamworksInventoryTools.ConsumeItem(instance.m_itemId, instance.m_unQuantity,
                    (status, results) =>
                    {
                        if (!status)
                        {
                            Debug.LogWarning("Failed to consume (" + instance.m_unQuantity.ToString() + ") units of item [" + instance.m_iDefinition.m_SteamItemDef.ToString() + "]");
                            SteamSettings.Client.inventory.evtItemsConsumed.Invoke(status, results);
                        }
                    });

                var edit = instance;
                edit.m_unQuantity = 0;
                Edits.Add(edit);
            }
            else
            {
                //We only need some of these
                var need = count - ConsumedSoFar;
                ConsumedSoFar += need;
                SteamworksInventoryTools.ConsumeItem(instance.m_itemId, Convert.ToUInt32(need),
                    (status, results) =>
                    {
                        if (!status)
                            Debug.LogWarning("Failed to consume (" + need.ToString() + ") units of item [" + instance.m_iDefinition.m_SteamItemDef.ToString() + "]");

                        if (SteamSettings.Client.inventory != null)
                        {
                            SteamSettings.Client.inventory.evtItemsConsumed.Invoke(status, results);
                        }
                    });

                var edit = instance;
                edit.m_unQuantity -= Convert.ToUInt16(need);
                Edits.Add(edit);
            }

            //Manually update our instances to account for the quantity changes we expect to see
            foreach (var edit in Edits)
            {
                instances.RemoveAll(p => p.m_itemId == edit.m_itemId);
                instances.Add(edit);
            }

            if (Edits != null && Edits.Count > 0)
                inventoryChanged.Invoke();
        }

        /// <summary>
        /// Get a list of items to fill the desired count
        /// </summary>
        /// <param name="count">The count to fetch</param>
        /// <param name="decriment">Should the cashed values be decrimented to match that which was used</param>
        /// <returns></returns>
        public List<ExchangeItemCount> FetchItemCount(uint count, bool decriment)
        {
            if (Sum >= count)
            {
                var ConsumedSoFar = 0;
                List<ExchangeItemCount> resultCounts = new List<ExchangeItemCount>();

                List<SteamItemDetails_t> Edits = new List<SteamItemDetails_t>();

                foreach (var instance in instances)
                {
                    if (count - ConsumedSoFar >= instance.m_unQuantity)
                    {
                        //We need to consume all of these
                        ConsumedSoFar += instance.m_unQuantity;

                        resultCounts.Add(new ExchangeItemCount() { instanceId = instance.m_itemId, quantity = instance.m_unQuantity });

                        var edit = instance;
                        edit.m_unQuantity = 0;
                        Edits.Add(edit);
                    }
                    else
                    {
                        //We only need some of these
                        int need = Convert.ToInt32(count - ConsumedSoFar);
                        ConsumedSoFar += need;

                        resultCounts.Add(new ExchangeItemCount() { instanceId = instance.m_itemId, quantity = Convert.ToUInt32(need) });

                        var edit = instance;
                        edit.m_unQuantity -= Convert.ToUInt16(need);
                        Edits.Add(edit);

                        break;
                    }
                }

                if (decriment)
                {
                    //Manually update our instances to account for the quantity changes we expect to see
                    foreach (var edit in Edits)
                    {
                        instances.RemoveAll(p => p.m_itemId == edit.m_itemId);
                        instances.Add(edit);
                    }
                }

                return resultCounts;
            }
            else
                return null;
        }

        /// <summary>
        /// Splits an instance quantity, if the destination instance is -1 this will create a new stack of the defined quantity.
        /// </summary>
        /// <param name="source">The stack by index to split</param>
        /// <param name="quantity">The number of items to remove from the source stack</param>
        /// <param name="destination">The stack to move the quantity to, if this is -1 it will create a new stack.</param>
        /// <returns></returns>
        public bool TransferQuantity(int source, uint quantity, int destination)
        {
            var instance = instances[source];
            var dest = SteamItemInstanceID_t.Invalid;
            if (destination > -1)
                dest = instances[destination].m_itemId;

            return TransferQuantity(instance, quantity, dest);
        }

        /// <summary>
        /// Splits an instance quantity, if the destination instance is -1 this will create a new stack of the defined quantity.
        /// </summary>
        /// <param name="source">The instance to split</param>
        /// <param name="quantity">The number of items to remove from the source stack</param>
        /// <param name="destination">The target to move the quanity to</param>
        /// <returns></returns>
        public bool TransferQuantity(SteamItemDetails_t source, uint quantity, SteamItemInstanceID_t destination)
        {
            if (source.m_unQuantity >= quantity)
            {
                
                var ret = SteamworksInventoryTools.TransferQuantity(source.m_itemId, quantity, destination, null);

                return ret;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Moves the quantity from the source into a new stack
        /// </summary>
        /// <param name="source">Source instance to move units from</param>
        /// <param name="quantity">The number of units to move</param>
        /// <returns></returns>
        public bool SplitInstance(SteamItemDetails_t source, uint quantity)
        {
            if (source.m_unQuantity >= quantity)
            {

                var ret = SteamworksInventoryTools.TransferQuantity(source.m_itemId, quantity, SteamItemInstanceID_t.Invalid, null);

                return ret;
            }
            else
            {
                Debug.LogWarning("Unable to split instance, insufficent units available to move.");
                return false;
            }
        }

        /// <summary>
        /// Moves the source instance in its entirety to the destination. 
        /// </summary>
        /// <param name="source">The source to move</param>
        /// <param name="destination">The target destination</param>
        /// <returns></returns>
        public bool StackInstance(SteamItemDetails_t source, SteamItemInstanceID_t destination)
        {
            return TransferQuantity(source, source.m_unQuantity, destination);
        }

        /// <summary>
        /// Attempts to grant this item player as a one time promotional item
        /// </summary>
        /// <param name="callback">
        /// A handler for the resulting steam callback. This is optional and can be left null. If provided this will be invoked on completion of the request and will indicate success and results. Rather or not this is provided the Item's inventoryChanged event will invoke if new items where granted.
        /// </param>
        public void AddPromoItem(Action<bool, SteamItemDetails_t[]> callback = null)
        {
            SteamworksInventoryTools.AddPromoItem(definitionID, callback);
        }

        /// <summary>
        /// Consolodate all stacks of this into a single stack
        /// </summary>
        /// <returns></returns>
        public void Consolidate()
        {
            if (instances != null)
            {
                if (instances.Count > 1)
                {
                    var primary = instances[0];
                    for (int i = 1; i < instances.Count; i++)
                    {
                        var toMove = instances[i];
                        var ret = SteamworksInventoryTools.TransferQuantity(toMove.m_itemId, toMove.m_unQuantity, primary.m_itemId, (result) =>
                        {
                            if (!result)
                            {
                                Debug.LogError("Failed to stack an instance, please refresh the item instances for item definition [" + name + "].");
                            }
                        });

                        if (!ret)
                        {
                            Debug.LogError("Steamworks activly refused a TransferItemQuantity request during the Consolodate operation. No further requests will be sent.");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Unable to consolodate items, this item only has 1 instance. No action will be taken.");
                }
            }
            else
            {
                Debug.LogWarning("Unable to consolodate items, this item only has no instances. No action will be taken.");
            }
        }

        /// <summary>
        /// Start a purchase for this item
        /// </summary>
        /// <remarks>
        /// <para>Instructs the Steamworks backend to start a purchase for this item and the quantity indicated</para>
        /// <para>Note that this process is tightly integrated with the item definition as configured on your Steamworks partner backend. It is keenly important that you have set up proper priceses for your items before this method will work correctly.</para>
        /// <para>If the purchase is successful e.g. if the user competes the purchase then a results ready message will be processed and handled by the Heathen Steamworks Inventory system updating the item instances and quantities available of the items purchased.</para>
        /// </remarks>
        /// <param name="quantity"></param>
        public void StartPurchase(uint quantity)
        {
            InventoryItemDefinitionCount[] itemCounts = { new InventoryItemDefinitionCount() { item = this, count = quantity } };

            SteamworksInventoryTools.StartPurchase(itemCounts, null);
        }
#endif

        public IEnumerator<SteamItemDetails_t> GetEnumerator()
        {
            return instances.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return instances.GetEnumerator();
        }

        public void Add(SteamItemDetails_t item)
        {
            if (instances == null)
                instances = new List<SteamItemDetails_t>();

            if (item.m_iDefinition != definitionID)
                return;

            instances.RemoveAll(p => p.m_itemId == item.m_itemId);

            instances.Add(item);

            inventoryChanged.Invoke();
        }

        public void AddRange(IEnumerable<SteamItemDetails_t> details)
        {
            if (instances == null)
                instances = new List<SteamItemDetails_t>();

            bool hasChange = false;
            foreach (var detail in details)
            {
                if (detail.m_iDefinition != definitionID)
                    continue;

                instances.RemoveAll(p => p.m_itemId == detail.m_itemId);

                instances.Add(detail);
                hasChange = true;
            }

            if (hasChange)
                inventoryChanged.Invoke();
        }

        public void RemoveAll(Predicate<SteamItemDetails_t> match)
        {
            instances.RemoveAll(match);
        }

        public void Clear()
        {
            if (instances == null)
                instances = new List<SteamItemDetails_t>();
            else if (instances.Count > 0)
            {
                instances.Clear();
                inventoryChanged.Invoke();
            }
        }

        public bool Contains(SteamItemDetails_t item)
        {
            return instances.Contains(item);
        }

        public void CopyTo(SteamItemDetails_t[] array, int arrayIndex)
        {
            instances.CopyTo(array, arrayIndex);
        }

        public bool Remove(SteamItemDetails_t item)
        {
            if (instances.Remove(item))
            {
                inventoryChanged.Invoke();
                return true;
            }
            else
                return false;
        }
    }
}
#endif