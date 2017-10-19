using Merlin.API.Direct;
using Merlin.Pathing;
using Merlin.Pathing.Worldmap;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;
using System.Reflection;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        Worldmap worldmapInstance = GameGui.Instance.WorldMap;
        private PositionPathingRequest _craftingPathingRequest;
        private ClusterPathingRequest _craftingClusterPathingRequest;
        private PositionPathingRequest _craftFindPathingRequest;
        private bool _craftreachedPointInBetween;
        private bool _uiShown;




        Town Town = null;// = new List<Building>();


        Vector2 _buildingSize = Vector2.zero;



        Vector3 _building3dcenter = Vector3.zero;

        Vector2 _building2dcenter = Vector3.zero;

        Vector3 _building2d3dcenter = Vector3.zero;

        Vector3 _closestpoint = Vector3.zero;

        Vector3 randomSpot;

        Vector3 finalDestination;

        CraftBuildingView _craftTarget = null;
        DateTime stuckTimer = DateTime.MinValue;
        int pathFindingCounter = 0;
        bool reachedFinalDestination = false;
        bool _craftPathFound = false;
        Building craftBuilding = null;
        DateTime timeCooldownMax = DateTime.MaxValue;
        public void Craft()
        {

            Worldmap worldmapInstance = GameGui.Instance.WorldMap;

            Vector3 playerCenter = _localPlayerCharacterView.transform.position;

            ClusterDescriptor currentWorldCluster = _world.GetCurrentCluster();

            ClusterDescriptor townCluster = worldmapInstance.GetCluster(TownClusterNames[_selectedCraftTownClusterIndex]).Info;

            //Vector2 buildingSize = Vector2.zero;

            Collider buildingCollider;

            Vector3 building3dcenter;

            Vector2 building2dcenter;

            Vector3 building2d3dcenter;

            Vector3 closestpoint;

            ClusterMapUI clustermapinstance = null;

            List<Vector3> posBoundary = null;



            if (HandlePathing(ref _worldPathingRequest))
                return;
            //Core.Log("Here 66");
            //if (HandlePathing(ref _craftFindPathingRequest, () => _client.GetEntities<RepairBuildingView>((x) => { return true; }).Count > 0))
            //    return;

            //if (HandlePathing(ref _craftFindPathingRequest, () => _client.GetEntities<CraftBuildingView>((x) => { return true; }).Count > 0))
            //   return;

            if (HandlePathing(ref _craftingClusterPathingRequest))
                return;


            if (HandlePathing(ref _craftingPathingRequest, null, () => _craftreachedPointInBetween = true))
            {
                return;
            }

            //Core.Log($"CraftreachedPointInBetween {_craftreachedPointInBetween}");

            // Core.Log("Here 67");
            //GameManager.GetInstance().GetLocalPlayerCharacterView().useGUILayout
            if (currentWorldCluster.GetName() == townCluster.GetName())
            {

                try
                {
                    // Core.Log("Number of buildings " + Convert.ToString(Town._building.Length));
                    //Core.Log("Notes2");
                }
                catch (NullReferenceException)
                {

                }

                if (Town == null || Town._building.Length <= 0)
                {
                    try
                    {

                        if (!GameGui.Instance.ClusterMapGui.gameObject.activeInHierarchy)

                        {
                            Core.Log("Opening Map");
                            GameGui.Instance.ClusterMapGui.Show();
                           // Core.Log("here3");
                        }


                        if (GameGui.Instance.ClusterMapGui.gameObject.activeInHierarchy)
                        {
                            // Core.Log("Map is open");
                            clustermapinstance = GameGui.Instance.ClusterMapGui;
                            List<ClusterBuilding> tempClusterBuildingList = new List<ClusterBuilding>();
                            List<int> idlist = new List<int>();
                            // Core.Log("here4");
                            for (int i = 0; i < 2000; i++)
                            {
                                try
                                {
                                    ClusterBuilding tempClusterBuilding = clustermapinstance.GetBuilding(i);

                                    if (tempClusterBuilding.CanUse)
                                    {

                                        tempClusterBuildingList.Add(tempClusterBuilding);
                                        idlist.Add(i);

                                        //Core.Log($"added {i}");
                                    }

                                }
                                catch (NullReferenceException)
                                {
                                    //Core.Log("caught");
                                }

                            }
                            Building[] buildingArray = new Building[tempClusterBuildingList.Count];
                            int counter = 0;
                           // Core.Log("here5");
                            foreach (ClusterBuilding CB in tempClusterBuildingList)
                            {

                                GameGui.Instance.ClusterMapGui.ShowDetails(CB);
                                string Title = GameGui.Instance.ClusterMapGui.BuildingUI.Title.text;
                                string Owner = GameGui.Instance.ClusterMapGui.BuildingUI.Owner.text;
                                // string objTitle = CB.Hitbox.name;

                                //Core.Log("making buidling");
                                string UsageFeeS = GameGui.Instance.ClusterMapGui.BuildingUI.UsageFee.text;
                                string DurabilityS = GameGui.Instance.ClusterMapGui.BuildingUI.Durability.text;
                                string FoodS = GameGui.Instance.ClusterMapGui.BuildingUI.Food.text;

                                String[] substringsDurabilityS = DurabilityS.Split('%');

                                String[] substringsUsageFee = UsageFeeS.Split(' ', '%');

                                String[] substringsFood= FoodS.Split(' ', '%');

                               // Core.Log(FoodS);
                                float Food= Convert.ToSingle(substringsFood[0]);

                               
                                float DurabilityF = Convert.ToSingle(substringsDurabilityS[0]);

                                float UsageFeeF = Convert.ToSingle(substringsUsageFee[1]);
                                //Core.Log("making buidling2");
                                bool CanUse = CB.CanUse;

                                Vector2 Size = CB.Size;
                                //Core.Log("making buidling2");
                                int Tier = Convert.ToInt32(CB.Tier);

                                

                                Vector2 Position = CB.Position;
                                Collider collider = CB.Hitbox;

                                GameObject obj = collider.gameObject;
                                // Core.Log($" collider object name {obj.name}");
                                var pP = obj.transform.position.c();

                                var point = pP;

                                float Buildingy = _landscape.GetTerrainHeight(point, out RaycastHit hit);

                                long ID = idlist[counter];

                                Building tempBuilding = new Building(Title, Tier, Food, CanUse, Position, Owner, Size, collider, Buildingy, UsageFeeF, ID, DurabilityF);

                                buildingArray[counter] = tempBuilding;
                                counter++;

                            }

                            Town = new Town(buildingArray);
                            Core.Log("Town made");

                        }

                        //if (!GameGui.Instance.ClusterMapGui.gameObject.activeInHierarchy)

                        //{
                        //    Core.Log("Closing Map");
                        //    GameGui.Instance.ClusterMapGui.Close();
                        //}
                        // GameGui.Instance.CloseAllGui();

                    }

                    catch (NullReferenceException)
                    {
                        //Core.Log("here2");
                        //return;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Core.Log("here3");
                    }



                }

                else
                {
                    try
                    {

                        int minTier = 1;

                        //Building[] clusterBuildings = Town._building;
                        //for(int i = 0; i < clusterBuildings.Length;i++)
                        //{
                        //   Core.Log($" {clusterBuildings[i].Title}");
                        //}

                        //string bname = "toolmaker";
                        string bname = craftRec.CraftBuilding;
                        // Core.Log("Here 7");
                        //var TchosenB = clusterBuildings.TakeWhile( b => b.Title.ToLowerInvariant().Contains(bname) && b.Tier > minTier);

                        //var TchosenB = clusterBuildings.First(b => b.Title.ToLowerInvariant().Contains(bname) && b.Tier > minTier);

                        // var TchosenB = clusterBuildings.Where(BItem => BItem.Tier > minTier)
                        //.Where(BItem => BItem.Title.ToLowerInvariant().Contains(bname))
                        //   .Select(BItem =>BItem.Title);

                        //var minUsage = TchosenB.Min(b => b.UsageFee);

                        //Building B1 = (Building)TchosenB.Select(b => b.UsageFee == minUsage);



                        // Core.Log(chosenB.);


                        // Building[] minTierBuildings = clusterBuildings.All<Building>(B)
                        //Core.Log("here");
                        List<Building> reducedBuildings = new List<Building>();
                        foreach (Building B in Town)
                        {
                            //Core.Log($"{ B.Title}");
                            if (B.Title.ToLowerInvariant().Contains(bname) && B.Durability > 50 && B.Food >= 4)
                            {
                                if (B.Tier > minTier)
                                {
                                    reducedBuildings.Add(B);
                                }

                            }

                        }

                        if (reducedBuildings.Count <= 0)
                        {
                            Core.Log("There are no builings available for crafting");
                            return;
                        }


                       // Core.Log($"Number of buildings { reducedBuildings.Count}");
                        //var minUsage = reducedBuildings.Min(b => b.UsageFee);
                        var minUsage = 1e20;
                        for (int i = 0; i < reducedBuildings.Count; i++)
                        {
                            if (reducedBuildings[i].UsageFee < minUsage)
                            {
                                minUsage = reducedBuildings[i].UsageFee;
                                // Core.Log("min feeset");
                            }
                        }





                        foreach (Building bb in reducedBuildings)
                        {
                            if (bb.UsageFee == minUsage)
                            {
                                // Core.Log("Found min. Building chosen");
                                this.craftBuilding = bb;
                                break;
                            }
                            else
                            {
                                // Core.Log("not min");
                            }
                        }
                        // Core.Log($"made it{B1.Title}");
                        //Building B1 = reducedBuildings.First(b => (b.UsageFee == minUsage));

                        for (int i = 0; i < reducedBuildings.Count; i++)
                        {
                            // Core.Log($" {reducedBuildings[i].UsageFee}");
                        }

 

                        building2dcenter = this.craftBuilding.Position;


                        if (this._buildingSize == Vector2.zero)
                        {
                            // Core.Log("setting building size");
                            this._buildingSize = this.craftBuilding.Size;
                        }
                        else
                        {
                            // Core.Log("building size is set");
                        }


                        buildingCollider = this.craftBuilding.BuildingCollider;

                        float buildingy = this.craftBuilding.Buildingy;

                        List<Vector3> getPosBoundary(Vector2 _size, Vector2 _center, float buildingH)
                        {

                            float sizecheck = 0.1f;
                            if (_size.x > 5)
                            {
                                sizecheck = 0.1f;
                            }
                            float _xmax = _center.x + (_size.x / 2);
                            float _xmin = _center.x - (_size.x / 2);
                            float _ymax = _center.y + (_size.y / 2);
                            float _ymin = _center.y - (_size.y / 2);

                            List<Vector3> _posBoundary = new List<Vector3>();

                            for (float i = _xmin; i < _xmax; i++)
                            {
                                Vector3 temp = new Vector3(i, buildingH, _ymin);

                                _posBoundary.Add(temp);

                            }

                            for (float i = _ymin; i < _ymax; i++)
                            {
                                Vector3 temp = new Vector3(_xmax, buildingH, i);

                                _posBoundary.Add(temp);

                            }

                            for (float i = _xmax; i > _xmin; i--)
                            {
                                Vector3 temp = new Vector3(i, buildingH, _ymax);

                                _posBoundary.Add(temp);

                            }

                            for (float i = _ymax; i > _ymin; i--)
                            {
                                Vector3 temp = new Vector3(_xmax, buildingH, i);

                                _posBoundary.Add(temp);

                            }
                            return _posBoundary;
                        }


                        bool Mover(List<Vector3> vList)
                        {
                            bool pathFound = false;
                            int failedAttempts = 1;
                            int iterator = 1;

                            foreach (Vector3 tryV in vList)
                            {
                                if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), tryV, IsBlockedWithExitCheck, out List<Vector3> pathing))
                                {
                                    pathFound = true;
                                    Core.Log($"PATH FOUND TO {tryV} !!!!!!!!!!!!!!!!!!!!!!!!!!!!! ");
                                    finalDestination = tryV;

                                    _craftingPathingRequest = new PositionPathingRequest(_localPlayerCharacterView, tryV, pathing, true, true);
                                    if (GameGui.Instance.ClusterMapGui.gameObject.activeInHierarchy)

                                    {
                                        Core.Log("Closing Map");
                                        GameGui.Instance.ClusterMapGui.Close();
                                    }
                                    return pathFound;
                                }
                                else
                                {
                                    Core.Log($"No path to {tryV} found. Attempt {iterator} / {vList.Count}");
                                    failedAttempts++;

                                    _failedFindAttempts = 0;
                                }
                                iterator++;
                            }

                            return pathFound;

                        }

                        List<List<Vector3>> ListReducer(List<Vector3> full)
                        {
                            var lShort = new List<List<Vector3>>();
                            int maxListLength = 2;
                            for (int i = 0; i < full.Count; i += maxListLength)
                            {
                                lShort.Add(full.GetRange(i, Math.Min(maxListLength, full.Count - i)));
                            }

                            return lShort;

                        }

                        //if (_craftreachedPointInBetween)
                        //    return;
                       var distanceToFinalDestination =0f;
                        Vector2 PC2D = new Vector2(playerCenter.x, playerCenter.z);
                        Vector2 FD2D = new Vector2(finalDestination.x, finalDestination.z);
                        
                        var distanceToBuildingCentre = Vector2.Distance(building2dcenter, PC2D);
                        var minimumDistance = -40f;
                        if (FD2D != Vector2.zero)
                        {
                            distanceToFinalDestination = Vector2.Distance(PC2D, FD2D);
                            minimumDistance = 40f;

                        }
                        //If gets stuck
                        // if (_craftreachedPointInBetween && finalDestination != Vector3.zero && distanceToFinalDestination>minimumDistance)
                        List<CraftBuildingView> craftBuildings = null;
                        List<CraftBuildingView> reducedCraftBuildings = new List<CraftBuildingView>();
                        craftBuildings = _client.GetEntities<CraftBuildingView>((x) => { return true; });

                        if (this._craftTarget == null)
                        {
                            for (int i = 0; i < craftBuildings.Count; i++)
                            {
                               // Core.Log($"Building name {craftBuildings[i].PrefabName}");

                                string craftname = craftBuildings[i].PrefabName.ToLowerInvariant();

                                string B1name = this.craftBuilding.Title.ToLowerInvariant();

                                B1name = B1name.Replace(" ", "");
                                craftname = craftname.Replace(" ", "");

                                if (B1name.Contains(craftname))
                                {

                                    Core.Log($"Target Acquired with {craftname} using string {B1name}. Adding to reduced list ");
                                    reducedCraftBuildings.Add(craftBuildings[i]);
                                }
                                

                            }

                            Core.Log($"Reduced Craft building list count is {reducedCraftBuildings.Count}");


                            if (reducedCraftBuildings != null && reducedCraftBuildings.Count > 0)
                            {

                                float closestDistance = 1e20f;

                                for (int i = 0; i < reducedCraftBuildings.Count; i++)
                                {
                                    var CB = reducedCraftBuildings[i];
                                    var CBC = CB.GetComponentsInChildren<Collider>().First(c => c.name.ToLowerInvariant().Contains("clickable"));
                                    var CBCpos = new Vector2(CBC.transform.position.x, CBC.transform.position.z);
                                    var finalDestination2D = new Vector2(finalDestination.x, finalDestination.z);
                                    var maxDistanceToCollider = Vector2.Distance(this.craftBuilding.Position, new Vector2(this.craftBuilding.Position.x + this.craftBuilding.Size.x * 0.5f, this.craftBuilding.Position.y + this.craftBuilding.Size.y * 0.5f));
                                    var distanceBetween = Vector2.Distance(CBCpos, this.craftBuilding.Position);
                                    Core.Log($"Reduced Building name {reducedCraftBuildings[i].PrefabName}");
                                    Core.Log($"Collider {CBC}");
                                    Core.Log($"Collider Position {CBCpos}");
                                    Core.Log($"Building Center {this.craftBuilding.Position}");
                                    Core.Log($"Max distance Between Collider and Center {maxDistanceToCollider}");
                                    Core.Log($"Final Destination {finalDestination2D}");
                                    Core.Log($"Distance Between {distanceBetween}");
                                    Core.Log($"Closest distance {closestDistance}");
                                    if (distanceBetween < maxDistanceToCollider)
                                    {
                                        //closestDistance = distanceBetween;
                                        //Core.Log($"Closest distance {closestDistance}");
                                        Core.Log($"Craft Target Located ");
                                        this._craftTarget = reducedCraftBuildings[i];

                                    }
                                }
                                if (_craftreachedPointInBetween && finalDestination != Vector3.zero)
                                {
                                    Vector3 rangedFinalDestination = Vector3.zero;
                                    if (stuckTimer < DateTime.Now)
                                    {
                                        isMovingUpdate();
                                    }
                                    else
                                    {
                                        _localPlayerCharacterView.RequestMove(randomSpot);
                                        return;
                                    }

                                    if (!IsMoving)
                                    {
                                        Core.Log("Not Moving. Going To random Spot");
                                        stuckTimer = DateTime.Now + TimeSpan.FromSeconds(2);
                                        randomSpot = new Vector3(UnityEngine.Random.Range(-100f, 100f), 0, UnityEngine.Random.Range(-100f, 100f)) + _localPlayerCharacterView.transform.position;
                                        rangedFinalDestination = new Vector3(finalDestination.x + UnityEngine.Random.Range(-10f, 10f), finalDestination.y + UnityEngine.Random.Range(-10f, 10f), finalDestination.z + UnityEngine.Random.Range(-10f, 10f));
                                    }
                                    else
                                    {
                                        Core.Log("Moving CLoser with request move 1");


                                        _localPlayerCharacterView.RequestMove(rangedFinalDestination);
                                        return;
                                    }
                                }
                                //if (_craftreachedPointInBetween)
                                //{
                                //    if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), finalDestination, IsBlockedGathering, out List<Vector3> pathing))
                                //    {
                                //        Core.Log("Creating new path from crafting ");
                                //        _craftingPathingRequest = new PositionPathingRequest(_localPlayerCharacterView, finalDestination, pathing, true, true, _skipUnrestrictedPvPZones );

                                //    }
                                //    Core.Log($"Still not at destination. {distanceToFinalDestination} away");
                                //    return;

                                //}

                            }
                            else
                            {
                                if (_craftreachedPointInBetween && finalDestination !=Vector3.zero)
                                {
                                    Vector3 rangedFinalDestination = Vector3.zero;
                                    if (stuckTimer < DateTime.Now)
                                    {
                                        isMovingUpdate();
                                    }
                                    else
                                    {
                                        _localPlayerCharacterView.RequestMove(randomSpot);
                                        return;
                                    }

                                    if (!IsMoving)
                                    {
                                        Core.Log("Not Moving. Going To random Spot");
                                        stuckTimer = DateTime.Now + TimeSpan.FromSeconds(2);
                                        randomSpot = new Vector3(UnityEngine.Random.Range(-100f, 100f), 0, UnityEngine.Random.Range(-100f, 100f)) + _localPlayerCharacterView.transform.position;
                                        //rangedFinalDestination = new Vector3(finalDestination.x + UnityEngine.Random.Range(-10f, 10f), finalDestination.y + UnityEngine.Random.Range(-10f, 10f), finalDestination.z + UnityEngine.Random.Range(-10f, 10f));
                                    }
                                    else
                                    {
                                        Core.Log($"Moving CLoser with request move to final destination at {finalDestination}");

                                        
                                        _localPlayerCharacterView.RequestMove(finalDestination);
                                        return;
                                    }
                                }

                            }
                        }
                        else if (!reachedFinalDestination)
                        {

                            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), this._craftTarget, IsBlockedWithExitCheck, out List<Vector3> pathing))
                            {
                                Core.Log("FOUND A CLUSTER PATH");
                                _craftingClusterPathingRequest = new ClusterPathingRequest(_localPlayerCharacterView, this._craftTarget, pathing);
                                return;
                            }
                            else
                            {
                                Core.Log("Couldnt find a cluster path");
                            }

                        }





                            //if (this._craftTarget != null && this._craftTarget is CraftBuildingView cb)
                            //{
                            //    Core.Log("Close Enough ");
                            //    _craftingPathingRequest = null;
                            //    _craftreachedPointInBetween = false;
                            //    return;
                            //}


                            //if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), finalDestination, IsBlockedGathering, out List<Vector3> pathing))
                            //{
                            //    Core.Log("Creating new path from crafting ");
                            //    _craftingPathingRequest = new PositionPathingRequest(_localPlayerCharacterView, finalDestination, pathing, true, true);

                            //}
                            //else
                            //{
                            //    Core.Log($" Not Moving Timer {notMovingTimer}");
                            //    Core.Log($"Time Now {DateTime.Now}");
                            //    if (DateTime.Now > notMovingTimer)
                            //    {
                            //        Core.Log("Moving from crafting");

                            //        isMovingUpdate();
                            //    }
                            //    else
                            //    {
                            //        Core.Log($"Moving To random position (Crafting) {randomSpot}");
                            //        _localPlayerCharacterView.RequestMove(randomSpot);
                            //        return;
                            //    }

                            //    if (!IsMoving)
                            //    {
                            //        Core.Log("Not moving called from Crafting");
                            //        randomSpot = (_localPlayerCharacterView.transform.position + new Vector3(UnityEngine.Random.Range(-100f, 100f), 0, UnityEngine.Random.Range(-100f, 100f)));
                            //        notMovingTimer = DateTime.Now + TimeSpan.FromSeconds(2);
                            //    }
                            //}
                        

                        //if (_craftreachedPointInBetween)
                        //{
                        //    Core.Log("Pathing Request Restarting");
                        //    _craftPathFound = !_craftreachedPointInBetween;
                        //}

                        if (!this._craftPathFound)
                        {

                            Core.Log("[Move closer to craftStation]");


                            if (posBoundary == null || posBoundary.Count <= 0)
                            {
                                posBoundary = getPosBoundary(_buildingSize, building2dcenter, buildingy);
                            }

                            var shortPosBoundary = ListReducer(posBoundary);

                            Core.Log($"Counter at {pathFindingCounter} out of {shortPosBoundary.Count}");
                            Core.Log($"current building size {_buildingSize}");

                            if (pathFindingCounter < shortPosBoundary.Count && !this._craftPathFound)
                            {
                                this._craftPathFound = Mover(shortPosBoundary[pathFindingCounter]);
                                pathFindingCounter++;
                            }

                            else
                            {
                                Core.Log("Increasing Boundary Size by 0.5");
                                _buildingSize = new Vector2(_buildingSize.x + .5f, _buildingSize.x + .5f);
                                posBoundary = getPosBoundary(_buildingSize, building2dcenter, buildingy);

                                Core.Log($"Changed building size {_buildingSize}");

                                pathFindingCounter = 0;
                            }
                        }


                       Core.Log($" Distance is { distanceToFinalDestination} between points {PC2D} and {FD2D}. {reachedFinalDestination}");



                        if (distanceToFinalDestination > minimumDistance && reachedFinalDestination==false )
                        {
                            //if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), finalDestination, IsBlockedGathering, out List<Vector3> pathing))
                            //{
                            //    Core.Log("Creating new path from crafting ");
                            //    _craftingPathingRequest = new PositionPathingRequest(_localPlayerCharacterView, finalDestination, pathing, true, true);

                            //}
                            Core.Log($"Still not at destination. {distanceToFinalDestination} away");
                            return;

                        }
                        else
                        {
                            Core.Log($" Distance { distanceToFinalDestination} between points {playerCenter} and {finalDestination}. {reachedFinalDestination}");
                            Core.Log("At final Destination");
                            reachedFinalDestination = true;
                        }





                        //Core.Log($"Target building name {this.craftBuilding.Title.ToLowerInvariant()}");



                        if (this._craftTarget == null)
                        {
                            Core.Log("No Craft Target");
                            return;
                        }


                        //Core.Log("Ready to interact2");

                        if (this._craftTarget is CraftBuildingView craftbuilding)
                        {
                            string name = this._craftTarget.PrefabName;
                            // Core.Log($"Building is {this.craftBuilding.Title}");
                            // Core.Log("Current Target is " + name);
                            GameGui.Instance.MessageBox.CloseImmediately();

                            if (!GameGui.Instance.BuildingUsageAndManagementGui.gameObject.activeInHierarchy)

                                if (!craftbuilding.IsInUseRange(_localPlayerCharacterView.LocalPlayerCharacter))
                                {

                                    if (stuckTimer < DateTime.Now)
                                    {
                                        isMovingUpdate();
                                    }
                                    else
                                    {
                                        _localPlayerCharacterView.RequestMove(randomSpot);
                                        return;
                                    }

                                    if (!IsMoving)
                                    {
                                        Core.Log("Not Moving. Going To random Spot");
                                        stuckTimer = DateTime.Now + TimeSpan.FromSeconds(2);
                                        randomSpot = new Vector3(UnityEngine.Random.Range(-100f, 100f), 0, UnityEngine.Random.Range(-100f, 100f)) + _localPlayerCharacterView.transform.position;

                                    }
                                    else
                                    {
                                        Core.Log("getcloser");
                                        _localPlayerCharacterView.Interact(craftbuilding);
                                        return;
                                    }
                                }
                                else
                                {
                                    Core.Log($"Close Enough {playerCenter}");
                                }

                            else
                            {
                                var craftUsage = GameGui.Instance.BuildingUsageAndManagementGui.BuildingUsage;

                                var silverUI = GameGui.Instance.PaySilverDetailGui;

                                //Select the correct craft tab
                                if (!GameGui.Instance.ItemCraftingGui.gameObject.activeInHierarchy)
                                {
                                    BigTabEntry[] BTE = craftUsage.TabButtons.TabEntries;
                                    string tabname = craftRec.CraftTab;
                                    foreach (BigTabEntry BT in BTE)
                                    {
                                        string tabn = BT.Tooltip.TextToDisplay.ToLowerInvariant();
                                        craftUsage.OnShowCrafting();
                                        if (tabn.Contains(tabname))
                                        {
                                            Core.Log($"{tabn} contains{BT.Tooltip.TextToDisplay}. Setting tab to display");
                                            BT.OnClickTab();
                                        }
                                    }

                                }

                                if (!craftUsage.CraftItemView.FilterListToggle.value)
                                {

                                    craftUsage.CraftItemView.FilterListToggle.value = true;
                                    return;
                                }

                                if (timeCooldownMax == DateTime.MaxValue)
                                {
                                    timeCooldownMax = DateTime.Now + TimeSpan.FromSeconds(1);
                                }

                                if (timeCooldownMax > DateTime.Now)
                                {
                                    Core.Log($"Time Now {DateTime.Now}");
                                    return;
                                }


                                // Core.Log("Here2");
                                if (silverUI.gameObject.activeInHierarchy)
                                {
                                    Core.Log("[Paying silver costs]");
                                    silverUI.OnPay();
                                    return;
                                }

                                if (craftUsage.gameObject.activeInHierarchy)
                                {
                                    if (_localPlayerCharacterView.IsCrafting())
                                        return;


                                    //craftUsage.OnShowCrafting();

                                   
                                    else
                                    {

                                        KguiCraftItemSlot[] craftSlots = craftUsage.GetComponentsInChildren<KguiCraftItemSlot>();
                                        // Core.Log("Here3");
                                        //Core.Log($"Number of Slots craftitem {comps.Length}");
                                        if (craftUsage.CraftItemView.gameObject.activeInHierarchy)
                                        {

                                            if (craftUsage.CraftItemView.FilterListToggle.value)
                                            {

                                                if (!GameGui.Instance.ItemCraftingGui.gameObject.activeInHierarchy)
                                                {
                                                    Core.Log("Activating CraftingGui");
                                                    KguiCraftItemSlot craftSlot = craftSlots[craftSlots.Length - 1];


                                                    try
                                                    {
                                                        if (craftSlots.Length < 1)
                                                        {
                                                            Core.Log("Cannot craft any items");
                                                            return;
                                                        }

                                                        if (craftSlots.Length > 1)
                                                        {
                                                            for (int i = 0; i < craftSlots.Length; i++)
                                                            {
                                                                string recipeName;
                                                                string[] subRecipeName;
                                                                string keyWord;
                                                                string slotLabelName = craftSlots[i].ItemNameLabel.text.ToLowerInvariant();
                                                                slotLabelName = slotLabelName.Replace(" ", "");
                                                                if (craftRec.AltName == null)
                                                                {
                                                                    recipeName = craftRec.RecipeName;

                                                                    subRecipeName = recipeName.Split('_');
                                                                    keyWord = subRecipeName[subRecipeName.Length - 1];
                                                                }
                                                                else
                                                                {
                                                                    keyWord = craftRec.AltName;

                                                                }


                                                                // string recipeName = "t3_2h_tool_pick";

                                                                if (slotLabelName.Contains(keyWord))
                                                                {
                                                                    Core.Log($"{slotLabelName} CONTAINED {keyWord} ");
                                                                    craftSlot = craftSlots[i];
                                                                    craftUsage.CraftItemView.OnCraftSlotClicked(craftSlot);
                                                                    // buildingLoaded = false;
                                                                    //timeCooldownMax = DateTime.MaxValue;
                                                                    return;
                                                                }
                                                                else
                                                                {
                                                                    Core.Log($"{slotLabelName} didn't contain {keyWord} ");
                                                                    //return;
                                                                }
                                                                // Core.Log($"{slot.ItemNameLabel.text}");
                                                            }



                                                            return;
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
                                                    if (craftSlots.Length > 0 && !_localPlayerCharacterView.IsCrafting())
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
                                    return;
                                }

                            }

                        }




                    }
                    catch (NullReferenceException)
                    {
                        Core.Log("here 6");
                    }

                }

            }         //_isUIshown = true;

            else
            {
                var pathfinder = new WorldmapPathfinder();
                Core.Log("Finding path to cluser");
                if (pathfinder.TryFindPath(currentWorldCluster, townCluster, StopClusterFunction, out var path, out var pivots))
                {
                    _worldPathingRequest = new WorldPathingRequest(currentWorldCluster, townCluster, path, _skipUnrestrictedPvPZones);
                }
            }
            void reset()
            {

                _buildingSize = Vector2.zero;

                buildingCollider = null;

                building3dcenter = Vector3.zero;

                building2dcenter = Vector3.zero;

                building2d3dcenter = Vector3.zero;

                closestpoint = Vector3.zero;

                CraftBuildingView _craftTarget = null;

            }






        }
    }


}

