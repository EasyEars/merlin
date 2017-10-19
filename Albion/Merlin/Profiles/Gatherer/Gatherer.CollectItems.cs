using Merlin.API.Direct;
using Merlin.Pathing;
using Merlin.Pathing.Worldmap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private WorldPathingRequest _collectItemsWorldPathingRequest;

        private ClusterPathingRequest _collectItemsBankPathingRequest;
        private bool _isRetrieving;
        int playermaxweight = 1500;
        List<Resource> RL = null;
        List<UIItemSlot> ToRetrieve = null;
        bool sortStack = false;
        List<Resource> initialScan = null;
        List<Resource> afterSplitScan = new List<Resource>();
        List<Resource> resourceCollectList = new List<Resource>();
        bool scanTest = false;
        bool scanTest2 = false;
        private IEnumerator Scan2Coroutine;
        private IEnumerator ScanCoroutine;
        Recipe craftRec = null;
        int splitCounter = 1;
        DateTime stackTimer = DateTime.MinValue;
        bool detailsGui = false;
        DateTime timeCooldownMin = DateTime.MinValue;
        List<string> toCraft = new List<string> { "t3_head_cloth_set1" };
       // List<Tuple<string, int>> toCraftList;
        public void Collect()
        {
            // Core.Log("Here1 ");
            //_state.Fire(Trigger.GotItems);

            var player = _localPlayerCharacterView.GetLocalPlayerCharacter();

            if (!HandleMounting(Vector3.zero))
                return;

            //Core.Log("Here2");
            if (HandlePathing(ref _collectItemsWorldPathingRequest))
                return;
            // Core.Log("Here3");
            if (HandlePathing(ref _collectItemsBankPathingRequest))

                return;
            //Core.Log("Here4");
            API.Direct.Worldmap worldmapInstance = GameGui.Instance.WorldMap;

            Vector3 playerCenter = _localPlayerCharacterView.transform.position;
            ClusterDescriptor currentWorldCluster = _world.GetCurrentCluster();
            ClusterDescriptor townCluster = worldmapInstance.GetCluster(TownClusterNames[_selectedCraftTownClusterIndex]).Info;
            ClusterDescriptor bankCluster = townCluster.GetExits().Find(e => e.GetDestination().GetName().Contains("Bank")).GetDestination();

            if (currentWorldCluster.GetName() == bankCluster.GetName())
            {
                //Core.Log("Collecting Items");
                var banks = _client.GetEntities<BankBuildingView>((x) => { return true; });

                if (banks.Count == 0)
                    return;



                //  Core.Log("Moving To bank");

                _currentTarget = banks.First();
                if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), _currentTarget, IsBlockedWithExitCheck, out List<Vector3> pathing))
                {

                    Core.Log("Moving To bank");
                    _collectItemsBankPathingRequest = new ClusterPathingRequest(_localPlayerCharacterView, _currentTarget, pathing);
                    return;
                }



                if (_currentTarget is BankBuildingView resource)
                {
                    if (!GameGui.Instance.BankBuildingVaultGui.gameObject.activeInHierarchy)
                    {
                        _localPlayerCharacterView.Interact(resource);
                        return;
                    }

                    var playerStorage = GameGui.Instance.CharacterInfoGui.InventoryItemStorage;
                    var playerListBox = GameGui.Instance.CharacterInfoGui.InventoryListBox;
                    var vaultStorage = GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryStorage;
                    var vaultListBox = GameGui.Instance.BankBuildingVaultGui.BankVault.VaultListBox;
                    var ToDeposit = new List<UIItemSlot>();


                    //Get all items we need
                    if (!this.scanTest)
                    {
                        Core.Log("MOVING INTEMS FROM STORAGE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        string resourceTypes = "equipmentitem";
                        List<int> dontMoveSlots = new List<int>{ 0, 1, 2, 3, 4 } ;
                        foreach (var slot in playerStorage.ItemsSlotsRegistered)
                        {

                            if (slot != null && slot.ObservedItemView != null)
                            {
                                var slotItemName = slot.ObservedItemView.name.ToLowerInvariant();
                                // Core.Log(slotItemName);
                                var slotObject = slot.gameObject;
                                var slotPos = playerListBox.GetIndexOfVisibleItemObject(slotObject);
                                Core.Log($"Current Slot Pos {slotPos}");
                                // if (resourceTypes.Any(r => !slotItemName.Contains(r)))
                                if (dontMoveSlots.Contains(slotPos))
                                {
                                    if (slot.GetItemStackSize() > 1)
                                    {
                                        GameGui.Instance.ItemDetailsGui.ItemDetailsWindow.CloseImmediately();
                                        if (!GameGui.Instance.ItemDetailsGui.ItemDetailsView.gameObject.activeInHierarchy)
                                        {
                                            GameGui.Instance.ItemDetailsGui.Show(slot, new Vector3(1, 1, 1));
                                        }
                                        var detailsGui = GameGui.Instance.ItemDetailsGui;

                                        detailsGui.ItemDetailsView.SplitSlider.enabled = true;
                                        detailsGui.ItemDetailsView.SplitButton.enabled = true;

                                        Core.Log($"Itemdetails Gui is for {detailsGui.ItemDetailsWindow.ItemName.text}");

                                        if (detailsGui.ItemDetailsView.SplitButton.isActiveAndEnabled)
                                        {
                                            var slider = GameGui.Instance.ItemDetailsGui.ItemDetailsView.SplitSlider;
                                            detailsGui.ItemDetailsView.SplitSlider.value = 1;
                                            detailsGui.ItemDetailsView.OnClickSplit();
                                            GameGui.Instance.ItemDetailsGui.Close();
                                        }
                                    }
                                }
                                else
                                {
                                    //Core.Log($"Moving {slotPos}");
                                    ToDeposit.Add(slot);
                                }
                            }
                        }
                    _isDepositing = ToDeposit != null && ToDeposit.Count > 0;
                    foreach (var item in ToDeposit)
                    {
                        GameGui.Instance.MoveItemToItemContainer(item, vaultStorage.ItemContainerProxy);
                    }

                    if (_isDepositing)
                        return;
                    else
                    {
                        //Core.Log("Transfer To Storage Done");
                        // _state.Fire(Trigger.BankDone);
                    }
                }


                    if (stackTimer == DateTime.MinValue)
                    {
                        stackTimer = DateTime.Now + TimeSpan.FromSeconds(3);
                    }

                    if (stackTimer > DateTime.Now)
                    {
                        GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryUtil.Stack();
                        GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryUtil.Sort();
                        Core.Log("Stacking");
                    }
                    else
                    {
                        //Core.Log("Stacked");
                        sortStack = true;
                    }


                    if (initialScan == null || initialScan.Count == 0)
                    {
                        Core.Log("MAKING NEW LIST FOR SCANNING");
                        initialScan = new List<Resource>();
                    }


                    if (this.scanTest && sortStack)
                    {
                        //Core.Log("ScanComplete");
                        //Core.Log("stopping Coroutine");
                        StopCoroutine(ScanCoroutine);
                    }
                    else if (sortStack)
                    {
                        ScanCoroutine = scanCheck();
                        StartCoroutine(ScanCoroutine);
                        IEnumerator scanCheck()
                        {
                            for (int ji = 0; ji < 3; ji++)
                            {
                                int oldCount = this.initialScan.Count;
                                this.initialScan = bankstorageScanner(this.initialScan);
                                int newCount = this.initialScan.Count;
                                Core.Log($" At coroutine iteration {ji}");
                                if (ji == 2)
                                {
                                    Core.Log($"Old Count {oldCount} New Count {newCount}");

                                    this.scanTest = (oldCount == newCount);

                                }
                                yield return null;
                            }

                        }
                        return;

                    }
                    else
                    {
                        Core.Log("Not Sorted");
                        return;
                    }

                    if (resourceCollectList == null)
                    {
                        resourceCollectList = new List<Resource>();
                    }
                 

                    Tuple<bool, List<Resource>> ResourceCount(List<Resource> RL, List<Resource> IS, string RecipeName)
                    {
                        bool canCraft = true;

                        for (int i = 0; i < RL.Count; i++)
                        {
                            string resname = RL[i].Name;

                            foreach (Resource R in IS)
                            {
                                if (R.Name.Contains(resname) && !R.Name.Contains("level"))
                                {
                                    RL[i].TotalAmount = RL[i].TotalAmount + R.StackSize;
                                }

                            }
                            Core.Log($"The Count of {RL[i].Name} is {RL[i].TotalAmount}. The base amount for {RecipeName} is {RL[i].BaseAmountForSelectedRecipe} ");
                            if (RL[i].TotalAmount == 0 || RL[i].TotalAmount < RL[i].BaseAmountForSelectedRecipe)
                            {
                                Core.Log($"Not Enough / Missing { RL[i].Name}");
                                canCraft = false;
                            }
                            else
                            {
                                Core.Log($"There is enough {RL[i].Name}. There is {RL[i].TotalAmount}");
                                RL[i].Available = true;
                            }
                        }
                        Core.Log("Resource Count Complete");
                       return new Tuple<bool, List<Resource>>(canCraft, RL);

                    }

                    


                    // Core.Log("here1");
                    List<Recipe> currentRecipes = getCurrentRecipes();
                    // Core.Log("here2");
                    
                    // Core.Log("here3");



                     

                    
                    if (this.craftRec == null || !this.craftRec.ResourceListUpdated)
                    {
                        //List<Resource> resList = craftRec.Resources;

                        // Core.Log("here4");




                        foreach(Tuple<string, int> T in toCraftList)
                        {

                            Core.Log(T.Item1 + "\t");
                        }


                        for (int i = 0; i < toCraftList.Count; i++)
                        {
                            Core.Log("Getting resource list from recipe names");
                            Tuple<string, int> TCL = toCraftList[i];
                            this.craftRec = currentRecipes.First(R => R.RecipeName.Contains(toCraftList[i].Item1));
                            var result = ResourceCount(craftRec.Resources, initialScan, this.craftRec.RecipeName);

                            this.craftRec.AmountToCraft = TCL.Item2;

                            bool resourceAvailables = result.Item1;
                            this.craftRec.Resources = result.Item2;
                            Core.Log($"Can craft recipe {craftRec.RecipeName}?: {resourceAvailables}");
                            if (!resourceAvailables)
                            { 
                                //Core.Log($"Not Enough ingredients to craft {craftRec.RecipeName}");

                                foreach (Resource CR in craftRec.Resources)
                                {
                                    if (!CR.Available) { 

                                        if (currentRecipes.Any(R => R.RecipeName.Contains(CR.Name)))
                                        {
                                            var recipe = currentRecipes.First(R => R.RecipeName.Contains(CR.Name));
                                            if (!toCraftList.Any(R => R.Item1 == recipe.RecipeName))
                                            {
                                                Core.Log($"Adding {recipe.RecipeName} to craft list. Need {CR.BaseAmountForSelectedRecipe * craftRec.AmountToCraft}");

                                                toCraftList.Add(new Tuple<string, int>(recipe.RecipeName, CR.BaseAmountForSelectedRecipe * craftRec.AmountToCraft));
                                            }
                                            else
                                            {
                                                Core.Log($"Recipe for {recipe.RecipeName} is already in the craft list");
                                            }

                                           // toCraft.Add(recipe.RecipeName);
                                            //return;
                                        }
                                        else
                                        {
                                            if (i == toCraftList.Count-1)
                                            {

                                                //Core.Log($"No recipe available for {CR.Name}");
                                                Core.Log($"Cannot Craft any recipes");
                                                _state.Fire(Trigger.CraftDone);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Core.Log($"There is enough {CR.Name} available");
                                    }
                                    

                                }
                            
                            }
                            else
                            {
                                Core.Log($"Gathering resources to craft {craftRec.RecipeName}");
                                break;
                            }
                            // this.craftRec = null;

                        }




                        float totalRecipeWeight = 0;

                        for (int i = 0; i < this.craftRec.Resources.Count; i++)
                        {
                            //  Core.Log("here5");
                            Resource tempR = null;
                            for (int j = 0; j < initialScan.Count; j++)
                            {

                                if (initialScan[j].Name.Contains(this.craftRec.Resources[i].Name) && !initialScan[j].Name.Contains("level"))
                                {
                                    tempR = initialScan[j];
                                    break;
                                }
                            }

                            //this.craftRec.Resources[i].BaseAmountForSelectedRecipe = this.craftRec.ResourceRatio[i];
                            if (tempR != null)
                            {
                                this.craftRec.Resources[i].Weight = tempR.Weight;
                                // Core.Log("here6");
                                totalRecipeWeight = totalRecipeWeight + this.craftRec.Resources[i].Weight * this.craftRec.Resources[i].BaseAmountForSelectedRecipe;
                                this.craftRec.Resources[i].SlotPos = tempR.SlotPos;
                            }


                        }
                        Core.Log($"Recipe weight is {totalRecipeWeight}");
                        this.craftRec.TotalWeight = totalRecipeWeight;
                        float craftCount;
                        if (craftRec.AmountToCraft < 0)
                        {
                            craftCount = (float)Math.Floor(playermaxweight / this.craftRec.TotalWeight);
                        }
                        else if ((float)Math.Floor(playermaxweight / this.craftRec.TotalWeight) < craftRec.AmountToCraft)
                        {
                            craftCount = (float)Math.Floor(playermaxweight / this.craftRec.TotalWeight);
                            
                        }
                        else {
                            craftCount = craftRec.AmountToCraft;
                        }


                        foreach (Resource R in this.craftRec.Resources)
                        {
                            Core.Log($"{R.Name} initially has {R.TotalAmount}");
                        }

                        //check for resources in storage and calculate total amounts of each resource
                       

                        //Core.Log("here7");

                        float limitingResourceCount = 1e20f;
                        Resource limitingResource = null;
                        foreach (Resource R in this.craftRec.Resources)
                        {
                            Core.Log($"{R.Name} has {R.TotalAmount} in total");
                            R.MaxAmountForselectedRecipe = craftCount * R.BaseAmountForSelectedRecipe;
                            if ((R.TotalAmount  - (craftCount * R.BaseAmountForSelectedRecipe)) < limitingResourceCount)
                            {
                                limitingResource = R;
                                limitingResourceCount = R.TotalAmount - (craftCount * R.BaseAmountForSelectedRecipe);
                            }
                        }

                        Core.Log($"Limiting amount of resources is {limitingResourceCount} from {limitingResource.Name}");
                        //check if weight is the limiting factor
                        if (limitingResourceCount < 0)
                        {
                            //limited by resources calculated new craftCount
                            Core.Log("Resources Limited Number Available");
                            craftCount = (float) Math.Floor(limitingResource.TotalAmount / limitingResource.BaseAmountForSelectedRecipe);
                        }


                        
                       


                        for (int i = 0; i < this.craftRec.Resources.Count; i++)
                        {
                            Core.Log($"Craft Count is {craftCount}");

                            this.craftRec.Resources[i].CurrentAmountNeeded = craftCount * this.craftRec.Resources[i].BaseAmountForSelectedRecipe;
                            Resource CR = this.craftRec.Resources[i];

                            Core.Log($"Need {CR.CurrentAmountNeeded} of {CR.Name} in total");

                            for (int j = 0; j < initialScan.Count(); j++)
                            {
                                Resource BR = initialScan[j];

                                if (BR.Name.Contains(CR.Name) && !BR.Name.Contains("level"))
                                {
                                    if (CR.CurrentAmountNeeded <= 0)
                                    {
                                        Core.Log("Resource Gathered list Complete");
                                        break;
                                    }


                                    if (BR.StackSize <= CR.CurrentAmountNeeded)
                                    {
                                        Core.Log($"Stack size {BR.StackSize} of {CR.Name} in slot {BR.SlotPos}");


                                        BR.NeededSplitting = false;
                                        BR.Split = true;
                                        BR.SplitAmount = BR.StackSize;
                                        BR.Name = CR.Name;
                                        CR.CurrentAmountNeeded = CR.CurrentAmountNeeded - BR.StackSize;
                                         Core.Log($"Need {CR.CurrentAmountNeeded} more of {CR.Name}. Last Split amount {BR.SplitAmount}");
                                        resourceCollectList.Add(BR);
                                    }
                                    else
                                    {
                                        Core.Log($"Stack size {BR.StackSize} of {CR.Name} in slot {BR.SlotPos}");
                                        BR.SplitAmount = CR.CurrentAmountNeeded;
                                        BR.Name = CR.Name;
                                        CR.CurrentAmountNeeded = CR.CurrentAmountNeeded - BR.SplitAmount;
                                        Core.Log($"Need {CR.CurrentAmountNeeded} more of {CR.Name}. Last Split amount {BR.SplitAmount}");
                                        resourceCollectList.Add(BR);

                                    }
                                
                                }


                            }

                        }




                        //for (int i = 0; i < this.craftRec.Resources.Count; i++)
                        //{
                        //    craftRec.Resources[i].SplitAmount = (float)Math.Floor(craftCount * this.craftRec.Resources[i].BaseAmountForSelectedRecipe);
                        //    if (this.craftRec.Resources[i].SplitAmount == this.craftRec.Resources[i].TotalAmount)
                        //    {
                        //         Core.Log("This resource doesnt need splitting");
                        //        this.craftRec.Resources[i].NeededSplitting = false;
                        //        this.craftRec.Resources[i].Split = true;
                        //    }
                        //}


                        this.craftRec.ResourceListUpdated = true;
                        foreach (Resource R in resourceCollectList)
                        {
                            Core.Log($"Need {R.SplitAmount} of {R.Name}");
                        }
                    }
                    //Get inventory
                    playerStorage = GameGui.Instance.CharacterInfoGui.InventoryItemStorage;
                    vaultStorage = GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryStorage;
                    vaultListBox = GameGui.Instance.BankBuildingVaultGui.BankVault.VaultListBox;

                    int arrayLength = vaultStorage.ItemsSlotsRegistered.Count;
                    int firstItem = vaultListBox.FirstVisibleItem;
                    int lastItem = vaultListBox.LastVisibleItem;


                    bool allSplit = resourceCollectList.All<Resource>(r => r.Split == true);


                    if (!allSplit)
                    {
                        //coroutine = itemSplit();
                        //StartCoroutine(coroutine);
                        //vaultListBox.SetFirstVisibleRowVertical(2);

                        itemSplit();
                        return;
                    }
                    else
                    {
                        //Core.Log("stopping Coroutine");
                        //StopCoroutine(coroutine);
                    }


                    //IEnumerator 
                    void itemSplit()
                    {
                        for (int i = 0; i < resourceCollectList.Count; i++)
                        {
                            if (resourceCollectList[i].Split)
                            {
                                Core.Log($"{resourceCollectList[i].Name} is Split");
                                // GameGui.Instance.CloseAllGui();
                            }
                            else
                            {
                                if ((firstItem < resourceCollectList[i].SlotPos && lastItem > resourceCollectList[i].SlotPos))
                                {

                                    foreach (var slot in vaultStorage.ItemsSlotsRegistered)
                                        if (slot != null && slot.ObservedItemView != null)
                                        {

                                            var slotItemName = slot.ObservedItemView.name.ToLowerInvariant();

                                            int stackSize = slot.GetItemStackSize();
                                            var slotObject = slot.gameObject;
                                            var slotPos = vaultListBox.GetIndexOfVisibleItemObject(slotObject);

                                            if (slotItemName.Contains(resourceCollectList[i].Name) && !slotItemName.Contains("level") && resourceCollectList[i].SplitAmount < stackSize )
                                            {
                                                Core.Log($"Name Found. {resourceCollectList[i].Name} was contained in {slotItemName}.  ");


                                                if (slotPos == resourceCollectList[i].SlotPos)
                                                { 


                                                GameGui.Instance.ItemDetailsGui.ItemDetailsWindow.CloseImmediately();
                                                if (!GameGui.Instance.ItemDetailsGui.ItemDetailsView.gameObject.activeInHierarchy)
                                                {
                                                    GameGui.Instance.ItemDetailsGui.Show(slot, new Vector3(1, 1, 1));
                                                    //return;

                                                }
                                                var detailsGui = GameGui.Instance.ItemDetailsGui;

                                                detailsGui.ItemDetailsView.SplitSlider.enabled = true;
                                                detailsGui.ItemDetailsView.SplitButton.enabled = true;



                                                Core.Log($"Itemdetails Gui is for {detailsGui.ItemDetailsWindow.ItemName.text}");

                                                if (detailsGui.ItemDetailsView.SplitButton.isActiveAndEnabled && !resourceCollectList[i].Split)
                                                {
                                                    float sliderValue = (resourceCollectList[i].SplitAmount) / (stackSize - 1);

                                                    var slider = GameGui.Instance.ItemDetailsGui.ItemDetailsView.SplitSlider;

                                                    var sliderType = slider.GetType();

                                                    detailsGui.ItemDetailsView.SplitSlider.value = sliderValue;



                                                    detailsGui.ItemDetailsView.OnClickSplit();
                                                    resourceCollectList[i].Split = true;
                                                    Core.Log($"{slotItemName} Split into {sliderValue * (stackSize - 1)} from slot {slotPos}");
                                                    resourceCollectList[i].SlotPos = initialScan.Count + splitCounter;
                                                    splitCounter++;
                                                    Core.Log($"{slotItemName} Split into {sliderValue * (stackSize - 1)} from slot {slotPos}. Split item slot pos is {resourceCollectList[i].SlotPos}");
                                                    GameGui.Instance.ItemDetailsGui.Close();
                                                    //GameGui.Instance.CloseAllGui();
                                                    vaultListBox.Panel.ResetPosition();
                                                    return;
                                                }
                                            }
                                            }
                                        }
                                }
                                else
                                {
                                    int row = (int)Math.Floor((double)resourceCollectList[i].SlotPos / 5) - 2;
                                    vaultListBox.SetFirstVisibleRowVertical(row);
                                    Core.Log($"Moving slider to row {row}");
                                    //yield break;
                                    return;
                                }
                            }
                            // yield return new WaitForSeconds(.1f);
                        }
                    }


                    if (timeCooldownMin > DateTime.Now)
                    {
                        return;
                    }


                    bool allRetrieved = resourceCollectList.All<Resource>(r => r.Retrieved == true);
                    bool allMoved = resourceCollectList.All<Resource>(r => r.Moved == true);

                    if (ToRetrieve == null || ToRetrieve.Count == 0)
                    {
                        ToRetrieve = new List<UIItemSlot>();
                    }

                  //  Core.Log($"Slots to retrieve length is {ToRetrieve.Count}");

                    foreach (Resource R in resourceCollectList)
                    {
                       // Core.Log($"Resource Name {R.Name}");
                      //  Core.Log($"Split: {R.Split} Retrieved: {R.Retrieved} Moved: {R.Moved}");
                    }
                   // Core.Log($"All Split: {allSplit} All Retrieved: {allRetrieved} All Moved: {allMoved}");



                    Core.Log($"Resource Count {resourceCollectList.Count}");
                    if (allSplit && !allRetrieved)
                    {
                        for (int i = 0; i < resourceCollectList.Count; i++)
                        {
                          //  Core.Log($"Count i = {i}");

                           // Core.Log($"Resource Name {resourceCollectList[i].Name}");

                            if (resourceCollectList[i].Retrieved)
                            {
                             //   Core.Log("Item is retrieved");

                            }
                            else
                            {
                                Core.Log("Item NOT retrieved");
                                firstItem = vaultListBox.FirstVisibleItem;
                                lastItem = vaultListBox.LastVisibleItem;
                                //if (lastItem >= vaultStorage.ItemsSlotsRegistered.Count - 2)
                                //{
                                //    vaultListBox.Panel.ResetPosition();
                                //    Core.Log("Resetting Panel Slider");
                                //}
                                //else
                                //{
                                //    vaultListBox.Panel.currentMomentum = new Vector3(0f, .5f, 0f);

                                //}

                                if ((firstItem < resourceCollectList[i].SlotPos && lastItem > resourceCollectList[i].SlotPos))
                                {

                                    vaultStorage.UpdateStorage();
                                    playerStorage.UpdateStorage();
                                    foreach (var slot in vaultStorage.ItemsSlotsRegistered)
                                        if (slot != null && slot.ObservedItemView != null)
                                        {

                                            var slotItemName = slot.ObservedItemView.name.ToLowerInvariant();
                                            var slotObject = slot.gameObject;
                                            var slotPos = vaultListBox.GetIndexOfVisibleItemObject(slotObject);

                                            if (slotItemName.Contains(resourceCollectList[i].Name) && !slotItemName.Contains("level"))
                                            {

                                                Core.Log($"SECOND TIME: Name Found. {resourceCollectList[i].Name} was contained in {slotItemName} ");

                                                Core.Log($"Slot stack Size {slot.GetItemStackSize()}. Required resource stack size {resourceCollectList[i].SplitAmount}");

                                                if (slot.GetItemStackSize() < resourceCollectList[i].SplitAmount + 1 && slot.GetItemStackSize() > resourceCollectList[i].SplitAmount - 1)
                                                {




                                                    GameGui.Instance.MoveItemToItemContainer(slot, playerStorage.ItemContainerProxy);

                                                    _localPlayerCharacterView.RequestMove(playerCenter + _localPlayerCharacterView.transform.forward * -1);
                                                    resourceCollectList[i].Retrieved = true;
                                                    timeCooldownMin = DateTime.Now + TimeSpan.FromSeconds(1);
                                                   
                                                    return;
                                                    // break;

                                                }

                                            }

                                        }
                                }
                                else
                                {
                                    int row = (int)Math.Floor((double)resourceCollectList[i].SlotPos / 5) - 2;
                                    vaultListBox.SetFirstVisibleRowVertical(row);
                                    Core.Log($"Moving slider to row {row}");
                                    //yield break;
                                    timeCooldownMin = DateTime.Now + TimeSpan.FromSeconds(1);
                                    return;
                                }


                            }
                        }
                    }

                   
                        if(allRetrieved)
                        {
                            _state.Fire(Trigger.GotItems);
                            _localPlayerCharacterView.CreateTextEffect("Finished banking");
                            Core.Log("[Bank Done]");
                            ResetCriticalVariables();
                            // ResetCraftingVariables();

                        }





                    }
            }
            else
            {
                _localPlayerCharacterView.CreateTextEffect($"Finding Path to Bank in {bankCluster.GetName()}");
                var pathfinder = new WorldmapPathfinder();
                if (pathfinder.TryFindPath(currentWorldCluster, bankCluster, StopClusterFunction, out var path, out var pivots))
                    _collectItemsWorldPathingRequest = new WorldPathingRequest(currentWorldCluster, bankCluster, path, _skipUnrestrictedPvPZones);
            }
        }

        List<Resource> bankstorageScanner(List<Resource> resourceList)
        {
            var bankVault = GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryStorage;
            var bankListBox = GameGui.Instance.BankBuildingVaultGui.BankVault.VaultListBox;

            int arrayLength = bankVault.ItemsSlotsRegistered.Count;
            int firstItem = bankListBox.FirstVisibleItem;
            int lastItem = bankListBox.LastVisibleItem;
            // Core.Log("inside1");
            int maxSlotPos = 0;
            if (resourceList != null && resourceList.Count != 0)
            {
                maxSlotPos = resourceList.Max<Resource>(r => r.SlotPos);
                Core.Log($" Max Slot pos{ maxSlotPos}");
            }
            Core.Log($"First item {firstItem}");

            Core.Log($"{arrayLength}");
            if (maxSlotPos > firstItem + 5)
            {
                int row = 1 + (int)Math.Floor((double)maxSlotPos / 5);
                bankListBox.SetFirstVisibleRowVertical(row);
                Core.Log($"moving slider to row {row}");
                return resourceList;
            }

            bankVault = GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryStorage;
            bankListBox = GameGui.Instance.BankBuildingVaultGui.BankVault.VaultListBox;




            for (int i = 0; i < arrayLength; i++)
            {
                var slot = bankVault.ItemsSlotsRegistered[i];

                if (slot == null)
                {
                    //Core.Log($"slot {i} is null");
                }


                if (slot != null && slot.ObservedItemView != null)
                {
                    var slotItemName = slot.ObservedItemView.name.ToLowerInvariant();
                    var slotObject = slot.gameObject;
                    var slotPos = bankListBox.GetIndexOfVisibleItemObject(slotObject);

                    Resource tR = new Resource(slotItemName);
                    GameGui.Instance.ItemDetailsGui.Show(slot, new Vector3(1, 1, 1));
                    //Core.Log("here");
                    string W = GameGui.Instance.ItemDetailsGui.ItemDetailsWindow.WeightLabel.text;
                    // Core.Log(W);

                    String[] subs = W.Split('k');
                    W = subs[0];

                    GameGui.Instance.ItemDetailsGui.Close();
                    GameGui.Instance.ItemDetailsGui.ItemDetailsWindow.CloseImmediately();

                    int stackSize = slot.GetItemStackSize();
                    tR.StackSize = stackSize;

                    tR.Weight = (Convert.ToSingle(W)) / ((float)stackSize);
                    tR.SlotPos = slotPos;


                    if (resourceList != null && resourceList.Count != 0)
                    {
                        bool slotObtained = resourceList.Any<Resource>(r => r.SlotPos == slotPos);

                        //Core.Log($"Slot obtained {slotObtained}");


                        if (!slotObtained)
                        {
                            resourceList.Add(tR);
                        }
                        else
                        {
                            Core.Log($"Already taken {slotPos}");
                        }

                    }
                    else
                    {
                        resourceList.Add(tR);
                    }




                }

            }
            Core.Log($"There are this many resources {resourceList.Count}");
            return resourceList;

        }




    }
}