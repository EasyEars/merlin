using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.Profiles.Gatherer
{
    public class Resource
    {
       public bool Split { get; set; }
        public int SlotPos { get; set; }
       public string Name { get; set; }
        public float SplitAmount { get; set; }
        public bool Retrieved { get; set; }
        public bool Moved { get; set; }
        public float Weight { get; set; }
        public float StackSize { get; set; }
        public float TotalAmount { get; set; }
        public bool NeededSplitting { get; set; }
        public int BaseAmountForSelectedRecipe { get; set; } // base amount to satifie the ratio of resources for the recipe. Eg for t3_plank base amount for t3_wood is 2
        public bool Available { get; set; }

        public float CurrentAmountNeeded{ get; set; } //Keeps track of the amount of the resource needed to reach the maximum weight/ craft count
        public float MaxAmountForselectedRecipe { get; set; } // total amount needed to reach maximum weight based on the selected recipe
        public Resource(string _name, bool _split = false, int _slotPos = -1, bool _retrieved = false, float _totalamount = 0f, bool _neededsplitting = true, bool _moved = false, bool _available = false)
        {
            this.Name = _name;
            this.Split = _split;
                this.SlotPos = _slotPos;
            this.Retrieved = _retrieved;
            this.TotalAmount = _totalamount;
            this.NeededSplitting = _neededsplitting;
            this.Moved = _moved;
            this.Available = _available;


        }

        

    }
}
