using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GUILayout;

namespace Merlin.Profiles.Gatherer
{
    public partial class Gatherer
    {
        #region Fields

        static int SpaceBetweenSides = 40;
        static int SpaceBetweenItems = 4;

        bool _isUIshown;
        bool _isUIcraftshown;
        bool _showESP;

        public Dropdown craftList;

        Tuple<Vector2, int, int> shopcategoryScrollView = new Tuple<Vector2, int, int>(Vector2.zero, 0, 0);
        Tuple<Vector2, int, int> shopsubcategory1ScrollView = new Tuple<Vector2, int, int>(Vector2.zero, 0, 0);
        Tuple<Vector2, int, int> tierScrollView = new Tuple<Vector2, int, int>(Vector2.zero, 0, 0);
        Tuple<Vector2, int, int> maxqualitylevelScrollView = new Tuple<Vector2, int, int>(Vector2.zero, 0, 0);

        List<Tuple<string, string>> filterList;

        string[] shopsubcategory1 = new string[] { "Values" };

        //string[] tier = new string[] { "Values" };

        string[] maxqualitylevel = new string[] { "Values" };

        // string[] enchantmentlevel;


        //   GameGui


        #endregion Fields

        #region Properties

        // int [] enchantmentlevel { get { return Enum.GetValues(typeof(EnchantmentLevel)).ToArray(); } }

        int[] enchantmentlevel = new int[] { 0, 1, 2, 3 };
        string[] qualitylevel = new string[] { "Normal", "Good", "G2", "G3", "G4", "G5" };

        //static string[] tier { get { return Enum.GetNames(typeof(Tier)).Select(n => n.Insert(0, "Tier ")).ToArray(); } }
        string[] tier = { "All", "Tier 1", "Tier 2", "Tier 3", "Tier 4", "Tier 5", "Tier 6", "Tier 7", "Tier 8" };
       // int tierLength = tier.Length + 1;
       

        // foreach (var tier in Enum.GetValues(typeof(Tier)).Cast<Tier>())
        static Rect UnloadButtonRect { get; } = new Rect((Screen.width / 2) - 300, 0, 100, 20);
        static Rect GatheringUiButtonRect { get; } = new Rect((Screen.width / 2) - 200, 0, 100, 20);
        static Rect GatheringBotButtonRect { get; } = new Rect((Screen.width / 2) - 100, 0, 100, 20);
        static Rect CraftingUIButtonRect { get; } = new Rect((Screen.width / 2), 0, 100, 20);
        static Rect CraftingBotButtonRect { get; } = new Rect((Screen.width / 2) + 100, 0, 100, 20);

        static Vector2 craftListScrollPosition = Vector2.zero;
        int craftSelectionGridInt = 0;
        string craftCountString = "-1";

         static List<Recipe> availableRecipes = getCurrentRecipes();
        static string[] recipeList = availableRecipes.Select(R => R.RecipeName).ToArray();
        int craftAmount = -1;

        //toCraftList = new List<Tuple<string, int>>();

        

        static Rect InventoryToolReaderButtonRect { get; } = new Rect((Screen.width / 2) + 200, 0, 100, 20);

        Rect CraftingWindowRect { get; set; } = new Rect((Screen.width / 2) - 506, 0, 0, 0);

        Rect GatheringWindowRect { get; set; } = new Rect((Screen.width / 2) - 506, 0, 0, 0);

        string[] TownClusterNames { get { return Enum.GetNames(typeof(TownClusterName)).Select(n => n.Replace("_", " ")).ToArray(); } }

        string[] TierNames { get { return Enum.GetNames(typeof(Tier)).ToArray(); } }

        Tier SelectedMinimumTier { get { return (Tier)Enum.Parse(typeof(Tier), TierNames[_selectedMininumTierIndex]); } }
        
        #endregion Properties

        #region Methods

        void DrawGatheringUIButton()
        {
            if (GUI.Button(GatheringUiButtonRect, "Gathering UI"))
                _isUIshown = true;

            DrawRunButton(false);

        }
        void DrawCraftingUIButton()
        {
            if (GUI.Button(CraftingUIButtonRect, "Crafting UI"))
                _isUIcraftshown = true;


            DrawCraftingBotButton(false);
        }
        void DrawGatheringUIWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            DrawGatheringUILeft();
            GUILayout.Space(SpaceBetweenSides);
            DrawGatheringUIRight();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        void DrawUnloadButton()
        {
            if(GUI.Button(UnloadButtonRect, "Unload"))
            {
                Core.Unload();
            }

        }



        void DrawNameReaderUIButton()
        {
            if (_state.State == State.ReadToolNames)
            {

                if (GUI.Button(InventoryToolReaderButtonRect, "Stop Reader"))
                {
                    _state.Fire(Trigger.GetToolNames);
                }

            }
            else
            {
                if (GUI.Button(InventoryToolReaderButtonRect, "Start Reader"))
                {

                    _state.Fire(Trigger.GetToolNames);
                }
            }


        }

        void DrawDropDown()
        {
            List<string> values = new List<string> { "Test", "names" };
            //Dropdown craftList.AddOptions(values);
        }



        void DrawCraftingBotButton(bool layouted)
        {
            var text = _isCraftRunning ? "Stop Crafting" : "Start Crafting";
            if (layouted ? GUILayout.Button(text) : GUI.Button(CraftingBotButtonRect, text))
            {
                _isCraftRunning = !_isCraftRunning;
                if (_isCraftRunning)
                {
                    //other resets
                    
                    

                    ResetCraftingVariables();
                    Core.Log("Resetting Crafting Variables");
                    //
                    ResetCriticalVariables();
                    if (_selectedGatherCluster == "Unknown" && _world.GetCurrentCluster() != null)
                        _selectedGatherCluster = _world.GetCurrentCluster().GetName();
                    _localPlayerCharacterView.CreateTextEffect("[Start Crafting]");
                    if (_state.CanFire(Trigger.Failure))
                        _state.Fire(Trigger.Failure);

                    _state.Fire(Trigger.GetItems);

                }
                else if (!_isCraftRunning)
                {
                    ResetCriticalVariables();
                    ResetCraftingVariables();
                    Core.Log("Resetting Crafting Variables");

                    _state.Fire(Trigger.CraftDone);
                    _localPlayerCharacterView.CreateTextEffect("[Collecting Items]");
                }


            }
        }

        void DrawCraftingUIWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            DrawCraftingUILeft();
            GUILayout.Space(SpaceBetweenSides);
            DrawCraftingUIRight();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        void DrawCraftingUILeft()
        {
            filterList = new List<Tuple<string, string>>();
            GUILayout.BeginVertical();
            DrawCraftingUI_Buttons();
            DrawCraftingBotButton(true);
            DrawCraftingUI_SelectionGrids();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Categories");
            shopcategoryScrollView = DrawItemSortingScrollView(shopcategory, shopcategoryScrollView);
            filterList.Add(new Tuple<string, string>("shopcategory", shopcategory[shopcategoryScrollView.Item2]));
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("Tier");
            //compare current gridselection index to  previous 
            if (shopcategoryScrollView.Item2 != shopcategoryScrollView.Item3)
            {
                //Core.Log("Index Changed");
               // tier = ReadXML("tier", filterList);

                // Array.Sort(tier,1,tier.Count()-1);
                tierScrollView = DrawItemSortingScrollView(tier, tierScrollView);
            }
            else
            {
                tierScrollView = DrawItemSortingScrollView(tier, tierScrollView);
            }
            //filterList.Add(new Tuple<string, string>("tier", tier[tierScrollView.Item2]));
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("SubCategories");
            //compare current gridselection index to  previous 
            if (shopcategoryScrollView.Item2 != shopcategoryScrollView.Item3)
            {
               // Core.Log("Index Changed");
                shopsubcategory1 = ReadXML("shopsubcategory1", filterList);
                shopsubcategory1ScrollView = DrawItemSortingScrollView(shopsubcategory1, shopsubcategory1ScrollView);
            }
            else
            {
                shopsubcategory1ScrollView = DrawItemSortingScrollView(shopsubcategory1, shopsubcategory1ScrollView);
            }
            filterList.Add(new Tuple<string, string>("shopsubcategory1", shopcategory[shopsubcategory1ScrollView.Item2]));
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        void DrawCraftingUIRight()
        {

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
        
            GUILayout.Label("Resources to craft:");
            
          
            DrawCraftingUI_CraftingScrollView();
      

            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            craftAmount = DrawCraftingUi_CraftTextFields();
            DrawAddToCraft(craftSelectionGridInt, craftAmount);
           toCraftList = DrawCurrentList(toCraftList);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();


           
        }

        List<Tuple<string, int>> DrawCurrentList(List<Tuple<string, int>> list)
        {
            GUILayout.Label("Recipe Name" + "   " + "Amount To Craft");
            for (int i = 0; i< list.Count; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(list[i].Item1 + "   " +list[i].Item2.ToString());
                if (GUILayout.Button("X")) {
                    list.Remove(list[i]);
                }

                GUILayout.EndHorizontal();
            }

            return list;
        }

        Tuple<Vector2, int,int> DrawItemSortingScrollView(string [] SA, Tuple<Vector2, int,int> T )
        {
            int previousGridSelection = T.Item2;
            Vector2 ScrollPosition = T.Item1;
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, false, true, GUILayout.ExpandWidth(true), GUILayout.MinWidth(100), Width(150),Height(300));

            int SelectionGridInt = T.Item2;

            SelectionGridInt = GUILayout.SelectionGrid(SelectionGridInt, SA, 1);

            EndScrollView();
            return new Tuple<Vector2, int, int>(ScrollPosition, SelectionGridInt, previousGridSelection);
        }



        void DrawCraftingUI_CraftingScrollView()
        {


            //var templist = readXML();
            //var temparray = templist.ToArray();
           
            craftListScrollPosition = GUILayout.BeginScrollView(craftListScrollPosition,false,true,GUILayout.ExpandWidth(true),GUILayout.MinWidth(300));
            craftSelectionGridInt = GUILayout.SelectionGrid(craftSelectionGridInt, recipeList, 1);
            //if (xmlstringarray.Length > 0)
            //{
            //    craftSelectionGridInt = GUILayout.SelectionGrid(craftSelectionGridInt, xmlstringarray, 1);
            //}
            GUILayout.EndScrollView();
            
 
        }

        void DrawAddToCraft(int gridSelection, int amount)
        {
            if(GUILayout.Button("Add recipe to craft list"))
            {
                string recipeName = recipeList[gridSelection];
                if (!toCraftList.Any(R => R.Item1 == recipeName))
                {
                    toCraftList.Add(new Tuple<string, int>(recipeList[gridSelection], amount));
                }
            }

        }


        int DrawCraftingUi_CraftTextFields()
        {
            GUILayout.Label("Enter amount to craft");
            craftCountString = GUILayout.TextField(craftCountString, GUILayout.MinWidth(100));
            int f1 = -1;
            try
            {
                 f1 = Convert.ToInt32(craftCountString);
                
            }
            catch (FormatException e)
            {
                Core.Log(e);
            }
            catch (OverflowException)
            {

            }
            return f1;
        }

        void DrawCraftingUI_Buttons()
        {
            if (GUILayout.Button("Close Crafting UI"))
                _isUIcraftshown = !_isUIcraftshown;
        }
        void DrawGatheringUILeft()
        {
            GUILayout.BeginVertical();
            DrawGatheringUI_Buttons();
            DrawGatheringUI_Toggles();
            DragGatheringUI_Sliders();
            DrawGatheringUI_SelectionGrids();
            DrawGatheringUI_TextFields();
            GUILayout.EndVertical();
        }


        void DrawCraftingUI_SelectionGrids()
        {
            GUILayout.Label("Selected city cluster for crafting:");
            _selectedCraftTownClusterIndex = GUILayout.SelectionGrid(_selectedCraftTownClusterIndex, TownClusterNames, TownClusterNames.Length);
        }

        void DrawCraftingUI_TextFields()
        {
            GUILayout.Label("Selected cluster for gathering:");
            var currentClusterInfo = _world.GetCurrentCluster() != null ? _world.GetCurrentCluster().GetName() : "Unknown";
            var selectedCraftGatherCluster = string.IsNullOrEmpty(_selectedCraftCluster) ? currentClusterInfo : _selectedCraftCluster;
            _selectedCraftCluster = GUILayout.TextField(selectedCraftGatherCluster);
        }


        void DrawGatheringUI_Toggles()
        {
            _allowMobHunting = GUILayout.Toggle(_allowMobHunting, "Allow hunting of living mobs (exerimental - can cause issues)");
            _skipUnrestrictedPvPZones = GUILayout.Toggle(_skipUnrestrictedPvPZones, "Skip unrestricted PvP zones while gathering");
            _skipKeeperPacks = GUILayout.Toggle(_skipKeeperPacks, "Skip keeper mobs while gathering");
            _allowSiegeCampTreasure = GUILayout.Toggle(_allowSiegeCampTreasure, "Allow usage of siege camp treasures");
            _skipRedAndBlackZones = GUILayout.Toggle(_skipRedAndBlackZones, "Skip red and black zones for traveling");
            UpdateESP(GUILayout.Toggle(_showESP, "Show ESP"));
        }

        void UpdateESP(bool newValue)
        {
            var oldValue = _showESP;
            _showESP = newValue;

            if (oldValue != _showESP)
            {
                if (_showESP)
                    gameObject.AddComponent<ESP.ESP>().StartESP(_gatherInformations);
                else if (gameObject.GetComponent<ESP.ESP>() != null)
                    Destroy(gameObject.GetComponent<ESP.ESP>());
            }
        }

        void DragGatheringUI_Sliders()
        {
            if (_skipKeeperPacks)
            {
                GUILayout.Label($"Skip keeper range: {_keeperSkipRange}");
                _keeperSkipRange = GUILayout.HorizontalSlider(_keeperSkipRange, 5, 50);
            }

            GUILayout.Label($"Minimum health percentage for gathering: {_minimumHealthForGathering.ToString("P2")}");
            _minimumHealthForGathering = GUILayout.HorizontalSlider(_minimumHealthForGathering, 0.01f, 1f);

            GUILayout.Label($"Weight percentage needed for banking: {_percentageForBanking}");
            _percentageForBanking = Mathf.Round(GUILayout.HorizontalSlider(_percentageForBanking, 1, 400));

            if (_allowSiegeCampTreasure)
            {
                GUILayout.Label($"Weight percentage needed for siege camp treasure: {_percentageForSiegeCampTreasure}");
                _percentageForSiegeCampTreasure = Mathf.Round(GUILayout.HorizontalSlider(_percentageForSiegeCampTreasure, 1, 400));
            }
        }

        void DrawGatheringUI_SelectionGrids()
        {
            GUILayout.Label("Selected city cluster for banking:");
            _selectedTownClusterIndex = GUILayout.SelectionGrid(_selectedTownClusterIndex, TownClusterNames, TownClusterNames.Length);

            GUILayout.Label("Selected minimum resource tier of interest:");
            _selectedMininumTierIndex = GUILayout.SelectionGrid(_selectedMininumTierIndex, TierNames, TierNames.Length);
        }

        void DrawGatheringUI_TextFields()
        {
            GUILayout.Label("Selected cluster for gathering:");
            var currentClusterInfo = _world.GetCurrentCluster() != null ? _world.GetCurrentCluster().GetName() : "Unknown";
            var selectedGatherCluster = string.IsNullOrEmpty(_selectedGatherCluster) ? currentClusterInfo : _selectedGatherCluster;
            _selectedGatherCluster = GUILayout.TextField(selectedGatherCluster);
        }

        void DrawGatheringUIRight()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Resources to gather:");
            DrawGatheringUI_GatheringToggles();
            GUILayout.EndVertical();
        }

        void DrawGatheringUI_Buttons()
        {
            if (GUILayout.Button("Close Gathering UI"))
                _isUIshown = !_isUIshown;

            DrawRunButton(true);

            if (GUILayout.Button("Unload"))
                Core.Unload();
        }

        void DrawGatheringUI_GatheringToggles()
        {
            GUILayout.BeginHorizontal();
            var selectedMinimumTier = SelectedMinimumTier;
            var groupedKeys = _gatherInformations.Keys.GroupBy(i => i.ResourceType).ToArray();
            for (var i = 0; i < groupedKeys.Count(); i++)
            {
                var keys = groupedKeys[i].ToArray();

                GUILayout.BeginVertical();
                for (var j = 0; j < keys.Length; j++)
                {
                    var info = keys[j];
                    if (info.Tier < selectedMinimumTier)
                        _gatherInformations[info] = false;
                    else
                        _gatherInformations[info] = GUILayout.Toggle(_gatherInformations[info], info.ToString());
                }
                GUILayout.EndVertical();
                GUILayout.Space(SpaceBetweenItems);
            }
            GUILayout.EndHorizontal();
        }

        void DrawRunButton(bool layouted)
        {
            var text = _isGatherRunning ? "Stop Gathering" : "Start Gathering";
            if (layouted ? GUILayout.Button(text) : GUI.Button(GatheringBotButtonRect, text))
            {
                _isGatherRunning = !_isGatherRunning;
                if (_isGatherRunning)
                {
                    LoginGui.AutoLogin = true;
                    _isCraftRunning = false;

                    ResetCriticalVariables();
                    if (_selectedGatherCluster == "Unknown" && _world.GetCurrentCluster() != null)
                        _selectedGatherCluster = _world.GetCurrentCluster().GetName();
                    _localPlayerCharacterView.CreateTextEffect("[Start]");
                    if (_state.CanFire(Trigger.Failure))
                        _state.Fire(Trigger.Failure);
                }
                else if (!_isGatherRunning)
                {
                    LoginGui.AutoLogin = false;
                }


            }
        }

        protected override void OnUI()
        {
            if (_isUIshown)
            {
                GatheringWindowRect = GUILayout.Window(0, GatheringWindowRect, DrawGatheringUIWindow, "Gathering UI");
                _isUIcraftshown = false;
            }


            else if (_isUIcraftshown)
            {
                CraftingWindowRect = GUILayout.Window(1, CraftingWindowRect, DrawCraftingUIWindow, "Crafting UI", MinWidth(1100));
                _isUIshown = false;
            }
            else
            {
                DrawGatheringUIButton();
                DrawCraftingUIButton();
                DrawNameReaderUIButton();
                DrawUnloadButton();
                DrawDropDown();
            }
        }
        #endregion Methods
    }
}
