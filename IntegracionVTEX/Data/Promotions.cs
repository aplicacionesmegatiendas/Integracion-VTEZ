using IntegracionVTEX.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using static IntegracionVTEX.Models.Price;
using static System.Collections.Specialized.BitVector32;

namespace IntegracionVTEX.Data
{
	public class Promotions
	{
		Configuration config = null;
		AppSettingsSection section = null;

		int loop_size = 0;
		int pause = 0;
		//public static List<string> PromoConError = null;
		public Promotions()
		{
			config = ConfigurationManager.OpenExeConfiguration(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
			section = config.AppSettings;

			loop_size = Convert.ToInt32(section.Settings["loop_size"].Value);
			pause = Convert.ToInt32(section.Settings["pause"].Value);
		}

		#region FUNCIONES VTEX
		private async Task<string> VTEXSearchPromotionByName(string id_promo, bool master)
		{
			string res = null;
			try
			{

				string uri = Configuracion.UrlSearchPromotionByName;//"https://whitelabelspruebas--megatiendas.vtexcommercestable.com.br/api/rnb/pvt/benefits/calculatorconfiguration/search?byName=";
				HttpClient client = new HttpClient();
				string app_key = "";
				string app_token = "";
				if (master == true)
				{
					app_key = Configuracion.AppKeyMaster;
					app_token = Configuracion.AppTokenMaster;
				}
				else
				{
					app_key = Configuracion.AppKey;
					app_token = Configuracion.AppToken;
					//app_key = "vtexappkey-megatiendas-MKNBXJ";//Configuracion.AppKeyMaster;
					//app_token = "XMZJPRXNLALRINBLJILOUMOQQSYDHTPYOUCMJRQNFYZLRIQXPBBFDJFAMUMMFALAHPXWQBHIBZAVQZELAOVKCGNUMZRPDADCXYWJTFNZHHZADSWQTHRLNBLALKIXUYIM"; //Configuracion.AppTokenMaster;
				}

				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}{id_promo}"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", app_key },
					{ "X-VTEX-API-AppToken", app_token },
				},
					Method = HttpMethod.Get,
				};
				System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
				using (var response = await client.SendAsync(request))
				{
					response.EnsureSuccessStatusCode();
					res = await response.Content.ReadAsStringAsync();
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al obtener promoción por nombre: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al obtener promoción por nombre: " + ex.Message);
			}
			return res;
		}

		public async Task<string> VTEXGetAllPromotions(bool master, string wl)
		{
			string res = null;
			try
			{
				string uri = $"https://megatiendaswl{wl}.vtexcommercestable.com.br/api/rnb/pvt/benefits/calculatorconfiguration";
				HttpClient client = new HttpClient();
				string app_key = "";
				string app_token = "";
				if (master == true)
				{
					app_key = Configuracion.AppKeyMaster;
					app_token = Configuracion.AppTokenMaster;
				}
				else
				{
					app_key = Configuracion.AppKey;
					app_token = Configuracion.AppToken;
					//app_key = "vtexappkey-megatiendas-MKNBXJ";//Configuracion.AppKeyMaster;
					//app_token = "XMZJPRXNLALRINBLJILOUMOQQSYDHTPYOUCMJRQNFYZLRIQXPBBFDJFAMUMMFALAHPXWQBHIBZAVQZELAOVKCGNUMZRPDADCXYWJTFNZHHZADSWQTHRLNBLALKIXUYIM"; //Configuracion.AppTokenMaster;
				}

				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", app_key },
					{ "X-VTEX-API-AppToken", app_token },
				},
					Method = HttpMethod.Get,
				};
				System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
				using (var response = await client.SendAsync(request))
				{
					response.EnsureSuccessStatusCode();
					res = await response.Content.ReadAsStringAsync();
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al obtener promociones: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al obtener promociones: " + ex.Message);
			}
			return res;
		}

		public async Task<string> VTEXArchivePromotion(string id, string wl)
		{
			string res = null;
			try
			{
				string uri = $"https://megatiendaswl{wl}.vtexcommercestable.com.br/api/rnb/pvt/archive/calculatorConfiguration/{id}";//Configuracion.UrlCreateOrUpdatePromotion;
				HttpClient client = new HttpClient();
				string app_key = "";
				string app_token = "";

				app_key = Configuracion.AppKey;//"vtexappkey-megatiendas-MKNBXJ";
				app_token = Configuracion.AppToken;//"XMZJPRXNLALRINBLJILOUMOQQSYDHTPYOUCMJRQNFYZLRIQXPBBFDJFAMUMMFALAHPXWQBHIBZAVQZELAOVKCGNUMZRPDADCXYWJTFNZHHZADSWQTHRLNBLALKIXUYIM"

				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", app_key } ,
					{ "X-VTEX-API-AppToken", app_token},
				},
					Method = HttpMethod.Post,
				};
				using (var response = await client.SendAsync(request))
				{
					string rta = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						res = rta;
					}
					else
					{
						res = $"Error: {rta}";
					}
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				throw new Exception("Error al archivar promoción: " + errores.Trim().Trim(','));
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al archivar promoción: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al archivar promoción: " + ex.Message);
			}
			return res;
		}
		//PARA CREAR PROMOCIONES REGULAR//
		private async Task<string> VTEXCreateUpdatePromotion(Models.PromotionRegular promotion, bool master)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlCreateOrUpdatePromotion;// "https://whitelabelspruebas--megatiendas.vtexcommercestable.com.br/api/rnb/pvt/calculatorconfiguration";
				string json = JsonConvert.SerializeObject(promotion);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				string app_key = "";
				string app_token = "";
				if (master == true)
				{
					app_key = Configuracion.AppKeyMaster;
					app_token = Configuracion.AppTokenMaster;
				}
				else
				{
					app_key = Configuracion.AppKey;//"vtexappkey-megatiendas-MKNBXJ";
					app_token = Configuracion.AppToken;//"XMZJPRXNLALRINBLJILOUMOQQSYDHTPYOUCMJRQNFYZLRIQXPBBFDJFAMUMMFALAHPXWQBHIBZAVQZELAOVKCGNUMZRPDADCXYWJTFNZHHZADSWQTHRLNBLALKIXUYIM";
				}
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", app_key } ,
					{ "X-VTEX-API-AppToken", app_token},
				},
					Method = HttpMethod.Post,
					Content = data,
				};
				using (var response = await client.SendAsync(request))
				{
					string rta = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						res = rta;
					}
					else
					{
						res = $"Error: {rta}";
					}
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				throw new Exception("Error al crear promoción: " + errores.Trim().Trim(','));
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al crear promoción: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al crear promoción: " + ex.Message);
			}
			return res;
		}

		private async Task<string> VTEXCreateUpdatePromotionIdcalculator(Models.PromotionIdCalculatorRegular promotion, bool master)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlCreateOrUpdatePromotion;
				string json = JsonConvert.SerializeObject(promotion);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				string app_key = "";
				string app_token = "";
				if (master == true)
				{
					app_key = Configuracion.AppKeyMaster;
					app_token = Configuracion.AppTokenMaster;
				}
				else
				{
					app_key = Configuracion.AppKey;
					app_token = Configuracion.AppToken;
				}
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", app_key } ,
					{ "X-VTEX-API-AppToken", app_token},
				},
					Method = HttpMethod.Post,
					Content = data,
				};
				using (var response = await client.SendAsync(request))
				{
					string rta = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						res = rta;
					}
					else
					{
						res = $"Error: {rta}";
					}
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				throw new Exception("Error al crear promoción: " + errores.Trim().Trim(',') + ": " + res);
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al crear promoción: " + ex.Message + ": " + res);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al crear promoción: " + ex.Message + ": " + res);
			}
			return res;
		}
		//--------------------------------------------------------------------------------------------------------//

		//PARA CREAR PROMOCIONES FORTHEPRICEOF//
		private async Task<string> VTEXCreateUpdatePromotion(Models.PromotionForThePriceOf promotion, bool master)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlCreateOrUpdatePromotion;//"https://whitelabelspruebas--megatiendas.vtexcommercestable.com.br/api/rnb/pvt/calculatorconfiguration";
				string json = JsonConvert.SerializeObject(promotion);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				string app_key = "";
				string app_token = "";
				if (master == true)
				{
					app_key = Configuracion.AppKeyMaster;
					app_token = Configuracion.AppTokenMaster;
				}
				else
				{
					app_key = Configuracion.AppKey;//"vtexappkey-megatiendas-MKNBXJ";
					app_token = Configuracion.AppToken;//"XMZJPRXNLALRINBLJILOUMOQQSYDHTPYOUCMJRQNFYZLRIQXPBBFDJFAMUMMFALAHPXWQBHIBZAVQZELAOVKCGNUMZRPDADCXYWJTFNZHHZADSWQTHRLNBLALKIXUYIM";
				}
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", app_key } ,
					{ "X-VTEX-API-AppToken", app_token},
				},
					Method = HttpMethod.Post,
					Content = data,
				};
				using (var response = await client.SendAsync(request))
				{
					string rta = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						res = rta;
					}
					else
					{
						res = $"Error: {rta}";
					}
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				throw new Exception("Error al crear promoción: " + errores.Trim().Trim(','));
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al crear promoción: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al crear promoción: " + ex.Message);
			}
			return res;
		}

		private async Task<string> VTEXCreateUpdatePromotionIdcalculator(Models.PromotionIdCalculatorForThePriceOf promotion, bool master)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlCreateOrUpdatePromotion;
				string json = JsonConvert.SerializeObject(promotion);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				string app_key = "";
				string app_token = "";
				if (master == true)
				{
					app_key = Configuracion.AppKeyMaster;
					app_token = Configuracion.AppTokenMaster;
				}
				else
				{
					app_key = Configuracion.AppKey;
					app_token = Configuracion.AppToken;
				}
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", app_key } ,
					{ "X-VTEX-API-AppToken", app_token},
				},
					Method = HttpMethod.Post,
					Content = data,
				};
				using (var response = await client.SendAsync(request))
				{
					string rta = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						res = rta;
					}
					else
					{
						res = $"Error: {rta}";
					}
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				throw new Exception("Error al crear promoción: " + errores.Trim().Trim(',') + ": " + res);
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al crear promoción: " + ex.Message + ": " + res);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al crear promoción: " + ex.Message + ": " + res);
			}
			return res;
		}
		//------------------------------------------------------------------------------------------------------------//

		private async Task<string> VTEXGetProductByRefId(string ref_id)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlGetProductRefId;//"https://whitelabelspruebas--megatiendas.vtexcommercestable.com.br/api/catalog_system/pvt/products/productgetbyrefid/"

				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}{ref_id}"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKeyMaster } ,//"vtexappkey-megatiendas-MKNBXJ"
					{ "X-VTEX-API-AppToken", Configuracion.AppTokenMaster},//"XMZJPRXNLALRINBLJILOUMOQQSYDHTPYOUCMJRQNFYZLRIQXPBBFDJFAMUMMFALAHPXWQBHIBZAVQZELAOVKCGNUMZRPDADCXYWJTFNZHHZADSWQTHRLNBLALKIXUYIM"
				},
					Method = HttpMethod.Get,
				};

				using (var response = await client.SendAsync(request))
				{
					response.EnsureSuccessStatusCode();//inicia una excepción si es false.
					res = await response.Content.ReadAsStringAsync();
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al obtener producto: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al obtener producto: " + ex.Message);
			}
			return res;
		}

		#endregion
		#region FUNCIONES DE CONSULTA, INSERCION Y ACTUALIZACION
		public DataTable GetPromotionRegularListCentroOperacion()
		{
			DataTable res = null;
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = File.ReadAllText("Data\\PromotionCentroOperacionQuery.txt");

				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["unoee"].ConnectionString);
				conn.Open();
				SqlCommand cmd = new SqlCommand(SQL, conn);
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 600;
				cmd.Parameters.AddWithValue("@id_cia", id_cia);
				cmd.Parameters.AddWithValue("@id_portafolio", Configuracion.Portafolio);
				cmd.Parameters.AddWithValue("@id_co", Configuracion.CentroOperacion);

				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				if (reader.HasRows)
				{
					res = new DataTable();
					res.Load(reader);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al consultar productos: " + ex.Message);
			}
			return res;
		}

		public DataTable GetPromotionRegularList()
		{
			DataTable res = null;
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = File.ReadAllText("Data\\PromotionQuery.txt");

				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["unoee"].ConnectionString);
				conn.Open();
				SqlCommand cmd = new SqlCommand(SQL, conn);
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 600;
				cmd.Parameters.AddWithValue("@id_cia", id_cia);
				cmd.Parameters.AddWithValue("@id_portafolio", Configuracion.Portafolio);

				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				if (reader.HasRows)
				{
					res = new DataTable();
					res.Load(reader);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al consultar productos: " + ex.Message);
			}
			return res;
		}

		public DataTable GetPromotionForThePriceOfListCentroOperacion()
		{
			DataTable res = null;
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = File.ReadAllText("Data\\PromotionForThePriceOfCentroOperacionQuery.txt");

				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["unoee"].ConnectionString);
				conn.Open();
				SqlCommand cmd = new SqlCommand(SQL, conn);
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 600;
				cmd.Parameters.AddWithValue("@id_cia", id_cia);
				cmd.Parameters.AddWithValue("@id_portafolio", Configuracion.Portafolio);
				cmd.Parameters.AddWithValue("@id_co", Configuracion.CentroOperacion);

				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				if (reader.HasRows)
				{
					res = new DataTable();
					res.Load(reader);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al consultar productos: " + ex.Message);
			}
			return res;
		}

		public DataTable GetPromotionForThePriceOfList()
		{
			DataTable res = null;
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = File.ReadAllText("Data\\PromotionForThePriceOfQuery.txt");

				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["unoee"].ConnectionString);
				conn.Open();
				SqlCommand cmd = new SqlCommand(SQL, conn);
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 600;
				cmd.Parameters.AddWithValue("@id_cia", id_cia);
				cmd.Parameters.AddWithValue("@id_portafolio", Configuracion.Portafolio);

				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				if (reader.HasRows)
				{
					res = new DataTable();
					res.Load(reader);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al consultar productos: " + ex.Message);
			}
			return res;
		}

		public int[] CreateUpdateForThePriceOfPromotion(string name, DataRow[] rows, bool master)
		{
			string id_calculator_configuration = "";
			int[] total = new int[2];
			try
			{
				Models.PromotionForThePriceOf promotion_for_the_price_of = null;
				Models.PromotionIdCalculatorForThePriceOf promotion_id_calculator_for_the_price_of = null;

				Task<string> task_get_promotion = Task.Run(() => VTEXSearchPromotionByName(name, master));
				task_get_promotion.Wait();
				string res = Convert.ToString(task_get_promotion.Result);
				task_get_promotion.Dispose();
				if (res.TrimStart('[').TrimEnd(']') != "")
				{
					dynamic results_get_promotion = JsonConvert.DeserializeObject<dynamic>(res);
					id_calculator_configuration = Convert.ToString(results_get_promotion[0].idCalculatorConfiguration);
					Console.WriteLine(id_calculator_configuration);

					TimeZoneInfo britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

					string beginDateUtc = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["beginDateUtc"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
					string endDateUtc = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["endDateUtc"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
					string lastModified = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["lastModified"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");

					promotion_id_calculator_for_the_price_of = new Models.PromotionIdCalculatorForThePriceOf();
					promotion_id_calculator_for_the_price_of.idCalculatorConfiguration = id_calculator_configuration;
					promotion_id_calculator_for_the_price_of.name = name;
					promotion_id_calculator_for_the_price_of.description = name;
					promotion_id_calculator_for_the_price_of.beginDateUtc = beginDateUtc;
					promotion_id_calculator_for_the_price_of.endDateUtc = endDateUtc;
					promotion_id_calculator_for_the_price_of.lastModified = lastModified;
					promotion_id_calculator_for_the_price_of.daysAgoOfPurchases = 0;
					promotion_id_calculator_for_the_price_of.isActive = true;
					promotion_id_calculator_for_the_price_of.isArchived = false;
					promotion_id_calculator_for_the_price_of.isFeatured = true;//aqui era true
					promotion_id_calculator_for_the_price_of.disableDeal = false;
					promotion_id_calculator_for_the_price_of.offset = -5;
					promotion_id_calculator_for_the_price_of.activateGiftsMultiplier = false;
					promotion_id_calculator_for_the_price_of.newOffset = -5.0f;
					promotion_id_calculator_for_the_price_of.maxPricesPerItems = new string[] { };
					promotion_id_calculator_for_the_price_of.cumulative = false;

					promotion_id_calculator_for_the_price_of.discountType = "forThePriceOf";
					promotion_id_calculator_for_the_price_of.nominalDiscountValue = 0.0f;

					float percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
					promotion_id_calculator_for_the_price_of.percentualDiscountValue = percentualDiscountValue;

					promotion_id_calculator_for_the_price_of.discountExpression = "";

					promotion_id_calculator_for_the_price_of.nominalShippingDiscountValue = 0.0f;
					promotion_id_calculator_for_the_price_of.absoluteShippingDiscountValue = 0.0f;
					promotion_id_calculator_for_the_price_of.nominalDiscountType = "cart";
					promotion_id_calculator_for_the_price_of.maximumUnitPriceDiscount = 0.0f;
					promotion_id_calculator_for_the_price_of.rebatePercentualDiscountValue = 0.0f;
					promotion_id_calculator_for_the_price_of.percentualShippingDiscountValue = 0.0f;
					promotion_id_calculator_for_the_price_of.percentualTax = 0.0f;
					promotion_id_calculator_for_the_price_of.shippingPercentualTax = 0.0f;
					promotion_id_calculator_for_the_price_of.percentualDiscountValueList1 = 0.0f;
					promotion_id_calculator_for_the_price_of.percentualDiscountValueList2 = 0.0f;
					promotion_id_calculator_for_the_price_of.nominalRewardValue = 0.0f;
					promotion_id_calculator_for_the_price_of.percentualRewardValue = 0.0f;
					promotion_id_calculator_for_the_price_of.orderStatusRewardValue = "invoiced";
					promotion_id_calculator_for_the_price_of.maxNumberOfAffectedItems = 0;
					promotion_id_calculator_for_the_price_of.maxNumberOfAffectedItemsGroupKey = "perCart";
					promotion_id_calculator_for_the_price_of.applyToAllShippings = false;
					promotion_id_calculator_for_the_price_of.nominalTax = 0.0f;
					if (master == true)
						promotion_id_calculator_for_the_price_of.origin = "Marketplace";
					else
						promotion_id_calculator_for_the_price_of.origin = "Fulfillment";
					promotion_id_calculator_for_the_price_of.idSellerIsInclusive = true;
					promotion_id_calculator_for_the_price_of.idsSalesChannel = new string[1] { "1" };//new string[] { };
					promotion_id_calculator_for_the_price_of.areSalesChannelIdsExclusive = false;
					promotion_id_calculator_for_the_price_of.marketingTags = new string[] { };
					promotion_id_calculator_for_the_price_of.marketingTagsAreNotInclusive = false;
					promotion_id_calculator_for_the_price_of.paymentsMethods = new string[] { };
					promotion_id_calculator_for_the_price_of.stores = new string[] { };
					promotion_id_calculator_for_the_price_of.campaigns = new string[] { };
					promotion_id_calculator_for_the_price_of.storesAreInclusive = true;//nuevo
					promotion_id_calculator_for_the_price_of.categories = new string[] { };
					promotion_id_calculator_for_the_price_of.categoriesAreInclusive = true;
					promotion_id_calculator_for_the_price_of.brands = new string[] { };
					promotion_id_calculator_for_the_price_of.brandsAreInclusive = true;

					List<PromotionIdCalculatorForThePriceOf.Product> products = new List<PromotionIdCalculatorForThePriceOf.Product>();
					int minimum_quantity_buy_together = Convert.ToInt32(rows[0]["compra"]);
					int quantity_to_affect_buy_together = Convert.ToInt32(rows[0]["lleva"]);

					foreach (DataRow row in rows)
					{
						string product_id = "";
						try
						{
							PromotionIdCalculatorForThePriceOf.Product product = new PromotionIdCalculatorForThePriceOf.Product();
							product_id = row["products.id"].ToString().Trim();

							Task<string> task_get = Task.Run(() => VTEXGetProductByRefId(product_id));
							task_get.Wait();
							dynamic results_get = JsonConvert.DeserializeObject<dynamic>(task_get.Result);
							task_get.Dispose();
							if (results_get != null)
							{
								product.id = results_get.Id;
								product.name = results_get.Name;
								products.Add(product);
							}
						}
						catch (Exception)
						{
							continue;
						}
					}

					promotion_id_calculator_for_the_price_of.skusGift = new PromotionIdCalculatorForThePriceOf.SkusGift() { quantitySelectable = 0, gifts = new object[] { } };//quantitySelectable=cantida de items de regalo

					promotion_id_calculator_for_the_price_of.products = new object[] { };
					promotion_id_calculator_for_the_price_of.productsAreInclusive = true;
					promotion_id_calculator_for_the_price_of.skus = new string[] { };
					promotion_id_calculator_for_the_price_of.skusAreInclusive = true;
					promotion_id_calculator_for_the_price_of.collections1BuyTogether = new string[] { };
					promotion_id_calculator_for_the_price_of.collections2BuyTogether = new string[] { };
					promotion_id_calculator_for_the_price_of.minimumQuantityBuyTogether = minimum_quantity_buy_together;
					promotion_id_calculator_for_the_price_of.quantityToAffectBuyTogether = quantity_to_affect_buy_together;
					promotion_id_calculator_for_the_price_of.enableBuyTogetherPerSku = true;
					promotion_id_calculator_for_the_price_of.listSku1BuyTogether = products;
					promotion_id_calculator_for_the_price_of.listSku2BuyTogether = new string[] { };
					promotion_id_calculator_for_the_price_of.coupon = new string[] { };
					promotion_id_calculator_for_the_price_of.totalValueFloor = 0;
					promotion_id_calculator_for_the_price_of.totalValueCeling = 0;
					promotion_id_calculator_for_the_price_of.totalValueMode = "IncludeMatchedItems";
					promotion_id_calculator_for_the_price_of.collections = new string[] { };
					promotion_id_calculator_for_the_price_of.collectionsIsInclusive = true;
					promotion_id_calculator_for_the_price_of.restrictionsBins = new string[] { };
					promotion_id_calculator_for_the_price_of.cardIssuers = new string[] { };
					promotion_id_calculator_for_the_price_of.totalValuePurchase = 0.0f;
					promotion_id_calculator_for_the_price_of.slasIds = new string[] { };
					promotion_id_calculator_for_the_price_of.isSlaSelected = false;
					promotion_id_calculator_for_the_price_of.isFirstBuy = false;
					promotion_id_calculator_for_the_price_of.firstBuyIsProfileOptimistic = true;
					promotion_id_calculator_for_the_price_of.compareListPriceAndPrice = false;
					promotion_id_calculator_for_the_price_of.isDifferentListPriceAndPrice = false;
					promotion_id_calculator_for_the_price_of.zipCodeRanges = new string[] { };
					promotion_id_calculator_for_the_price_of.itemMaxPrice = 0.0f;
					promotion_id_calculator_for_the_price_of.itemMinPrice = 0.0f;
					promotion_id_calculator_for_the_price_of.isMinMaxInstallments = false;
					promotion_id_calculator_for_the_price_of.minInstallment = 0;
					promotion_id_calculator_for_the_price_of.maxInstallment = 0;
					promotion_id_calculator_for_the_price_of.merchants = new string[] { };
					promotion_id_calculator_for_the_price_of.clusterExpressions = new string[] { };
					promotion_id_calculator_for_the_price_of.piiClusterExpressions = new string[] { };
					promotion_id_calculator_for_the_price_of.clusterOperator = "all";
					promotion_id_calculator_for_the_price_of.paymentsRules = new string[] { };
					promotion_id_calculator_for_the_price_of.giftListTypes = new string[] { };
					promotion_id_calculator_for_the_price_of.productsSpecifications = new string[] { };
					promotion_id_calculator_for_the_price_of.affiliates = new string[] { };
					promotion_id_calculator_for_the_price_of.maxUsage = 0;
					promotion_id_calculator_for_the_price_of.maxUsagePerClient = 0;
					promotion_id_calculator_for_the_price_of.shouldDistributeDiscountAmongMatchedItems = false;
					promotion_id_calculator_for_the_price_of.multipleUsePerClient = false;
					promotion_id_calculator_for_the_price_of.accumulateWithManualPrice = false;
					promotion_id_calculator_for_the_price_of.type = "forThePriceOf";
					promotion_id_calculator_for_the_price_of.useNewProgressiveAlgorithm = false;
					promotion_id_calculator_for_the_price_of.percentualDiscountValueList = new int[] { };

					if (products.Count > 0)
					{
						string respuesta = "";
						try
						{
							Task<string> task_create_update_promotion = Task.Run(() => VTEXCreateUpdatePromotionIdcalculator(promotion_id_calculator_for_the_price_of, master));
							task_create_update_promotion.Wait();
							respuesta = Convert.ToString(task_create_update_promotion.Result);
							task_create_update_promotion.Dispose();
							dynamic results_create_update_promotion = JsonConvert.DeserializeObject<dynamic>(respuesta);
							total[0]++;
						}
						catch (AggregateException ex)
						{
							string errores = "";
							foreach (Exception item in ex.InnerExceptions)
							{
								errores += item.Message + ", ";
							}
							Auxiliary.SaveResultCreatePromo(name, errores.Trim().Trim(',') + "-" + respuesta);
						}
						catch (Exception ex)
						{
							Auxiliary.SaveResultCreatePromo(name, ex.Message + "-" + respuesta);
						}
					}
				}
				else
				{
					TimeZoneInfo britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

					string beginDateUtc = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["beginDateUtc"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
					string endDateUtc = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["endDateUtc"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
					string lastModified = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["lastModified"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
					Console.WriteLine($"Creando promoción {name}");
					promotion_for_the_price_of = new Models.PromotionForThePriceOf();
					promotion_for_the_price_of.name = name;
					promotion_for_the_price_of.description = name;
					promotion_for_the_price_of.beginDateUtc = beginDateUtc;
					promotion_for_the_price_of.endDateUtc = endDateUtc;
					promotion_for_the_price_of.lastModified = lastModified;
					promotion_for_the_price_of.daysAgoOfPurchases = 0;
					promotion_for_the_price_of.isActive = true;
					promotion_for_the_price_of.isArchived = false;
					promotion_for_the_price_of.isFeatured = true;
					promotion_for_the_price_of.disableDeal = false;
					promotion_for_the_price_of.offset = -5;
					promotion_for_the_price_of.activateGiftsMultiplier = false;
					promotion_for_the_price_of.newOffset = -5.0f;
					promotion_for_the_price_of.maxPricesPerItems = new string[] { };
					promotion_for_the_price_of.cumulative = false;
					promotion_for_the_price_of.nominalShippingDiscountValue = 0.0f;
					promotion_for_the_price_of.absoluteShippingDiscountValue = 0.0f;

					promotion_for_the_price_of.discountType = "forThePriceOf";
					promotion_for_the_price_of.nominalDiscountValue = 0.0f;

					float percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
					promotion_for_the_price_of.percentualDiscountValue = percentualDiscountValue;

					promotion_for_the_price_of.discountExpression = "";

					promotion_for_the_price_of.nominalDiscountType = "cart";
					promotion_for_the_price_of.maximumUnitPriceDiscount = 0.0f;
					promotion_for_the_price_of.rebatePercentualDiscountValue = 0.0f;
					promotion_for_the_price_of.percentualShippingDiscountValue = 0.0f;
					promotion_for_the_price_of.percentualTax = 0.0f;
					promotion_for_the_price_of.shippingPercentualTax = 0.0f;
					promotion_for_the_price_of.percentualDiscountValueList1 = 0.0f;
					promotion_for_the_price_of.percentualDiscountValueList2 = 0.0f;

					promotion_for_the_price_of.skusGift = new PromotionForThePriceOf.SkusGift() { quantitySelectable = 0, gifts = new object[] { } };//quantitySelectable=cantida de items de regalo

					promotion_for_the_price_of.nominalRewardValue = 0.0f;
					promotion_for_the_price_of.percentualRewardValue = 0.0f;
					promotion_for_the_price_of.orderStatusRewardValue = "invoiced";
					promotion_for_the_price_of.maxNumberOfAffectedItems = 0;
					promotion_for_the_price_of.maxNumberOfAffectedItemsGroupKey = "perCart";
					promotion_for_the_price_of.applyToAllShippings = false;
					promotion_for_the_price_of.nominalTax = 0.0f;
					if (master == true)
						promotion_for_the_price_of.origin = "Marketplace";
					else
						promotion_for_the_price_of.origin = "Fulfillment";
					promotion_for_the_price_of.idSellerIsInclusive = true;
					promotion_for_the_price_of.idsSalesChannel = new string[1] { "1" };//new string[] { };
					promotion_for_the_price_of.areSalesChannelIdsExclusive = false;
					promotion_for_the_price_of.marketingTags = new string[] { };
					promotion_for_the_price_of.marketingTagsAreNotInclusive = false;
					promotion_for_the_price_of.paymentsMethods = new string[] { };
					promotion_for_the_price_of.stores = new string[] { };
					promotion_for_the_price_of.campaigns = new string[] { };
					promotion_for_the_price_of.storesAreInclusive = true;//nuevo
					promotion_for_the_price_of.categories = new string[] { };
					promotion_for_the_price_of.categoriesAreInclusive = true;
					promotion_for_the_price_of.brands = new string[] { };
					promotion_for_the_price_of.brandsAreInclusive = true;

					List<PromotionForThePriceOf.Product> products = new List<PromotionForThePriceOf.Product>();
					int minimum_quantity_buy_together = Convert.ToInt32(rows[0]["compra"]);
					int quantity_to_affect_buy_together = Convert.ToInt32(rows[0]["lleva"]);

					foreach (DataRow row in rows)
					{
						string product_id = "";
						try
						{
							PromotionForThePriceOf.Product product = new PromotionForThePriceOf.Product();
							product_id = row["products.id"].ToString().Trim();

							Task<string> task_get = Task.Run(() => VTEXGetProductByRefId(product_id));
							task_get.Wait();
							dynamic results_get = JsonConvert.DeserializeObject<dynamic>(task_get.Result);
							task_get.Dispose();
							if (results_get != null)
							{
								product.id = results_get.Id;
								product.name = results_get.Name;
								products.Add(product);
							}
						}
						catch (Exception)
						{
							continue;
						}
					}

					promotion_for_the_price_of.products = new object[] { };
					promotion_for_the_price_of.productsAreInclusive = true;
					promotion_for_the_price_of.skus = new string[] { };
					promotion_for_the_price_of.skusAreInclusive = true;
					promotion_for_the_price_of.collections1BuyTogether = new string[] { };
					promotion_for_the_price_of.collections2BuyTogether = new string[] { };
					promotion_for_the_price_of.minimumQuantityBuyTogether = minimum_quantity_buy_together;//cantidad minima que se debe comprar para que aplique la promo
					promotion_for_the_price_of.quantityToAffectBuyTogether = quantity_to_affect_buy_together;//cantidad que se lleva de regalo
					promotion_for_the_price_of.enableBuyTogetherPerSku = true;
					promotion_for_the_price_of.listSku1BuyTogether = products;
					promotion_for_the_price_of.listSku2BuyTogether = new string[] { };
					promotion_for_the_price_of.coupon = new string[] { };
					promotion_for_the_price_of.totalValueFloor = 0;
					promotion_for_the_price_of.totalValueCeling = 0;
					promotion_for_the_price_of.totalValueMode = "IncludeMatchedItems";
					promotion_for_the_price_of.collections = new string[] { };
					promotion_for_the_price_of.collectionsIsInclusive = true;
					promotion_for_the_price_of.restrictionsBins = new string[] { };
					promotion_for_the_price_of.cardIssuers = new string[] { };
					promotion_for_the_price_of.totalValuePurchase = 0.0f;
					promotion_for_the_price_of.slasIds = new string[] { };
					promotion_for_the_price_of.isSlaSelected = false;
					promotion_for_the_price_of.isFirstBuy = false;
					promotion_for_the_price_of.firstBuyIsProfileOptimistic = true;
					promotion_for_the_price_of.compareListPriceAndPrice = false;
					promotion_for_the_price_of.isDifferentListPriceAndPrice = false;
					promotion_for_the_price_of.zipCodeRanges = new string[] { };
					promotion_for_the_price_of.itemMaxPrice = 0.0f;
					promotion_for_the_price_of.itemMinPrice = 0.0f;
					promotion_for_the_price_of.isMinMaxInstallments = false;
					promotion_for_the_price_of.minInstallment = 0;
					promotion_for_the_price_of.maxInstallment = 0;
					promotion_for_the_price_of.merchants = new string[] { };
					promotion_for_the_price_of.clusterExpressions = new string[] { };
					promotion_for_the_price_of.piiClusterExpressions = new string[] { };
					promotion_for_the_price_of.clusterOperator = "all";
					promotion_for_the_price_of.paymentsRules = new string[] { };
					promotion_for_the_price_of.giftListTypes = new string[] { };
					promotion_for_the_price_of.productsSpecifications = new string[] { };
					promotion_for_the_price_of.affiliates = new string[] { };
					promotion_for_the_price_of.maxUsage = 0;
					promotion_for_the_price_of.maxUsagePerClient = 0;
					promotion_for_the_price_of.shouldDistributeDiscountAmongMatchedItems = false;
					promotion_for_the_price_of.multipleUsePerClient = false;
					promotion_for_the_price_of.accumulateWithManualPrice = false;
					promotion_for_the_price_of.type = "forThePriceOf";
					promotion_for_the_price_of.useNewProgressiveAlgorithm = false;
					promotion_for_the_price_of.percentualDiscountValueList = new int[] { };
					if (products.Count > 0)
					{
						string respuesta = "";
						try
						{
							Task<string> task_create_update_promotion = Task.Run(() => VTEXCreateUpdatePromotion(promotion_for_the_price_of, master));
							task_create_update_promotion.Wait();
							respuesta = Convert.ToString(task_create_update_promotion.Result);
							task_create_update_promotion.Dispose();
							dynamic results_create_update_promotion = JsonConvert.DeserializeObject<dynamic>(respuesta);
							total[1]++;
						}
						catch (AggregateException ex)
						{
							string errores = "";
							foreach (Exception item in ex.InnerExceptions)
							{
								errores += item.Message + ", ";
							}
							Auxiliary.SaveResultCreatePromo(name, errores.Trim().Trim(',') + "-" + respuesta);
						}
						catch (Exception ex)
						{
							Auxiliary.SaveResultCreatePromo(name, ex.Message + "-" + respuesta);
						}
					}
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				Auxiliary.SaveResultCreatePromo(name, errores.Trim().Trim(','));
			}
			catch (Exception ex)
			{
				Auxiliary.SaveResultCreatePromo(name, ex.Message);
			}
			return total;
		}

		public int[] CreateUpdateRegularPromotion(string name, DataRow[] rows, bool master)
		{
			string id_calculator_configuration = "";
			int[] total = new int[2];
			try
			{
				Models.PromotionRegular promotion_regular = null;
				Models.PromotionIdCalculatorRegular promotion_id_calculator_regular = null;

				Task<string> task_get_promotion = Task.Run(() => VTEXSearchPromotionByName(name, master));
				task_get_promotion.Wait();
				string res = Convert.ToString(task_get_promotion.Result);
				task_get_promotion.Dispose();

				if (res.TrimStart('[').TrimEnd(']') != "")
				{
					dynamic results_get_promotion = JsonConvert.DeserializeObject<dynamic>(res);
					id_calculator_configuration = Convert.ToString(results_get_promotion[0].idCalculatorConfiguration);
					Console.WriteLine(id_calculator_configuration);

					TimeZoneInfo britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

					string beginDateUtc = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["beginDateUtc"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
					string endDateUtc = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["endDateUtc"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
					string lastModified = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["lastModified"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");

					promotion_id_calculator_regular = new Models.PromotionIdCalculatorRegular();
					promotion_id_calculator_regular.idCalculatorConfiguration = id_calculator_configuration;
					promotion_id_calculator_regular.name = name;
					promotion_id_calculator_regular.description = name;// rows[0]["description"].ToString().Trim();
					promotion_id_calculator_regular.beginDateUtc = beginDateUtc;
					promotion_id_calculator_regular.endDateUtc = endDateUtc;
					promotion_id_calculator_regular.lastModified = lastModified;
					promotion_id_calculator_regular.daysAgoOfPurchases = 0;
					promotion_id_calculator_regular.isActive = true;
					promotion_id_calculator_regular.isArchived = false;
					promotion_id_calculator_regular.isFeatured = true;//aqui era true
					promotion_id_calculator_regular.disableDeal = false;
					promotion_id_calculator_regular.offset = -5;
					promotion_id_calculator_regular.activateGiftsMultiplier = false;
					promotion_id_calculator_regular.newOffset = -5.0f;
					promotion_id_calculator_regular.maxPricesPerItems = new string[] { };
					promotion_id_calculator_regular.cumulative = false;

					float percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
					float nominalDiscountValue = Convert.ToSingle(rows[0]["nominalDiscountValue"]);
					if (nominalDiscountValue > 0)
					{
						promotion_id_calculator_regular.discountType = "nominal";//percentual
						promotion_id_calculator_regular.nominalDiscountValue = 0;// Convert.ToSingle(rows[0]["nominalDiscountValue"]);
						promotion_id_calculator_regular.percentualDiscountValue = 0.0f;
						promotion_id_calculator_regular.discountExpression = $"quantity * {Convert.ToString(rows[0]["nominalDiscountValue"]).Replace(",", ".")}";
					}
					if (percentualDiscountValue > 0)
					{
						promotion_id_calculator_regular.discountType = "percentual";
						promotion_id_calculator_regular.nominalDiscountValue = 0.0f;
						promotion_id_calculator_regular.percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
						promotion_id_calculator_regular.discountExpression = "";
					}

					promotion_id_calculator_regular.nominalShippingDiscountValue = 0.0f;
					promotion_id_calculator_regular.absoluteShippingDiscountValue = 0.0f;
					promotion_id_calculator_regular.nominalDiscountType = "cart";
					promotion_id_calculator_regular.maximumUnitPriceDiscount = 0.0f;
					promotion_id_calculator_regular.rebatePercentualDiscountValue = 0.0f;
					promotion_id_calculator_regular.percentualShippingDiscountValue = 0.0f;
					promotion_id_calculator_regular.percentualTax = 0.0f;
					promotion_id_calculator_regular.shippingPercentualTax = 0.0f;
					promotion_id_calculator_regular.percentualDiscountValueList1 = 0.0f;
					promotion_id_calculator_regular.percentualDiscountValueList2 = 0.0f;
					promotion_id_calculator_regular.skusGift = new PromotionIdCalculatorRegular.SkusGift() { quantitySelectable = 1, gifts = new object[] { } };
					promotion_id_calculator_regular.nominalRewardValue = 0.0f;
					promotion_id_calculator_regular.percentualRewardValue = 0.0f;
					promotion_id_calculator_regular.orderStatusRewardValue = "invoiced";
					promotion_id_calculator_regular.maxNumberOfAffectedItems = 0;
					promotion_id_calculator_regular.maxNumberOfAffectedItemsGroupKey = "perCart";
					promotion_id_calculator_regular.applyToAllShippings = false;
					promotion_id_calculator_regular.nominalTax = 0.0f;
					if (master == true)
						promotion_id_calculator_regular.origin = "Marketplace";
					else
						promotion_id_calculator_regular.origin = "Fulfillment";
					promotion_id_calculator_regular.idSellerIsInclusive = true;
					promotion_id_calculator_regular.idsSalesChannel = new string[1] { "1" };//new string[] { };
					promotion_id_calculator_regular.areSalesChannelIdsExclusive = false;
					promotion_id_calculator_regular.marketingTags = new string[] { };
					promotion_id_calculator_regular.marketingTagsAreNotInclusive = false;
					promotion_id_calculator_regular.paymentsMethods = new string[] { };
					promotion_id_calculator_regular.stores = new string[] { };
					promotion_id_calculator_regular.campaigns = new string[] { };
					promotion_id_calculator_regular.storesAreInclusive = true;//nuevo
					promotion_id_calculator_regular.categories = new string[] { };
					promotion_id_calculator_regular.categoriesAreInclusive = true;
					promotion_id_calculator_regular.brands = new string[] { };
					promotion_id_calculator_regular.brandsAreInclusive = true;

					List<PromotionIdCalculatorRegular.Product> products = new List<PromotionIdCalculatorRegular.Product>();

					foreach (DataRow row in rows)
					{
						string product_id = "";
						try
						{
							PromotionIdCalculatorRegular.Product product = new PromotionIdCalculatorRegular.Product();
							product_id = row["products.id"].ToString().Trim();

							Task<string> task_get = Task.Run(() => VTEXGetProductByRefId(product_id));
							task_get.Wait();
							dynamic results_get = JsonConvert.DeserializeObject<dynamic>(task_get.Result);
							task_get.Dispose();
							if (results_get != null)
							{
								product.id = results_get.Id;
								product.name = results_get.Name;
								products.Add(product);
							}
						}
						catch (Exception)
						{
							continue;
						}
					}

					promotion_id_calculator_regular.products = products;
					promotion_id_calculator_regular.productsAreInclusive = true;
					promotion_id_calculator_regular.skus = new string[] { };
					promotion_id_calculator_regular.skusAreInclusive = true;
					promotion_id_calculator_regular.collections1BuyTogether = new string[] { };
					promotion_id_calculator_regular.collections2BuyTogether = new string[] { };
					promotion_id_calculator_regular.minimumQuantityBuyTogether = 0;
					promotion_id_calculator_regular.quantityToAffectBuyTogether = 0;
					promotion_id_calculator_regular.enableBuyTogetherPerSku = false;
					promotion_id_calculator_regular.listSku1BuyTogether = new string[] { };
					promotion_id_calculator_regular.listSku2BuyTogether = new string[] { };
					promotion_id_calculator_regular.coupon = new string[] { };
					promotion_id_calculator_regular.totalValueFloor = 0;
					promotion_id_calculator_regular.totalValueCeling = 0;
					promotion_id_calculator_regular.totalValueMode = "IncludeMatchedItems";
					promotion_id_calculator_regular.collections = new string[] { };
					promotion_id_calculator_regular.collectionsIsInclusive = true;
					promotion_id_calculator_regular.restrictionsBins = new string[] { };
					promotion_id_calculator_regular.cardIssuers = new string[] { };
					promotion_id_calculator_regular.totalValuePurchase = 0.0f;
					promotion_id_calculator_regular.slasIds = new string[] { };
					promotion_id_calculator_regular.isSlaSelected = false;
					promotion_id_calculator_regular.isFirstBuy = false;
					promotion_id_calculator_regular.firstBuyIsProfileOptimistic = true;
					promotion_id_calculator_regular.compareListPriceAndPrice = false;
					promotion_id_calculator_regular.isDifferentListPriceAndPrice = false;
					promotion_id_calculator_regular.zipCodeRanges = new string[] { };
					promotion_id_calculator_regular.itemMaxPrice = 0.0f;
					promotion_id_calculator_regular.itemMinPrice = 0.0f;
					promotion_id_calculator_regular.isMinMaxInstallments = false;
					promotion_id_calculator_regular.minInstallment = 0;
					promotion_id_calculator_regular.maxInstallment = 0;
					promotion_id_calculator_regular.merchants = new string[] { };
					promotion_id_calculator_regular.clusterExpressions = new string[] { };
					promotion_id_calculator_regular.piiClusterExpressions = new string[] { };
					promotion_id_calculator_regular.clusterOperator = "all";
					promotion_id_calculator_regular.paymentsRules = new string[] { };
					promotion_id_calculator_regular.giftListTypes = new string[] { };
					promotion_id_calculator_regular.productsSpecifications = new string[] { };
					promotion_id_calculator_regular.affiliates = new string[] { };
					promotion_id_calculator_regular.maxUsage = 0;
					promotion_id_calculator_regular.maxUsagePerClient = 0;
					promotion_id_calculator_regular.shouldDistributeDiscountAmongMatchedItems = false;
					promotion_id_calculator_regular.multipleUsePerClient = false;
					promotion_id_calculator_regular.accumulateWithManualPrice = false;
					promotion_id_calculator_regular.type = "regular";
					promotion_id_calculator_regular.useNewProgressiveAlgorithm = false;
					promotion_id_calculator_regular.percentualDiscountValueList = new int[] { };

					if (products.Count > 0)
					{
						string respuesta = "";
						try
						{
							Task<string> task_create_update_promotion = Task.Run(() => VTEXCreateUpdatePromotionIdcalculator(promotion_id_calculator_regular, master));
							task_create_update_promotion.Wait();
							respuesta = Convert.ToString(task_create_update_promotion.Result);
							task_create_update_promotion.Dispose();
							dynamic results_create_update_promotion = JsonConvert.DeserializeObject<dynamic>(respuesta);
							total[0]++;
						}
						catch (AggregateException ex)
						{
							string errores = "";
							foreach (Exception item in ex.InnerExceptions)
							{
								errores += item.Message + ", ";
							}
							Auxiliary.SaveResultCreatePromo(name, errores.Trim().Trim(',') + "-" + respuesta);
						}
						catch (Exception ex)
						{
							Auxiliary.SaveResultCreatePromo(name, ex.Message + "-" + respuesta);
						}
					}
				}
				else
				{
					TimeZoneInfo britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

					string beginDateUtc = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["beginDateUtc"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
					string endDateUtc = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["endDateUtc"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
					string lastModified = TimeZoneInfo.ConvertTime(Convert.ToDateTime(rows[0]["lastModified"]), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
					Console.WriteLine($"Creando promoción {name}");
					promotion_regular = new Models.PromotionRegular();
					promotion_regular.name = name;
					promotion_regular.description = name;// rows[0]["description"].ToString().Trim();
					promotion_regular.beginDateUtc = beginDateUtc;
					promotion_regular.endDateUtc = endDateUtc;
					promotion_regular.lastModified = lastModified;
					promotion_regular.daysAgoOfPurchases = 0;
					promotion_regular.isActive = true;
					promotion_regular.isArchived = false;
					promotion_regular.isFeatured = true;
					promotion_regular.disableDeal = false;
					promotion_regular.offset = -5;
					promotion_regular.activateGiftsMultiplier = false;
					promotion_regular.newOffset = -5.0f;
					promotion_regular.maxPricesPerItems = new string[] { };
					promotion_regular.cumulative = false;
					promotion_regular.nominalShippingDiscountValue = 0.0f;
					promotion_regular.absoluteShippingDiscountValue = 0.0f;

					float percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
					float nominalDiscountValue = Convert.ToSingle(rows[0]["nominalDiscountValue"]);
					if (nominalDiscountValue > 0)
					{
						promotion_regular.discountType = "nominal";
						promotion_regular.nominalDiscountValue = 0;// Convert.ToSingle(rows[0]["nominalDiscountValue"]);
						promotion_regular.percentualDiscountValue = 0.0f;
						promotion_regular.discountExpression = $"quantity * {Convert.ToString(rows[0]["nominalDiscountValue"]).Replace(",", ".")}";
					}
					if (percentualDiscountValue > 0)
					{
						promotion_regular.discountType = "percentual";
						promotion_regular.nominalDiscountValue = 0.0f;
						promotion_regular.percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
						promotion_regular.discountExpression = "";
					}

					promotion_regular.nominalDiscountType = "cart";
					promotion_regular.maximumUnitPriceDiscount = 0.0f;
					promotion_regular.rebatePercentualDiscountValue = 0.0f;
					promotion_regular.percentualShippingDiscountValue = 0.0f;
					promotion_regular.percentualTax = 0.0f;
					promotion_regular.shippingPercentualTax = 0.0f;
					promotion_regular.percentualDiscountValueList1 = 0.0f;
					promotion_regular.percentualDiscountValueList2 = 0.0f;
					promotion_regular.skusGift = new PromotionRegular.SkusGift() { quantitySelectable = 1, gifts = new object[] { } };
					promotion_regular.nominalRewardValue = 0.0f;
					promotion_regular.percentualRewardValue = 0.0f;
					promotion_regular.orderStatusRewardValue = "invoiced";
					promotion_regular.maxNumberOfAffectedItems = 0;
					promotion_regular.maxNumberOfAffectedItemsGroupKey = "perCart";
					promotion_regular.applyToAllShippings = false;
					promotion_regular.nominalTax = 0.0f;
					if (master == true)
						promotion_regular.origin = "Marketplace";
					else
						promotion_regular.origin = "Fulfillment";
					promotion_regular.idSellerIsInclusive = true;
					promotion_regular.idsSalesChannel = new string[1] { "1" };//new string[] { };
					promotion_regular.areSalesChannelIdsExclusive = false;
					promotion_regular.marketingTags = new string[] { };
					promotion_regular.marketingTagsAreNotInclusive = false;
					promotion_regular.paymentsMethods = new string[] { };
					promotion_regular.stores = new string[] { };
					promotion_regular.campaigns = new string[] { };
					promotion_regular.storesAreInclusive = true;//nuevo
					promotion_regular.categories = new string[] { };
					promotion_regular.categoriesAreInclusive = true;
					promotion_regular.brands = new string[] { };
					promotion_regular.brandsAreInclusive = true;

					List<PromotionRegular.Product> products = new List<PromotionRegular.Product>();

					foreach (DataRow row in rows)
					{
						string product_id = "";
						try
						{
							PromotionRegular.Product product = new PromotionRegular.Product();
							product_id = row["products.id"].ToString().Trim();
							Task<string> task_get = Task.Run(() => VTEXGetProductByRefId(product_id));
							task_get.Wait();
							dynamic results_get = JsonConvert.DeserializeObject<dynamic>(task_get.Result);
							task_get.Dispose();
							if (results_get != null)
							{
								product.id = results_get.Id;
								product.name = results_get.Name;
								products.Add(product);
							}
						}
						catch (Exception)
						{
							continue;
						}
					}

					promotion_regular.products = products;
					promotion_regular.productsAreInclusive = true;
					promotion_regular.skus = new string[] { };
					promotion_regular.skusAreInclusive = true;
					promotion_regular.collections1BuyTogether = new string[] { };
					promotion_regular.collections2BuyTogether = new string[] { };
					promotion_regular.minimumQuantityBuyTogether = 0;
					promotion_regular.quantityToAffectBuyTogether = 0;
					promotion_regular.enableBuyTogetherPerSku = false;
					promotion_regular.listSku1BuyTogether = new string[] { };
					promotion_regular.listSku2BuyTogether = new string[] { };
					promotion_regular.coupon = new string[] { };
					promotion_regular.totalValueFloor = 0;
					promotion_regular.totalValueCeling = 0;
					promotion_regular.totalValueMode = "IncludeMatchedItems";
					promotion_regular.collections = new string[] { };
					promotion_regular.collectionsIsInclusive = true;
					promotion_regular.restrictionsBins = new string[] { };
					promotion_regular.cardIssuers = new string[] { };
					promotion_regular.totalValuePurchase = 0.0f;
					promotion_regular.slasIds = new string[] { };
					promotion_regular.isSlaSelected = false;
					promotion_regular.isFirstBuy = false;
					promotion_regular.firstBuyIsProfileOptimistic = true;
					promotion_regular.compareListPriceAndPrice = false;
					promotion_regular.isDifferentListPriceAndPrice = false;
					promotion_regular.zipCodeRanges = new string[] { };
					promotion_regular.itemMaxPrice = 0.0f;
					promotion_regular.itemMinPrice = 0.0f;
					promotion_regular.isMinMaxInstallments = false;
					promotion_regular.minInstallment = 0;
					promotion_regular.maxInstallment = 0;
					promotion_regular.merchants = new string[] { };
					promotion_regular.clusterExpressions = new string[] { };
					promotion_regular.piiClusterExpressions = new string[] { };
					promotion_regular.clusterOperator = "all";
					promotion_regular.paymentsRules = new string[] { };
					promotion_regular.giftListTypes = new string[] { };
					promotion_regular.productsSpecifications = new string[] { };
					promotion_regular.affiliates = new string[] { };
					promotion_regular.maxUsage = 0;
					promotion_regular.maxUsagePerClient = 0;
					promotion_regular.shouldDistributeDiscountAmongMatchedItems = false;
					promotion_regular.multipleUsePerClient = false;
					promotion_regular.accumulateWithManualPrice = false;
					promotion_regular.type = "regular";
					promotion_regular.useNewProgressiveAlgorithm = false;
					promotion_regular.percentualDiscountValueList = new int[] { };
					if (products.Count > 0)
					{
						string respuesta = "";
						try
						{
							Task<string> task_create_update_promotion = Task.Run(() => VTEXCreateUpdatePromotion(promotion_regular, master));
							task_create_update_promotion.Wait();
							respuesta = Convert.ToString(task_create_update_promotion.Result);
							task_create_update_promotion.Dispose();
							dynamic results_create_update_promotion = JsonConvert.DeserializeObject<dynamic>(respuesta);
							total[1]++;
						}
						catch (AggregateException ex)
						{
							string errores = "";
							foreach (Exception item in ex.InnerExceptions)
							{
								errores += item.Message + ", ";
							}
							Auxiliary.SaveResultCreatePromo(name, errores.Trim().Trim(',') + "-" + respuesta);
						}
						catch (Exception ex)
						{
							Auxiliary.SaveResultCreatePromo(name, ex.Message + "-" + respuesta);
						}
					}
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				Auxiliary.SaveResultCreatePromo(name, errores.Trim().Trim(','));
			}
			catch (Exception ex)
			{
				Auxiliary.SaveResultCreatePromo(name, ex.Message);
			}
			return total;
		}
		#endregion
	}
}
