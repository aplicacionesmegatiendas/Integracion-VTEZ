using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegracionVTEX.Models
{
	#region PROMO REGULAR
	public class PromotionRegular
	{
        public string name { get; set; }
        public string description { get; set; }
        public string beginDateUtc { get; set; }
        public string endDateUtc { get; set; }
        public string lastModified { get; set; }
        public int daysAgoOfPurchases { get; set; }
        public bool isActive { get; set; }
        public bool isArchived { get; set; }
        public bool isFeatured { get; set; }
        public bool disableDeal { get; set; }
        public int offset { get; set; }
		public bool activateGiftsMultiplier { get; set; }
		public float newOffset { get; set; }
        public object[] maxPricesPerItems { get; set; }
		public bool cumulative { get; set; }
		public string discountType { get; set; }
        public float nominalShippingDiscountValue { get; set; }
        public float absoluteShippingDiscountValue { get; set; }
        public float nominalDiscountValue { get; set; }
        public string nominalDiscountType { get; set; }
        public float maximumUnitPriceDiscount { get; set; }
        public float percentualDiscountValue { get; set; }
        public float rebatePercentualDiscountValue { get; set; }
        public float percentualShippingDiscountValue { get; set; }
        public float percentualTax { get; set; }
        public float shippingPercentualTax { get; set; }
        public float percentualDiscountValueList1 { get; set; }
        public float percentualDiscountValueList2 { get; set; }
        public SkusGift skusGift { get; set; }
        public float nominalRewardValue { get; set; }
        public float percentualRewardValue { get; set; }
        public string orderStatusRewardValue { get; set; }
        public int maxNumberOfAffectedItems { get; set; }
        public string maxNumberOfAffectedItemsGroupKey { get; set; }
        public bool applyToAllShippings { get; set; }
        public float nominalTax { get; set; }
        public string origin { get; set; }
        public bool idSellerIsInclusive { get; set; }
        public string[] idsSalesChannel { get; set; }
        public bool areSalesChannelIdsExclusive { get; set; }
        public string[] marketingTags { get; set; }
        public bool marketingTagsAreNotInclusive { get; set; }
        public object[] paymentsMethods { get; set; }
        public object[] stores { get; set; }
        public object[] campaigns { get; set; }
        public bool storesAreInclusive { get; set; }
        public object[] categories { get; set; }
        public bool categoriesAreInclusive { get; set; }
        public object[] brands { get; set; }
        public bool brandsAreInclusive { get; set; }
        public List<Product> products { get; set; }
        public bool productsAreInclusive { get; set; }
        public object[] skus { get; set; }
        public bool skusAreInclusive { get; set; }
        public string[] collections1BuyTogether { get; set; }
        public object[] collections2BuyTogether { get; set; }
        public int minimumQuantityBuyTogether { get; set; }
        public int quantityToAffectBuyTogether { get; set; }
        public bool enableBuyTogetherPerSku { get; set; }
        public object[] listSku1BuyTogether { get; set; }
        public object[] listSku2BuyTogether { get; set; }
        public object[] coupon { get; set; }
        public float totalValueFloor { get; set; }
        public float totalValueCeling { get; set; }
        public string totalValueMode { get; set; }
        public object[] collections { get; set; }
        public bool collectionsIsInclusive { get; set; }
        public string[] restrictionsBins { get; set; }
        public object[] cardIssuers { get; set; }
        public float totalValuePurchase { get; set; }
        public string[] slasIds { get; set; }
        public bool isSlaSelected { get; set; }
        public bool isFirstBuy { get; set; }
        public bool firstBuyIsProfileOptimistic { get; set; }
        public bool compareListPriceAndPrice { get; set; }
        public bool isDifferentListPriceAndPrice { get; set; }
        public object[] zipCodeRanges { get; set; }
        public float itemMaxPrice { get; set; }
        public float itemMinPrice { get; set; }
        public bool isMinMaxInstallments { get; set; }
        public int minInstallment { get; set; }
        public int maxInstallment { get; set; }
        public object[] merchants { get; set; }
        public string[] clusterExpressions { get; set; }
        public string[] piiClusterExpressions { get; set; }
        public string clusterOperator { get; set; }
        public object[] paymentsRules { get; set; }
        public string[] giftListTypes { get; set; }
        public object[] productsSpecifications { get; set; }
        public object[] affiliates { get; set; }
        public int maxUsage { get; set; }
        public int maxUsagePerClient { get; set; }
        public bool shouldDistributeDiscountAmongMatchedItems { get; set; }
        public bool multipleUsePerClient { get; set; }
        public bool accumulateWithManualPrice { get; set; }
        public string type { get; set; }
        public bool useNewProgressiveAlgorithm { get; set; }
        public int[] percentualDiscountValueList { get; set; }
        
        public class Product
		{
			public string id { get; set; }
			public string name { get; set; }
		}

        public class SkusGift
        {
            public int quantitySelectable { get; set; }
            public object[] gifts { get; set; }
        }
	}

	public class PromotionIdCalculatorRegular
	{
		public string idCalculatorConfiguration { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public string beginDateUtc { get; set; }
		public string endDateUtc { get; set; }
		public string lastModified { get; set; }
		public int daysAgoOfPurchases { get; set; }
		public bool isActive { get; set; }
		public bool isArchived { get; set; }
		public bool isFeatured { get; set; }
		public bool disableDeal { get; set; }
		public int offset { get; set; }
		public bool activateGiftsMultiplier { get; set; }
		public float newOffset { get; set; }
		public object[] maxPricesPerItems { get; set; }
		public bool cumulative { get; set; }
		public string discountType { get; set; }
		public float nominalShippingDiscountValue { get; set; }
		public float absoluteShippingDiscountValue { get; set; }
		public float nominalDiscountValue { get; set; }
		public string nominalDiscountType { get; set; }
		public float maximumUnitPriceDiscount { get; set; }
		public float percentualDiscountValue { get; set; }
		public float rebatePercentualDiscountValue { get; set; }
		public float percentualShippingDiscountValue { get; set; }
		public float percentualTax { get; set; }
		public float shippingPercentualTax { get; set; }
		public float percentualDiscountValueList1 { get; set; }
		public float percentualDiscountValueList2 { get; set; }
		public SkusGift skusGift { get; set; }
		public float nominalRewardValue { get; set; }
		public float percentualRewardValue { get; set; }
		public string orderStatusRewardValue { get; set; }
		public int maxNumberOfAffectedItems { get; set; }
		public string maxNumberOfAffectedItemsGroupKey { get; set; }
		public bool applyToAllShippings { get; set; }
		public float nominalTax { get; set; }
		public string origin { get; set; }
		public bool idSellerIsInclusive { get; set; }
		public string[] idsSalesChannel { get; set; }
		public bool areSalesChannelIdsExclusive { get; set; }
		public string[] marketingTags { get; set; }
		public bool marketingTagsAreNotInclusive { get; set; }
		public object[] paymentsMethods { get; set; }
		public object[] stores { get; set; }
		public object[] campaigns { get; set; }
		public bool storesAreInclusive { get; set; }
		public object[] categories { get; set; }
		public bool categoriesAreInclusive { get; set; }
		public object[] brands { get; set; }
		public bool brandsAreInclusive { get; set; }
		public List<Product> products { get; set; }
		public bool productsAreInclusive { get; set; }
		public object[] skus { get; set; }
		public bool skusAreInclusive { get; set; }
		public string[] collections1BuyTogether { get; set; }
		public object[] collections2BuyTogether { get; set; }
		public int minimumQuantityBuyTogether { get; set; }
		public int quantityToAffectBuyTogether { get; set; }
		public bool enableBuyTogetherPerSku { get; set; }
		public object[] listSku1BuyTogether { get; set; }
		public object[] listSku2BuyTogether { get; set; }
		public object[] coupon { get; set; }
		public float totalValueFloor { get; set; }
		public float totalValueCeling { get; set; }
		public string totalValueMode { get; set; }
		public object[] collections { get; set; }
		public bool collectionsIsInclusive { get; set; }
		public string[] restrictionsBins { get; set; }
		public object[] cardIssuers { get; set; }
		public float totalValuePurchase { get; set; }
		public string[] slasIds { get; set; }
		public bool isSlaSelected { get; set; }
		public bool isFirstBuy { get; set; }
		public bool firstBuyIsProfileOptimistic { get; set; }
		public bool compareListPriceAndPrice { get; set; }
		public bool isDifferentListPriceAndPrice { get; set; }
		public object[] zipCodeRanges { get; set; }
		public float itemMaxPrice { get; set; }
		public float itemMinPrice { get; set; }
		public bool isMinMaxInstallments { get; set; }
		public int minInstallment { get; set; }
		public int maxInstallment { get; set; }
		public object[] merchants { get; set; }
		public string[] clusterExpressions { get; set; }
		public string[] piiClusterExpressions { get; set; }
		public string clusterOperator { get; set; }
		public object[] paymentsRules { get; set; }
		public string[] giftListTypes { get; set; }
		public object[] productsSpecifications { get; set; }
		public object[] affiliates { get; set; }
		public int maxUsage { get; set; }
		public int maxUsagePerClient { get; set; }
		public bool shouldDistributeDiscountAmongMatchedItems { get; set; }
		public bool multipleUsePerClient { get; set; }
		public bool accumulateWithManualPrice { get; set; }
		public string type { get; set; }
		public bool useNewProgressiveAlgorithm { get; set; }
		public int[] percentualDiscountValueList { get; set; }

		public class Product
		{
			public string id { get; set; }
			public string name { get; set; }
		}

		public class SkusGift
		{
			public int quantitySelectable { get; set; }
			public object[] gifts { get; set; }
		}
	}
	#endregion

	#region PROMO BUYANDWIN
	public class PromotionBuyAndWin
	{
		public string name { get; set; }
		public string description { get; set; }
		public string beginDateUtc { get; set; }
		public string endDateUtc { get; set; }
		public string lastModified { get; set; }
		public int daysAgoOfPurchases { get; set; }
		public bool isActive { get; set; }
		public bool isArchived { get; set; }
		public bool isFeatured { get; set; }
		public bool disableDeal { get; set; }
		public int offset { get; set; }
		public bool activateGiftsMultiplier { get; set; }
		public float newOffset { get; set; }
		public object[] maxPricesPerItems { get; set; }
		public bool cumulative { get; set; }
		public string discountType { get; set; }
		public float nominalShippingDiscountValue { get; set; }
		public float absoluteShippingDiscountValue { get; set; }
		public float nominalDiscountValue { get; set; }
		public string nominalDiscountType { get; set; }
		public float maximumUnitPriceDiscount { get; set; }
		public float percentualDiscountValue { get; set; }
		public float rebatePercentualDiscountValue { get; set; }
		public float percentualShippingDiscountValue { get; set; }
		public float percentualTax { get; set; }
		public float shippingPercentualTax { get; set; }
		public float percentualDiscountValueList1 { get; set; }
		public float percentualDiscountValueList2 { get; set; }
		public SkusGift skusGift { get; set; }
		public float nominalRewardValue { get; set; }
		public float percentualRewardValue { get; set; }
		public string orderStatusRewardValue { get; set; }
		public int maxNumberOfAffectedItems { get; set; }
		public string maxNumberOfAffectedItemsGroupKey { get; set; }
		public bool applyToAllShippings { get; set; }
		public float nominalTax { get; set; }
		public string origin { get; set; }
		public bool idSellerIsInclusive { get; set; }
		public string[] idsSalesChannel { get; set; }
		public bool areSalesChannelIdsExclusive { get; set; }
		public string[] marketingTags { get; set; }
		public bool marketingTagsAreNotInclusive { get; set; }
		public object[] paymentsMethods { get; set; }
		public object[] stores { get; set; }
		public object[] campaigns { get; set; }
		public bool storesAreInclusive { get; set; }
		public object[] categories { get; set; }
		public bool categoriesAreInclusive { get; set; }
		public object[] brands { get; set; }
		public bool brandsAreInclusive { get; set; }
		public object[] products { get; set; }
		public bool productsAreInclusive { get; set; }
		public object[] skus { get; set; }
		public bool skusAreInclusive { get; set; }
		public string[] collections1BuyTogether { get; set; }
		public object[] collections2BuyTogether { get; set; }
		public int minimumQuantityBuyTogether { get; set; }
		public int quantityToAffectBuyTogether { get; set; }
		public bool enableBuyTogetherPerSku { get; set; }
		public List<Product> listSku1BuyTogether { get; set; }
		public object[] listSku2BuyTogether { get; set; }
		public object[] coupon { get; set; }
		public float totalValueFloor { get; set; }
		public float totalValueCeling { get; set; }
		public string totalValueMode { get; set; }
		public object[] collections { get; set; }
		public bool collectionsIsInclusive { get; set; }
		public string[] restrictionsBins { get; set; }
		public object[] cardIssuers { get; set; }
		public float totalValuePurchase { get; set; }
		public string[] slasIds { get; set; }
		public bool isSlaSelected { get; set; }
		public bool isFirstBuy { get; set; }
		public bool firstBuyIsProfileOptimistic { get; set; }
		public bool compareListPriceAndPrice { get; set; }
		public bool isDifferentListPriceAndPrice { get; set; }
		public object[] zipCodeRanges { get; set; }
		public float itemMaxPrice { get; set; }
		public float itemMinPrice { get; set; }
		public bool isMinMaxInstallments { get; set; }
		public int minInstallment { get; set; }
		public int maxInstallment { get; set; }
		public object[] merchants { get; set; }
		public string[] clusterExpressions { get; set; }
		public string[] piiClusterExpressions { get; set; }
		public string clusterOperator { get; set; }
		public object[] paymentsRules { get; set; }
		public string[] giftListTypes { get; set; }
		public object[] productsSpecifications { get; set; }
		public object[] affiliates { get; set; }
		public int maxUsage { get; set; }
		public int maxUsagePerClient { get; set; }
		public bool shouldDistributeDiscountAmongMatchedItems { get; set; }
		public bool multipleUsePerClient { get; set; }
		public bool accumulateWithManualPrice { get; set; }
		public string type { get; set; }
		public bool useNewProgressiveAlgorithm { get; set; }
		public int[] percentualDiscountValueList { get; set; }

		public class Product
		{
			public string id { get; set; }
			public string name { get; set; }
		}

		public class Gift
		{
			public string id { get; set; }
			public string name { get; set; }
			public int quantity { get; set; }
		}

		public class SkusGift
		{
			public int quantitySelectable { get; set; }
			public List<Gift> gifts { get; set; }
		}
	}

	public class PromotionIdCalculatorBuyAndWin
	{
		public string idCalculatorConfiguration { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public string beginDateUtc { get; set; }
		public string endDateUtc { get; set; }
		public string lastModified { get; set; }
		public int daysAgoOfPurchases { get; set; }
		public bool isActive { get; set; }
		public bool isArchived { get; set; }
		public bool isFeatured { get; set; }
		public bool disableDeal { get; set; }
		public int offset { get; set; }
		public bool activateGiftsMultiplier { get; set; }
		public float newOffset { get; set; }
		public object[] maxPricesPerItems { get; set; }
		public bool cumulative { get; set; }
		public string discountType { get; set; }
		public float nominalShippingDiscountValue { get; set; }
		public float absoluteShippingDiscountValue { get; set; }
		public float nominalDiscountValue { get; set; }
		public string nominalDiscountType { get; set; }
		public float maximumUnitPriceDiscount { get; set; }
		public float percentualDiscountValue { get; set; }
		public float rebatePercentualDiscountValue { get; set; }
		public float percentualShippingDiscountValue { get; set; }
		public float percentualTax { get; set; }
		public float shippingPercentualTax { get; set; }
		public float percentualDiscountValueList1 { get; set; }
		public float percentualDiscountValueList2 { get; set; }
		public SkusGift skusGift { get; set; }
		public float nominalRewardValue { get; set; }
		public float percentualRewardValue { get; set; }
		public string orderStatusRewardValue { get; set; }
		public int maxNumberOfAffectedItems { get; set; }
		public string maxNumberOfAffectedItemsGroupKey { get; set; }
		public bool applyToAllShippings { get; set; }
		public float nominalTax { get; set; }
		public string origin { get; set; }
		public bool idSellerIsInclusive { get; set; }
		public string[] idsSalesChannel { get; set; }
		public bool areSalesChannelIdsExclusive { get; set; }
		public string[] marketingTags { get; set; }
		public bool marketingTagsAreNotInclusive { get; set; }
		public object[] paymentsMethods { get; set; }
		public object[] stores { get; set; }
		public object[] campaigns { get; set; }
		public bool storesAreInclusive { get; set; }
		public object[] categories { get; set; }
		public bool categoriesAreInclusive { get; set; }
		public object[] brands { get; set; }
		public bool brandsAreInclusive { get; set; }
		public object[] products { get; set; }
		public bool productsAreInclusive { get; set; }
		public object[] skus { get; set; }
		public bool skusAreInclusive { get; set; }
		public string[] collections1BuyTogether { get; set; }
		public object[] collections2BuyTogether { get; set; }
		public int minimumQuantityBuyTogether { get; set; }
		public int quantityToAffectBuyTogether { get; set; }
		public bool enableBuyTogetherPerSku { get; set; }
		public List<Product> listSku1BuyTogether { get; set; }
		public object[] listSku2BuyTogether { get; set; }
		public object[] coupon { get; set; }
		public float totalValueFloor { get; set; }
		public float totalValueCeling { get; set; }
		public string totalValueMode { get; set; }
		public object[] collections { get; set; }
		public bool collectionsIsInclusive { get; set; }
		public string[] restrictionsBins { get; set; }
		public object[] cardIssuers { get; set; }
		public float totalValuePurchase { get; set; }
		public string[] slasIds { get; set; }
		public bool isSlaSelected { get; set; }
		public bool isFirstBuy { get; set; }
		public bool firstBuyIsProfileOptimistic { get; set; }
		public bool compareListPriceAndPrice { get; set; }
		public bool isDifferentListPriceAndPrice { get; set; }
		public object[] zipCodeRanges { get; set; }
		public float itemMaxPrice { get; set; }
		public float itemMinPrice { get; set; }
		public bool isMinMaxInstallments { get; set; }
		public int minInstallment { get; set; }
		public int maxInstallment { get; set; }
		public object[] merchants { get; set; }
		public string[] clusterExpressions { get; set; }
		public string[] piiClusterExpressions { get; set; }
		public string clusterOperator { get; set; }
		public object[] paymentsRules { get; set; }
		public string[] giftListTypes { get; set; }
		public object[] productsSpecifications { get; set; }
		public object[] affiliates { get; set; }
		public int maxUsage { get; set; }
		public int maxUsagePerClient { get; set; }
		public bool shouldDistributeDiscountAmongMatchedItems { get; set; }
		public bool multipleUsePerClient { get; set; }
		public bool accumulateWithManualPrice { get; set; }
		public string type { get; set; }
		public bool useNewProgressiveAlgorithm { get; set; }
		public int[] percentualDiscountValueList { get; set; }

		public class Product
		{
			public string id { get; set; }
			public string name { get; set; }
		}

		public class Gift
		{
			public string id { get; set; }
			public string name { get; set; }
			public int quantity { get; set; }
		}

		public class SkusGift
		{
			public int quantitySelectable { get; set; }
			public List<Gift> gifts { get; set; }
		}
	}
	#endregion
}
