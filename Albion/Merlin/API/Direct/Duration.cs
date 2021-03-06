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
    /* Internal type: ak3 */
    public partial struct Duration
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        
        private ak3 _internal;
        
        #region Properties
        
        public ak3 Duration_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public Duration(ak3 instance)
        {
            _internal = instance;
        }
        
        static Duration()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator ak3(Duration instance)
        {
            return instance._internal;
        }
        
        public static implicit operator Duration(ak3 instance)
        {
            return new Duration(instance);
        }
        #endregion
    }
}
