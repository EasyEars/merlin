////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.0.332.98217-prod
////////////////////////////////////////////////////////////////////////////////////
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Merlin.API.Direct
{
    /* Internal type: g7.SpellTarget */
    public enum SpellTarget
    {
        None = 0,
        Self = 1,
        Other = 2,
        All = 3,
        AllMobs = 4,
        AllPlayers = 5,
        Enemy = 6,
        EnemyMobs = 7,
        EnemyPlayers = 8,
        FriendOther = 9,
        FriendOtherMobs = 10,
        FriendOtherPlayers = 11,
        FriendAll = 12,
        FriendAllMobs = 13,
        FriendAllPlayers = 14,
        Ground = 15,
        KnockedDownPlayer = 16,
        DeadPlayer = 17
    }
    
    public static class SpellTargetExtensions
    {
        public static g7.SpellTarget ToInternal(this SpellTarget instance)
        {
            return (g7.SpellTarget)instance;
        }
        
        public static SpellTarget ToWrapped(this g7.SpellTarget instance)
        {
            return (SpellTarget)instance;
        }
    }
}
