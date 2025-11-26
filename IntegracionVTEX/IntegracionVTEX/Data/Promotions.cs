using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using IntegracionVTEX.Models;
using static IntegracionVTEX.Models.Price;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.Schema;

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

		private async Task<string> VTEXCreateUpdatePromotion(Models.Promotion promotion, bool master)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlCreateOrUpdatePromotion;// "https://whitelabelspruebas--megatiendas.vtexcommercestable.com.br/api/rnb/pvt/calculatorconfiguration"
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

		private async Task<string> VTEXCreateUpdatePromotionIdcalculator(Models.PromotionIdCalculator promotion, bool master)
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

		public DataTable GetPromotionBuyAndWinListCentroOperacion()
		{
			DataTable res = null;
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = File.ReadAllText("Data\\PromotionBuyAndWinCentroOperacionQuery.txt");

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

		public int[] CreateUpdateBuyAndWinPromotion(string name, DataRow[] rows, bool master)
		{
			string id_calculator_configuration = "";
			int[] total = new int[2];
			try
			{
				Models.Promotion promotion = null;
				Models.PromotionIdCalculator promotion_id_calculator = null;

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

					promotion_id_calculator = new Models.PromotionIdCalculator();
					promotion_id_calculator.idCalculatorConfiguration = id_calculator_configuration;
					promotion_id_calculator.name = name;
					promotion_id_calculator.description = name;// rows[0]["description"].ToString().Trim();
					promotion_id_calculator.beginDateUtc = beginDateUtc;
					promotion_id_calculator.endDateUtc = endDateUtc;
					promotion_id_calculator.lastModified = lastModified;
					promotion_id_calculator.daysAgoOfPurchases = 0;
					promotion_id_calculator.isActive = true;
					promotion_id_calculator.isArchived = false;
					promotion_id_calculator.isFeatured = true;//aqui era true
					promotion_id_calculator.disableDeal = false;
					promotion_id_calculator.offset = -5;
					promotion_id_calculator.activateGiftsMultiplier = false;
					promotion_id_calculator.newOffset = -5.0f;
					promotion_id_calculator.maxPricesPerItems = new string[] { };
					promotion_id_calculator.cumulative = false;

					float percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
					float nominalDiscountValue = Convert.ToSingle(rows[0]["nominalDiscountValue"]);
					if (nominalDiscountValue > 0)
					{
						promotion_id_calculator.discountType = "nominal";//percentual
						promotion_id_calculator.nominalDiscountValue = Convert.ToSingle(rows[0]["nominalDiscountValue"]);
						promotion_id_calculator.percentualDiscountValue = 0.0f;
					}
					if (percentualDiscountValue > 0)
					{
						promotion_id_calculator.discountType = "percentual";
						promotion_id_calculator.nominalDiscountValue = 0.0f;
						promotion_id_calculator.percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
					}

					promotion_id_calculator.nominalShippingDiscountValue = 0.0f;
					promotion_id_calculator.absoluteShippingDiscountValue = 0.0f;
					promotion_id_calculator.nominalDiscountType = "cart";
					promotion_id_calculator.maximumUnitPriceDiscount = 0.0f;
					promotion_id_calculator.rebatePercentualDiscountValue = 0.0f;
					promotion_id_calculator.percentualShippingDiscountValue = 0.0f;
					promotion_id_calculator.percentualTax = 0.0f;
					promotion_id_calculator.shippingPercentualTax = 0.0f;
					promotion_id_calculator.percentualDiscountValueList1 = 0.0f;
					promotion_id_calculator.percentualDiscountValueList2 = 0.0f;
					promotion_id_calculator.skusGift = new PromotionIdCalculator.SkusGift() { quantitySelectable = 1, gifts = new object[] { } };
					promotion_id_calculator.nominalRewardValue = 0.0f;
					promotion_id_calculator.percentualRewardValue = 0.0f;
					promotion_id_calculator.orderStatusRewardValue = "invoiced";
					promotion_id_calculator.maxNumberOfAffectedItems = 0;
					promotion_id_calculator.maxNumberOfAffectedItemsGroupKey = "perCart";
					promotion_id_calculator.applyToAllShippings = false;
					promotion_id_calculator.nominalTax = 0.0f;
					if (master == true)
						promotion_id_calculator.origin = "Marketplace";
					else
						promotion_id_calculator.origin = "Fulfillment";
					promotion_id_calculator.idSellerIsInclusive = true;
					promotion_id_calculator.idsSalesChannel = new string[1] { "1" };//new string[] { };
					promotion_id_calculator.areSalesChannelIdsExclusive = false;
					promotion_id_calculator.marketingTags = new string[] { };
					promotion_id_calculator.marketingTagsAreNotInclusive = false;
					promotion_id_calculator.paymentsMethods = new string[] { };
					promotion_id_calculator.stores = new string[] { };
					promotion_id_calculator.campaigns = new string[] { };
					promotion_id_calculator.storesAreInclusive = true;//nuevo
					promotion_id_calculator.categories = new string[] { };
					promotion_id_calculator.categoriesAreInclusive = true;
					promotion_id_calculator.brands = new string[] { };
					promotion_id_calculator.brandsAreInclusive = true;

					List<PromotionIdCalculator.Product> products = new List<PromotionIdCalculator.Product>();

					foreach (DataRow row in rows)
					{
						string product_id = "";
						try
						{
							PromotionIdCalculator.Product product = new PromotionIdCalculator.Product();
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

					promotion_id_calculator.products = products;
					promotion_id_calculator.productsAreInclusive = true;
					promotion_id_calculator.skus = new string[] { };
					promotion_id_calculator.skusAreInclusive = true;
					promotion_id_calculator.collections1BuyTogether = new string[] { };
					promotion_id_calculator.collections2BuyTogether = new string[] { };
					promotion_id_calculator.minimumQuantityBuyTogether = 0;
					promotion_id_calculator.quantityToAffectBuyTogether = 0;
					promotion_id_calculator.enableBuyTogetherPerSku = false;
					promotion_id_calculator.listSku1BuyTogether = new string[] { };
					promotion_id_calculator.listSku2BuyTogether = new string[] { };
					promotion_id_calculator.coupon = new string[] { };
					promotion_id_calculator.totalValueFloor = 0;
					promotion_id_calculator.totalValueCeling = 0;
					promotion_id_calculator.totalValueMode = "IncludeMatchedItems";
					promotion_id_calculator.collections = new string[] { };
					promotion_id_calculator.collectionsIsInclusive = true;
					promotion_id_calculator.restrictionsBins = new string[] { };
					promotion_id_calculator.cardIssuers = new string[] { };
					promotion_id_calculator.totalValuePurchase = 0.0f;
					promotion_id_calculator.slasIds = new string[] { };
					promotion_id_calculator.isSlaSelected = false;
					promotion_id_calculator.isFirstBuy = false;
					promotion_id_calculator.firstBuyIsProfileOptimistic = true;
					promotion_id_calculator.compareListPriceAndPrice = false;
					promotion_id_calculator.isDifferentListPriceAndPrice = false;
					promotion_id_calculator.zipCodeRanges = new string[] { };
					promotion_id_calculator.itemMaxPrice = 0.0f;
					promotion_id_calculator.itemMinPrice = 0.0f;
					promotion_id_calculator.isMinMaxInstallments = false;
					promotion_id_calculator.minInstallment = 0;
					promotion_id_calculator.maxInstallment = 0;
					promotion_id_calculator.merchants = new string[] { };
					promotion_id_calculator.clusterExpressions = new string[] { };
					promotion_id_calculator.piiClusterExpressions = new string[] { };
					promotion_id_calculator.clusterOperator = "all";
					promotion_id_calculator.paymentsRules = new string[] { };
					promotion_id_calculator.giftListTypes = new string[] { };
					promotion_id_calculator.productsSpecifications = new string[] { };
					promotion_id_calculator.affiliates = new string[] { };
					promotion_id_calculator.maxUsage = 0;
					promotion_id_calculator.maxUsagePerClient = 0;
					promotion_id_calculator.shouldDistributeDiscountAmongMatchedItems = false;
					promotion_id_calculator.multipleUsePerClient = false;
					promotion_id_calculator.accumulateWithManualPrice = false;
					promotion_id_calculator.type = "buyAndWin";
					promotion_id_calculator.useNewProgressiveAlgorithm = false;
					promotion_id_calculator.percentualDiscountValueList = new int[] { };

					if (products.Count > 0)
					{
						string respuesta = "";
						try
						{
							Task<string> task_create_update_promotion = Task.Run(() => VTEXCreateUpdatePromotionIdcalculator(promotion_id_calculator, master));
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
					promotion = new Models.Promotion();
					promotion.name = name;
					promotion.description = name;// rows[0]["description"].ToString().Trim();
					promotion.beginDateUtc = beginDateUtc;
					promotion.endDateUtc = endDateUtc;
					promotion.lastModified = lastModified;
					promotion.daysAgoOfPurchases = 0;
					promotion.isActive = true;
					promotion.isArchived = false;
					promotion.isFeatured = true;
					promotion.disableDeal = false;
					promotion.offset = -5;
					promotion.activateGiftsMultiplier = false;
					promotion.newOffset = -5.0f;
					promotion.maxPricesPerItems = new string[] { };
					promotion.cumulative = false;
					promotion.nominalShippingDiscountValue = 0.0f;
					promotion.absoluteShippingDiscountValue = 0.0f;

					promotion.discountType = "buyAndWin";
					promotion.nominalDiscountValue = 0.0f;
					promotion.percentualDiscountValue = 0.0f;

					promotion.nominalDiscountType = "cart";
					promotion.maximumUnitPriceDiscount = 0.0f;
					promotion.rebatePercentualDiscountValue = 0.0f;
					promotion.percentualShippingDiscountValue = 0.0f;
					promotion.percentualTax = 0.0f;
					promotion.shippingPercentualTax = 0.0f;
					promotion.percentualDiscountValueList1 = 0.0f;
					promotion.percentualDiscountValueList2 = 0.0f;
					promotion.skusGift = new Promotion.SkusGift() { quantitySelectable = 1, gifts = new object[] { } };//quantitySelectable=cantida de items de regalo
					promotion.nominalRewardValue = 0.0f;
					promotion.percentualRewardValue = 0.0f;
					promotion.orderStatusRewardValue = "invoiced";
					promotion.maxNumberOfAffectedItems = 0;
					promotion.maxNumberOfAffectedItemsGroupKey = "perCart";
					promotion.applyToAllShippings = false;
					promotion.nominalTax = 0.0f;
					if (master == true)
						promotion.origin = "Marketplace";
					else
						promotion.origin = "Fulfillment";
					promotion.idSellerIsInclusive = true;
					promotion.idsSalesChannel = new string[1] { "1" };//new string[] { };
					promotion.areSalesChannelIdsExclusive = false;
					promotion.marketingTags = new string[] { };
					promotion.marketingTagsAreNotInclusive = false;
					promotion.paymentsMethods = new string[] { };
					promotion.stores = new string[] { };
					promotion.campaigns = new string[] { };
					promotion.storesAreInclusive = true;//nuevo
					promotion.categories = new string[] { };
					promotion.categoriesAreInclusive = true;
					promotion.brands = new string[] { };
					promotion.brandsAreInclusive = true;

					List<Promotion.Product> products = new List<Promotion.Product>();

					foreach (DataRow row in rows)
					{
						string product_id = "";
						try
						{
							Promotion.Product product = new Promotion.Product();
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

					promotion.products = products;
					promotion.productsAreInclusive = true;
					promotion.skus = new string[] { };
					promotion.skusAreInclusive = true;
					promotion.collections1BuyTogether = new string[] { };
					promotion.collections2BuyTogether = new string[] { };
					promotion.minimumQuantityBuyTogether = 0;
					promotion.quantityToAffectBuyTogether = 0;
					promotion.enableBuyTogetherPerSku = false;
					promotion.listSku1BuyTogether = new string[] { };
					promotion.listSku2BuyTogether = new string[] { };
					promotion.coupon = new string[] { };
					promotion.totalValueFloor = 0;
					promotion.totalValueCeling = 0;
					promotion.totalValueMode = "IncludeMatchedItems";
					promotion.collections = new string[] { };
					promotion.collectionsIsInclusive = true;
					promotion.restrictionsBins = new string[] { };
					promotion.cardIssuers = new string[] { };
					promotion.totalValuePurchase = 0.0f;
					promotion.slasIds = new string[] { };
					promotion.isSlaSelected = false;
					promotion.isFirstBuy = false;
					promotion.firstBuyIsProfileOptimistic = true;
					promotion.compareListPriceAndPrice = false;
					promotion.isDifferentListPriceAndPrice = false;
					promotion.zipCodeRanges = new string[] { };
					promotion.itemMaxPrice = 0.0f;
					promotion.itemMinPrice = 0.0f;
					promotion.isMinMaxInstallments = false;
					promotion.minInstallment = 0;
					promotion.maxInstallment = 0;
					promotion.merchants = new string[] { };
					promotion.clusterExpressions = new string[] { };
					promotion.piiClusterExpressions = new string[] { };
					promotion.clusterOperator = "all";
					promotion.paymentsRules = new string[] { };
					promotion.giftListTypes = new string[] { };
					promotion.productsSpecifications = new string[] { };
					promotion.affiliates = new string[] { };
					promotion.maxUsage = 0;
					promotion.maxUsagePerClient = 0;
					promotion.shouldDistributeDiscountAmongMatchedItems = false;
					promotion.multipleUsePerClient = false;
					promotion.accumulateWithManualPrice = false;
					promotion.type = "buyAndWin";
					promotion.useNewProgressiveAlgorithm = false;
					promotion.percentualDiscountValueList = new int[] { };
					if (products.Count > 0)
					{
						string respuesta = "";
						try
						{
							Task<string> task_create_update_promotion = Task.Run(() => VTEXCreateUpdatePromotion(promotion, master));
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
				Models.Promotion promotion = null;
				Models.PromotionIdCalculator promotion_id_calculator = null;

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

					promotion_id_calculator = new Models.PromotionIdCalculator();
					promotion_id_calculator.idCalculatorConfiguration = id_calculator_configuration;
					promotion_id_calculator.name = name;
					promotion_id_calculator.description = name;// rows[0]["description"].ToString().Trim();
					promotion_id_calculator.beginDateUtc = beginDateUtc;
					promotion_id_calculator.endDateUtc = endDateUtc;
					promotion_id_calculator.lastModified = lastModified;
					promotion_id_calculator.daysAgoOfPurchases = 0;
					promotion_id_calculator.isActive = true;
					promotion_id_calculator.isArchived = false;
					promotion_id_calculator.isFeatured = true;//aqui era true
					promotion_id_calculator.disableDeal = false;
					promotion_id_calculator.offset = -5;
					promotion_id_calculator.activateGiftsMultiplier = false;
					promotion_id_calculator.newOffset = -5.0f;
					promotion_id_calculator.maxPricesPerItems = new string[] { };
					promotion_id_calculator.cumulative = false;

					float percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
					float nominalDiscountValue = Convert.ToSingle(rows[0]["nominalDiscountValue"]);
					if (nominalDiscountValue > 0)
					{
						promotion_id_calculator.discountType = "nominal";//percentual
						promotion_id_calculator.nominalDiscountValue = Convert.ToSingle(rows[0]["nominalDiscountValue"]);
						promotion_id_calculator.percentualDiscountValue = 0.0f;
					}
					if (percentualDiscountValue > 0)
					{
						promotion_id_calculator.discountType = "percentual";
						promotion_id_calculator.nominalDiscountValue = 0.0f;
						promotion_id_calculator.percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
					}

					promotion_id_calculator.nominalShippingDiscountValue = 0.0f;
					promotion_id_calculator.absoluteShippingDiscountValue = 0.0f;
					promotion_id_calculator.nominalDiscountType = "cart";
					promotion_id_calculator.maximumUnitPriceDiscount = 0.0f;
					promotion_id_calculator.rebatePercentualDiscountValue = 0.0f;
					promotion_id_calculator.percentualShippingDiscountValue = 0.0f;
					promotion_id_calculator.percentualTax = 0.0f;
					promotion_id_calculator.shippingPercentualTax = 0.0f;
					promotion_id_calculator.percentualDiscountValueList1 = 0.0f;
					promotion_id_calculator.percentualDiscountValueList2 = 0.0f;
					promotion_id_calculator.skusGift = new PromotionIdCalculator.SkusGift() { quantitySelectable = 1, gifts = new object[] { } };
					promotion_id_calculator.nominalRewardValue = 0.0f;
					promotion_id_calculator.percentualRewardValue = 0.0f;
					promotion_id_calculator.orderStatusRewardValue = "invoiced";
					promotion_id_calculator.maxNumberOfAffectedItems = 0;
					promotion_id_calculator.maxNumberOfAffectedItemsGroupKey = "perCart";
					promotion_id_calculator.applyToAllShippings = false;
					promotion_id_calculator.nominalTax = 0.0f;
					if (master == true)
						promotion_id_calculator.origin = "Marketplace";
					else
						promotion_id_calculator.origin = "Fulfillment";
					promotion_id_calculator.idSellerIsInclusive = true;
					promotion_id_calculator.idsSalesChannel = new string[1] { "1" };//new string[] { };
					promotion_id_calculator.areSalesChannelIdsExclusive = false;
					promotion_id_calculator.marketingTags = new string[] { };
					promotion_id_calculator.marketingTagsAreNotInclusive = false;
					promotion_id_calculator.paymentsMethods = new string[] { };
					promotion_id_calculator.stores = new string[] { };
					promotion_id_calculator.campaigns = new string[] { };
					promotion_id_calculator.storesAreInclusive = true;//nuevo
					promotion_id_calculator.categories = new string[] { };
					promotion_id_calculator.categoriesAreInclusive = true;
					promotion_id_calculator.brands = new string[] { };
					promotion_id_calculator.brandsAreInclusive = true;

					List<PromotionIdCalculator.Product> products = new List<PromotionIdCalculator.Product>();

					foreach (DataRow row in rows)
					{
						string product_id = "";
						try
						{
							PromotionIdCalculator.Product product = new PromotionIdCalculator.Product();
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

					promotion_id_calculator.products = products;
					promotion_id_calculator.productsAreInclusive = true;
					promotion_id_calculator.skus = new string[] { };
					promotion_id_calculator.skusAreInclusive = true;
					promotion_id_calculator.collections1BuyTogether = new string[] { };
					promotion_id_calculator.collections2BuyTogether = new string[] { };
					promotion_id_calculator.minimumQuantityBuyTogether = 0;
					promotion_id_calculator.quantityToAffectBuyTogether = 0;
					promotion_id_calculator.enableBuyTogetherPerSku = false;
					promotion_id_calculator.listSku1BuyTogether = new string[] { };
					promotion_id_calculator.listSku2BuyTogether = new string[] { };
					promotion_id_calculator.coupon = new string[] { };
					promotion_id_calculator.totalValueFloor = 0;
					promotion_id_calculator.totalValueCeling = 0;
					promotion_id_calculator.totalValueMode = "IncludeMatchedItems";
					promotion_id_calculator.collections = new string[] { };
					promotion_id_calculator.collectionsIsInclusive = true;
					promotion_id_calculator.restrictionsBins = new string[] { };
					promotion_id_calculator.cardIssuers = new string[] { };
					promotion_id_calculator.totalValuePurchase = 0.0f;
					promotion_id_calculator.slasIds = new string[] { };
					promotion_id_calculator.isSlaSelected = false;
					promotion_id_calculator.isFirstBuy = false;
					promotion_id_calculator.firstBuyIsProfileOptimistic = true;
					promotion_id_calculator.compareListPriceAndPrice = false;
					promotion_id_calculator.isDifferentListPriceAndPrice = false;
					promotion_id_calculator.zipCodeRanges = new string[] { };
					promotion_id_calculator.itemMaxPrice = 0.0f;
					promotion_id_calculator.itemMinPrice = 0.0f;
					promotion_id_calculator.isMinMaxInstallments = false;
					promotion_id_calculator.minInstallment = 0;
					promotion_id_calculator.maxInstallment = 0;
					promotion_id_calculator.merchants = new string[] { };
					promotion_id_calculator.clusterExpressions = new string[] { };
					promotion_id_calculator.piiClusterExpressions = new string[] { };
					promotion_id_calculator.clusterOperator = "all";
					promotion_id_calculator.paymentsRules = new string[] { };
					promotion_id_calculator.giftListTypes = new string[] { };
					promotion_id_calculator.productsSpecifications = new string[] { };
					promotion_id_calculator.affiliates = new string[] { };
					promotion_id_calculator.maxUsage = 0;
					promotion_id_calculator.maxUsagePerClient = 0;
					promotion_id_calculator.shouldDistributeDiscountAmongMatchedItems = false;
					promotion_id_calculator.multipleUsePerClient = false;
					promotion_id_calculator.accumulateWithManualPrice = false;
					promotion_id_calculator.type = "regular";
					promotion_id_calculator.useNewProgressiveAlgorithm = false;
					promotion_id_calculator.percentualDiscountValueList = new int[] { };

					if (products.Count > 0)
					{
						string respuesta = "";
						try
						{
							Task<string> task_create_update_promotion = Task.Run(() => VTEXCreateUpdatePromotionIdcalculator(promotion_id_calculator, master));
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
					promotion = new Models.Promotion();
					promotion.name = name;
					promotion.description = name;// rows[0]["description"].ToString().Trim();
					promotion.beginDateUtc = beginDateUtc;
					promotion.endDateUtc = endDateUtc;
					promotion.lastModified = lastModified;
					promotion.daysAgoOfPurchases = 0;
					promotion.isActive = true;
					promotion.isArchived = false;
					promotion.isFeatured = true;
					promotion.disableDeal = false;
					promotion.offset = -5;
					promotion.activateGiftsMultiplier = false;
					promotion.newOffset = -5.0f;
					promotion.maxPricesPerItems = new string[] { };
					promotion.cumulative = false;
					promotion.nominalShippingDiscountValue = 0.0f;
					promotion.absoluteShippingDiscountValue = 0.0f;

					float percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
					float nominalDiscountValue = Convert.ToSingle(rows[0]["nominalDiscountValue"]);
					if (nominalDiscountValue > 0)
					{
						promotion.discountType = "nominal";
						promotion.nominalDiscountValue = Convert.ToSingle(rows[0]["nominalDiscountValue"]);
						promotion.percentualDiscountValue = 0.0f;
					}
					if (percentualDiscountValue > 0)
					{
						promotion.discountType = "percentual";
						promotion.nominalDiscountValue = 0.0f;
						promotion.percentualDiscountValue = Convert.ToSingle(rows[0]["percentualDiscountValue"]);
					}

					promotion.nominalDiscountType = "cart";
					promotion.maximumUnitPriceDiscount = 0.0f;
					promotion.rebatePercentualDiscountValue = 0.0f;
					promotion.percentualShippingDiscountValue = 0.0f;
					promotion.percentualTax = 0.0f;
					promotion.shippingPercentualTax = 0.0f;
					promotion.percentualDiscountValueList1 = 0.0f;
					promotion.percentualDiscountValueList2 = 0.0f;
					promotion.skusGift = new Promotion.SkusGift() { quantitySelectable = 1, gifts = new object[] { } };
					promotion.nominalRewardValue = 0.0f;
					promotion.percentualRewardValue = 0.0f;
					promotion.orderStatusRewardValue = "invoiced";
					promotion.maxNumberOfAffectedItems = 0;
					promotion.maxNumberOfAffectedItemsGroupKey = "perCart";
					promotion.applyToAllShippings = false;
					promotion.nominalTax = 0.0f;
					if (master == true)
						promotion.origin = "Marketplace";
					else
						promotion.origin = "Fulfillment";
					promotion.idSellerIsInclusive = true;
					promotion.idsSalesChannel = new string[1] { "1" };//new string[] { };
					promotion.areSalesChannelIdsExclusive = false;
					promotion.marketingTags = new string[] { };
					promotion.marketingTagsAreNotInclusive = false;
					promotion.paymentsMethods = new string[] { };
					promotion.stores = new string[] { };
					promotion.campaigns = new string[] { };
					promotion.storesAreInclusive = true;//nuevo
					promotion.categories = new string[] { };
					promotion.categoriesAreInclusive = true;
					promotion.brands = new string[] { };
					promotion.brandsAreInclusive = true;

					List<Promotion.Product> products = new List<Promotion.Product>();

					foreach (DataRow row in rows)
					{
						string product_id = "";
						try
						{
							Promotion.Product product = new Promotion.Product();
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

					promotion.products = products;
					promotion.productsAreInclusive = true;
					promotion.skus = new string[] { };
					promotion.skusAreInclusive = true;
					promotion.collections1BuyTogether = new string[] { };
					promotion.collections2BuyTogether = new string[] { };
					promotion.minimumQuantityBuyTogether = 0;
					promotion.quantityToAffectBuyTogether = 0;
					promotion.enableBuyTogetherPerSku = false;
					promotion.listSku1BuyTogether = new string[] { };
					promotion.listSku2BuyTogether = new string[] { };
					promotion.coupon = new string[] { };
					promotion.totalValueFloor = 0;
					promotion.totalValueCeling = 0;
					promotion.totalValueMode = "IncludeMatchedItems";
					promotion.collections = new string[] { };
					promotion.collectionsIsInclusive = true;
					promotion.restrictionsBins = new string[] { };
					promotion.cardIssuers = new string[] { };
					promotion.totalValuePurchase = 0.0f;
					promotion.slasIds = new string[] { };
					promotion.isSlaSelected = false;
					promotion.isFirstBuy = false;
					promotion.firstBuyIsProfileOptimistic = true;
					promotion.compareListPriceAndPrice = false;
					promotion.isDifferentListPriceAndPrice = false;
					promotion.zipCodeRanges = new string[] { };
					promotion.itemMaxPrice = 0.0f;
					promotion.itemMinPrice = 0.0f;
					promotion.isMinMaxInstallments = false;
					promotion.minInstallment = 0;
					promotion.maxInstallment = 0;
					promotion.merchants = new string[] { };
					promotion.clusterExpressions = new string[] { };
					promotion.piiClusterExpressions = new string[] { };
					promotion.clusterOperator = "all";
					promotion.paymentsRules = new string[] { };
					promotion.giftListTypes = new string[] { };
					promotion.productsSpecifications = new string[] { };
					promotion.affiliates = new string[] { };
					promotion.maxUsage = 0;
					promotion.maxUsagePerClient = 0;
					promotion.shouldDistributeDiscountAmongMatchedItems = false;
					promotion.multipleUsePerClient = false;
					promotion.accumulateWithManualPrice = false;
					promotion.type = "regular";
					promotion.useNewProgressiveAlgorithm = false;
					promotion.percentualDiscountValueList = new int[] { };
					if (products.Count > 0)
					{
						string respuesta = "";
						try
						{
							Task<string> task_create_update_promotion = Task.Run(() => VTEXCreateUpdatePromotion(promotion, master));
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
