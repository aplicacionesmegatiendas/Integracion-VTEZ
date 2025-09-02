using IntegracionVTEX.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Xml.Linq;

namespace IntegracionVTEX.Data
{
	public class Collection
	{
		Configuration config = null;
		AppSettingsSection section = null;

		int loop_size = 0;
		int pause = 0;

		public Collection()
		{
			config = ConfigurationManager.OpenExeConfiguration(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
			section = config.AppSettings;

			loop_size = Convert.ToInt32(section.Settings["loop_size"].Value);
			pause = Convert.ToInt32(section.Settings["pause"].Value);
		}

		#region FUNCIONES XTEX
		private async Task<string> VTEXGetProductsFromCollection(int colecccion_id, int pagina)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlGetProductsFromCollection;
				HttpClient client = new HttpClient();
				string request_uri = String.Format(uri, colecccion_id, pagina);
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(request_uri),
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
			catch (Exception ex)
			{
				throw new Exception("Error al obtener listado de ordenes: " + ex.Message);
			}
			return res;
		}

		private async Task VTEXDeleteSkuFromSubcollection(int colecccion_id, int sku)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlDeleteSkuFromSubcollection;
				HttpClient client = new HttpClient();
				string request_uri = String.Format(uri, colecccion_id, sku);
				HttpRequestMessage request = new HttpRequestMessage
				{

					RequestUri = new Uri(request_uri),
					Headers =
				 {
					 { "Accept", "application/json" },
					 { "X-VTEX-API-AppKey", Configuracion.AppKeyMaster },
					 { "X-VTEX-API-AppToken", Configuracion.AppTokenMaster },
				 },
					Method = HttpMethod.Delete,
				};

				using (var response = await client.SendAsync(request))
				{
					response.EnsureSuccessStatusCode();
					//res = await response.Content.ReadAsStringAsync();
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al borrar sku: " + ex.Message + ":" + res);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al borrar sku: " + ex.Message + ":" + res);
			}
		}
		#endregion

		#region FUNCIONES DE CONSULTA, INSERCION Y ACTUALIZACION
		public int RunDeleteSkuFromSubcollectionProcess(int colecccion_id)
		{
			int subcollection_id = -1;
			try
			{
				Task<string> task_get_products = Task.Run(() => VTEXGetProductsFromCollection(colecccion_id, 1));
				task_get_products.Wait();
				dynamic results_get_products = JsonConvert.DeserializeObject<dynamic>(task_get_products.Result);
				task_get_products.Dispose();
				int pages = Convert.ToInt32(results_get_products.TotalPage);

				foreach (dynamic result in results_get_products.Data)
				{
					int sku_id = -1;

					try
					{

						sku_id = Convert.ToInt32(result.SkuId);
						subcollection_id = Convert.ToInt32(result.SubCollectionId);
						Task task_delete = Task.Run(() => VTEXDeleteSkuFromSubcollection(subcollection_id, sku_id));
						task_delete.Wait();
					}
					catch (AggregateException ex)
					{
						string errores = "";
						foreach (Exception item in ex.InnerExceptions)
						{
							errores += item.Message + ", ";
						}
						Auxiliary.SaveResultDeleteSkuSubcollection(subcollection_id, sku_id, errores.Trim().Trim(','));
						continue;
					}
					catch (Exception ex)
					{
						Auxiliary.SaveResultDeleteSkuSubcollection(subcollection_id, sku_id, ex.Message);
						continue;
					}
				}
				if (pages > 1)
				{
					for (int i = 2; i < pages + 1; i++)
					{
						Task<string> task_get_products_page = Task.Run(() => VTEXGetProductsFromCollection(colecccion_id, i));
						task_get_products_page.Wait();
						dynamic results_get_products_page = JsonConvert.DeserializeObject<dynamic>(task_get_products_page.Result);
						task_get_products_page.Dispose();

						foreach (dynamic result in results_get_products_page.Data)
						{
							int sku_id = -1;
							try
							{
								sku_id = Convert.ToInt32(result.SkuId);
								subcollection_id = Convert.ToInt32(result.SubCollectionId);
								Task task_delete = Task.Run(() => VTEXDeleteSkuFromSubcollection(subcollection_id, sku_id));
								task_delete.Wait();
							}
							catch (AggregateException ex)
							{
								string errores = "";
								foreach (Exception item in ex.InnerExceptions)
								{
									errores += item.Message + ", ";
								}
								Auxiliary.SaveResultDeleteSkuSubcollection(subcollection_id, sku_id, errores.Trim().Trim(','));
								continue;
							}
							catch (Exception ex)
							{
								Auxiliary.SaveResultDeleteSkuSubcollection(subcollection_id, sku_id, ex.Message);
								continue;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error al correr proceso de eliminar sku coleccion: " + ex.Message);
			}
			return subcollection_id;
		}

		public string GetProductsDiscount(string portafolio, string plan = "", string criterio = "")
		{
			string res = "";
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = "";
				if (!(plan.Equals(string.Empty) && criterio.Equals(string.Empty)))
					SQL = File.ReadAllText("Data\\ProductsDiscountPlanCriterioQuery.txt");
				else
					SQL = File.ReadAllText("Data\\ProductsDiscountQuery.txt");

				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["unoee"].ConnectionString);
				conn.Open();
				SqlCommand cmd = new SqlCommand(SQL, conn);
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 600;
				cmd.Parameters.AddWithValue("@id_cia", id_cia);
				cmd.Parameters.AddWithValue("@id_portafolio", portafolio);
				if (!(plan.Equals(string.Empty) && criterio.Equals(string.Empty)))
				{
					cmd.Parameters.AddWithValue("@id_plan", plan);
					cmd.Parameters.AddWithValue("@id_criterio_mayor", criterio);
				}
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				if (reader.HasRows)
				{
					while (reader.Read())
					{
						res += $"{reader.GetInt32(0)},";
					}
				}
				reader.Close();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al consultar productos descuento: " + ex.Message);
			}
			return res.Trim(',');
		}
		#endregion
	}
}
