using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegracionVTEX.Models
{
    public class SKU
    {
        public class SkuWithoutId
        {
            public int ProductId { get; set; }
            public bool IsActive { get; set; }
            public bool ActivateIfPossible { get; set; }
            public string Name { get; set; }
            public string RefId { get; set; }
            public string Ean { get; set; }
            public float PackagedHeight { get; set; }
            public float PackagedLength { get; set; }
            public float PackagedWidth { get; set; }
            public int PackagedWeightKg { get; set; }
            public float? Height { get; set; }
            public float? Length { get; set; }
            public float? Width { get; set; }
            public float? WeightKg { get; set; }
            public float CubicWeight { get; set; }
            public bool IsKit { get; set; }
            public string CreationDate { get; set; }
            public float? RewardValue { get; set; }
            public string EstimatedDateArrival { get; set; }
            public string ManufacturerCode { get; set; }
            public int CommercialConditionId { get; set; }
            public string MeasurementUnit { get; set; }
            public float UnitMultiplier { get; set; }
            public string ModalType { get; set; }
            public bool KitItensSellApart { get; set; }
            public string[] Videos { get; set; }
        }

        public class Inventory
        {
            public bool unlimitedQuantity { get; set; }
            public string dateUtcOnBalanceSystem { get; set; }
            public int quantity { get; set; }
        }

        public class SkuToSubcollection
        {
            public int SkuId { get; set; }
        }

        public class SkuFile
        {
            public bool IsMain { get; set; }
            public string Label { get; set; }
            public string Name { get; set; }
            public string Text { get; set; }
            public string Url { get; set; }
        }
    }
}
