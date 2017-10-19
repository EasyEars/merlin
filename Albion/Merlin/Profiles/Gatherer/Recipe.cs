using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.Profiles.Gatherer
{
    public class Recipe
    {
        public string RecipeName { get; set; }
        public List<Resource> Resources { get; set; }
        public List<int> ResourceRatio { get; set; }

        public float TotalWeight { get; set; }
        public bool ResourceListUpdated { get; set; }

        public string CraftBuilding { get; set; }

        public string CraftTab { get; set; }

        public string AltName { get; set; }

        public int AmountToCraft { get; set; }
        public Recipe(string _name, string _craftbuilding,string _crafttab, List<string> _resources, List<int> _resourceRatio, string _altname = null, bool _resourcelistupdated = false )
        {
            Resources = new List<Resource>();
            
            this.RecipeName = _name;
            this.CraftBuilding = _craftbuilding;
            
            for (int i = 0; i<_resources.Count;i++) 
            {
                
                Resource TR = new Resource(_resources[i]);
                TR.BaseAmountForSelectedRecipe = _resourceRatio[i];
                Resources.Add(TR);
            }
            this.ResourceListUpdated = _resourcelistupdated;

            this.ResourceRatio = _resourceRatio;
            this.CraftTab = _crafttab;
            this.AltName = _altname;
        }



    }
}
