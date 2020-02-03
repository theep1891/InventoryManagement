using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaTecInventory.Models
{
    public class ItemName
    {
        private string Name { get; set; }
        public string GetName 
        {
            get
            {
                return Name;
            }
            set
            {
                if (value != string.Empty)
                    Name = value;
            }
        }

        public ItemName() { }
    }
}