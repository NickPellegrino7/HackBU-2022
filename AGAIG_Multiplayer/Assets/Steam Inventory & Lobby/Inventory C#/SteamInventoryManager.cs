using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using UnityEngine.UI;

namespace BrettArnett
{
    public class SteamInventoryManager : MonoBehaviour
    {
        SteamInventoryResult_t myInventoryHandle;

        public Text GenerateItemsInputText;
        private SteamItemDef_t generateItemDef;

        // Inventory Displayer Content
        [SerializeField] private GameObject ContentPanel;

        public uint punItemDefIDsArraySize;
        public SteamItemDef_t[] pItemDefIDs;

        // Inventory Item Properties
        private UInt32 propertyValueBuffer;
        private string propertyName = "name";
        private string propertySpecialization = "specialization";
        private UInt32 PropertyValueStringLengthMax = 100;

        // Inventory List Items
        [SerializeField] private GameObject SteamInventoryListItemPrefab;
        private List<SteamInventoryListItem> steaminventoryListItems = new List<SteamInventoryListItem>();

        // Instance
        private SteamInventoryManager instance;

        void Awake()
        {
            MakeInstance();
        }

        void MakeInstance()
        {
            if (instance == null)
                instance = this;
        }

        public void LoadItemDefs()
        {
            SteamInventory.LoadItemDefinitions();
        }

        public void GetItemsPrepareItems()
        {
            GetItems("PrepareItems"); // Running this will get item definitions without creating any list items to display. This is necessary for things like GenerateItems that depend on existing item definitions.
        }
        public void GetItemsFullInventory()
        {
            GetItems("FullInventory");
        }
        public void GetItemsHats()
        {
            GetItems("HatsOnly");
        }


        // For help with properties, reference the JSON example.

        public void GetItems(string function)
        {
            SteamInventory.GetItemDefinitionIDs(null, ref punItemDefIDsArraySize);
            {
                Array.Resize(ref pItemDefIDs, (int)punItemDefIDsArraySize);
                SteamInventory.BrettArnettGetItemIDs(pItemDefIDs, ref punItemDefIDsArraySize);
            }

            foreach (SteamItemDef_t itemIdNumber in pItemDefIDs)
            {
                string nameValue;
                string specializationValue;

                propertyValueBuffer = PropertyValueStringLengthMax; // This property value buffer defines the maximum amount of characters allowed in the value string. It will get used up and end up at zero if you don't reset it back to max after, or before a property is fetched.
                SteamInventory.GetItemDefinitionProperty(itemIdNumber, propertyName, out nameValue, ref propertyValueBuffer);

                propertyValueBuffer = PropertyValueStringLengthMax;
                SteamInventory.GetItemDefinitionProperty(itemIdNumber, propertySpecialization, out specializationValue, ref propertyValueBuffer);

                if (function == "FullInventory")
                {
                    CreateInventoryListItem((int)itemIdNumber, nameValue, specializationValue);
                }

                if (function == "HatsOnly")
                {
                    if (specializationValue == "Hat")
                    {
                        CreateInventoryListItem((int)itemIdNumber, nameValue, specializationValue);
                    }
                    else
                    {
                        //Debug.Log((int)itemIdNumber + " is not of the specialization: Hat");
                    }
                }
            }
        }


        // Inventory List Items
        //
        public void CreateInventoryListItem(int itemIdNumber, string itemName, string itemSpecialization)
        {
            GameObject newSteamInventoryListItem = Instantiate(SteamInventoryListItemPrefab) as GameObject;

            SteamInventoryListItem newSteamInventoryListItemScript = newSteamInventoryListItem.GetComponent<SteamInventoryListItem>();

            newSteamInventoryListItemScript.itemIdNumber = itemIdNumber;
            newSteamInventoryListItemScript.itemName = itemName;
            newSteamInventoryListItemScript.itemSpecialization = itemSpecialization;

            newSteamInventoryListItemScript.SetSteamInventoryListItemValues();

            newSteamInventoryListItem.transform.SetParent(ContentPanel.transform);

            newSteamInventoryListItem.transform.localScale = Vector3.one;

            steaminventoryListItems.Add(newSteamInventoryListItemScript);

        }

        public void ClearSteamInventoryContent()
        {
            foreach (Transform item in ContentPanel.transform)
            {
                GameObject.Destroy(item.gameObject);
            }

            foreach (SteamInventoryListItem inventoryListItem in steaminventoryListItems)
            {
                GameObject inventoryListItemObject = inventoryListItem.gameObject;
                Destroy(inventoryListItemObject);
                inventoryListItemObject = null;
            }
            steaminventoryListItems.Clear();
        }
        //
        //



        // Get your own inventory items
        //
        public void GetMyOwnInventory()
        {
            SteamInventory.GetAllItems(out myInventoryHandle);
            StartCoroutine(GetMyOwnInventoryList());
        }

        IEnumerator GetMyOwnInventoryList()
        {
            yield return new WaitForSeconds(1);

            ClearSteamInventoryContent();

            uint itemCount = 0;
            SteamItemDetails_t[] itemDetails;

            if (SteamInventory.GetResultItems(myInventoryHandle, null, ref itemCount))
            {
                Debug.Log("Item Count:" + itemCount);

                itemDetails = new SteamItemDetails_t[itemCount];
                SteamInventory.GetResultItems(myInventoryHandle, itemDetails, ref itemCount);

                foreach (SteamItemDetails_t itemDetail in itemDetails)
                {
                    string nameValue;
                    string specializationValue;

                    // Note that these property fetchers are different from the ones above. This time they are relying on itemDetail.m_iDefinition for the item ID number. That is because here we are using GetResultItems which returns an array of SteamItemDetails_t

                    propertyValueBuffer = PropertyValueStringLengthMax;
                    SteamInventory.GetItemDefinitionProperty(itemDetail.m_iDefinition, propertyName, out nameValue, ref propertyValueBuffer);

                    propertyValueBuffer = PropertyValueStringLengthMax;
                    SteamInventory.GetItemDefinitionProperty(itemDetail.m_iDefinition, propertySpecialization, out specializationValue, ref propertyValueBuffer);

                    CreateInventoryListItem((int)itemDetail.m_iDefinition, nameValue, specializationValue);
                }
            }
        }
        //
        //


        // Generate Dev items
        //
        public void GenerateDevItems()
        {
            int generateItemID = Convert.ToInt32(GenerateItemsInputText.text);

            foreach (SteamItemDef_t itemIDnumber in pItemDefIDs)
            {
                if ((int)itemIDnumber == generateItemID)
                {
                    generateItemDef = itemIDnumber;
                    Debug.Log("Found item " + generateItemID + " attempting to generate");
                }
            }

            SteamItemDef_t[] generateItemArray = { generateItemDef };

            SteamInventoryResult_t pResultHandle;

            SteamInventory.GenerateItems(out pResultHandle, generateItemArray, null, (UInt32)1);
        }
    }
}