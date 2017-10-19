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
    /* Internal type: a0m */
    public partial class RemotePlayerCharacter : PlayerCharacter
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private a0m _internal;
        
        #region Properties
        
        public a0m RemotePlayerCharacter_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public RemotePlayerCharacter(a0m instance) : base(instance)
        {
            _internal = instance;
        }
        
        static RemotePlayerCharacter()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator a0m(RemotePlayerCharacter instance)
        {
            return instance._internal;
        }
        
        public static implicit operator RemotePlayerCharacter(a0m instance)
        {
            return new RemotePlayerCharacter(instance);
        }
        
        public static implicit operator bool(RemotePlayerCharacter instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}