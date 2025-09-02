using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using static IntegracionVTEX.Models.SKU;

namespace IntegracionVTEX.Data
{
	public class Price
	{
		Configuration config = null;
		AppSettingsSection section = null;

		int loop_size = 0;
		int pause = 0;
		public Price()
		{
			config = ConfigurationManager.OpenExeConfiguration(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
			section = config.AppSettings;

			loop_size = Convert.ToInt32(section.Settings["loop_size"].Value);
			pause = Convert.ToInt32(section.Settings["pause"].Value);
		}
		#region FUNCIONES VTEX
		private async Task<string> VTEXCreateUpdateBasePriceFixedPrices(Models.Price price, int sku)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlCreateUpdateBasePrice;
				string json = JsonConvert.SerializeObject(price);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri + sku),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppToken},
				},
					Method = HttpMethod.Put,
					Content = data,
				};
				using (var response = await client.SendAsync(request))
				{
					//response.EnsureSuccessStatusCode();
					res = await response.Content.ReadAsStringAsync();
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al crear precio: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al crear precio: " + ex.Message);
			}
			return res;
		}

		private async Task<string> VTEXDeletePrice(int sku)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlDeletePrice;
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri + sku),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKeyMaster } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppTokenMaster},
				},
					Method = HttpMethod.Delete,
				};
				using (var response = await client.SendAsync(request))
				{
					//response.EnsureSuccessStatusCode();
					res = await response.Content.ReadAsStringAsync();
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al eliminar precio: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al eliminar precio: " + ex.Message);
			}
			return res;
		}

		private async Task<string> GetSkuIdByReferenceId(string ref_id)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlGetSkuRefId;
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}{ref_id}"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKeyMaster },
					{ "X-VTEX-API-AppToken", Configuracion.AppTokenMaster },
				},
					Method = HttpMethod.Get,
				};
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                using (var response = await client.SendAsync(request))
				{
					//response.EnsureSuccessStatusCode();
					res = await response.Content.ReadAsStringAsync();
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al obtener sku producto: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al obtener sku producto: " + ex.Message);
			}
			return res;
		}

		#endregion

		#region FUNCIONES DE CONSULTA, INSERCION Y ACTUALIZACION
		public DataTable GetPriceList(string items = "")
		{
			DataTable res = null;
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = File.ReadAllText("Data\\PriceQuery.txt");
				if (!items.Equals(string.Empty))
				{
					SQL += $" and f120_id in({items})";
				}
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["unoee"].ConnectionString);
				conn.Open();
				SqlCommand cmd = new SqlCommand(SQL, conn);
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 600;
				cmd.Parameters.AddWithValue("@id_cia", id_cia);
				cmd.Parameters.AddWithValue("@id_instalacion", Configuracion.CentroOperacion);
				cmd.Parameters.AddWithValue("@id_lista_precios", Configuracion.ListaPrecios);
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
				throw new Exception("Error al consultar precios: " + ex.Message);
			}
			return res;
		}

		public int CreateUpdateBasePriceFixedPrices(DataRow row)
		{
			int sku_id = -1;
			string ref_id = "";
			int total = 0;
			try
			{
				ref_id = row["RefId"].ToString().Trim();
				Models.Price price = null;
				Models.Price.FixedPrice fixed_price = null;

				Task<string> task_get_sku_id = Task.Run(() => GetSkuIdByReferenceId(ref_id));
				task_get_sku_id.Wait();
				dynamic results_get_sku_id = JsonConvert.DeserializeObject<dynamic>(task_get_sku_id.Result);
				task_get_sku_id.Dispose();
				string rta = Convert.ToString(results_get_sku_id);

				if (int.TryParse(rta, out sku_id))
				{
					price = new Models.Price();
					price.markup = Convert.ToInt32(row["markup"]);
					price.listPrice = null;
					price.costPrice = Convert.ToSingle(row["costPrice"]);

					Models.Price.FixedPrice[] fixed_prices = new Models.Price.FixedPrice[1];
					fixed_price = new Models.Price.FixedPrice();
					fixed_price.tradePolicyId = Convert.ToString(row["tradePolicyId"]);
					fixed_price.value = Convert.ToSingle(row["value"]);
					fixed_price.listPrice = null;
					fixed_price.minQuantity = Convert.ToInt32(row["minQuantity"]);
					fixed_prices[0] = fixed_price;
					price.fixedPrices = fixed_prices;

					Task<string> task_create_update_base_price = Task.Run(() => VTEXCreateUpdateBasePriceFixedPrices(price, sku_id));
					task_create_update_base_price.Wait();
					string respuesta = Convert.ToString(task_create_update_base_price.Result);
					try
					{
						dynamic results_create_update_base_price = JsonConvert.DeserializeObject<dynamic>(respuesta);
						total++;
					}
					catch (Exception)
					{
						Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices(ref_id, respuesta);
					}

					task_create_update_base_price.Dispose();
				}
				else
				{
					Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices($"[{ref_id}]", rta);
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices(ref_id, errores.Trim().Trim(','));
			}
			catch (Exception ex)
			{
				Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices(ref_id, ex.Message);
			}
			return total;
		}

		public void DeletePrice(DataRow row)
		{
			int sku_id = -1;
			string ref_id = "";

			try
			{
				ref_id = row["RefId"].ToString().Trim();

				Task<string> task_get_sku_id = Task.Run(() => GetSkuIdByReferenceId(ref_id));
				task_get_sku_id.Wait();
				dynamic results_get_sku_id = JsonConvert.DeserializeObject<dynamic>(task_get_sku_id.Result);
				task_get_sku_id.Dispose();
				string id = Convert.ToString(results_get_sku_id);

				if (int.TryParse(id, out sku_id))
				{
					Task<string> task_delete_price = Task.Run(() => VTEXDeletePrice(sku_id));
					task_delete_price.Wait();
					string respuesta = Convert.ToString(task_delete_price.Result);
					try
					{
						dynamic results_delete_price = JsonConvert.DeserializeObject<dynamic>(respuesta);
					}
					catch (Exception)
					{
						Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices($"{ref_id}", $"{respuesta}");
					}
					
					task_delete_price.Dispose();
				}
				else
				{
					Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices($"[{ref_id}]", $"DELETE: {id}");
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices(ref_id, $"DELETE: {errores.Trim().Trim(',')}");
			}
			catch (Exception ex)
			{
				Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices(ref_id, $"DELETE: {ex.Message}");
			}
		}

		public string ObtenerItemsError()
		{
			string SQL = @"select 
							 f06_ref_id
						from
							t06_log_create_price
						where
							CONVERT(date,f06_fecha)=CONVERT(date,GETDATE())
							and ISNUMERIC(f06_ref_id)=1
							and f06_co=@id_instalacion
							and f06_lista_precios=@id_lista_precios
							and f06_portafolio=@id_portafolio";
			string res = "";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@id_instalacion", Configuracion.CentroOperacion);
				cmd.Parameters.AddWithValue("@id_lista_precios", Configuracion.ListaPrecios);
				cmd.Parameters.AddWithValue("@id_portafolio", Configuracion.Portafolio);
				SqlDataReader reader = cmd.ExecuteReader();
				if (reader.HasRows)
				{
					while (reader.Read())
					{
						res += $"'{reader.GetString(0)}',";
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Error al obtener items error create price: {ex.Message}");
			}
			return res.Trim(',');
		}
		#endregion
	}
}
