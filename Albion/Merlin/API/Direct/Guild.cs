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
    /* Internal type: as1 */
    public partial class Guild
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private as1 _internal;
        
        #region Properties
        
        public as1 Guild_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public GuildMember[] GetMembers() => _internal.ag().Select(x =>(GuildMember)x).ToArray();
        
        #endregion
        
        #region Constructor
        
        public Guild(as1 instance)
        {
            _internal = instance;
        }
        
        static Guild()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator as1(Guild instance)
        {
            return instance._internal;
        }
        
        public static implicit operator Guild(as1 instance)
        {
            return new Guild(instance);
        }
        
        public static implicit operator bool(Guild instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
