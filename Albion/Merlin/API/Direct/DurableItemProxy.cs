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
    /* Internal type: as8 */
    public partial class DurableItemProxy : ItemProxy
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private as8 _internal;
        
        #region Properties
        
        public as8 DurableItemProxy_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public Number GetUnfloatyFloat() => _internal.b3();
        
        #endregion
        
        #region Constructor
        
        public DurableItemProxy(as8 instance) : base(instance)
        {
            _internal = instance;
        }
        
        static DurableItemProxy()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator as8(DurableItemProxy instance)
        {
            return instance._internal;
        }
        
        public static implicit operator DurableItemProxy(as8 instance)
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
