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
    /* Internal type: ae4 */
    public partial class PlayerCharacterDescriptor : CharacterDescriptor
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private ae4 _internal;
        
        #region Properties
        
        public ae4 PlayerCharacterDescriptor_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public float GetHarvestRange() => _internal.dw();
        public int GetInventorySize() => _internal.dv();
        public float GetUseObjectRange() => _internal.dx();
        
        #endregion
        
        #region Constructor
        
        public PlayerCharacterDescriptor(ae4 instance) : base(instance)
        {
            _internal = instance;
        }
        
        static PlayerCharacterDescriptor()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator ae4(PlayerCharacterDescriptor instance)
        {
            return instance._internal;
        }
        
        public static implicit operator PlayerCharacterDescriptor(ae4 instance)
        {
            return new PlayerCharacterDescriptor(instance);
        }
        
        public static implicit operator bool(PlayerCharacterDescriptor instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
