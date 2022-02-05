#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System.Collections.Generic;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Simplifies Valve's exchange recipie process
    /// Note that at current ExchangeItems can only generate 1 item per call dispite handling an array
    /// For this reason we have limited the recipie to a single generated item definition. 
    /// <para>Note that it is a limitation of Steamworks from Valve that single item generation is the case.</para>
    /// </summary>
    public class ItemExchangeRecipe
    {
        /// <summary>
        /// The id of the item to generate
        /// </summary>
        public SteamItemDef_t itemToGenerate;
        /// <summary>
        /// The item ids and counts of the items to be consumed in the process of generation.
        /// </summary>
        public List<ExchangeItemCount> ItemsToConsume = new List<ExchangeItemCount>();

        public ItemExchangeRecipe()
        { }

        /// <summary>
        /// Constructs a new recipe based on a taget item to create and a collection of items to be consumed.
        /// </summary>
        /// <param name="toGenerate"></param>
        /// <param name="toBeConsumed"></param>
        public ItemExchangeRecipe(SteamItemDef_t toGenerate, IEnumerable<ExchangeItemCount> toBeConsumed)
        {
            itemToGenerate = toGenerate;
            ItemsToConsume = new List<ExchangeItemCount>(toBeConsumed);
        }

        /// <summary>
        /// Converts the items to consume into an array of SteamItemInstanceID_t for use with the steamAPI
        /// </summary>
        /// <returns></returns>
        public SteamItemInstanceID_t[] GetInstanceArray()
        {
            var instanceList = ItemsToConsume.ConvertAll((p) => { return p.instanceId; });
            return instanceList.ToArray();
        }

        /// <summary>
        /// Converts the items to consume into an array of item quantities for use with the steamAPI
        /// </summary>
        /// <returns></returns>
        public uint[] GetQuantityArray()
        {
            var instanceList = ItemsToConsume.ConvertAll((p) => { return p.quantity; });
            return instanceList.ToArray();
        }
    }
}
#endif