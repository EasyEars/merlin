////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.0.336.100246-prod
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
    /* Internal type: ati */
    public partial class DurableItemProxy : ItemProxy
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private ati _internal;
        
        #region Properties
        
        public ati DurableItemProxy_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public Number GetUnfloatyFloat() => _internal.b3();
        
        #endregion
        
        #region Constructor
        
        public DurableItemProxy(ati instance) : base(instance)
        {
            _internal = instance;
        }
        
        static DurableItemProxy()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator ati(DurableItemProxy instance)
        {
            return instance._internal;
        }
        
        public static implicit operator DurableItemProxy(ati instance)
        {
            return new DurableItemProxy(instance);
        }
        
        public static implicit operator bool(DurableItemProxy instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
