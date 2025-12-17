using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegracionVTEX.Models
{
    public class Price
    {
        public int markup { get; set; }
        public float? listPrice { get; set; }
        public float costPrice { get; set; }
        public FixedPrice[] fixedPrices { get; set; }

        public class FixedPrice
        {
            public string tradePolicyId { get; set; }
            public float value { get; set; }
            public float? listPrice { get; set; }
            public int minQuantity { get; set; }
        }
    }
}
