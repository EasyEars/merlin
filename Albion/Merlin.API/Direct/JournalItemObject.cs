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
    /* Internal type: ar0 */
    public class JournalItemObject
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        
        private ar0 _internal;
        
        #region Properties
        
        public ar0 Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public JournalItemObject(ar0 instance)
        {
            _internal = instance;
        }
        
        static JournalItemObject()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator ar0(JournalItemObject instance)
        {
            return instance._internal;
        }
        
        public static implicit operator JournalItemObject(ar0 instance)
        {
            return new JournalItemObject(instance);
        }
        
        #endregion
    }
}
