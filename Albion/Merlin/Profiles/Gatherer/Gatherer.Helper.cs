using Merlin.API.Direct;
using Merlin.Pathing;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{

   
    public partial class Gatherer
    {
        bool isPlayerStuck = false;
        public IEnumerator HandleNotMoving()
        {
            Core.Log("Not moving");

            int vScale = 20;
            Vector3 velocity = Vector3.zero;

            Vector3 unStick = _localPlayerCharacterView.transform.position + _localPlayerCharacterView.transform.forward * -1 * vScale;

            Vector3 target = Vector3.SmoothDamp(_localPlayerCharacterView.transform.position, unStick, ref velocity, 5f);

            _localPlayerCharacterView.RequestMove(unStick);
            yield return new WaitForSecondsRealtime(5);
            isPlayerStuck = false;
           // yield return true;
        }




        public bool HandleAttackers()
        {
            if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker))
            {
                Core.Log("[Attacked]");
                _state.Fire(Trigger.EncounteredAttacker);
                return true;
            }
            return false;
        }

        public bool HandlePathing(ref WorldPathingRequest request, Func<bool> breakFunc = null, Action onDone = null, bool ignoreMount = false)
        {
            if (request != null)
            {
                if (!ignoreMount && !HandleMounting(Vector3.zero)) {
                    //iCore.Log("Handle mounting via handlepathing worldpathing request");
                    return true;
                }
                if ((breakFunc?.Invoke()).GetValueOrDefault())
                    request = null;
                else if (request.IsRunning)
                {
                    //Core.Log("Continue via handlepathing worldpathing request");
                    request.Continue();
                    
                }
                else
                {
                    request = null;
                    onDone?.Invoke();
                    //Core.Log("Other via handlepathing worldpathing request");
                }
                //Core.Log("Defaulting to true handlepathing worldpathing request");
                return true;
            }

            return false;
        }

        public bool HandlePathing(ref ClusterPathingRequest request, Func<bool> breakFunc = null, Action onDone = null, bool ignoreMount = false)
        {
            if (request != null)
            {
                if (!ignoreMount && !HandleMounting(Vector3.zero))
                    return true;

                if ((breakFunc?.Invoke()).GetValueOrDefault())
                    request = null;
                else if (request.IsRunning)
                {
                        request.Continue();
                }
                else
                {
                    request = null;
                    onDone?.Invoke();
                }

                return true;
            }

            return false;
        }

        public bool HandlePathing(ref PositionPathingRequest request, Func<bool> breakFunc = null, Action onDone = null, bool ignoreMount = false)
        {
            if (request != null)
            {
                if (!ignoreMount && !HandleMounting(Vector3.zero))
                    return true;

                if ((breakFunc?.Invoke()).GetValueOrDefault())
                    request = null;
                else if (request.IsRunning) {

                        request.Continue();
                    
                }
                else
                {
                    request = null;
                    onDone?.Invoke();
                }

                return true;
            }

            return false;
        }

        public bool HandleMounting(Vector3 target)
        {
            if (!_localPlayerCharacterView.IsMounted)
            {
                LocalPlayerCharacter localPlayer = _localPlayerCharacterView.LocalPlayerCharacter;
                if (localPlayer.GetIsMounting())
                    return false;

                var mount = _client.GetEntities<MountObjectView>(m => m.IsInUseRange(localPlayer)).FirstOrDefault();
                if (mount != null)
                {
                    if (target != Vector3.zero && mount.IsInUseRange(localPlayer))
                        return true;

                    if (mount.IsInUseRange(localPlayer))
                        _localPlayerCharacterView.Interact(mount);
                    else
                        _localPlayerCharacterView.MountOrDismount();
                }
                else
                    _localPlayerCharacterView.MountOrDismount();

                return false;
            }

            return true;
        }
    }
}