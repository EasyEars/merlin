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
    /* Internal type: aw0 */
    public partial class CastSpellEventHandler
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private aw0 _internal;
        
        #region Properties
        
        public aw0 CastSpellEventHandler_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public bool IsReady(byte A_0) => _internal.f((byte)A_0);
        
        #endregion
        
        #region Constructor
        
        public CastSpellEventHandler(aw0 instance)
        {
            _internal = instance;
        }
        
        static CastSpellEventHandler()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator aw0(CastSpellEventHandler instance)
        {
            return instance._internal;
        }
        
        public static implicit operator CastSpellEventHandler(aw0 instance)
        {
            return new CastSpellEventHandler(instance);
        }
        
        public static implicit operator bool(CastSpellEventHandler instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
