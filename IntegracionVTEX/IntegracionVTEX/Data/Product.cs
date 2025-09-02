using IntegracionVTEX.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static IntegracionVTEX.Models.SKU;
namespace IntegracionVTEX.Data
{
	public class Product
	{
		Configuration config = null;
		AppSettingsSection section = null;

		int loop_size = 0;
		int pause = 0;
		public Product()
		{
			config = ConfigurationManager.OpenExeConfiguration(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
			section = config.AppSettings;

			loop_size = Convert.ToInt32(section.Settings["loop_size"].Value);
			pause = Convert.ToInt32(section.Settings["pause"].Value);
		}
		#region FUNCIONES VTEX
		private async Task<string> VTEXCreateProduct(Models.Product producto)
		{
			string res = null;
			try
			{
				//string uri = Configuracion.UrlCreateProduct;
				string uri = $"https://megatiendaswl{Configuracion.CentroOperacion}.vtexcommercestable.com.br/api/catalog/pvt/product";
				string json = JsonConvert.SerializeObject(producto);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();

				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey },
					{ "X-VTEX-API-AppToken", Configuracion.AppToken },
				},
					Method = HttpMethod.Post,
					Content = data,
				};
				using (HttpResponseMessage response = await client.SendAsync(request))
				{
					//response.EnsureSuccessStatusCode();
					//res = await response.Content.ReadAsStringAsync();
					string rta = await response.Content.ReadAsStringAsync();
					res = $"{response.StatusCode}|{rta}";
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error al crear producto: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error al crear producto: " + ex.Message);
			}
			return res;
		}
				
		private async Task<string> VTEXGetProductByRefId(string ref_id)
		{
			string res = null;
			try
			{
				string uri = $"https://megatiendaswl{Configuracion.CentroOperacion}.vtexcommercestable.com.br/api/catalog_system/pvt/products/productgetbyrefid/";// Configuracion.UrlGetProductRefId;

				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}{ref_id}"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppToken },
				},
					Method = HttpMethod.Get,
				};

				using (var response = await client.SendAsync(request))
				{
					string rta = await response.Content.ReadAsStringAsync();
					res = $"{response.StatusCode}|{rta}";
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

		private async Task<string> VTEXCreateSku(Models.SKU.SkuWithoutId sku)
		{
			string res = null;
			try
			{
				//string uri = Configuracion.UrlCreateSku;
				string uri = $"https://megatiendaswl{Configuracion.CentroOperacion}.vtexcommercestable.com.br/api/catalog/pvt/stockkeepingunit";
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
					//response.EnsureSuccessStatusCode();
					//res = await response.Content.ReadAsStringAsync();
					string rta = await response.Content.ReadAsStringAsync();
					res = $"{response.StatusCode}|{rta}";
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
				//string uri = Configuracion.UrlCreateEanSku;
				string uri = $"https://megatiendaswl{Configuracion.CentroOperacion}.vtexcommercestable.com.br/api/catalog/pvt/stockkeepingunit/";
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
					//response.EnsureSuccessStatusCode();
					//res = await response.Content.ReadAsStringAsync();
					string rta = await response.Content.ReadAsStringAsync();
					res = $"{response.StatusCode}|{rta}";
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

		private async Task<string> VTEXCreateSkuFile(SkuFile file, int sku_id)
		{
			string res = null;
			try
			{
				string uri = $"https://megatiendaswl{Configuracion.CentroOperacion}.vtexcommercestable.com.br/api/catalog/pvt/stockkeepingunit/{sku_id}/file";
				string json = JsonConvert.SerializeObject(file);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
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

		private async Task<string> VTEXUpdateProductSpecification(List<Models.Specification> specifications, string product_id)
		{
			string res = null;
			try
			{
				string uri = $"https://megatiendaswl{Configuracion.CentroOperacion}.vtexcommercestable.com.br/api/catalog_system/pvt/products/";
				string json = JsonConvert.SerializeObject(specifications);

				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}{product_id}/specification"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppToken },
				},
					Method = HttpMethod.Post,
					Content = data,
				};

				using (HttpResponseMessage response = await client.SendAsync(request))
				{
					/*status_code = response.StatusCode.ToString();
					response.EnsureSuccessStatusCode();
					res = await response.Content.ReadAsStringAsync();*/
					string rta = await response.Content.ReadAsStringAsync();
					res = $"{response.StatusCode}|{rta}";
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

		#endregion

		#region FUNCIONES DE CONSULTA, INSERCION Y ACTUALIZACION
		public DataTable GetProductListFull(string portafolio, int row_ini, int row_fin)
		{
			DataTable res = null;
			try
			{
				int id_cia = Convert.ToInt32(section.Settings["id_cia"].Value);
				string SQL = File.ReadAllText("Data\\ProductsSkuQuery.txt");
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["unoee"].ConnectionString);
				conn.Open();
				SqlCommand cmd = new SqlCommand(SQL, conn);
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 600;
				cmd.Parameters.AddWithValue("@id_cia", id_cia);
				cmd.Parameters.AddWithValue("@id_portafolio", portafolio);
				cmd.Parameters.AddWithValue("@row_ini", row_ini);
				cmd.Parameters.AddWithValue("@row_fin", row_fin);
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

		public int CreateProduct(DataRow row)
		{
			int total = 0;
			try
			{
				Models.Product product = null;
				try
				{
					//CREA EL PRODUCTO
					product = new Models.Product();
					product.Name = row["Nombre del producto"].ToString().Trim();
					product.DepartmentId = 0;
					product.CategoryId =Convert.ToInt32(row["ID de categoria"].ToString().Split('.')[0]);
					product.BrandId = Convert.ToInt32(row["Id de la marca"].ToString().Split('.')[0]);
					product.LinkId = row["Link Texto"].ToString().Trim(); ;
					product.RefId = row["RefId"].ToString().Trim();
					product.IsVisible = true;//Convert.ToBoolean(row["IsVisible"]);
					product.Description = row["Descripcion del producto"].ToString().Trim();
					product.DescriptionShort = "";
					product.ReleaseDate = DateTime.Now.Date.ToString("yyyy-MM-dd") + "T00:00:00";
					product.KeyWords = row["Palabras Claves"].ToString().Trim();
					product.Title = row["Titulo del Sitio"].ToString().Trim();
					product.IsActive = true;//Convert.ToBoolean(row["IsActive"]);
					product.TaxCode = "";
					product.MetaTagDescription = row["Descripcion MetaTag"].ToString().Trim();
					product.ShowWithoutStock = false;// Convert.ToBoolean(row["ShowWithoutStock"]);
					product.Score = 1;// Convert.ToInt32(row["Score"]);

					Task<string> task_get = Task.Run(() => VTEXGetProductByRefId(product.RefId));
					task_get.Wait();
					string rta_get = task_get.Result;
					task_get.Dispose();

					if (rta_get != null)
					{
						if (rta_get.Split('|')[0].Equals("NotFound"))
						{
							Task<string> task_create = Task.Run(() => VTEXCreateProduct(product));
							task_create.Wait();

							string rta_create = task_create.Result.Split('|')[0];
							string json_create = task_create.Result.Split('|')[1];

							task_create.Dispose();

							int product_id = 0;
							try
							{
								dynamic results_create = JsonConvert.DeserializeObject<dynamic>(json_create);
								product_id = Convert.ToInt32(results_create.Id);

							}
							catch (Exception ex)
							{
								Auxiliary.SaveResultCreateProduct(product.RefId, ex.Message);
							}
							if (product_id != 0)
							{
								//CREA EL SKU
								Models.SKU.SkuWithoutId sku = new Models.SKU.SkuWithoutId();
								sku.ProductId = product_id;
								sku.IsActive = true;
								sku.ActivateIfPossible = true;// Convert.ToBoolean(row["ActivateIfPossible"]);
								sku.Name = Convert.ToString(row["nombre sku"]).Trim();//nombre sku
								sku.RefId = Convert.ToString(row["RefId"]).Trim();
								sku.Ean = null;
								sku.PackagedHeight = 0;// Convert.ToSingle(row["PackagedHeight"]);
								sku.PackagedLength = 0;// = Convert.ToSingle(row["PackagedLength"]);
								sku.PackagedWidth = 0;// Convert.ToSingle(row["PackagedWidth"]);
								sku.PackagedWeightKg = 0;// Convert.ToInt32(row["PackagedWidth"]);

								//if (Convert.IsDBNull(row["Height"]))
								sku.Height = null;
								//else
								//sku.Height = Convert.ToSingle(row["Height"]);

								//if (Convert.IsDBNull(row["Length"]))
								sku.Length = null;
								//else
								//sku.Length = Convert.ToSingle(row["Length"]);

								//if (Convert.IsDBNull(row["Width"]))
								sku.Width = null;
								//else
								//sku.Width = Convert.ToSingle(row["Width"]);

								//if (Convert.IsDBNull(row["WeightKg"]))
								sku.WeightKg = null;
								//else
								//sku.WeightKg = Convert.ToSingle(row["WeightKg"]);

								sku.CubicWeight = 0;// Convert.ToSingle(row["CubicWeight"]);
								sku.IsKit = false;// Convert.ToBoolean(row["IsKit"]);
								sku.CreationDate = DateTime.Now.Date.ToString("yyyy-MM-dd") + "T00:00:00";
								sku.RewardValue = null;
								sku.EstimatedDateArrival = null;
								sku.ManufacturerCode = null;
								sku.CommercialConditionId = 1;
								sku.MeasurementUnit = Convert.ToString(row["Unidad de Medida"]).Trim();//unidad de medida
								sku.UnitMultiplier = Convert.ToSingle(row["Multiplicador Unidad"]);//Multiplicador Unidad
								sku.ModalType = null;
								sku.KitItensSellApart = false;// Convert.ToBoolean(row["KitItensSellApart"]);
								sku.Videos = new string[0];

								Task<string> task_create_sku = Task.Run(() => VTEXCreateSku(sku));
								task_create_sku.Wait();

								string rta_create_sku = task_create_sku.Result.Split('|')[0];
								string json_create_sku = task_create_sku.Result.Split('|')[1];

								task_create_sku.Dispose();
								int sku_id = 0;
								if (rta_create_sku.Equals("OK"))
								{
									try
									{
										dynamic results_create_sku = JsonConvert.DeserializeObject<dynamic>(json_create_sku);
										sku_id = Convert.ToInt32(results_create_sku.Id);
									}
									catch (Exception ex)
									{
										Auxiliary.SaveResultCreateSku(sku.RefId, ex.Message);
									}
								}//PROBAR HASTA AQUI...
								if (sku_id != 0)
								{
									//CREA EL EAN SKU
									string ean = Convert.ToString(row["RefId"]).Trim();
									Task<string> task_create_ean_sku = Task.Run(() => VTEXCreateEanSku(sku_id, ean));
									task_create_ean_sku.Wait();
									dynamic results_create_ean_sku = JsonConvert.DeserializeObject<dynamic>(task_create_ean_sku.Result);
									task_create_ean_sku.Dispose();

									//CREA EL SKU FILE
									Models.SKU.SkuFile sku_file = new Models.SKU.SkuFile();
									sku_file.IsMain = true;
									sku_file.Label = "1";
									sku_file.Name = Convert.ToString(row["Link Texto"]).Trim();
									sku_file.Text = Convert.ToString(row["Nombre del producto"]).Trim();
									sku_file.Url = $"https://{Convert.ToString(row["URL Foto"]).Trim()}";

									Task<string> task_create_sku_file = Task.Run(() => VTEXCreateSkuFile(sku_file, sku_id));
									task_create_sku_file.Wait();
									dynamic results_create_sku_file = JsonConvert.DeserializeObject<dynamic>(task_create_sku_file.Result);
									task_create_sku_file.Dispose();
								}
								//ACTUALIZA LAS ESPECIFICACIONES
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
								task_update_product_specification.Dispose();
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
					Auxiliary.SaveResultCreateProduct(product.RefId, errores.Trim().Trim(','));
				}
				catch (Exception ex)
				{
					Auxiliary.SaveResultCreateProduct(product.RefId, ex.Message);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error al crear productos: " + ex.Message);
			}
			return total;
		}
		#endregion
	}
}
