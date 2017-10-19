using Merlin.API.Direct;
using Merlin.Pathing;
using Merlin.Pathing.Worldmap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private WorldPathingRequest _worldPathingRequest;
        private ClusterPathingRequest _bankPathingRequest;
        private bool _isDepositing;

        public void Bank()
        {
            var player = _localPlayerCharacterView.GetLocalPlayerCharacter();

            if (!HandleMounting(Vector3.zero))
                return;

            //if (!_isDepositing && _localPlayerCharacterView.GetLoadPercent() <= _percentageForBanking)
            //{
            //    Core.Log("[Restart]");
            //    _state.Fire(Trigger.Restart);
            //    return;
            //}

            if (HandlePathing(ref _worldPathingRequest))
                return;

            if (HandlePathing(ref _bankPathingRequest))
                return;

            API.Direct.Worldmap worldmapInstance = GameGui.Instance.WorldMap;

            Vector3 playerCenter = _localPlayerCharacterView.transform.position;
            ClusterDescriptor currentWorldCluster = _world.GetCurrentCluster();
            ClusterDescriptor townCluster = worldmapInstance.GetCluster(TownClusterNames[_selectedTownClusterIndex]).Info;
            ClusterDescriptor bankCluster = townCluster.GetExits().Find(e => e.GetDestination().GetName().Contains("Bank")).GetDestination();

            if (currentWorldCluster.GetName() == bankCluster.GetName())
            {
                var banks = _client.GetEntities<BankBuildingView>((x) => { return true; });

                if (banks.Count == 0)
                    return;

                _currentTarget = banks.First();
                if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), _currentTarget, IsBlockedWithExitCheck, out List<Vector3> pathing))
                {
                    _bankPathingRequest = new ClusterPathingRequest(_localPlayerCharacterView, _currentTarget, pathing);
                    return;
                }

                if (_currentTarget is BankBuildingView resource)
                {
                    if (!GameGui.Instance.BankBuildingVaultGui.gameObject.activeInHierarchy)
                    {
                        _localPlayerCharacterView.Interact(resource);
                        return;
                    }

                    //Get inventory
                    var playerStorage = GameGui.Instance.CharacterInfoGui.InventoryItemStorage;
                    var vaultStorage = GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryStorage;
                    var playerListBox = GameGui.Instance.CharacterInfoGui.InventoryListBox;
                    var ToDeposit = new List<UIItemSlot>();

                    //Get all items we need
                    //    var resourceTypes = Enum.GetNames(typeof(ResourceType)).Select(r => r.ToLowerInvariant()).ToArray();
                    //    foreach (var slot in playerStorage.ItemsSlotsRegistered)
                    //        if (slot != null && slot.ObservedItemView != null)
                    //        {
                    //            var slotItemName = slot.ObservedItemView.name.ToLowerInvariant();
                    //            if (resourceTypes.Any(r => slotItemName.Contains(r)))
                    //                ToDeposit.Add(slot);
                    //        }

                    //    _isDepositing = ToDeposit != null && ToDeposit.Count > 0;
                    //    foreach (var item in ToDeposit)
                    //    {
                    //        GameGui.Instance.MoveItemToItemContainer(item, vaultStorage.ItemContainerProxy);
                    //    }

                    //    if (_isDepositing)
                    //        return;
                    //    else
                    //    {
                    //        Core.Log("[Bank Done]");
                    //        _state.Fire(Trigger.BankDone);
                    //    }
                    //}

                    List<int> dontMoveSlots = new List<int> { 0, 1, 2, 3, 4 };
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
                    {
                        return;
                    }
                    else
                    {
                        Core.Log("Transfer To Storage Done");
                         _state.Fire(Trigger.BankDone);
                    }
                }

            }
            else
            {
                var pathfinder = new WorldmapPathfinder();
                if (pathfinder.TryFindPath(currentWorldCluster, bankCluster, StopClusterFunction, out var path, out var pivots))
                    _worldPathingRequest = new WorldPathingRequest(currentWorldCluster, bankCluster, path, _skipUnrestrictedPvPZones);
            }
        }
    }
}