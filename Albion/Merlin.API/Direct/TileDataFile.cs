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
    /* Internal type: mo */
    public class TileDataFile
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        
        private mo _internal;
        
        #region Properties
        
        public mo Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public CompoundTileDescriptor GetCompoundtile(string A_0) => _internal.c((string)A_0);
        public TileDescriptor GetTile(string A_0) => _internal.b((string)A_0);
        public CompoundTileDescriptor ReadCompoundTile(System.Xml.XmlReader A_0) => (CompoundTileDescriptor)_methodReflectionPool[0].Invoke(_internal,new object[]{(System.Xml.XmlReader)A_0});
        public TileDescriptor ReadTile(System.Xml.XmlReader A_0) => (TileDescriptor)_methodReflectionPool[1].Invoke(_internal,new object[]{(System.Xml.XmlReader)A_0});
        
        #endregion
        
        #region Constructor
        
        public TileDataFile(mo instance)
        {
            _internal = instance;
        }
        
        static TileDataFile()
        {
            _methodReflectionPool.Add(typeof(mo).GetMethod("b", new Type[]{typeof(System.Xml.XmlReader)}));
            _methodReflectionPool.Add(typeof(mo).GetMethod("d", new Type[]{typeof(System.Xml.XmlReader)}));
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator mo(TileDataFile instance)
        {
            return instance._internal;
        }
        
        public static implicit operator TileDataFile(mo instance)
        {
            return new TileDataFile(instance);
        }
        
        #endregion
    }
}
