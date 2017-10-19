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
    /* Internal type: atr */
    public partial class ItemObjectCollection
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private atr _internal;
        
        #region Properties
        
        public atr ItemObjectCollection_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public ItemObjectCollection(atr instance)
        {
            _internal = instance;
        }
        
        static ItemObjectCollection()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator atr(ItemObjectCollection instance)
        {
            return instance._internal;
        }
        
        public static implicit operator ItemObjectCollection(atr instance)
        {
            return new ItemObjectCollection(instance);
        }
        
        public static implicit operator bool(ItemObjectCollection instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
