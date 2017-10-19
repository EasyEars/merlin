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
    /* Internal type: e5 */
    public partial class EquipmentItemDescriptor : DurableItemDescriptor
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private e5 _internal;
        
        #region Properties
        
        public e5 EquipmentItemDescriptor_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public EquipmentItemDescriptor(e5 instance) : base(instance)
        {
            _internal = instance;
        }
        
        static EquipmentItemDescriptor()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator e5(EquipmentItemDescriptor instance)
        {
            return instance._internal;
        }
        
        public static implicit operator EquipmentItemDescriptor(e5 instance)
        {
            return new EquipmentItemDescriptor(instance);
        }
        
        public static implicit operator bool(EquipmentItemDescriptor instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
