using Albion.Common.Time;
using Merlin.API;
using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private static List<Tuple<SpellTarget, SpellCategory, bool>> SpellPriorityList = new List<Tuple<SpellTarget, SpellCategory, bool>>
        {
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Self, SpellCategory.Buff, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Self, SpellCategory.Damage, true),
           // new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Ground, SpellCategory.CrowdControl, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Self, SpellCategory.CrowdControl, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Enemy, SpellCategory.Damage, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Enemy, SpellCategory.Buff, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Ground, SpellCategory.Damage, true), 
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Enemy, SpellCategory.MovementBuff, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Self, SpellCategory.MovementBuff, true),
        };

        private LocalPlayerCharacter _combatPlayer;
        private FightingObjectView _combatTarget;
        private IEnumerable<SpellSlot> _combatSpells;
        private float _combatCooldown;
        GameTimeStamp castEnd5 = GameTimeStamp.MinValue;
        GameTimeStamp castEnd4 = GameTimeStamp.MinValue;
        GameTimeStamp castEnd3 = GameTimeStamp.MinValue;
        GameTimeStamp castEnd2 = GameTimeStamp.MinValue;
        GameTimeStamp castEnd = GameTimeStamp.MinValue;

        GameTimeStamp castStart1 = GameTimeStamp.MinValue;
        GameTimeStamp castStart2 = GameTimeStamp.MinValue;
        GameTimeStamp castStart3 = GameTimeStamp.MinValue;
        public void Fight()
        {
            if (_localPlayerCharacterView.IsMounted)
            {
                _localPlayerCharacterView.MountOrDismount();
                return;
            }

            if (_combatCooldown > 0)
            {
                _combatCooldown -= UnityEngine.Time.deltaTime;
                return;
            }
            //Core.Log($" Game Time Now {GameTimeStamp.Now.ToString()}");
            //Core.Log($" Cast End Time  {castEnd.ToString()}");
            //Core.Log($" Cast2 End Time  {castEnd2.ToString()}");


            //Core.Log($" Cast start Time  {castStart1.ToString()}");
            //Core.Log($" Cast2 start Time  {castStart2.ToString()}");


            //Core.Log($"{castEnd2.CompareTo(GameTimeStamp.Now)}");



            //if (castEnd2.CompareTo(GameTimeStamp.Now) == 1)
            //{
            //    Core.Log("Channelling");
            //    return;
            //}

            _combatPlayer = _localPlayerCharacterView.GetLocalPlayerCharacter();
            _combatTarget = _localPlayerCharacterView.GetAttackTarget();
            _combatSpells = _combatPlayer.GetSpellSlotsIndexed().Ready(_localPlayerCharacterView).Ignore("ESCAPE_DUNGEON").Ignore("PLAYER_COUPDEGRACE").Ignore("AMBUSH");

            foreach (var cs in _combatSpells)
            { 
               // Core.Log($"{cs.GetSpellDescriptor().TryGetName()}");
                //Core.Log($"{cs.GetSpellDescriptor().TryGetTarget()}");
               //Core.Log($"{cs.GetSpellDescriptor().TryGetCategory()}");
            }
                



            if (_combatTarget != null && !_combatTarget.IsDead() && SpellPriorityList.Any(s => TryToCastSpell(s.Item1, s.Item2, s.Item3)))
                return;

            if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker))
            {
                _localPlayerCharacterView.SetSelectedObject(attacker);
                _localPlayerCharacterView.AttackSelectedObject();
                return;
            }

           // if (GameTimeStamp.Now < channelEnd)
               // Core.Log($"{GameTimeStamp.Now.ToString()}");
            if (_combatPlayer.GetIsCasting())
                return;

            if (_combatPlayer.GetHealth().GetValue() < (_combatPlayer.GetHealth().GetMaximum() * 0.8f))
            {
                var healSpell = _combatSpells.Target(SpellTarget.Self).Category(SpellCategory.Heal);

                if (healSpell.Any())
                    _localPlayerCharacterView.CastOnSelf(healSpell.FirstOrDefault().Slot);
                return;
            }

            _currentTarget = null;
            _harvestPathingRequest = null;

            Core.Log("[Eliminated]");
            _state.Fire(Trigger.EliminatedAttacker);
        }

        private bool TryToCastSpell(SpellTarget target, SpellCategory category, bool checkCastState)
        {
            try
            {
                //if (checkCastState && _localPlayerCharacterView.IsCasting())
                //    return true;

                if (checkCastState && _localPlayerCharacterView.IsCasting())
                    return false;
                



                var spells = _combatSpells.Target(target).Category(category);
                // Core.Log($"{_localPlayerCharacterView.IsCasting()}");

                SpellSlot spellToCast;
                if (spells.Count() > 1)
                {
                    Random random = new Random(DateTime.Now.Millisecond);
                    spellToCast = spells.ElementAt(random.Next(0, spells.Count()));
                }
                else
                {
                    //var spellToCast = spells.Any() ? spells.First() : null;
                    spellToCast = spells.Any() ? spells.First() : null;
                }


                if (spellToCast == null)
                    return false;

                var spellName = "Unknown";
                try
                {
                    spellName = spellToCast.GetSpellDescriptor().TryGetName();
                    Core.Log($"[Casting {spellName}]");

                    var spellSlot = spellToCast.Slot;

                    castStart1 = _localPlayerCharacterView.GetCastStartTime();
                    castStart2 = _localPlayerCharacterView.GetChannelingStartTimeStamp();

                    castEnd = _localPlayerCharacterView.GetCastEndTime();
                    castEnd2 = _localPlayerCharacterView.GetCastFinishedEndTimeStamp();
                    
                    switch (target)
                    {
                        case (SpellTarget.Self):
                            _localPlayerCharacterView.CastOnSelf(spellSlot);
                            break;

                        case (SpellTarget.Enemy):
                            _localPlayerCharacterView.CastOn(spellSlot, _combatTarget);
                            break;

                        case (SpellTarget.Ground):
                            _localPlayerCharacterView.CastAt(spellSlot, _combatTarget.GetPosition());
                            break;

                        default:
                            Core.Log($"[SpellTarget {target} is not supported. Spell skipped]");
                            return false;
                    }

                    _combatCooldown = 0.1f;
                    return true;
                }
                catch (Exception e)
                {
                    Core.Log($"[Error while casting {spellName} ({target}/{category}/{checkCastState})]");
                    Core.Log(e);
                    return false;
                }
            }
            catch (Exception e)
            {
                Core.Log($"[Generic casting error ({target}/{category}/{checkCastState})]");
                Core.Log(e);
                return false;
            }
        }
    }
}