using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Merlin.API.Direct;
using Merlin.Pathing;
using Merlin.Pathing.Worldmap;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;
using System.Reflection;
using Merlin.API;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private WorldPathingRequest _worldTestPathingRequest;
        private ClusterPathingRequest _bankTestPathingRequest;
        private bool _isTestDepositing;

        private bool buildingLoaded = false;
        //timeCooldownMax = DateTime.MaxValue;

        public void tester()
        {

            List<CraftBuildingView> craftBuildings = null;
            craftBuildings = _client.GetEntities<CraftBuildingView>((x) => { return true; });

            for (int i = 0; i < craftBuildings.Count; i++)
            {
                // Core.Log(craftBuildings[i].PrefabName);
            }

            if (GameGui.Instance.CharacterInfoGui.gameObject.activeInHierarchy)
            {
                var playerStorage = GameGui.Instance.CharacterInfoGui.InventoryItemStorage;
                foreach (var slot in playerStorage.ItemsSlotsRegistered)
                    if (slot != null && slot.ObservedItemView != null)
                    {
                        var slotItemName = slot.ObservedItemView.name.ToLowerInvariant();

                        Core.Log($"Item Name {slotItemName} ");

                    }

            }

            var t = new Vector3(195f, -5.8f, -153f);
            var t2D = new Vector2(t.x, t.z);
            Vector2 PC2D = new Vector2(_localPlayerCharacterView.transform.position.x, _localPlayerCharacterView.transform.position.z);

            var distance = Vector2.Distance(t2D, PC2D);

            Core.Log($"Current Pos {_localPlayerCharacterView.transform.position} ");
            Core.Log($"Distance to  {t2D} from {PC2D} is {distance} ");


            _combatPlayer = _localPlayerCharacterView.GetLocalPlayerCharacter();
            _combatSpells = _combatPlayer.GetSpellSlotsIndexed().Ready(_localPlayerCharacterView).Ignore("ESCAPE_DUNGEON").Ignore("PLAYER_COUPDEGRACE").Ignore("AMBUSH");
            foreach (var cs in _combatSpells)
            {
                Core.Log($"{cs.GetSpellDescriptor().TryGetName()}");
                Core.Log($"{cs.GetSpellDescriptor().TryGetTarget()}");
                Core.Log($"{cs.GetSpellDescriptor().TryGetCategory()}");
            }


           //var temp1 = ReadXML();




            if (GameGui.Instance.BankBuildingVaultGui.gameObject.activeInHierarchy)
            {
                //_localPlayerCharacterView.Interact(resource);
               // return;
            
            
            var vaultStorage = GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryStorage;
            var vaultListBox = GameGui.Instance.BankBuildingVaultGui.BankVault.VaultListBox;
            var ToDeposit = new List<UIItemSlot>();

            foreach (var slot in vaultStorage.ItemsSlotsRegistered)
                if (slot != null && slot.ObservedItemView != null)
                {
                    var slotItemName = slot.ObservedItemView.name.ToLowerInvariant();

                    Core.Log($"Item Name {slotItemName} ");

                }

            }

            // Core.Log($"{playerStorage.ItemsSlotsRegistered.Count}");
            //Core.Log($"{vaultStorage.ItemsSlotsRegistered.Count}");




            if (GameGui.Instance.BuildingUsageAndManagementGui.gameObject.activeInHierarchy)
            {

                var craftUsage = GameGui.Instance.BuildingUsageAndManagementGui.BuildingUsage;

                var silverUI = GameGui.Instance.PaySilverDetailGui;


             

                
                
                Core.Log($"There are this many tabs {craftUsage.TabButtons.TabEntries.Count()}");

                //craftUsage.TabButtons.SetSelectedTabButton(2);
                BigTabEntry[] BTE = craftUsage.TabButtons.TabEntries;

                string tabname = "armor";

                foreach (BigTabEntry BT in BTE)
                {
                    Core.Log($"Big tab name {BT.Tooltip.TextToDisplay}");
                    string tabn = BT.Tooltip.TextToDisplay.ToLowerInvariant();


                    //BT.SetSelected(true);
                    craftUsage.OnShowCrafting();
                    if (tabn.Contains(tabname)){

                        Core.Log($"{tabn} contains{BT.Tooltip.TextToDisplay}. Setting tab to display");
                        BT.OnClickTab();
                        craftUsage.UpdateData();

                    }

                   // BT.Tooltip.TextToDisplay;
                }

                if (timeCooldownMax == DateTime.MaxValue)
                {
                    timeCooldownMax = DateTime.Now + TimeSpan.FromSeconds(2);
                }

                if (timeCooldownMax > DateTime.Now)
                {
                    Core.Log($"Time Now {DateTime.Now}");
                    return;
                }


                if (silverUI.gameObject.activeInHierarchy)
                {
                    Core.Log("[Paying silver costs]");
                    silverUI.OnPay();
                    return;
                }


               
                    if (_localPlayerCharacterView.IsCrafting())
                        return;

                    if (!craftUsage.CraftItemView.FilterListToggle.value)
                {

                    craftUsage.CraftItemView.FilterListToggle.value = true;
                    return;
                }
                else
                {


                    KguiCraftItemSlot[] comps = craftUsage.GetComponentsInChildren<KguiCraftItemSlot>();

                    if (craftUsage.CraftItemView.gameObject.activeInHierarchy)
                    {

                        if (craftUsage.CraftItemView.FilterListToggle.value)
                        {

                            if (!GameGui.Instance.ItemCraftingGui.gameObject.activeInHierarchy)
                            {
                                

                                if (comps.Length < 1)
                                {
                                    Core.Log("Cannot craft any items");
                                    return;
                                }
                                Core.Log("Activating CraftingGui");
                                KguiCraftItemSlot craftSlot = comps[comps.Length - 1];

                                Core.Log($"Craft Slots Count {comps.Length}");

                                foreach(var slot in comps)
                                {
                                    Core.Log($"{slot.ItemNameLabel.text}");
                                }


                                try
                                {
                                   

                                    if (comps.Length > 1)
                                    {

                                        for (int i = 0; i<comps.Length;i++ )
                                       
                                        {
                                            string slotLabelName = comps[i].ItemNameLabel.text.ToLowerInvariant();
                                            //string recipeName = craftRec.RecipeName;
                                            string recipeName = "t3_2h_tool_pick";
                                            string[] subRecipeName = recipeName.Split('_');
                                            string keyWord = subRecipeName[subRecipeName.Length - 1];
                                            if (slotLabelName.Contains(keyWord))
                                            {
                                                Core.Log($"{slotLabelName} CONTAINED {keyWord} ");
                                                craftSlot = comps[i];
                                                craftUsage.CraftItemView.OnCraftSlotClicked(craftSlot);
                                               // buildingLoaded = false;
                                                return;
                                            }
                                            else
                                            {
                                                Core.Log($"{slotLabelName} didn't contain {keyWord} ");
                                                //return;
                                            }
                                            // Core.Log($"{slot.ItemNameLabel.text}");
                                        }



                                       // return;
                                    }

                                    craftUsage.CraftItemView.OnCraftSlotClicked(craftSlot);
                                    Core.Log("CraftingGui activated");
                                    return;
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    Core.Log("You dont have the resources");
                                    //TODO add trigger to get the resources
                                }
                                return;
                            }
                            else
                            {
                                if (comps.Length > 0 && !_localPlayerCharacterView.IsCrafting())
                                {
                                    var craftUi = GameGui.Instance.ItemCraftingGui.CraftingView;

                                    if (craftUi.QuantityInput.isActiveAndEnabled)
                                    {
                                        craftUi.QuantityInput.value = 1;
                                    }

                                    var craftUiType = craftUi.GetType();

                                    var onCraftButtonmethod = craftUiType.GetMethod("OnCraftButton", BindingFlags.NonPublic | BindingFlags.Instance);

                                    onCraftButtonmethod.Invoke(craftUi, null);


                                }
                                else
                                {
                                    Core.Log("Crafting Done. Getting Items");
                                    ResetCraftingVariables();
                                    ResetCriticalVariables();

                                    GameGui.Instance.MessageBox.CloseImmediately();
                                    GameGui.Instance.ItemCraftingGui.CraftingView.Close();
                                    _state.Fire(Trigger.GetItems);
                                    Core.Log("Trigger Change");

                                }

                            }
                        }
                        else
                        {
                            Core.Log("not filtered");
                            return;
                        }

                    }

                }


            }
            //isMovingUpdate();
            // bool ismoving = IsMoving;
            // Core.Log($"{ismoving}");

        }

        //  Vector3 facing =  _localPlayerCharacterView.transform.forward;
        // Core.Log($"{facing}");

        // _localPlayerCharacterView.

        //Set this to the transform you want to check
        //private Transform objectTransfom = _localPlayerCharacterView.transform;

   


    
    }
}