using Merlin.API.Direct;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Pathing
{
    public class PositionPathingRequest
    {
        #region Fields

        private bool _useCollider;
        private bool _allowMinDistCheck;

        private LocalPlayerCharacterView _player;
        private Vector3 _target;

        private List<Vector3> _path;
        private List<Vector3> _completedpath;

        private StateMachine<State, Trigger> _state;

        DateTime _pauseTimer;
        DateTime _notMovingTimer;
        public bool _stuckCheck;

        private GameManager _client;
        private LandscapeManager _landscape;
        private bool _skipUnrestrictedPvPZones;
        private CollisionManager _collision;
        private ObjectManager _world;
        //Moving fields
        private float noMovementThreshold = 0.01f;
        private const int noMovementFrames = 2;
        Vector3[] previousLocations = new Vector3[noMovementFrames];
        private bool isMoving;

        #endregion Fields

        #region Properties and Events

        public bool IsRunning => _state.State != State.Finish;

        #endregion Properties and Events

        #region Constructors and Cleanup

        public PositionPathingRequest(LocalPlayerCharacterView player, Vector3 target, List<Vector3> path, bool useCollider = true, bool allowMinDistCheck=false, bool skipUnrestrictedPvPZones =true)
        {
            _client = GameManager.GetInstance();
            _world = ObjectManager.GetInstance();
            _collision = _world.GetCollisionManager();
            _player = player;
            _target = target;
            _stuckCheck = false;
            _path = path;
            _allowMinDistCheck = allowMinDistCheck;
            _completedpath = new List<Vector3>();
        _useCollider = useCollider;
            DateTime _pauseTimer = DateTime.Now;
            _skipUnrestrictedPvPZones = skipUnrestrictedPvPZones;
            DateTime _notMovingTimer = DateTime.Now;

            _state = new StateMachine<State, Trigger>(State.Start);

            _state.Configure(State.Start)
                .Permit(Trigger.ApproachTarget, State.Running);
            

            _state.Configure(State.Running)
                .Permit(Trigger.ReachedTarget, State.Finish)
            .Permit(Trigger.Stuck, State.Pause);

            _state.Configure(State.Pause)
                .Permit(Trigger.ReachedTarget, State.Finish)
                .Permit(Trigger.ApproachTarget, State.Running)
            .Permit(Trigger.Restart, State.Start);
        }

        #endregion Constructors and Cleanup

        #region Methods

        public void Continue()
        {
            switch (_state.State)
            {

                case State.Pause:
                    {

                        var previousNode = _completedpath[_completedpath.Count - 1];
                        var playerPosV2 = new Vector2(_player.transform.position.x, _player.transform.position.z);
                        var previousNodeV2 = new Vector2(previousNode.x, previousNode.z);
                        var target2D = new Vector2(_target.x, _target.z);
                        var distanceToTarget = Vector2.Distance(playerPosV2, target2D);
                        var distancePreviousToNode = (playerPosV2 - previousNodeV2).sqrMagnitude;
                        var minimumDistance = 2f;
                        _landscape = _client.GetLandscapeManager();

                        Core.Log("Paused");
                        if (DateTime.Now < _notMovingTimer)
                        {
                            Core.Log("Checking for movement restart");
                            isMovingUpdate();
                            if (IsMoving)
                            {
                                Core.Log("MOVING AGAIN");
                                _state.Fire( Trigger.ApproachTarget);
                                break;
                            }

                            break;
                        }

                        if(distanceToTarget < 40 && _allowMinDistCheck)
                        {
                            Core.Log($"The target {target2D} is {distanceToTarget} from this position {playerPosV2}");
                            Core.Log("@ target");
                            _state.Fire(Trigger.ReachedTarget);
                            break;
                        }





                        if (DateTime.Now > _pauseTimer)
                        {
                            _state.Fire(Trigger.ReachedTarget);
                            break;
                        }
                       

                        
                        
                        if (_completedpath.Count < 1)
                        {
                            Vector3 randomSpot = new Vector3(UnityEngine.Random.Range(-100f, 100f), 0, UnityEngine.Random.Range(-100f, 100f)) + _player.transform.position;
                            _completedpath.Add(randomSpot);
                            break;
                        }

                        if (_path.Count < 2)
                        {
                            _player.RequestMove(_player.transform.position + new Vector3(UnityEngine.Random.Range(-100f, 100f), 0, UnityEngine.Random.Range(-100f, 100f)));
                        }


                        if (distancePreviousToNode < minimumDistance)
                        {
                            if (_player.TryFindPath(new ClusterPathfinder(), _target, IsBlockedWithExitCheck, out List<Vector3> pathing))
                            {

                                Core.Log("Reached Previous Node and found path");
                                
                                _path = pathing;
                                _state.Fire(Trigger.Restart);
                                break;
                            } else
                            {
                                Core.Log("Reached Previous Node and didnt find path. Moving to next previous node");
                                _completedpath.RemoveAt(_completedpath.Count - 1);

                                _pauseTimer = DateTime.Now + TimeSpan.FromSeconds(15);
                            }


                            
                        }
                        else
                        {
                            _player.RequestMove(previousNode);
                        }
                        
                       

                        break;
                    }

                case State.Start:
                    {
                        Core.Log($"Position pathing request path count {_path.Count}");
                        if (_path.Count > 0)
                            _state.Fire(Trigger.ApproachTarget);
                        else
                            _state.Fire(Trigger.ReachedTarget);

                        break;
                    }

                case State.Running:
                    {
                        //Early exit if player is null.
                        if (_player == null)
                        {
                            _state.Fire(Trigger.ReachedTarget);
                            break;
                        }

                        isMovingUpdate();
                      // Core.Log($"Position pathing request is Moving {IsMoving} at { _player.transform.position}");
                        if (!IsMoving)
                        {
                            _notMovingTimer = DateTime.Now + TimeSpan.FromSeconds(1.5);
                            _pauseTimer = DateTime.Now + TimeSpan.FromSeconds(15);
                            Core.Log($"Stuck Position Pathing Request During Running. State is {_state}");
                            _state.Fire(Trigger.Stuck);
                            Core.Log($"Position Pathing Request During Running 2. State is {_state}");
                        }


                        var currentNode = _path[0];
                        var minimumDistance = 3f;

                        if (_path.Count < 2 && _useCollider)
                        {
                            minimumDistance = _player.GetColliderExtents();

                            var directionToPlayer = (_player.transform.position - _target).normalized;
                            var bufferDistance = directionToPlayer * minimumDistance;

                            currentNode = _target + bufferDistance;
                        }

                        var playerPosV2 = new Vector2(_player.transform.position.x, _player.transform.position.z);
                        var currentNodeV2 = new Vector2(currentNode.x, currentNode.z);

                        var distanceToNode = (playerPosV2 - currentNodeV2).sqrMagnitude;

                        if (distanceToNode < minimumDistance)
                        {
                            _completedpath.Add(_path[0]);
                            _path.RemoveAt(0);
                        }
                        else
                        {
                            _player.RequestMove(currentNode);
                        }

                        if (_path.Count > 0)
                            break;

                        _state.Fire(Trigger.ReachedTarget);
                        break;
                    }
            }
        }

        public bool IsBlockedWithExitCheck(Vector2 location)
        {
            var vector = new Vector3(location.x, 0, location.y);

            if (_skipUnrestrictedPvPZones && _landscape.IsInAnyUnrestrictedPvpZone(vector))
                return true;

            byte cf = _collision.GetCollision(location.b(), 2.0f);
            if (cf == 255)
            {
                //if the location contains an exit return false (passable), otherwise return true
                var location3d = new Vector3(location.x, 0, location.y);
                var locationContainsExit = Physics.OverlapSphere(location3d, 2.0f).Any(c => c.name.ToLowerInvariant().Equals("exit"));
                return !locationContainsExit;
            }
            else
                return (((cf & 0x01) != 0) || ((cf & 0x02) != 0));
        }



        //check for moving
        public bool IsMoving
        {
            get { return isMoving; }
        }

        void isMovingUpdate()
        {
           
            //previousLocations = new Vector3[noMovementFrames];
            
            for (int i = 0; i < previousLocations.Length - 1; i++)
            {
                previousLocations[i] = previousLocations[i + 1];
            }
            previousLocations[previousLocations.Length - 1] = _player.transform.position;

         
            for (int i = 0; i < previousLocations.Length - 1; i++)
            {
               // Core.Log($"distance between frames {Vector3.Distance(previousLocations[i], previousLocations[i + 1])} for points {previousLocations[i]} and {previousLocations[i+1]}");

                if (Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold)
                {


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
            ApproachTarget,
            ReachedTarget,
            Stuck,
                Restart
        }

        private enum State
        {
            Start,
            Running,
            Finish,
            Pause
        }
    }
}