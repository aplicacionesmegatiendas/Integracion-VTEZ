using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IntegracionVTEX.Models;
using static IntegracionVTEX.Models.SKU;
using System.Security.Cryptography;

namespace IntegracionVTEX.Data
{
	public class SKU
	{
		Configuration config = null;
		AppSettingsSection section = null;

		int loop_size = 0;
		int pause = 0;

		public SKU()
		{
			config = ConfigurationManager.OpenExeConfiguration(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
			section = config.AppSettings;

			loop_size = Convert.ToInt32(section.Settings["loop_size"].Value);
			pause = Convert.ToInt32(section.Settings["pause"].Value);
		}

		#region FUNCIONES VTEX
		private async Task<string> VTEXCreateSku(Models.SKU.SkuWithoutId sku)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlCreateSku;
				string json = JsonConvert.SerializeObject(sku);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppToken },
				},
					Method = HttpMethod.Post,
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
				throw new Exception("Error al crear sku: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al crear sku: " + ex.Message);
			}
			return res;
		}

		private async Task<string> VTEXCreateEanSku(int sku_id, string ean)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlCreateEanSku;
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}{sku_id}/ean/{ean}"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppToken },
				},
					Method = HttpMethod.Post
				};
				using (var response = await client.SendAsync(request))
				{
					response.EnsureSuccessStatusCode();
					res = await response.Content.ReadAsStringAsync();
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al crear ean sku: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al crear ean sku: " + ex.Message);
			}
			return res;
		}

		private async Task<string> VTEXAddSkuSubcollection(SkuToSubcollection sku, int collection, bool master)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlAddSkuSubCollection;
				string json = JsonConvert.SerializeObject(sku);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}{collection}/stockkeepingunit"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", master==true?Configuracion.AppKeyMaster: Configuracion.AppKey } ,
					{ "X-VTEX-API-AppToken", master==true?Configuracion.AppTokenMaster: Configuracion.AppToken },
				},
					Method = HttpMethod.Post,
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
				throw new Exception($"Error al agregar sku {sku} a colección {collection}: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error al agregar sku {sku} a colección {collection}: " + ex.Message);
			}
			return res;
		}

		private async Task<string> VTEXCreateSkuFile(SkuFile file, int sku_id)
		{
			string res = null;
			try
			{
				string uri = $"https://megatiendas.vtexcommercestable.com.br/api/catalog/pvt/stockkeepingunit/{sku_id}/file"; 
				//string uri = Configuracion.UrlAddSkuSubCollection;
				string json = JsonConvert.SerializeObject(file);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					//RequestUri = new Uri($"{uri}{sku_id}/stockkeepingunit"),
					RequestUri = new Uri($"{uri}"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppToken },
				},
					Method = HttpMethod.Post,
					Content = data,
				};
				using (var response = await client.SendAsync(request))
				{
					//response.EnsureSuccessStatusCode();
					//res = await response.Content.ReadAsStringAsync();
					string rta = await response.Content.ReadAsStringAsync();
					res = $"{response.StatusCode}|{rta}";
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception($"Error al crear file de sku {sku_id}: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error al crear file de sku {sku_id}: " + ex.Message);
			}
			return res;
		}

		private async Task<string[]> VTEXGetProductByRefId(string ref_id)
		{
			string[] res = new string[2];
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
					//res = await response.Content.ReadAsStringAsync();
					string rta = await response.Content.ReadAsStringAsync();
					res[0] = response.StatusCode.ToString();
					res[1] = rta;
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

		private async Task<string> VTEXGetSkuIdByReferenceId(string ref_id)
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

		#endregion

		#region FUNCIONES DE CONSULTA, INSERCION Y ACTUALIZACION
		public DataTable GetProductSkuList(string portafolio, string items = "")
		{
			DataTable res = null;
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = File.ReadAllText("Data\\SkuQuery.txt");
				if (!items.Equals(""))
					SQL += $" and f120_id in({items})";
				SQL += @"order by
							5,6;";
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["unoee"].ConnectionString);
				conn.Open();
				SqlCommand cmd = new SqlCommand(SQL, conn);
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 600;
				cmd.Parameters.AddWithValue("@id_cia", id_cia);
				cmd.Parameters.AddWithValue("@id_portafolio", portafolio);
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
				throw new Exception("Error al consultar productos sku: " + ex.Message);
			}
			return res;
		}

		private int GetCollectionByRefid(string portafolio, string referencia)
		{
			int res = -1;
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = File.ReadAllText("Data\\SubcollectionQuery.txt");
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["unoee"].ConnectionString);
				conn.Open();
				SqlCommand cmd = new SqlCommand(SQL, conn);
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 600;
				cmd.Parameters.AddWithValue("@id_cia", id_cia);
				cmd.Parameters.AddWithValue("@id_portafolio", portafolio);
				cmd.Parameters.AddWithValue("@referencia", referencia);
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				if (reader.HasRows)
				{
					reader.Read();
					res = Convert.ToInt32(reader["subcollection"]);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al consultar colección por referencia: " + ex.Message);
			}
			return res;
		}

		public void CreateSku(string portafolio)
		{
			try
			{
				DataTable data = GetProductSkuList(portafolio);
				if (data != null)
				{
					int ciclo = 1;
					for (int i = 0; i < data.Rows.Count; i += loop_size)
					{
						Console.WriteLine($"Ciclo {ciclo.ToString()}...");
						for (int j = i; j < i + loop_size && j < data.Rows.Count; j++)
						{
							Models.SKU.SkuWithoutId sku = null;
							try
							{
								Task<string[]> task_get = Task.Run(() => VTEXGetProductByRefId(data.Rows[j]["RefId"].ToString().Trim()));
								task_get.Wait();
								dynamic results_get = JsonConvert.DeserializeObject<dynamic>(task_get.Result[1]);
								task_get.Dispose();

								int product_id = -1;
								if (results_get != null)
								{
									if (Convert.ToString(results_get.Id) != "")
									{
										product_id = Convert.ToInt32(results_get.Id);
									}
									else
									{
										Auxiliary.SaveResultCreateSku(data.Rows[j]["RefId"].ToString().Trim(), "No hay producto registrado con esta referencia");
										continue;
									}
								}
								else
								{
									Auxiliary.SaveResultCreateSku(data.Rows[j]["RefId"].ToString().Trim(), "No hay producto registrado con esta referencia");
									continue;
								}

								sku = new Models.SKU.SkuWithoutId();
								sku.ProductId = product_id;
								sku.IsActive = false;
								sku.ActivateIfPossible = Convert.ToBoolean(data.Rows[j]["ActivateIfPossible"]);
								sku.Name = Convert.ToString(data.Rows[j]["Name"]).Trim();
								sku.RefId = Convert.ToString(data.Rows[j]["RefId"]).Trim();
								sku.Ean = null;
								sku.PackagedHeight = Convert.ToSingle(data.Rows[j]["PackagedHeight"]);
								sku.PackagedLength = Convert.ToSingle(data.Rows[j]["PackagedLength"]);
								sku.PackagedWidth = Convert.ToSingle(data.Rows[j]["PackagedWidth"]);
								sku.PackagedWeightKg = Convert.ToInt32(data.Rows[j]["PackagedWidth"]);

								if (Convert.IsDBNull(data.Rows[j]["Height"]))
									sku.Height = null;
								else
									sku.Height = Convert.ToSingle(data.Rows[j]["Height"]);

								if (Convert.IsDBNull(data.Rows[j]["Length"]))
									sku.Length = null;
								else
									sku.Length = Convert.ToSingle(data.Rows[j]["Length"]);

								if (Convert.IsDBNull(data.Rows[j]["Width"]))
									sku.Width = null;
								else
									sku.Width = Convert.ToSingle(data.Rows[j]["Width"]);

								if (Convert.IsDBNull(data.Rows[j]["WeightKg"]))
									sku.WeightKg = null;
								else
									sku.WeightKg = Convert.ToSingle(data.Rows[j]["WeightKg"]);

								sku.CubicWeight = Convert.ToSingle(data.Rows[j]["CubicWeight"]);
								sku.IsKit = Convert.ToBoolean(data.Rows[j]["IsKit"]);
								sku.CreationDate = DateTime.Now.Date.ToString("yyyy-MM-dd") + "T00:00:00";
								sku.RewardValue = null;
								sku.EstimatedDateArrival = null;
								sku.ManufacturerCode = null;
								sku.CommercialConditionId = 1;
								sku.MeasurementUnit = Convert.ToString(data.Rows[j]["MeasurementUnit"]).Trim();
								sku.UnitMultiplier = Convert.ToSingle(data.Rows[j]["UnitMultiplier"]);
								sku.ModalType = null;
								sku.KitItensSellApart = Convert.ToBoolean(data.Rows[j]["KitItensSellApart"]);
								sku.Videos = new string[0];

								//CREA EL SKU
								Task<string> task_create_sku = Task.Run(() => VTEXCreateSku(sku));
								task_create_sku.Wait();
								dynamic results_create = JsonConvert.DeserializeObject<dynamic>(task_create_sku.Result);
								task_create_sku.Dispose();

								Auxiliary.SaveResultCreateSku(sku.RefId, Convert.ToString(results_create));

								if (results_create != null)
								{
									//CREA EL EAN DEL SKU
									int sku_id = -1;
									sku_id = Convert.ToInt32(results_create.Id);
									string ean = Convert.ToString(data.Rows[j]["RefId"]).Trim();

									Task<string> task_create_ean_sku = Task.Run(() => VTEXCreateEanSku(sku_id, ean));
									task_create_ean_sku.Wait();
									dynamic results_create_ean_sku = JsonConvert.DeserializeObject<dynamic>(task_create_ean_sku.Result);
									task_create_ean_sku.Dispose();
									Auxiliary.SaveResultCreateSku(sku.RefId, $"Ean sku {sku_id.ToString()}: {Convert.ToString(results_create_ean_sku)}");

									//CREA EL SKU FILE DE LA IMAGEN
									Models.SKU.SkuFile sku_file = new Models.SKU.SkuFile();
									sku_file.IsMain= true;
									sku_file.Label = "1";
									sku_file.Name = "";
									sku_file.Text = null;
									sku_file.Url = "";

									Task<string> task_create_sku_file = Task.Run(() => VTEXCreateSkuFile(sku_file,sku_id));
									task_create_sku_file.Wait();
									dynamic results_create_sku_file = JsonConvert.DeserializeObject<dynamic>(task_create_sku_file.Result);
									task_create_sku_file.Dispose();
								}
							}
							catch (AggregateException ex)
							{
								string errores = "";
								foreach (Exception item in ex.InnerExceptions)
								{
									errores += item.Message + ", ";
								}
								Console.WriteLine(errores);
								Auxiliary.SaveResultCreateSku("", errores.Trim().Trim(','));
								continue;
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.ToString());
								Auxiliary.SaveResultCreateSku("", ex.Message);
								continue;
							}
						}
						Console.WriteLine($"Esperando {pause} segundos para seguir guardando...");
						System.Threading.Thread.Sleep(pause * 1000);
						ciclo++;
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error al crear sku: " + ex.Message);
			}
		}

		public int CreateSubcollectionSku(DataRow row, int subcollection, bool master)
		{
			int sku_id = -1;
			string ean = "";
			int total = 0;
			try
			{
				sku_id = -1;
				ean = Convert.ToString(row["RefId"]).Trim();

				Task<string> task_get_sku_id = Task.Run(() => VTEXGetSkuIdByReferenceId(ean));
				task_get_sku_id.Wait();
				dynamic results_get_sku_id = JsonConvert.DeserializeObject<dynamic>(task_get_sku_id.Result);
				task_get_sku_id.Dispose();
				string rta = Convert.ToString(results_get_sku_id);
								
				if (int.TryParse(rta, out sku_id))
				{
					if (subcollection == -1)
						subcollection = GetCollectionByRefid(Configuracion.Portafolio, ean);

					if (subcollection > 0)
					{
						SkuToSubcollection sku_to_subcollection = new SkuToSubcollection();
						sku_to_subcollection.SkuId = sku_id;

						Task<string> task_sku_collection = Task.Run(() => VTEXAddSkuSubcollection(sku_to_subcollection, subcollection, master));
						task_sku_collection.Wait();
						string respuesta = Convert.ToString(task_sku_collection.Result);
						try
						{
							dynamic results_collection = JsonConvert.DeserializeObject<dynamic>(task_sku_collection.Result);
							total++;
						}
						catch (Exception)
						{
							Auxiliary.SaveResultAddSkuSubcollection(ean, $"SKU {sku_id.ToString()}: {Convert.ToString(respuesta)}");
						}

						task_sku_collection.Dispose();
					}
					else
					{
						Auxiliary.SaveResultAddSkuSubcollection(ean, "No se encontro colección para esta referencia");
					}
				}
				else
				{
					Auxiliary.SaveResultAddSkuSubcollection(ean, rta);
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				Console.WriteLine(errores);
				Auxiliary.SaveResultAddSkuSubcollection(ean, errores.Trim().Trim(','));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Auxiliary.SaveResultAddSkuSubcollection(ean, ex.Message);
			}
			return total;
		}

		public int CreateSubcollectionSku(string  ref_id, int subcollection, bool master)
		{
			int sku_id = -1;
			string ean = "";
			int total = 0;
			try
			{
				sku_id = -1;
				ean = ref_id.Trim();

				Task<string> task_get_sku_id = Task.Run(() => VTEXGetSkuIdByReferenceId(ean));
				task_get_sku_id.Wait();
				dynamic results_get_sku_id = JsonConvert.DeserializeObject<dynamic>(task_get_sku_id.Result);
				task_get_sku_id.Dispose();
				string rta = Convert.ToString(results_get_sku_id);

				if (int.TryParse(rta, out sku_id))
				{
					if (subcollection == -1)
						subcollection = GetCollectionByRefid(Configuracion.Portafolio, ean);

					if (subcollection > 0)
					{
						SkuToSubcollection sku_to_subcollection = new SkuToSubcollection();
						sku_to_subcollection.SkuId = sku_id;

						Task<string> task_sku_collection = Task.Run(() => VTEXAddSkuSubcollection(sku_to_subcollection, subcollection, master));
						task_sku_collection.Wait();
						string respuesta = Convert.ToString(task_sku_collection.Result);
						try
						{
							dynamic results_collection = JsonConvert.DeserializeObject<dynamic>(task_sku_collection.Result);
							total++;
						}
						catch (Exception)
						{
							Auxiliary.SaveResultAddSkuSubcollection(ean, $"SKU {sku_id.ToString()}: {Convert.ToString(respuesta)}");
						}

						task_sku_collection.Dispose();
					}
					else
					{
						Auxiliary.SaveResultAddSkuSubcollection(ean, "No se encontro colección para esta referencia");
					}
				}
				else
				{
					Auxiliary.SaveResultAddSkuSubcollection(ean, rta);
				}
			}
			catch (AggregateException ex)
			{
				string errores = "";
				foreach (Exception item in ex.InnerExceptions)
				{
					errores += item.Message + ", ";
				}
				Console.WriteLine(errores);
				Auxiliary.SaveResultAddSkuSubcollection(ean, errores.Trim().Trim(','));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Auxiliary.SaveResultAddSkuSubcollection(ean, ex.Message);
			}
			return total;
		}
		#endregion
	}
}
