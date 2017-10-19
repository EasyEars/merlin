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
    /* Internal type: aec<a> */
    public partial class ObservableRange<a> where a:ad8
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private aec<a> _internal;
        
        #region Properties
        
        public aec<a> ObservableRange_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public float GetMaximum() => _internal.l();
        public float GetMinimum() => _internal.k();
        public long GetObjectId() => _internal.q();
        public float GetValue() => _internal.r();
        public void SetMinimum(float A_0) => _methodReflectionPool[0].Invoke(_internal,new object[]{(float)A_0});
        
        #endregion
        
        #region Constructor
        
        public ObservableRange(aec<a> instance)
        {
            _internal = instance;
        }
        
        static ObservableRange()
        {
            _methodReflectionPool.Add(typeof(aec<a>).GetMethod("k", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, new Type[]{typeof(float)}, null));
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator aec<a>(ObservableRange<a> instance)
        {
            return instance._internal;
        }
        
        public static implicit operator ObservableRange<a>(aec<a> instance)
        {
            return new ObservableRange<a>(instance);
        }
        
        public static implicit operator bool(ObservableRange<a> instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
