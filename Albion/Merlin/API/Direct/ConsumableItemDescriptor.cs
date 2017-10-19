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
    /* Internal type: dp */
    public partial class ConsumableItemDescriptor : SimpleItemDescriptor
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private dp _internal;
        
        #region Properties
        
        public dp ConsumableItemDescriptor_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public ConsumableItemDescriptor(dp instance) : base(instance)
        {
            _internal = instance;
        }
        
        static ConsumableItemDescriptor()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator dp(ConsumableItemDescriptor instance)
        {
            return instance._internal;
        }
        
        public static implicit operator ConsumableItemDescriptor(dp instance)
        {
            return new ConsumableItemDescriptor(instance);
        }
        
        public static implicit operator bool(ConsumableItemDescriptor instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
