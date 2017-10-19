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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;

using Albion.Common.Time;

namespace Merlin.API.Direct
{
    /* Internal type: aty */
    public partial class FightingObject : MovingObject
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private aty _internal;
        
        #region Properties
        
        public aty FightingObject_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public FightingAttributes GetAttributes() => _internal.at();
        public ObservableRange<ad3> GetEnergy() => _internal.wz();
        public ObservableRange<ad3> GetHealth() => _internal.wy();
        public CharacterDescriptor GetCharacterDescriptor() => _internal.w6();
        public bool GetIsAttacking() => _internal.yu();
        public bool GetIsCasting() => _internal.yq();
        public bool GetIsDead() => _internal.jg();
        public bool GetIsChanneling() => _internal.yr();
        public bool GetIsIdle() => _internal.yp();
        public float GetLoad() => _internal.ji();
        public float GetLoadPercentage() => _internal.w9();
        public float GetLoadSpeedFactor() => _internal.xa();
        public float GetMaxLoad() => _internal.w8();
        public string GetName() => _internal.iu();
        public long GetTargetId() => _internal.w4();
        public a GetEventHandler<a>() where a:atz => (a)_internal.yg<a>();
        
        #endregion
        
        #region Constructor
        
        public FightingObject(aty instance) : base(instance)
        {
            _internal = instance;
        }
        
        static FightingObject()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator aty(FightingObject instance)
        {
            return instance._internal;
        }
        
        public static implicit operator FightingObject(aty instance)
        {
            return new FightingObject(instance);
        }
        
        public static implicit operator bool(FightingObject instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
