using Merlin.API.Direct;
using Stateless;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    public enum State
    {
        Search,
        Harvest,
        Combat,
        Bank,
        Repair,
        Travel,
        SiegeCampTreasure,
        Crafting, //new
        CollectItems, //new
        ReadToolNames,
    }

    public enum Trigger
    {
        Restart,
        DiscoveredResource,
        BankDone,
        RepairDone,
        DepletedResource,
        Overweight,
        Damaged,
        EncounteredAttacker,
        EliminatedAttacker,
        StartTravelling,
        TravellingDone,
        StartSiegeCampTreasure,
        OnSiegeCampTreasureDone,
        Failure,
        GetItems, //new
        GotItems,
        CraftDone, //new
        GetToolNames,
    }

    public sealed partial class Gatherer : Profile
    {
        private bool _isGatherRunning = false;
        private bool _isCraftRunning = false;
        string[] shopcategory;

        private StateMachine<State, Trigger> _state;
        private Dictionary<SimulationObjectView, Blacklisted> _blacklist;
        private Dictionary<Point2, GatherInformation> _gatheredSpots;
        private List<Point2> _keeperSpots;
        private List<MountObjectView> _mounts;
        private bool _knockedDown;

        public override string Name => "Gatherer";

        protected override void OnStart()
        {
            _blacklist = new Dictionary<SimulationObjectView, Blacklisted>();
            _gatheredSpots = new Dictionary<Point2, GatherInformation>();
            _keeperSpots = new List<Point2>();
            LoginGui.AutoLogin = false;
            shopcategory = ReadXML("shopcategory");
            Array.Sort(shopcategory, 1, shopcategory.Length - 1);
            LoadSettings();

            _state = new StateMachine<State, Trigger>(State.Search);
            _state.Configure(State.Search)
                .Permit(Trigger.StartSiegeCampTreasure, State.SiegeCampTreasure)
                .Permit(Trigger.StartTravelling, State.Travel)
                .Permit(Trigger.EncounteredAttacker, State.Combat)
                .Permit(Trigger.DiscoveredResource, State.Harvest)
                .Permit(Trigger.Overweight, State.Bank)
                .Permit(Trigger.Damaged, State.Repair)
                .Permit(Trigger.GetItems, State.CollectItems) 
                .Permit(Trigger.GetToolNames, State.ReadToolNames)
                .PermitReentry(Trigger.CraftDone)
                ;

            _state.Configure(State.Combat)
                .Permit(Trigger.EliminatedAttacker, State.Search)
            ;

            _state.Configure(State.Harvest)
                .Permit(Trigger.DepletedResource, State.Search)
                .Permit(Trigger.EncounteredAttacker, State.Combat)
                .Permit(Trigger.GetToolNames, State.ReadToolNames)
                ;

            _state.Configure(State.Bank)
                .Permit(Trigger.Restart, State.Search)
                .Permit(Trigger.BankDone, State.Search)
                .Permit(Trigger.GetToolNames, State.ReadToolNames)
               ;

            _state.Configure(State.Repair)
                .Permit(Trigger.RepairDone, State.Bank)
                .Permit(Trigger.GetToolNames, State.ReadToolNames)
                ;

            _state.Configure(State.Travel)
                .Permit(Trigger.TravellingDone, State.Search)
            .PermitReentry(Trigger.CraftDone)
            .Permit(Trigger.GetToolNames, State.ReadToolNames)
            ;

            _state.Configure(State.SiegeCampTreasure)
                .Permit(Trigger.OnSiegeCampTreasureDone, State.Search)
                ;

            _state.Configure(State.CollectItems)
                .Permit(Trigger.GotItems, State.Crafting)
            .Permit(Trigger.CraftDone, State.Search)
           ;

            _state.Configure(State.Crafting)
                .Permit(Trigger.CraftDone, State.Search)
            .Permit(Trigger.GetItems, State.CollectItems)
            ;

            _state.Configure(State.ReadToolNames)
                .Permit(Trigger.GetToolNames, State.Search)
                ;

            foreach (State state in Enum.GetValues(typeof(State)))
            {
                if (state != State.Search)
                    _state.Configure(state).Permit(Trigger.Failure, State.Search);
            }
        }

        protected override void OnStop()
        {
            SaveSettings();

            _state = null;

            _blacklist.Clear();
            _blacklist = null;
        }

        protected override void OnUpdate()
        {
            //If we don't have a view, do not do anything!
            if (_localPlayerCharacterView == null)
                return;

            if (!_isGatherRunning)
                return;

            // if (!_isCraftRunning)
            //     return;


            if (_blacklist.Count > 0)
            {
                var whitelist = new List<SimulationObjectView>();

                foreach (var blacklisted in _blacklist.Values)
                {
                    if (DateTime.Now >= blacklisted.Timestamp)
                        whitelist.Add(blacklisted.Target);
                }

                foreach (var target in whitelist)
                    _blacklist.Remove(target);
            }

            try
            {
                foreach (var keeper in _client.GetEntities<MobView>(mob => !mob.IsDead() && mob.MobType().ToLowerInvariant().Contains("keeper")))
                {
                    var keeperPosition = keeper.GetInternalPosition();
                    if (!_keeperSpots.Contains(keeperPosition))
                        _keeperSpots.Add(keeperPosition);
                }

                _mounts = _client.GetEntities<MountObjectView>(mount => mount.IsInUseRange(_localPlayerCharacterView.LocalPlayerCharacter));

                if (_knockedDown != _localPlayerCharacterView.IsKnockedDown())
                {
                    _knockedDown = _localPlayerCharacterView.IsKnockedDown();
                    if (_knockedDown)
                    {
                        Core.Log("[DEAD - Currently knocked down!]");
                    }
                }

                switch (_state.State)
                {
                    case State.Search: Search(); break;
                    case State.Harvest: Harvest(); break;
                    case State.Combat: Fight(); break;
                    case State.Bank: Bank(); break;
                    case State.Repair: Repair(); break;
                    case State.Travel: Travel(); break;
                    case State.SiegeCampTreasure: SiegeCampTreasure(); break;
                    case State.CollectItems: Collect(); break;
                    case State.Crafting: Craft(); break;
                        case State.ReadToolNames: tester(); break;
                }
            }
            catch (Exception e)
            {
                Core.Log(e);

                ResetCriticalVariables();
                _state.Fire(Trigger.Failure);
            }
        }

        private void ResetCriticalVariables()
        {
            _worldPathingRequest = null;
            _bankPathingRequest = null;
            _repairPathingRequest = null;
            _repairFindPathingRequest = null;
            _harvestPathingRequest = null;
            _currentTarget = null;
            _failedFindAttempts = 0;
            _reachedPointInBetween = false;
            _changeGatheringPathRequest = null;
            _siegeCampTreasureCoroutine = null;
            _targetCluster = null;
            _travelPathingRequest = null;
            _collectItemsWorldPathingRequest = null;
            _collectItemsBankPathingRequest = null;

        }

        private void ResetCraftingVariables()
        {
            _craftTarget = null;
            pathFindingCounter = 0;
            RL = null;
            ToRetrieve = null;
            sortStack = false;
            _buildingSize = Vector2.zero;
            _collectItemsWorldPathingRequest = null;
            _collectItemsBankPathingRequest = null;
            _buildingSize = Vector2.zero;
            _craftreachedPointInBetween = false;
            _building3dcenter = Vector3.zero;
            _building2dcenter = Vector3.zero;
            _building2d3dcenter = Vector3.zero;
            _closestpoint = Vector3.zero;
            _craftPathFound = false;
            initialScan = null;
            scanTest = false;
            scanTest2 = false;
            craftRec = null;
            Town = null;
            sortStack = false;
            stackTimer = DateTime.MinValue;
            detailsGui = false;
            stuckTimer = DateTime.MinValue;
            finalDestination = Vector3.zero;
            splitCounter = 1;
            reachedFinalDestination = false;
            resourceCollectList = null;
            buildingLoaded = false;
            timeCooldownMax = DateTime.MaxValue;
            notMovingTimer = DateTime.MinValue;
            //toCraftList = new List<Tuple<string, int>>{
           // new Tuple<string, int>( "t3_armor_leather_set1", 90)
    //};

        }


        private void Blacklist(SimulationObjectView target, TimeSpan duration)
        {
            _blacklist[target] = new Blacklisted()
            {
                Target = target,
                Timestamp = DateTime.Now + duration,
            };
        }

        private class Blacklisted
        {
            public SimulationObjectView Target { get; set; }

            public DateTime Timestamp { get; set; }
        }

        public struct GatherInformation
        {
            ResourceType _resourceType;
            Tier _tier;
            EnchantmentLevel _enchantmentLevel;

            public ResourceType ResourceType { get { return _resourceType; } }
            public Tier Tier { get { return _tier; } }
            public EnchantmentLevel EnchantmentLevel { get { return _enchantmentLevel; } }
            public DateTime? HarvestDate { get; set; }

            public GatherInformation(ResourceType resourceType, Tier tier, EnchantmentLevel enchantmentLevel)
            {
                _resourceType = resourceType;
                _tier = tier;
                _enchantmentLevel = enchantmentLevel;
                HarvestDate = null;
            }

            public override string ToString()
            {
                return $"{ResourceType} {Tier}.{(int)EnchantmentLevel}";
            }
        }

        public struct CraftInformation
        {
            ProcessedResourceType _processedResourceType;
            Tier _tier;
            EnchantmentLevel _enchantmentLevel;


            public ProcessedResourceType ProcessedResourceType { get { return _processedResourceType; } }
            public Tier Tier { get { return _tier; } }
            public EnchantmentLevel EnchantmentLevel { get { return _enchantmentLevel; } }

            public CraftInformation(ProcessedResourceType processedResourceType, Tier tier, EnchantmentLevel enchantmentLevel)
            {
                _processedResourceType = processedResourceType;
                _tier = tier;
                _enchantmentLevel = enchantmentLevel;
            }

            public override string ToString()
            {
                return $"{Tier}_{ProcessedResourceType} ";
            }

        }

        static string [] ReadXML(string attName, List<Tuple<string, string>> tupleList =  null)
        {
            XmlDocument doc = new XmlDocument();
            // doc.Load("D:\\Albion Bot\\merlin - development(29_09_17)\\merlin - development\\Albion\\Merlin\\items.xml");
            // string URLSTRING = ;

            // XmlTextReader items_xml = new XmlTextReader("items.xml");

            doc.Load("items.xml");

            Core.Log($"reading xml");
            List<string> sortlist = new List<string>();
            sortlist.Add("All");

            XmlNode root = doc.DocumentElement;

            Core.Log($"{root.Name}");
            //doc.
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {

                

                if (node.NodeType == XmlNodeType.Element)
                {
                    var nodeAttributes = node.Attributes;
                    if (nodeAttributes.Count > 0)
                    {
                        Core.Log($"Node Attribute count  {nodeAttributes.Count } ");

                        if (tupleList != null)
                        {
                            bool matched = true;
                            Core.Log($"Node Name {node.LocalName} and Node Parent {node.ParentNode.LocalName}");
                            foreach (var T in tupleList)
                            {
                                // Core.Log($"{ node.Attributes[match].Value}");

                                    if (node.Attributes[T.Item1] != null)
                                    {

                                    Core.Log($"Found match for {T.Item1} in attribute {node.Attributes[T.Item1].LocalName} ");
                                    if (node.Attributes[T.Item1].Value != T.Item2)
                                    {
                                        matched = false;
                                    }

                                    }
                                    else
                                    {
                                        Core.Log($"One didnt match {T.Item1}");
                                        matched = false;
                                        break;
                                    }

                            

                            }

                            if (matched)
                            {
                                if (node.Attributes[attName] != null) { 

                                    var nodeValue = node.Attributes[attName].Value;
                                Core.Log($"Node match value {nodeValue}");

                                if (!sortlist.Contains(nodeValue))
                                    {
                                    sortlist.Add(nodeValue);
                                    }
                                }
                            }


                        }
                        else
                        {
                            if (node.Attributes[attName] != null)
                            {
                                var nodeValue = node.Attributes[attName].Value;
                                Core.Log($"Node Attribute value  {nodeValue} ");

                                if (!sortlist.Contains(nodeValue))
                                {
                                    sortlist.Add(nodeValue);
                                }
                            }
                        }

                    }
                }
                
            }
       

            return sortlist.ToArray();
        }





        static List<Recipe> getCurrentRecipes()
        {
            List<Recipe> currentRecipes = new List<Recipe>();

            currentRecipes.Add(new Recipe("t2_metal", "smelter", "refining", new List<string> { "t2_ore" }, new List<int> { 1 }));
            currentRecipes.Add(new Recipe("t3_metal", "smelter", "refining", new List<string> { "t3_ore", "t2_metal" }, new List<int> { 2, 1 }));
            currentRecipes.Add(new Recipe("t4_metal", "smelter", "refining", new List<string> { "t4_ore", "t3_metal" }, new List<int> { 2, 1 },"steelbar"));
            currentRecipes.Add(new Recipe("t5_metal", "smelter", "refining", new List<string> { "t5_ore", "t4_metal" }, new List<int> { 3, 1 },"titaniumsteelbar"));

            currentRecipes.Add(new Recipe("t2_plank", "lumbermill", "refining", new List<string> { "t2_wood" }, new List<int> { 2 }));       
            currentRecipes.Add(new Recipe("t3_plank", "lumbermill", "refining", new List<string> { "t3_wood", "t2_plank" }, new List<int> { 2, 1 }));
            currentRecipes.Add(new Recipe("t4_plank", "lumbermill", "refining", new List<string> { "t4_wood", "t3_plank" }, new List<int> { 2, 1 }));

            currentRecipes.Add(new Recipe("t2_leather","tanner", "refining", new List<string> { "t2_hide" }, new List<int> { 1 }));
            currentRecipes.Add(new Recipe("t3_leather","tanner", "refining", new List<string> { "t3_hide", "t2_leather" }, new List<int> { 2, 1 }));
            currentRecipes.Add(new Recipe("t4_leather", "tanner", "refining", new List<string> { "t4_hide", "t3_leather" }, new List<int> { 2, 1 }));
            currentRecipes.Add(new Recipe("t5_leather", "tanner", "refining", new List<string> { "t5_hide", "t4_leather" }, new List<int> { 3, 1 }));

            currentRecipes.Add(new Recipe("t2_stoneblock", "stonemason", "refining", new List<string> { "t2_rock" }, new List<int> { 1 }));
            currentRecipes.Add(new Recipe("t3_stoneblock", "stonemason", "refining", new List<string> { "t3_rock", "t2_stoneblock" }, new List<int> { 2,1 }));
            currentRecipes.Add(new Recipe("t4_stoneblock", "stonemason", "refining", new List<string> { "t4_rock", "t3_stoneblock" }, new List<int> { 2, 1 }));

            currentRecipes.Add(new Recipe("t2_cloth", "weaver", "refining", new List<string> { "t2_fiber" }, new List<int> { 1 }));
            currentRecipes.Add(new Recipe("t3_cloth", "weaver", "refining", new List<string> { "t3_fiber", "t2_cloth" }, new List<int> { 2, 1 }));
            currentRecipes.Add(new Recipe("t4_cloth", "weaver", "refining", new List<string> { "t4_fiber", "t3_cloth" }, new List<int> { 2, 1 }));
            currentRecipes.Add(new Recipe("t5_cloth", "weaver", "refining", new List<string> { "t5_fiber", "t4_cloth" }, new List<int> { 3, 1 }));


            currentRecipes.Add(new Recipe("t3_2h_tool_pick", "toolmaker", "tools", new List<string> { "t3_plank", "t3_metal" }, new List<int> { 6, 2 }));
            currentRecipes.Add(new Recipe("t4_2h_tool_pick", "toolmaker", "tools", new List<string> { "t4_plank", "t4_metal" }, new List<int> { 6, 2 }));

            currentRecipes.Add(new Recipe("t3_2h_tool_sickle", "toolmaker", "tools", new List<string> { "t3_plank", "t3_metal" }, new List<int> { 6, 2 }));
            currentRecipes.Add(new Recipe("t3_2h_tool_knife", "toolmaker", "tools", new List<string> { "t3_plank", "t3_metal" }, new List<int> { 6, 2 }));
            currentRecipes.Add(new Recipe("t3_2h_tool_axe", "toolmaker", "tools", new List<string> { "t3_plank", "t3_metal" }, new List<int> { 6, 2 }));
            currentRecipes.Add(new Recipe("t3_2h_tool_hammer", "toolmaker", "tools", new List<string> { "t3_plank", "t3_metal" }, new List<int> { 6, 2 }));

            currentRecipes.Add(new Recipe("t3_head_cloth_set1", "mage", "armor", new List<string> { "t3_cloth",}, new List<int> { 8 }, "cowl"));
            currentRecipes.Add(new Recipe("t3_armor_cloth_set1", "mage", "armor",  new List<string> { "t3_cloth", }, new List<int> { 16 }, "robe") );
            currentRecipes.Add(new Recipe("t3_shoes_cloth_set1", "mage", "armor", new List<string> { "t3_cloth", }, new List<int> { 8 }, "sandal"));

            currentRecipes.Add(new Recipe("t3_main_firestaff", "mage", "weapon", new List<string> { "t3_plank","t3_metal" }, new List<int> {16, 8 }));

            currentRecipes.Add(new Recipe("t2_bag", "toolmaker", "accessories", new List<string> { "t2_cloth", "t2_leather" }, new List<int> { 4, 4 }));
            currentRecipes.Add(new Recipe("t2_cape", "toolmaker", "accessories", new List<string> { "t2_cloth", "t2_leather" }, new List<int> { 12, 4 }));

            currentRecipes.Add(new Recipe("t2_head_leather_set1", "lodge", "armor", new List<string> { "t2_leather", }, new List<int> { 8 }, "mercenaryhood"));
            currentRecipes.Add(new Recipe("t2_armor_leather_set1", "lodge", "armor", new List<string> { "t2_leather", }, new List<int> { 16 }, "mercenaryjacket"));
            currentRecipes.Add(new Recipe("t2_shoes_leather_set1", "lodge", "armor", new List<string> { "t2_leather", }, new List<int> { 8 }, "mercenaryshoe"));

            currentRecipes.Add(new Recipe("t3_head_leather_set1", "lodge", "armor", new List<string> { "t3_leather", }, new List<int> { 8 }, "mercenaryhood"));
            currentRecipes.Add(new Recipe("t3_armor_leather_set1", "lodge", "armor", new List<string> { "t3_leather", }, new List<int> { 16 }, "mercenaryjacket"));
            currentRecipes.Add(new Recipe("t3_shoes_leather_set1", "lodge", "armor", new List<string> { "t3_leather", }, new List<int> { 8 }, "mercenaryshoe"));

            currentRecipes.Add(new Recipe("t4_head_leather_set1", "lodge", "armor", new List<string> { "t4_leather", }, new List<int> { 8 }, "mercenaryhood"));
            currentRecipes.Add(new Recipe("t4_armor_leather_set1", "lodge", "armor", new List<string> { "t4_leather", }, new List<int> { 16 }, "mercenaryjacket"));
            currentRecipes.Add(new Recipe("t4_shoes_leather_set1", "lodge", "armor", new List<string> { "t4_leather", }, new List<int> { 8 }, "mercenaryshoe"));

            currentRecipes.Add(new Recipe("t5_head_leather_set1", "lodge", "armor", new List<string> { "t5_leather", }, new List<int> { 8 }, "mercenaryhood"));
            currentRecipes.Add(new Recipe("t5_armor_leather_set1", "lodge", "armor", new List<string> { "t5_leather", }, new List<int> { 16 }, "mercenaryjacket"));
            currentRecipes.Add(new Recipe("t5_shoes_leather_set1", "lodge", "armor", new List<string> { "t5_leather", }, new List<int> { 8 }, "mercenaryshoe"));


            //"t3_main_firestaff
            //"t3_cape
            //t3_head_cloth_set1
            //t3_bag
            //t3_shoes_cloth_set1
            //t3_armor_cloth_set1

            return currentRecipes;
        }

        public Dictionary<string, string> getCraftBuildingViewBuildingNames()
        {
            Dictionary<string, string> CraftBuildingViewBuildingNames = new Dictionary<string, string>();
            //CraftBuildingViewBuildingNames.Add("saddler");
            //CraftBuildingViewBuildingNames.Add("butcher");
            //CraftBuildingViewBuildingNames.Add("lumbermill");
            //CraftBuildingViewBuildingNames.Add("mage");//mage's tower
            //CraftBuildingViewBuildingNames.Add("hunter,");//hunter's lodge
            //CraftBuildingViewBuildingNames.Add("cook");
            //CraftBuildingViewBuildingNames.Add("stonemason");
            //CraftBuildingViewBuildingNames.Add("weaver");
            //CraftBuildingViewBuildingNames.Add("alchemist");//alchemist's lab
            //CraftBuildingViewBuildingNames.Add("tanner");
            //CraftBuildingViewBuildingNames.Add("warrior");//warriors' forge
            //CraftBuildingViewBuildingNames.Add("mill");
            //CraftBuildingViewBuildingNames.Add("toolmaker");
            //CraftBuildingViewBuildingNames.Add("smelter");
            //CraftBuildingViewBuildingNames.Add("merchant,"); //vanity merchant
            



            return CraftBuildingViewBuildingNames;
        }

        //check for moving
        private float noMovementThreshold = 0.01f;
        private const int noMovementFrames = 2;
        Vector3[] previousLocations = new Vector3[noMovementFrames];
        private bool isMoving;

        //Let other scripts see if the object is moving
        public bool IsMoving
        {
            get { return isMoving; }
        }

        void Awake()
        {
            //For good measure, set the previous locations
            for (int i = 0; i < previousLocations.Length; i++)
            {
                previousLocations[i] = Vector3.zero;
            }
        }

        void isMovingUpdate()
        {
            //Store the newest vector at the end of the list of vectors
            for (int i = 0; i < previousLocations.Length - 1; i++)
            {
                previousLocations[i] = previousLocations[i + 1];
            }
            previousLocations[previousLocations.Length - 1] = _localPlayerCharacterView.transform.position;

            //Check the distances between the points in your previous locations
            //If for the past several updates, there are no movements smaller than the threshold,
            //you can most likely assume that the object is not moving
            for (int i = 0; i < previousLocations.Length - 1; i++)
            {
                if (Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold)
                {
                    //The minimum movement has been detected between frames
                    isMoving = true;
                    break;
                }
                else
                {
                    isMoving = false;
                }
            }
        }









    }
}