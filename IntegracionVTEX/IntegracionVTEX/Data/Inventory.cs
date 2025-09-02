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
using static System.Collections.Specialized.BitVector32;

namespace IntegracionVTEX.Data
{
	public class Inventory
	{
		Configuration config = null;
		AppSettingsSection section = null;

		int loop_size = 0;
		int pause = 0;

		public Inventory()
		{
			config = ConfigurationManager.OpenExeConfiguration(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
			section = config.AppSettings;
			loop_size = Convert.ToInt32(section.Settings["loop_size"].Value);
			pause = Convert.ToInt32(section.Settings["pause"].Value);
		}
		#region FUNCIONES VTEX
		public async Task<string> VTEXGetSkuIdByReferenceId(string ref_id)
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
				throw new Exception("Error al obtener producto: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al obtener producto: " + ex.Message);
			}
			return res;
		}

		private async Task<string> VTEXUpdateInventoryBySkuAndWarehouse(Models.SKU.Inventory inventory, int sku_id, string warehouse_id)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlUpdateInventory;
				string json = JsonConvert.SerializeObject(inventory);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}{sku_id}/warehouses/{warehouse_id}"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppToken },
				},
					Method = HttpMethod.Put,
					Content = data,
				};
				using (var response = await client.SendAsync(request))
				{
					response.EnsureSuccessStatusCode();
					res = await response.Content.ReadAsStringAsync();
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al actualizar inventario: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al actualizar inventario: " + ex.Message);
			}
			return res;
		}
		#endregion

		#region FUNCIONES DE CONSULTA, INSERCION Y ACTUALIZACION
		public DataTable GetProductList(string items = "")
		{
			DataTable res = null;
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = File.ReadAllText("Data\\InventoryQuery.txt");
				if (!items.Equals(string.Empty))
				{
					SQL += $" and f120_id in({items})";
				}
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["unoee"].ConnectionString);
				conn.Open();
				SqlCommand cmd = new SqlCommand(SQL, conn);
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 600;
				cmd.Parameters.AddWithValue("@id_instalacion", Configuracion.CentroOperacion);
				cmd.Parameters.AddWithValue("@id_portafolio ", Configuracion.Portafolio);
				cmd.Parameters.AddWithValue("@id_cia", id_cia);
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

		public int UpdateInventory(DataRow row)
		{
			int sku_id = -1;
			string ref_id = "";
			int total = 0;
			try
			{
				ref_id = row["refid"].ToString().Trim();
				Models.SKU.Inventory inventory = null;

				Task<string> task_get_sku = Task.Run(() => VTEXGetSkuIdByReferenceId(ref_id));
				task_get_sku.Wait();
				dynamic results_get_sku = JsonConvert.DeserializeObject<dynamic>(task_get_sku.Result);
				task_get_sku.Dispose();

				string rta = Convert.ToString(results_get_sku);

				if (int.TryParse(rta, out sku_id))
				{
					inventory = new Models.SKU.Inventory();
					inventory.unlimitedQuantity = Convert.ToBoolean(row["unlimitedQuantity"]);
					inventory.dateUtcOnBalanceSystem = null;
					inventory.quantity = Convert.ToInt32(row["quantity"]);

					Task<string> task_update_inventory = Task.Run(() => VTEXUpdateInventoryBySkuAndWarehouse(inventory, sku_id, Configuracion.CentroOperacion));
					task_update_inventory.Wait();

					string respuesta = Convert.ToString(task_update_inventory.Result);
					try
					{
						dynamic results_update_inventory = JsonConvert.DeserializeObject<dynamic>(respuesta);
						total++;
					}
					catch (Exception)
					{
						Auxiliary.SaveResultUpdateInventory(ref_id, respuesta);
					}
					task_update_inventory.Dispose();
				}
				else
				{
					Auxiliary.SaveResultUpdateInventory($"[{ref_id}]", rta);
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				Auxiliary.SaveResultUpdateInventory(ref_id, errores.Trim().Trim(','));
			}
			catch (Exception ex)
			{
				Auxiliary.SaveResultUpdateInventory(ref_id, ex.Message);
			}
			return total;
		}

		public string ObtenerItemsError()
		{
			string SQL = @"select 
								f03_ref_id
							from
								t03_log_update_inventory
							where
								CONVERT(date,f03_fecha)=CONVERT(date,GETDATE())
								and ISNUMERIC(f03_ref_id)=1
								and f03_co=@id_instalacion
								and f03_portafolio=@id_portafolio";
			string res = "";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@id_instalacion", Configuracion.CentroOperacion);
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
				throw new Exception($"Error al obtener items error update inventory: {ex.Message}");
			}
			return res.Trim(',');
		}
		#endregion
	}
}
