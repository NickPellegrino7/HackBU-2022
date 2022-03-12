FIRST!!!
Make sure you have replaced the FizzySteamworks script AppID with your own. FizzySteamworks is attached to the GameManager object.
FIRST!!!

To begin, test that the application runs without errors.

Debug will tell you if Steam is having trouble initializing. This is probably because you are not signed in to Steam.

Make sure you are signed in to Steam before you run the project.

Start the project from the scene: Scene_Steamworks

This scene is the MainMenu.



If you are familiar with Steamworks and are only here for the inventory displayer, you may detach the Inventory canvas, scripts, and InventoryItem prefab to make use outside of this project.



If you are familiar with other Steamworks.NET builds, you will remember the NetworkManager.

The Network Manager script for this build is located in a renamed version of the original object, it is now called: GameManager

This object now contains a C# script named: InventoryManager

The example inventory displayer will be relying on this script to retrieve items as well as organize your display.






Once you have replaced the AppID, you will need to close the project and reopen it.

After you reopen the project, give it a test run and create a lobby to make sure it successfully creates a playable, joinable Steam lobby for your game.

Now that Steam is working with your AppID, we can add an Inventory Service JSON file.

An example JSON file is provided, upload it to your Steamworks App Admin.

Once you have added the items successfully, you should see them in Steamworks Admin, that means they're in the game.




You won't have any of the items until you grant them to yourself.

You can use the GenerateItem input field left of the Inventory to grant yourself items 1, 2, and 3. Just put the numbers in one-by-one and press the button. 

Once you have the items, you should see them in the Inventory after pressing the My Items button.

To view a list of all available items in your game, press Full Inventory, to view only Hats, press the Hats button.

You may customize these filters to your own need within the InventoryManager script, they rely on the JSON line: "specialization" 



