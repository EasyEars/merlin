using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    class Class1
    {


        List<Resource> bankstorageScanner( List<Resource> resourceList)
        {
            var bankVault = GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryStorage;
            var bankListBox = GameGui.Instance.BankBuildingVaultGui.BankVault.VaultListBox;
            int arrayLength = bankVault.ItemsSlotsRegistered.Count;
            int firstItem = bankListBox.FirstVisibleItem;
            int lastItem = bankListBox.LastVisibleItem;
            int maxSlotPos = resourceList.Max<Resource>(r => r.SlotPos);

            if (maxSlotPos > firstItem)
            {
                int row = (int) Math.Floor((double) maxSlotPos / 5);
                bankListBox.SetFirstVisibleRowVertical(row);
            }
            


            for (int i = 0; i < arrayLength; i++)
            {
                var slot = bankVault.ItemsSlotsRegistered[i];
                if (slot != null && slot.ObservedItemView != null)
                {
                    var slotItemName = slot.ObservedItemView.name.ToLowerInvariant();
                    var slotObject = slot.gameObject;
                    var slotPos = bankListBox.GetIndexOfVisibleItemObject(slotObject);

                    Resource tR = new Resource(slotItemName);
                    GameGui.Instance.ItemDetailsGui.Show(slot, new Vector3(1, 1, 1));
                    string W = GameGui.Instance.ItemDetailsGui.ItemDetailsWindow.WeightLabel.text;
                    GameGui.Instance.ItemDetailsGui.Close();
                    GameGui.Instance.ItemDetailsGui.ItemDetailsWindow.CloseImmediately();
                    tR.Weight = Convert.ToSingle(W);
                    tR.SlotPos = slotPos;
                   


                    bool slotObtained = resourceList.Any<Resource>(r => r.SlotPos == slotPos);

                    if (!slotObtained)
                    {
                        resourceList.Add(tR);
                    }
                    

                }

            }

            return resourceList;

        }



    }
}
