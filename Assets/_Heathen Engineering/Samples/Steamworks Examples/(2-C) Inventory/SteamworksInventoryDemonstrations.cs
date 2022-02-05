#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using UnityEngine;

namespace HeathenEngineering.SteamAPI.Demo
{
    /// <summary>
    /// Demonstrates the use of the <see cref="SteamworksInventoryTools"/> system
    /// </summary>
    public class SteamworksInventoryDemonstrations : MonoBehaviour
    {
        /// <summary>
        /// Fetch all items and print the results to the console.
        /// </summary>
        public void getAllTest()
        {
            if (SteamworksInventoryTools.GetAllItems((status, results) =>
             {
                 if (status)
                 {
                     Debug.Log("Query returned " + results.Length + " items.");
                 }
                 else
                 {
                     Debug.Log("Query failed.");
                 }
             }))
            {
                Debug.Log("Get All Items request sent to Steamworks.");
            }
            else
            {
                Debug.Log("Get All Items failed to send to Steamworks.");
            }
        }

        /// <summary>
        /// Grant promotion items and print the results to the console.
        /// </summary>
        public void grantPromoTest()
        {
            if (SteamworksInventoryTools.GrantPromoItems((status, results) =>
             {
                 if (status)
                 {
                     Debug.Log("Granted " + results.Length + " promo items.");
                 }
                 else
                 {
                     Debug.Log("Grant Promo Items Failed.");
                 }
             }))
            {
                Debug.Log("Grant Promo Items request sent to Steamworks.");
            }
            else
            {
                Debug.Log("Grant Promo Items failed to send to Steamworks.");
            }
        }
    }
}
#endif
