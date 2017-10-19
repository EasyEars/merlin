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
    /* Internal type: ns */
    public partial class CompoundTileDescriptor
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private ns _internal;
        
        #region Properties
        
        public ns CompoundTileDescriptor_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public CompoundTileDescriptor(ns instance)
        {
            _internal = instance;
        }
        
        static CompoundTileDescriptor()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator ns(CompoundTileDescriptor instance)
        {
            return instance._internal;
        }
        
        public static implicit operator CompoundTileDescriptor(ns instance)
        {
            return new CompoundTileDescriptor(instance);
        }
        
        public static implicit operator bool(CompoundTileDescriptor instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
