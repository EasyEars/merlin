using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private static string _prefsIdentifier = "gath_";
        private static string _craftPrefsIdentifier = "toCraft_";

        private bool _allowMobHunting;
        private bool _skipUnrestrictedPvPZones;
        private bool _skipKeeperPacks;
        private bool _allowSiegeCampTreasure;
        private bool _skipRedAndBlackZones;
        private float _keeperSkipRange;
        private float _minimumHealthForGathering;
        private float _percentageForBanking;
        private float _percentageForSiegeCampTreasure;
        private string _selectedGatherCluster;
        
        private int _selectedTownClusterIndex;
        private int _selectedMininumTierIndex;
        private Dictionary<GatherInformation, bool> _gatherInformations;

        private string _selectedCraftCluster;
        private int _selectedCraftTownClusterIndex;
        private List<Tuple<string, int>> toCraftList = new List<Tuple<string, int>>();
        private int _toCraftListCount;
        private void LoadSettings()
        {
            _allowMobHunting = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_allowMobHunting)}", bool.FalseString));
            _skipUnrestrictedPvPZones = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_skipUnrestrictedPvPZones)}", bool.TrueString));
            _skipKeeperPacks = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_skipKeeperPacks)}", bool.TrueString));
            _allowSiegeCampTreasure = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_allowSiegeCampTreasure)}", bool.TrueString));
            _skipRedAndBlackZones = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_skipRedAndBlackZones)}", bool.TrueString));
            _keeperSkipRange = PlayerPrefs.GetFloat($"{_prefsIdentifier}{nameof(_keeperSkipRange)}", 22);
            _minimumHealthForGathering = PlayerPrefs.GetFloat($"{_prefsIdentifier}{nameof(_minimumHealthForGathering)}", 0.8f);
            _percentageForBanking = PlayerPrefs.GetFloat($"{_prefsIdentifier}{nameof(_percentageForBanking)}", 99f);
            _percentageForSiegeCampTreasure = PlayerPrefs.GetFloat($"{_prefsIdentifier}{nameof(_percentageForSiegeCampTreasure)}", 33f);
            _selectedGatherCluster = PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_selectedGatherCluster)}", null);
            _selectedTownClusterIndex = PlayerPrefs.GetInt($"{_prefsIdentifier}{nameof(_selectedTownClusterIndex)}", 0);
            _selectedMininumTierIndex = PlayerPrefs.GetInt($"{_prefsIdentifier}{nameof(_selectedMininumTierIndex)}", 0);

            _selectedCraftCluster = PlayerPrefs.GetString($"{_craftPrefsIdentifier}{nameof(_selectedCraftCluster)}", null);
            _selectedCraftTownClusterIndex = PlayerPrefs.GetInt($"{_craftPrefsIdentifier}{nameof(_selectedCraftTownClusterIndex)}", 0);

            _gatherInformations = new Dictionary<GatherInformation, bool>();
            foreach (var resourceType in Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>())
                foreach (var tier in Enum.GetValues(typeof(Tier)).Cast<Tier>())
                    foreach (var enchantment in Enum.GetValues(typeof(EnchantmentLevel)).Cast<EnchantmentLevel>())
                    {
                        if ((tier < Tier.IV || resourceType == ResourceType.Rock) && enchantment != EnchantmentLevel.White)
                            continue;

                        var info = new GatherInformation(resourceType, tier, enchantment);
                        var val = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{info.ToString()}", (tier >= Tier.II).ToString()));
                        _gatherInformations.Add(info, val);
                    }
            //  foreach (var processedResourceType in Enum.GetValues(typeof(ProcessedResourceType)).Cast<ProcessedResourceType>)
            _toCraftListCount = PlayerPrefs.GetInt($"{_craftPrefsIdentifier}_toCraftListLength", 0);
            Core.Log($"Loading to craft list Settings {_toCraftListCount}");
            if (_toCraftListCount > 0)
            {
                for (int i = 0; i < _toCraftListCount ; i++)
                {
                    string S = PlayerPrefs.GetString($"{_craftPrefsIdentifier}{i}_1", null);
                    int I = PlayerPrefs.GetInt($"{_craftPrefsIdentifier}{i}_2", -1);
                    Core.Log($"{S}{I}");

                    Tuple<string, int> T = new Tuple<string, int>(S, I);
                    toCraftList.Add(T);
                }
            }

        }

        private void SaveSettings()
        {
            PlayerPrefs.SetString($"{_craftPrefsIdentifier}{nameof(_selectedCraftCluster)}", _selectedCraftCluster);
            PlayerPrefs.SetInt($"{_craftPrefsIdentifier}{nameof(_selectedCraftTownClusterIndex)}", _selectedCraftTownClusterIndex);
            PlayerPrefs.SetInt($"{_craftPrefsIdentifier}_toCraftListLength",toCraftList.Count);
            Core.Log($"SAving to craft list Settings {toCraftList.Count}");
            for (int i=0;i<toCraftList.Count;i++) {
                PlayerPrefs.SetString($"{_craftPrefsIdentifier}{i}_1", toCraftList[i].Item1);
                PlayerPrefs.SetInt($"{_craftPrefsIdentifier}{i}_2", toCraftList[i].Item2);
            }

            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_allowMobHunting)}", _allowMobHunting.ToString());
            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_skipUnrestrictedPvPZones)}", _skipUnrestrictedPvPZones.ToString());
            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_skipKeeperPacks)}", _skipKeeperPacks.ToString());
            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_allowSiegeCampTreasure)}", _allowSiegeCampTreasure.ToString());
            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_skipRedAndBlackZones)}", _skipRedAndBlackZones.ToString());
            PlayerPrefs.SetFloat($"{_prefsIdentifier}{nameof(_keeperSkipRange)}", _keeperSkipRange);
            PlayerPrefs.SetFloat($"{_prefsIdentifier}{nameof(_minimumHealthForGathering)}", _minimumHealthForGathering);
            PlayerPrefs.SetFloat($"{_prefsIdentifier}{nameof(_percentageForBanking)}", _percentageForBanking);
            PlayerPrefs.SetFloat($"{_prefsIdentifier}{nameof(_percentageForSiegeCampTreasure)}", _percentageForSiegeCampTreasure);
            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_selectedGatherCluster)}", _selectedGatherCluster);
            PlayerPrefs.SetInt($"{_prefsIdentifier}{nameof(_selectedTownClusterIndex)}", _selectedTownClusterIndex);
            PlayerPrefs.SetInt($"{_prefsIdentifier}{nameof(_selectedMininumTierIndex)}", _selectedMininumTierIndex);
            foreach (var kvp in _gatherInformations)
                PlayerPrefs.SetString($"{_prefsIdentifier}{kvp.Key.ToString()}", kvp.Value.ToString());
        }
    }
}