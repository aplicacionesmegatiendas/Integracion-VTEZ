using IntegracionVTEX.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntegracionVTEX.Data
{
	public class Orders
	{
		public async Task<string> ListOrders(string fecha_desde, string fecha_hasta, int pagina)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlGetListOrders;
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}?orderBy=orderId,asc&f_creationDate=creationDate:[{fecha_desde} TO {fecha_hasta}]&page={pagina}" +
					$"&f_status=ready-for-handling&incompleteOrders=false&per_page=50"),
					Headers =
				 {
					 { "Accept", "application/json" },
					 { "X-VTEX-API-AppKey", Configuracion.AppKey },
					 { "X-VTEX-API-AppToken", Configuracion.AppToken },
				 },
					Method = HttpMethod.Get,
				};
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                using (var response = await client.SendAsync(request))
				{
					string rta = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						res = rta;
					}
					else
					{
						res = $"Error list orders: {rta}";
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
				throw new Exception("Error list orders: " + errores.Trim().Trim(','));
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error list orders: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error list orders: " + ex.Message);
			}
			return res;
		}

		public async Task<string> GetOrder(string wl, string order_id)
		{
			string res = null;
			try
			{
				string uri = $"https://megatiendas{wl}.vtexcommercestable.com.br/api/oms/pvt/orders/";
				HttpClient client = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri($"{uri}{order_id}"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey},
					{ "X-VTEX-API-AppToken", Configuracion.AppToken },
				},
					Method = HttpMethod.Get,
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
						res = $"Error get orders: {rta}";
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
				throw new Exception("Error get order: " + errores.Trim().Trim(','));
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error get order: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error get order: " + ex.Message);
			}

			return res;
		}

		private async Task<string> VTEXCancelOrder(string wl, string order_id)
		{
			string res = null;
			try
			{
				string uri = $"https://megatiendas{wl}.vtexcommercestable.com.br/api/oms/pvt/orders/";

				HttpClient client = new HttpClient();

				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri + $"{order_id}/cancel"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppToken},
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
						res = $"Error cancel: {rta}";
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
				throw new Exception("Error cancel: " + errores.Trim().Trim(','));
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error cancel: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error cancel: " + ex.Message);
			}
			return res;
		}

		private async Task<string> VTEXStartHandling(string order_id)
		{
			string res = null;
			try
			{
				string uri = Configuracion.UrlStartHandling;
				//$"https://megatiendas{wl}.vtexcommercestable.com.br/api/oms/pvt/orders/";
				HttpClient client = new HttpClient();

				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri + $"{order_id}/start-handling"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppToken},
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
						res = $"Error start handling: {rta}";
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
				throw new Exception("Error start handling: " + errores.Trim().Trim(','));
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error start handling: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error start handling: " + ex.Message);
			}
			return res;
		}

		private async Task<string> VTEXInvoice(string wl, string order_id, Models.Invoice invoice)
		{
			string res = null;
			try
			{
				string uri = $"https://megatiendas{wl}.vtexcommercestable.com.br/api/oms/pvt/orders/";
				HttpClient client = new HttpClient();
				string json = JsonConvert.SerializeObject(invoice);
				StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
				HttpRequestMessage request = new HttpRequestMessage
				{
					RequestUri = new Uri(uri + $"{order_id}/invoice"),
					Headers =
				{
					{ "Accept", "application/json" },
					{ "X-VTEX-API-AppKey", Configuracion.AppKey } ,
					{ "X-VTEX-API-AppToken", Configuracion.AppToken},
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
						res = $"Error invoice: {rta}";
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
				throw new Exception("Error invoice: " + errores.Trim().Trim(','));
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Error invoice: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error invoice: " + ex.Message);
			}
			return res;
		}

		public int ProcessOrders(string fecha_ini, string fecha_fin)
		{
			int pages = 1;
			int total = 0;
			do
			{
				try
				{
					Console.OutputEncoding = System.Text.Encoding.UTF8;
					pages = 1;
					Orders orders = new Orders();

					Task<string> task_lista = Task.Run(() => orders.ListOrders(fecha_ini, fecha_fin, pages));
					task_lista.Wait();
					dynamic results_list = JsonConvert.DeserializeObject<dynamic>(task_lista.Result);
					task_lista.Dispose();

					if (results_list != null)
					{
						pages = Convert.ToInt32(results_list.paging.pages);
						Console.WriteLine($"PAGINAS {pages}");
						if (pages >= 1)
						{
							Console.WriteLine($"Pagina 1 de {pages}");
							string order_id = "";
							int c = 1;
							string respuesta = "";
							foreach (dynamic result in results_list.list)
							{
								try
								{
									order_id = Convert.ToString(result.orderId);

									Console.WriteLine($"{c} - {order_id}");

									Task<string> task_handling = Task.Run(() => orders.VTEXStartHandling(order_id));
									task_handling.Wait();
									respuesta = task_handling.Result;
									task_handling.Dispose();
									if (!respuesta.Equals(""))
									{
										Auxiliary.SaveResultStartHandling(order_id, respuesta);
										c++;
										continue;
									}
									total++;
									Thread.Sleep(300);
								}
								catch (Exception ex)
								{
									Auxiliary.SaveResultStartHandling(order_id, $"{ex.Message}: {respuesta}");
									c++;
									continue;
								}
								c++;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				if (pages > 1)
				{
					for (int i = 2; i < (pages + 1); i++)
					{
						try
						{
							Orders orders = new Orders();
							Console.WriteLine($"Pagina {i} de {pages}");
							Task<string> task_lista = Task.Run(() => orders.ListOrders(fecha_ini, fecha_fin, i));
							task_lista.Wait();
							dynamic results_list = JsonConvert.DeserializeObject<dynamic>(task_lista.Result);
							task_lista.Dispose();

							if (results_list != null)
							{
								string order_id = "";
								int c = 1;
								string respuesta = "";
								foreach (dynamic result in results_list.list)
								{
									try
									{
										order_id = Convert.ToString(result.orderId);

										Console.WriteLine($"{c.ToString()} - {order_id}");

										Task<string> task_handling = Task.Run(() => orders.VTEXStartHandling(order_id));
										task_handling.Wait();
										respuesta = task_handling.Result;
										task_handling.Dispose();
										if (!respuesta.Equals(""))
										{
											Auxiliary.SaveResultStartHandling(order_id, respuesta);
											c++;
											continue;
										}
										total++;
										Thread.Sleep(300);
									}
									catch (Exception ex)
									{
										Auxiliary.SaveResultStartHandling(order_id, $"{ex.Message}: {respuesta}");
										c++;
										continue;
									}
									c++;
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						Thread.Sleep(600);
					}
					Console.WriteLine("Espere...");
					Thread.Sleep(360000);
				}
				else
				{
					pages = 0;
				}
			} while (pages > 0);
			return total;
		}
	}
}
