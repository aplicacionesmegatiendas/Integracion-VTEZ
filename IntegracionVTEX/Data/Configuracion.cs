using IntegracionVTEX.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegracionVTEX.Data
{
	public class Configuracion
	{
		public static string CentroOperacion { get; set; }
		public static string ListaPrecios { get; set; }
		public static string Portafolio { get; set; }

		public static string UrlCreateProduct { get; set; }
		public static string UrlGetProductRefId { get; set; }
		public static string UrlCreateSku { get; set; }
		public static string UrlCreateEanSku { get; set; }
		public static string UrlUpdateInventory { get; set; }
		public static string UrlGetSkuRefId { get; set; }
		public static string UrlGetListOrders { get; set; }
		public static string UrlGetOrder { get; set; }
		public static string UrlCreateUpdateBasePrice { get; set; }
		public static string UrlAddSkuSubCollection { get; set; }
		public static string UrlUpdateProductSpecification { get; set; }
		public static string UrlDeletePrice { get; set; }
        public static string UrlGetProductsFromCollection { get; set; }
        public static string UrlDeleteSkuFromSubcollection { get; set; }

		public static string UrlSearchPromotionByName { get; set; }
		public static string UrlCreateOrUpdatePromotion { get; set; }
		public static string UrlStartHandling { get; set; }

		public static string AppKey { get; set; }
		public static string AppToken { get; set; }
		public static string AppKeyMaster { get; set; }
		public static string AppTokenMaster { get; set; }

		public bool ObtenerConfiguracion(string centro_operacion)
		{
			string SQL = @"select 
	                         f09_co,
	                         isnull(f09_create_product,'') f09_create_product,
	                         isnull(f09_get_product_refid,'') f09_get_product_refid,
	                         isnull(f09_create_sku,'') f09_create_sku,
	                         isnull(f09_create_ean_sku,'') f09_create_ean_sku,
	                         isnull(f09_update_inventory,'') f09_update_inventory,
	                         isnull(f09_get_sku_refid,'') f09_get_sku_refid,
	                         isnull(f09_get_list_order,'') f09_get_list_order,
	                         isnull(f09_get_order,'') f09_get_order,
	                         isnull(f09_create_update_baseprice,'') f09_create_update_baseprice,
	                         isnull(f09_add_sku_subcollection,'') f09_add_sku_subcollection,
							 isnull(f09_update_product_specification,'') f09_update_product_specification,
							 isnull(f09_delete_price,'') f09_delete_price,
							 isnull(f09_get_products_from_collection,'') f09_get_products_from_collection,
							 isnull(f09_delete_sku_subcollection,'') f09_delete_sku_subcollection,
							 isnull(f09_search_promotion_by_name,'') f09_search_promotion_by_name,
							 isnull(f09_create_update_promotion,'') f09_create_update_promotion,
							 isnull(f09_start_handling,'') f09_start_handling,
	                         isnull(f09_app_key,'') f09_app_key,
	                         isnull(f09_app_token,'') f09_app_token,
							 f09_app_key_master,
	                         f09_app_token_master
                        from 
	                        t09_configuracion
                        where
	                        f09_co=@co";
			bool res = false;
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@co", centro_operacion);
				SqlDataReader dataReader = cmd.ExecuteReader();
				if (dataReader.HasRows ) 
				{
					dataReader.Read();
					
					UrlCreateProduct = dataReader.GetString(1);
					UrlGetProductRefId = dataReader.GetString(2);
					UrlCreateSku = dataReader.GetString(3);
					UrlCreateEanSku = dataReader.GetString(4);
					UrlUpdateInventory = dataReader.GetString(5);
					UrlGetSkuRefId = dataReader.GetString(6);
					UrlGetListOrders = dataReader.GetString(7);
					UrlGetOrder = dataReader.GetString(8);
					UrlCreateUpdateBasePrice = dataReader.GetString(9);
					UrlAddSkuSubCollection = dataReader.GetString(10);
					UrlUpdateProductSpecification = dataReader.GetString(11);
					UrlDeletePrice = dataReader.GetString(12);
					UrlGetProductsFromCollection= dataReader.GetString(13);
					UrlDeleteSkuFromSubcollection= dataReader.GetString(14);
					UrlSearchPromotionByName = dataReader.GetString(15);
					UrlCreateOrUpdatePromotion = dataReader.GetString(16);
					UrlStartHandling= dataReader.GetString(17);
					AppKey = dataReader.GetString(18);
					AppToken=dataReader.GetString(19);
					AppKeyMaster = dataReader.GetString(20);
					AppTokenMaster = dataReader.GetString(21);

					res = true;
				}
				dataReader.Close();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception($"Error al obtener configuración: {ex.Message}");
			}
			return res;
		}
	}
}
