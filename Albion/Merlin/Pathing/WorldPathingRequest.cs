using Merlin.API.Direct;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Pathing
{
    public class WorldPathingRequest
    {
        #region Fields

        private static readonly Vector3 HeightDifference = new Vector3 { y = 1000 };

        private GameManager _client;
        private ObjectManager _world;
        private CollisionManager _collision;
        private LandscapeManager _landscape;

        private ClusterDescriptor _origin;
        private ClusterDescriptor _destination;

        private Vector3? _destinationPosition;
        private float? _destinationExtends;

        private List<ClusterDescriptor> _path;

        private StateMachine<State, Trigger> _state;

        private PositionPathingRequest _exitPathingRequest;

        private DateTime _timeout;
        private DateTime _stuckTimer;
        private bool _skipUnrestrictedPvPZones;
        private Vector3 _randomSpot;


        //check for moving
        private float noMovementThreshold = 0.01f;
        private const int noMovementFrames = 2;
        Vector3[] previousLocations = new Vector3[noMovementFrames];
        private bool isMoving;



        #endregion Fields

        #region Properties and Events

        public bool IsRunning => _state.State != State.Finish;

        #endregion Properties and Events

        #region Constructors and Cleanup

        public WorldPathingRequest(ClusterDescriptor start, ClusterDescriptor end, List<ClusterDescriptor> path, bool skipUnrestrictedPvPZones)
        {
            _client = GameManager.GetInstance();
            _world = ObjectManager.GetInstance();
            _collision = _world.GetCollisionManager();

            _origin = start;
            _destination = end;
            _skipUnrestrictedPvPZones = skipUnrestrictedPvPZones;

            _path = path;
            _timeout = DateTime.Now;
            _stuckTimer = DateTime.MinValue;
            _randomSpot = Vector3.zero;


            _state = new StateMachine<State, Trigger>(State.Start);

            _state.Configure(State.Start)
                .Permit(Trigger.ApproachDestination, State.Running);

            _state.Configure(State.Running)
                .Permit(Trigger.ReachedDestination, State.Finish)
            .Permit(Trigger.Stuck, State.Pause);

            _state.Configure(State.Pause)
               .Permit(Trigger.ReachedDestination, State.Finish)
               .Permit(Trigger.ApproachDestination, State.Running);
        }

        #endregion Constructors and Cleanup

        #region Methods

        public void Continue()
        {


            if (_timeout > DateTime.Now)
            {
               // Core.Log($"WORLD PATHING REQUEST TIMED OUT Current State {_state}");
                return;
            }
            switch (_state.State)
            {

                case State.Pause:
                    {
                        var player = _client.GetLocalPlayerCharacterView();
                        if (DateTime.Now > _stuckTimer)
                        {
                            _state.Fire(Trigger.ReachedDestination);
                        }


                            //Core.Log($"Moving to RandomSpot {_randomSpot}");
                            player.RequestMove(_randomSpot);

                            break;
                        
                        
                    }


                case State.Start:
                    {
                        if (_path.Count > 0)
                            _state.Fire(Trigger.ApproachDestination);
                        else
                            _state.Fire(Trigger.ReachedDestination);

                        break;
                    }

                case State.Running:
                    {
                        var nextCluster = _path[0];
                        var currentCluster = _world.GetCurrentCluster();
                        var player = _client.GetLocalPlayerCharacterView();
                        isMovingUpdate();
                        //Core.Log($"{IsMoving}");
                        if (!IsMoving)
                        {
                            Core.Log("World pathing Request not moving");
                            _randomSpot = new Vector3(UnityEngine.Random.Range(-100f, 100f), 0, UnityEngine.Random.Range(-100f, 100f)) + player.transform.position;
                            _stuckTimer = DateTime.Now + TimeSpan.FromSeconds(2);
                            _state.Fire(Trigger.Stuck);
                            break;
                        }

                       // Core.Log("Beggining run world pathing request");
                        if (currentCluster.GetIdent() != nextCluster.GetIdent())
                        {
                            //Core.Log("Indent Not in next Cluster");
                            if (_exitPathingRequest != null)
                            {
                                if (_exitPathingRequest.IsRunning)
                                {
                                   // Core.Log("Exit Pathing Request Running");
                                    _exitPathingRequest.Continue();
                                }
                                else
                                {
                                    //Core.Log("Exit Pathing Request Timed Out");
                                    _timeout = DateTime.Now + TimeSpan.FromSeconds(10);
                                    _exitPathingRequest = null;
                                }

                                break;
                            }

               
                             player = _client.GetLocalPlayerCharacterView();
                            var exits = currentCluster.GetExits();


                            var exit = exits.FirstOrDefault(e => e.GetDestination().GetIdent() == nextCluster.GetIdent());
                            var exitLocation = exit.GetPosition();

                            var destination = new Vector3(exitLocation.GetX(), 0, exitLocation.GetY());

                            var exitCollider = Physics.OverlapCapsule(destination - HeightDifference, destination + HeightDifference, 2f).FirstOrDefault(c => c.name.ToLowerInvariant().Equals("exit") || c.name.ToLowerInvariant().Contains("entrance"));
                            _destinationPosition = exitCollider?.transform?.position;
                            _destinationExtends = exitCollider?.GetColliderExtents();
                            if (_destinationPosition.HasValue)
                            {
                                var temp = _destinationPosition.Value;
                                temp.y = 0;
                                _destinationPosition = temp;
                            }

                            _landscape = _client.GetLandscapeManager();
                            if (player.TryFindPath(new ClusterPathfinder(), destination, IsBlockedWithExitCheck, out List<Vector3> pathing))
                            {
                               // Core.Log("Beginning new position pathing request from world pathing request");
                                _exitPathingRequest = new PositionPathingRequest(_client.GetLocalPlayerCharacterView(), destination, pathing, false);
                            }
                        }
                        else
                        {
                            //Core.Log("Path Point Removed");
                            _path.RemoveAt(0);
                            _exitPathingRequest = null;
                        }

                        if (_path.Count > 0)
                            break;





                        _state.Fire(Trigger.ReachedDestination);
                        break;
                    }
            }
        }

        public bool IsBlockedWithExitCheck(Vector2 location)
        {
            var vector = new Vector3(location.x, 0, location.y);

            if (_skipUnrestrictedPvPZones && _landscape.IsInAnyUnrestrictedPvpZone(vector))
                return true;

            var location3d = new Vector3(location.x, 0, location.y);
            if (_destinationPosition.HasValue && _destinationExtends.HasValue && Vector3.Distance(location3d, _destinationPosition.Value) <= _destinationExtends.Value)
                return false;

            byte cf = _collision.GetCollision(location.b(), 2.0f);
            if (cf == 255)
            {
                //if the location contains an exit return false (passable), otherwise return true
                var locationContainsExit = Physics.OverlapSphere(location3d, 2.0f).Any(c => c.name.ToLowerInvariant().Equals("exit") || c.name.ToLowerInvariant().Contains("entrance"));
                return !locationContainsExit;
            }
            else
                return (((cf & 0x01) != 0) || ((cf & 0x02) != 0));
        }

       

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
            var _player = _client.GetLocalPlayerCharacterView();
            previousLocations[previousLocations.Length - 1] = _player.transform.position;

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



        #endregion Methods

        private enum Trigger
        {
            ApproachDestination,
            ReachedDestination,
            Stuck,
            
        }

        private enum State
        {
            Start,
            Running,
            Finish,
                Pause,
        }
    }
}