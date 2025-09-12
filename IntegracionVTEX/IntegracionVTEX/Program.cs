using IntegracionVTEX.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IntegracionVTEX
{
	internal class Program
	{
		static void Main(string[] args)
		{
			int loop_size = 0;
			int pause = 0;
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
			AppSettingsSection section = config.AppSettings;

			loop_size = Convert.ToInt32(section.Settings["loop_size"].Value);
			pause = Convert.ToInt32(section.Settings["pause"].Value);

			try
			{
				Configuracion configuracion = new Configuracion();

				if (args.Length == 4)
				{
					switch (args[0].ToUpper())
					{
						case "R"://ACTUALIZAR PRECIO
							if (configuracion.ObtenerConfiguracion(args[1]) == true)//R, INSTALACION, LISTA PRECIOS, PORTAFOLIO
							{
								Configuracion.CentroOperacion = args[1];
								Configuracion.ListaPrecios = args[2];
								Configuracion.Portafolio = args[3];
								Price price = new Price();
								DataTable data = price.GetPriceList();
								if (data != null)
								{
									CreateUpdateBasePriceFixedPrices(data, price, loop_size, pause);
								}
								string items_error = price.ObtenerItemsError();
								if (!items_error.Equals(string.Empty))
								{
									DataTable data_error = price.GetPriceList(items_error);
									if (data_error != null)
									{
										CreateUpdateBasePriceFixedPrices(data_error, price, loop_size, pause);
									}
								}
							}
							break;
						case "D"://ELIMINA PRECIO
							if (configuracion.ObtenerConfiguracion(args[1]) == true)//D, INSTALACION, LISTA PRECIOS, PORTAFOLIO
							{
								Configuracion.CentroOperacion = args[1];
								Configuracion.ListaPrecios = args[2];
								Configuracion.Portafolio = args[3];
								Price price = new Price();
								DataTable data = price.GetPriceList();
								if (data != null)
								{
									DeletePrice(data, price, loop_size, pause);
								}
							}
							break;
						case "L"://ELIMINA SKU DE UNA COLECCION Y CREA NUEVAMENTE LAS SUBCOLECCIONES.
							if (configuracion.ObtenerConfiguracion(args[2]) == true)//L, COLECCION, INSTALACION, PORTAFOLIO
							{
								int nro_coleccion = Convert.ToInt32(args[1]);
								Configuracion.CentroOperacion = args[2];
								Configuracion.Portafolio = args[3];

								Collection collection = new Collection();

								string items = "";
								switch (nro_coleccion)
								{
									case 151://Megaoferta
										items = collection.GetProductsDiscount(Configuracion.Portafolio);
										break;
									case 146://Ofertas en Despensa
										items = collection.GetProductsDiscount(Configuracion.Portafolio, "018", "0004");
										break;
									case 144://Ofertas en Aseo Hogar
										items = collection.GetProductsDiscount(Configuracion.Portafolio, "018", "0008");
										break;
									case 143://Ofertas en Cuidado Personal
										items = collection.GetProductsDiscount(Configuracion.Portafolio, "018", "0009");
										break;
								}
								if (!items.Equals(""))
								{
									SKU sku = new SKU();
									int subcollection = collection.RunDeleteSkuFromSubcollectionProcess(nro_coleccion);
									CreateSubcollectionSku(items, sku, loop_size, pause, 21, true);
								}
							}
							break;
					}
				}
				if (args.Length == 3)
				{
					switch (args[0].ToUpper())
					{
						case "Q"://INVENTARIO(EXISTENCIAS)
							if (configuracion.ObtenerConfiguracion(args[1]) == true)//Q, INSTALACION, PORTAFOLIO
							{
								Configuracion.CentroOperacion = args[1];
								Configuracion.Portafolio = args[2];
								Inventory inventory = new Inventory();
								DataTable data = inventory.GetProductList();
								if (data != null)
								{
									UpdateInventory(data, inventory, loop_size, pause);
								}
							}
							break;
						case "E"://ACTUALIZAR ESPECIFICACION
							if (configuracion.ObtenerConfiguracion(args[1]) == true)//E, INSTALACION, PORTAFOLIO
							{
								Configuracion.CentroOperacion = args[1];
								Configuracion.Portafolio = args[2];
								Specification specification = new Specification();
								DataTable data = specification.GetSpecifications();
								if (data != null)
								{
									UpdateProductSpecification(data, specification, loop_size, pause);
								}
							}
							break;
						case "M"://PROMOCIONES POR WL
							if (configuracion.ObtenerConfiguracion(args[1]) == true)//M, INSTALACION, PORTAFOLIO
							{
								Configuracion.CentroOperacion = args[1];
								Configuracion.Portafolio = args[2];
								Promotions promotions = new Promotions();
								DataTable data = promotions.GetPromotionListCentroOperacion();

								if (data != null)
								{
									CreateUpdatePromotion(data, promotions, false,false);
								}
							}
							break;
						case "M2"://PROMOCIONES PARA EL PRINCIPAL
							if (configuracion.ObtenerConfiguracion(args[1]) == true)//M2, INSTALACION, PORTAFOLIO
							{
								Configuracion.CentroOperacion = args[1];
								Configuracion.Portafolio = args[2];
								Promotions promotions = new Promotions();
								DataTable data = promotions.GetPromotionList();

								if (data != null)
								{
									CreateUpdatePromotion(data, promotions, true,true);
								}
							}
							break;
						case "P"://CREAR PRODUCTO
							if (configuracion.ObtenerConfiguracion(args[2]) == true)//P, PORTAFOLIO, INSTALACION
							{
								Configuracion.CentroOperacion = args[2];
								Configuracion.Portafolio = args[1];
								Product product = new Product();
								int total_filas = 0;
								int nro_filas = 1;
								int row_ini = 1;
								int row_fin = 100;
								int grupo = 1;
								while (nro_filas > 0)
								{
									Task<DataTable> task = Task.Run(() => product.GetProductListFull(Configuracion.Portafolio, row_ini, row_fin));
									task.Wait();
									DataTable data = task.Result;
									task.Dispose();

									if (data != null)
									{
										Console.WriteLine($"GRUPO {grupo}");

										CreateProduct(data, product, loop_size, pause);

										nro_filas = data.Rows.Count;

										total_filas += nro_filas;

										row_ini = total_filas + 1;
										row_fin = row_ini + 99;

										grupo++;
									}
									else
									{
										nro_filas = 0;
									}
								}
							}
							break;
						case "K"://CREAR SKU
							if (configuracion.ObtenerConfiguracion(args[2]) == true)//K, PORTAFOLIO, INSTALACION
							{
								Configuracion.CentroOperacion = args[2];
								Configuracion.Portafolio = args[1];
								SKU sku = new SKU();
								sku.CreateSku(Configuracion.Portafolio);
							}
							break;
						case "S"://CREAR SUBCOLECCION
							if (configuracion.ObtenerConfiguracion(args[2]) == true)//S, PORTAFOLIO, INSTALACION
							{
								Configuracion.CentroOperacion = args[2];
								Configuracion.Portafolio = args[1];
								SKU sku = new SKU();
								DataTable data = sku.GetProductSkuList(Configuracion.Portafolio);
								if (data != null)
								{
									CreateSubcollectionSku(data, sku, loop_size, pause, -1, false);
								}
							}
							break;
					}
				}
				if (args.Length == 2)
				{
					switch (args[0])
					{
						case "C"://CAMBIA EL ESTADO DE LOS PEDIDOS QUE ESTAN EN READY-FOR-HANDLING A START HANDLING
							if (configuracion.ObtenerConfiguracion(args[1]) == true)//C, INSTALACION
							{
								Configuracion.CentroOperacion = args[1];
								TimeZoneInfo britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
								//string fecha_ini = TimeZoneInfo.ConvertTime(Convert.ToDateTime("2023-01-01"), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
								//string fecha_fin = TimeZoneInfo.ConvertTime(Convert.ToDateTime("2025-07-09"), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
								string fecha_ini = TimeZoneInfo.ConvertTime(DateTime.Now.Date.AddDays(-10), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
								string fecha_fin = TimeZoneInfo.ConvertTime(DateTime.Now.Date.AddDays(-3), TimeZoneInfo.Local, britishZone).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
								try
								{
									ProcessOrders(fecha_ini, fecha_fin);
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
								}
							}
							break;
						case "A":
							{
								if (configuracion.ObtenerConfiguracion(args[1]) == true)
								{
									Configuracion.CentroOperacion = args[1];
									Promotions promotion = new Promotions();
									Task<string> task_get = Task.Run(() => promotion.VTEXGetAllPromotions(false, Configuracion.CentroOperacion));
									task_get.Wait();
									task_get.Dispose();
									dynamic results_get = JsonConvert.DeserializeObject<dynamic>(task_get.Result);

									foreach (dynamic item in results_get.items)
									{
										string idCalculatorConfiguration = item.idCalculatorConfiguration;
										Console.WriteLine($"Archivando promo {idCalculatorConfiguration}");
										Task<string> task_post = Task.Run(() => promotion.VTEXArchivePromotion(idCalculatorConfiguration, Configuracion.CentroOperacion));
										task_post.Wait();
										task_post.Dispose();
									}
								}
							}
							break;
					}
				}
				if (args.Length == 1)
				{
					Auxiliary auxiliary = new Auxiliary();
					switch (args[0].ToUpper())
					{
						case "M":
							auxiliary.EnviarCorreoMañana();
							break;
						case "T":
							auxiliary = new Auxiliary();
							auxiliary.EnviarCorreoTarde();
							break;
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
				Console.WriteLine(errores.Trim().Trim(','));
				File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\LogIntegracionVTEX.txt",
								DateTime.Now.Date.ToShortDateString() + ": " + errores.Trim().Trim(',') + Environment.NewLine);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\LogIntegracionVTEX.txt",
								DateTime.Now.Date.ToShortDateString() + ": " + ex.ToString() + Environment.NewLine);
			}
			//Console.WriteLine("ya");
			//Console.ReadLine();
		}

		//FUNCIONES

		private static void CreateProduct(DataTable data, Product product, int loop_size, int pause)
		{
			int ciclo = 1;
			int total = 0;
			for (int i = 0; i < data.Rows.Count; i += loop_size)
			{
				try
				{
					Console.WriteLine($"Ciclo {ciclo.ToString()}...");

					for (int j = i; j < i + loop_size && j < data.Rows.Count; j++)
					{
						try
						{
							Thread thread = new Thread(() => total += product.CreateProduct(data.Rows[j]));
							thread.Start();
							Thread.Sleep(300);
						}

						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
							Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices(Convert.ToString(data.Rows[j]["RefId"]), ex.Message);
							continue;
						}
					}

					Console.WriteLine($"Esperando {pause} segundos para seguir guardando...");
					Thread.Sleep(pause * 1000);
					ciclo++;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices("Error", ex.Message);
					continue;
				}
			}
			//GUARDAR REGISTRO
			Auxiliary.SaveTotalResult("CreateUpdateBasePriceFixedPrices", total);
		}

		private static void CreateUpdateBasePriceFixedPrices(DataTable data, Price price, int loop_size, int pause)
		{
			int ciclo = 1;
			int total = 0;
			for (int i = 0; i < data.Rows.Count; i += loop_size)
			{
				try
				{
					Console.WriteLine($"Ciclo {ciclo.ToString()}...");

					for (int j = i; j < i + loop_size && j < data.Rows.Count; j++)
					{
						try
						{
							Thread thread = new Thread(() => total += price.CreateUpdateBasePriceFixedPrices(data.Rows[j]));
							thread.Start();
							Thread.Sleep(300);
                        }

						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
							Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices(Convert.ToString(data.Rows[j]["RefId"]), ex.Message);
							continue;
						}
					}

					Console.WriteLine($"Esperando {pause} segundos para seguir guardando...");
					Thread.Sleep(pause * 1000);
					ciclo++;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices("Error", ex.Message);
					continue;
				}
			}
			//GUARDAR REGISTRO
			Auxiliary.SaveTotalResult("CreateUpdateBasePriceFixedPrices", total);
		}

		private static void DeletePrice(DataTable data, Price price, int loop_size, int pause)
		{
			int ciclo = 1;
			for (int i = 0; i < data.Rows.Count; i += loop_size)
			{
				try
				{
					Console.WriteLine($"Ciclo {ciclo.ToString()}...");

					var taskList = new List<Task>();
					for (int j = i; j < i + loop_size && j < data.Rows.Count; j++)
					{
						try
						{
							Thread thread = new Thread(() => price.DeletePrice(data.Rows[j]));//hacer using
							thread.Start();
							Thread.Sleep(300);
						}

						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
							Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices(Convert.ToString(data.Rows[j]["RefId"]), ex.Message);
							continue;
						}
					}

					Console.WriteLine($"Esperando {pause} segundos para seguir eliminando...");
					Thread.Sleep(pause * 1000);
					ciclo++;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					Auxiliary.SaveResultCreateUpdateBasePriceFixedPrices("Error", ex.Message);
					continue;
				}
			}
		}

		private static void CreateSubcollectionSku(DataTable data, SKU sku, int loop_size, int pause, int subcollection, bool master)
		{
			int ciclo = 1;
			int total = 0;

			for (int i = 0; i < data.Rows.Count; i += loop_size)
			{
				try
				{
					Console.WriteLine($"Ciclo {ciclo.ToString()}...");

					for (int j = i; j < i + loop_size && j < data.Rows.Count; j++)
					{
						try
						{
							Thread thread = new Thread(() => total += sku.CreateSubcollectionSku(data.Rows[j], subcollection, master));
							thread.Start();
							Thread.Sleep(300);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
							Auxiliary.SaveResultAddSkuSubcollection(Convert.ToString(data.Rows[j]["RefId"]), ex.Message);
							continue;
						}
					}
					Console.WriteLine($"Esperando {pause} segundos para seguir guardando...");
					Thread.Sleep(pause * 1000);
					ciclo++;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					Auxiliary.SaveResultAddSkuSubcollection("Error", ex.Message);
					continue;
				}
			}
			//GUARDAR REGISTRO
			Auxiliary.SaveTotalResult("CreateSubcollectionSku", total);
		}

		private static void CreateSubcollectionSku(string items, SKU sku, int loop_size, int pause, int subcollection, bool master)
		{
			int ciclo = 1;
			int total = 0;
			string[] data = items.Split(',');
			for (int i = 0; i < data.Length; i += loop_size)
			{
				try
				{
					Console.WriteLine($"Ciclo {ciclo.ToString()}...");

					for (int j = i; j < i + loop_size && j < data.Length; j++)
					{
						try
						{
                            Thread thread = new Thread(() => total += sku.CreateSubcollectionSku(data[j], subcollection, master));
							thread.Start();
							Thread.Sleep(300);
        }
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
							Auxiliary.SaveResultAddSkuSubcollection(Convert.ToString(data[j]), ex.Message);
							continue;
						}
					}
					Console.WriteLine($"Esperando {pause} segundos para seguir guardando...");
					Thread.Sleep(pause * 1000);
					ciclo++;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					Auxiliary.SaveResultAddSkuSubcollection("Error", ex.Message);
					continue;
				}
			}
			//GUARDAR REGISTRO
			Auxiliary.SaveTotalResult("CreateSubcollectionSku", total);
		}

		private static void UpdateProductSpecification(DataTable data, Specification specification, int loop_size, int pause)
		{
			int ciclo = 1;
			for (int i = 0; i < data.Rows.Count; i += loop_size)
			{
				try
				{
					Console.WriteLine($"Ciclo {ciclo.ToString()}...");
					for (int j = i; j < i + loop_size && j < data.Rows.Count; j++)
					{
						Thread thread = new Thread(() => specification.UpdateProductSpecification(data.Rows[j]));
						thread.Start();
						Thread.Sleep(300);
					}
					Console.WriteLine($"Esperando {pause} segundos para seguir guardando...");
					Thread.Sleep(pause * 1000);
					ciclo++;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					Auxiliary.SaveResultUpdateProductSpecification("Error", ex.Message);
					continue;
				}
			}
		}

		private static void UpdateInventory(DataTable data, Inventory inventory, int loop_size, int pause)
		{
			int ciclo = 1;
			int total = 0;
			for (int i = 0; i < data.Rows.Count; i += loop_size)
			{
				try
				{
					Console.WriteLine($"Ciclo {ciclo.ToString()}...");
					for (int j = i; j < i + loop_size && j < data.Rows.Count; j++)
					{
                        Thread thread = new Thread(() => total += inventory.UpdateInventory(data.Rows[j]));
						thread.Start();
						Thread.Sleep(300);
        }
					Console.WriteLine($"Esperando {pause} segundos para seguir guardando...");
					Thread.Sleep(pause * 1000);
					ciclo++;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					Auxiliary.SaveResultUpdateInventory("Error", ex.Message);
					continue;
				}
			}
			//GUARDAR REGISTRO
			Auxiliary.SaveTotalResult("UpdateInventory", total);
		}

		private static void CreateUpdatePromotion(DataTable data, Promotions promotion, bool master, bool institucional)
		{
			var rangos_fechas = data.AsEnumerable()
					.Select(row => new
					{
                        beginDateUtc = row.Field<DateTime>("beginDateUtc"),
                        endDateUtc = row.Field<DateTime>("endDateUtc")
					})
					.Distinct()
					.OrderBy(r => r.beginDateUtc)
					.ThenBy(r => r.endDateUtc)
					.ToList();

			/*var rangos_vigentes = rangos_fechas
				.Where(p => DateTime.Now >= p.beginDateUtc && DateTime.Now <= p.endDateUtc)
				.Distinct()
				.OrderBy(r => r.beginDateUtc)
				.ThenBy(r => r.endDateUtc)
				.ToList();

			DateTime hoy = DateTime.Today;
			var activasHoy = rangos_fechas.Where(p => hoy >= p.beginDateUtc && hoy <= p.endDateUtc).ToList();*/

			int[] total;
            int total_act = 0;
            int total_nuevo = 0;
			
            foreach (var rango in rangos_fechas)
            {
                DateTime inicio = rango.beginDateUtc;
                DateTime fin = rango.endDateUtc;
				
                DataTable promos_rango = data.Select($"beginDateUtc='{inicio}' and endDateUtc='{fin}'").CopyToDataTable();
                List<decimal> nominal = promos_rango.AsEnumerable().Where(r => r.Field<decimal>("nominalDiscountValue") > 0).Select(r => r.Field<decimal>("nominalDiscountValue")).Distinct().OrderBy(v => v).ToList();
                List<decimal> percentual = promos_rango.AsEnumerable().Where(r => r.Field<decimal>("percentualDiscountValue") > 0).Select(r => r.Field<decimal>("percentualDiscountValue")).Distinct().OrderBy(v => v).ToList();
				//List<string> names = promos_rango.AsEnumerable().Select(r => r.Field<string>("name")).Distinct().ToList();

				foreach (decimal promo_percentual in percentual)
                {
					string name = institucional==true? 
						$"promo_institucional{promo_percentual.ToString("N2")}_{inicio.ToString("ddMMyyyy")}-{fin.ToString("ddMMyyyy")}":
						$"promo{promo_percentual.ToString("N2")}_{inicio.ToString("ddMMyyyy")}-{fin.ToString("ddMMyyyy")}";
                    DataRow[] rows_percentual = promos_rango.Select($"percentualDiscountValue='{promo_percentual}'");
					
                    try
					{
                        if (rows_percentual != null && rows_percentual.Length > 0)
                        {
                            if (rows_percentual.Length > 200)
                            {
                                int tamaño = 200;
                                int complemento = 0;
                                for (int i = 0; i < rows_percentual.Length; i += tamaño)
                                {
                                    complemento++;
                                    int tamaño_bloque_actual = Math.Min(tamaño, rows_percentual.Length - i);
                                    DataRow[] bloque_actual = new DataRow[tamaño_bloque_actual];
                                    Array.Copy(rows_percentual, i, bloque_actual, 0, tamaño_bloque_actual);
                                    string name_new = $"{name.Trim()}_{complemento.ToString("0#")}";
									total = promotion.CreateUpdatePromotion(name_new, bloque_actual, master);
									total_act += total[0];
                                    total_nuevo += total[1];
                                }
                            }
                            else
                            {
                                total = promotion.CreateUpdatePromotion(name.Trim(), rows_percentual, master);
                                total_act += total[0];
                                total_nuevo += total[1];
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
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Auxiliary.SaveResultCreatePromo(name, ex.Message);
                        continue;
                    }
                }

                foreach (decimal promo_nominal in nominal)
                {
					string name = institucional == true ?
						$"promo_institucional{promo_nominal.ToString("N0")}_{inicio.ToString("ddMMyyyy")}-{fin.ToString("ddMMyyyy")}" :
						$"promo{promo_nominal.ToString("N0")}_{inicio.ToString("ddMMyyyy")}-{fin.ToString("ddMMyyyy")}";
                    DataRow[] rows_nominal = promos_rango.Select($"nominalDiscountValue='{promo_nominal}'");
                    try
                    {
                        if (rows_nominal != null && rows_nominal.Length > 0)
                        {
                            if (rows_nominal.Length > 200)
                            {
                                int tamaño = 200;
                                int complemento = 0;
                                for (int i = 0; i < rows_nominal.Length; i += tamaño)
                                {
                                    complemento++;
                                    int tamaño_bloque_actual = Math.Min(tamaño, rows_nominal.Length - i);
                                    DataRow[] bloque_actual = new DataRow[tamaño_bloque_actual];
                                    Array.Copy(rows_nominal, i, bloque_actual, 0, tamaño_bloque_actual);
                                    string name_new = $"{name.Trim()}_{complemento.ToString("0#")}";
                                    total = promotion.CreateUpdatePromotion(name_new, bloque_actual, master);
                                    total_act += total[0];
                                    total_nuevo += total[1];
                                }
                            }
                            else
                            {
                                total = promotion.CreateUpdatePromotion(name.Trim(), rows_nominal, master);
                                total_act += total[0];
                                total_nuevo += total[1];
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
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Auxiliary.SaveResultCreatePromo(name, ex.Message);
                        continue;
                    }
                }
            }
            Auxiliary.SaveTotalResult("UpdatePromotion", total_act);
            Auxiliary.SaveTotalResult("CreatePromotion", total_nuevo);

        }

		private static void ProcessOrders(string fecha_ini, string fecha_fin)
		{
			try
			{
				int total = 0;
				Orders ordes = new Orders();
				total += ordes.ProcessOrders(fecha_ini, fecha_fin);

				Auxiliary.SaveTotalResult("StartHandling", total);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
