////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.0.327.94396-live
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
    /* Internal type: ar8 */
    public class InventoryItemObjectCollection
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        
        private ar8 _internal;
        
        #region Properties
        
        public ar8 Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public InventoryItemObjectCollection(ar8 instance)
        {
            _internal = instance;
        }
        
        static InventoryItemObjectCollection()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator ar8(InventoryItemObjectCollection instance)
        {
            return instance._internal;
        }
        
        public static implicit operator InventoryItemObjectCollection(ar8 instance)
        {
            return new InventoryItemObjectCollection(instance);
        }
        
        #endregion
    }
}
