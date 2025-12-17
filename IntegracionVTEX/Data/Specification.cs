using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Serialization;

namespace IntegracionVTEX.Data
{
	public class Specification
	{
		Configuration config = null;
		AppSettingsSection section = null;

		int loop_size = 0;
		int pause = 0;

		public Specification()
		{
			config = ConfigurationManager.OpenExeConfiguration(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
			section = config.AppSettings;
			loop_size = Convert.ToInt32(section.Settings["loop_size"].Value);
			pause = Convert.ToInt32(section.Settings["pause"].Value);
		}
		#region FUNCIONES VTEX
		private async Task<string> VTEXUpdateProductSpecification(List<Models.Specification> specifications, string product_id)
		{
			string res = null;
			string status_code = "";
			try
			{
				string uri = Configuracion.UrlUpdateProductSpecification;
				string json = JsonConvert.SerializeObject(specifications);

				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}{product_id}/specification"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKeyMaster } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppTokenMaster },
					//{ "Retry-After","3600"}
				},
					Method = HttpMethod.Post,
					Content = data,
				};

				using (HttpResponseMessage response = await client.SendAsync(request))
				{
					status_code = response.StatusCode.ToString();
					response.EnsureSuccessStatusCode();
					res = await response.Content.ReadAsStringAsync();
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al actualizar especificación de productos: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al actualizar especificación de productos: " + ex.Message);
			}

			return res;
		}

		private async Task<string> VTEXGetProductByRefId(string ref_id)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlGetProductRefId;

				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}{ref_id}"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKeyMaster } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppTokenMaster },
				},
					Method = HttpMethod.Get,
				};

				using (var response = await client.SendAsync(request))
				{
					//response.EnsureSuccessStatusCode();//inicia una excepción si es false.
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
		public DataTable GetSpecifications()
		{
			DataTable res = null;
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = File.ReadAllText("Data\\SpecificationQuery.txt");
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["unoee"].ConnectionString);
				conn.Open();
				SqlCommand cmd = new SqlCommand(SQL, conn);
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 600;
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
				throw new Exception("Error al consultar especificación de productos: " + ex.Message);
			}
			return res;
		}

		public void UpdateProductSpecification(DataRow row)
		{
			try
			{
				string ref_id = row["RefId"].ToString().Trim();
				int product_id = -1;

				try
				{
					Task<string> task_get = Task.Run(() => VTEXGetProductByRefId(ref_id));
					task_get.Wait();
					dynamic results_get = JsonConvert.DeserializeObject<dynamic>(task_get.Result);
					task_get.Dispose();

					if (results_get != null)
					{
						if (Convert.ToString(results_get) != $"Product not found by refId: {ref_id}")
						{
							string id = Convert.ToString(results_get.Id);

							if (int.TryParse(id, out product_id))
							{
								List<Models.Specification> specifications = new List<Models.Specification>();

								Models.Specification specification = new Models.Specification();
								string name = "Unidad de medida";
								string um = row["Unidad de medida"].ToString().Trim().ToLower();
								string[] value = new string[1] { $"{um.FirstOrDefault().ToString().ToUpper()}{um.Substring(1)}" };
								specification.Name = name;
								specification.Value = value;
								specifications.Add(specification);

								specification = new Models.Specification();
								name = "Factor conversión";
								value = new string[1] { row["Factor conversión"].ToString().Trim() };
								specification.Name = name;
								specification.Value = value;
								specifications.Add(specification);

								Task<string> task_update_product_specification = Task.Run(() => VTEXUpdateProductSpecification(specifications, product_id.ToString()));
								task_update_product_specification.Wait();
								dynamic results_update_product_specification = JsonConvert.DeserializeObject<dynamic>(task_update_product_specification.Result);
								task_update_product_specification.Dispose();
							}
						}
						else
						{
							Auxiliary.SaveResultUpdateProductSpecification(ref_id, "Producto no encontrado");
							
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
					Auxiliary.SaveResultUpdateProductSpecification(ref_id, errores.Trim().Trim(','));
				}
				catch (Exception ex)
				{
					Auxiliary.SaveResultUpdateProductSpecification(ref_id, ex.Message);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error al actualizar especificación de productos: " + ex.Message);
			}
		}

		#endregion
	}
}
