using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    public class Building
    {

        public string Title { get; set; }
        public int Tier { get; set; }
        public float Food { get; set; }
        public bool CanUse { get; set; }
        public Vector2 Position { get; set; }
        public string Owner { get; set; }
        public Vector2 Size { get; set; }
        public Collider BuildingCollider { get; set; }
        public float Buildingy { get; set; }

        public float UsageFee { get; set; }

        public long Id { get; set; }

        public string ObjTitle { get; set; }

        public float Durability{get; set;}

        public Building (string _title, int _tier, float _food, bool _canuse, Vector2 _position, string _owner, Vector2 _size, Collider _collider, float _buildingy, float _usagefee , long _id, float _durability)//, string _objTitle)
        {
            this.Title = _title;
            this.Tier = _tier;
            this.Food = _food;
            this.CanUse = _canuse;
            this.Position = _position;
            this.Owner = _owner;
            this.Size = _size;
            this.BuildingCollider = _collider;
            this.Buildingy = _buildingy;
            this.UsageFee = _usagefee;
            this.Id = _id;
            this.Durability = _durability;
            //this.ObjTitle = _objTitle;

        }
        
    }

    public class Town: IEnumerable
    {
        public Building[] _building { get; set; }
        public Town(Building[] bArray)
        {
            _building = new Building[bArray.Length];

            for (int i = 0; i < bArray.Length; i++)
            {
                _building[i] = bArray[i];
            }
        }

        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public TownEnum GetEnumerator()
        {
            return new TownEnum(_building);
        }
    }
    // When you implement IEnumerable, you must also implement IEnumerator.
    public class TownEnum : IEnumerator
    {
        public Building[] _town;
        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public TownEnum(Building [] list)
        {
            _town = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < _town.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        public void Select(int index)
        {
            position = index;
        }


        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public Building Current
        {
            get
            {
                try
                {
                    return _town[position];
                 }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

    }

}
