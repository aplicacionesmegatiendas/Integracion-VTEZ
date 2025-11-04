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
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace IntegracionVTEX.Data
{
	public class Auxiliary
	{
		public static void SaveResultCreateProduct(string ref_id, string result)
		{
			string SQL = @"insert into
								t01_log_create_product
							(
								f01_ref_id,
								f01_respuesta,
                                f01_co,
                                f01_portafolio
							)
							values
							(
								@ref_id,
								@respuesta,
                                @co,
                                @portafolio
							)";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@ref_id", ref_id);
				cmd.Parameters.AddWithValue("@respuesta", result);
				cmd.Parameters.AddWithValue("@co", Configuracion.CentroOperacion);
				cmd.Parameters.AddWithValue("@portafolio", Configuracion.Portafolio);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar resultado create product: " + ex.Message);
			}
		}

		public static void SaveResultCreateSku(string ref_id, string result)
		{
			string SQL = @"insert into
								t02_log_create_sku
							(
								f02_ref_id,
								f02_respuesta,
                                f02_co,
                                f02_portafolio
							)
							values
							(
								@ref_id,
								@respuesta,
                                @co,
                                @portafolio
							)";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@ref_id", ref_id);
				cmd.Parameters.AddWithValue("@respuesta", result);
				cmd.Parameters.AddWithValue("@co", Configuracion.CentroOperacion);
				cmd.Parameters.AddWithValue("@portafolio", Configuracion.Portafolio);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar resultado create sku: " + ex.Message);
			}
		}

		public static void SaveResultUpdateInventory(string ref_id, string result)
		{
			string SQL = @"insert into
								t03_log_update_inventory
							(
								f03_ref_id,
								f03_respuesta,
                                f03_co,
                                f03_portafolio
							)
							values
							(
								@ref_id,
								@respuesta,
                                @co,
                                @portafolio
							)";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@ref_id", ref_id);
				cmd.Parameters.AddWithValue("@respuesta", result);
				cmd.Parameters.AddWithValue("@co", Configuracion.CentroOperacion);
				cmd.Parameters.AddWithValue("@portafolio", Configuracion.Portafolio);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar resultado update inventory: " + ex.Message);
			}
		}

		public static void SaveResultCreateUpdateBasePriceFixedPrices(string ref_id, string result)
		{
			string SQL = @"insert into
	                            t06_log_create_price
                            (
	                            f06_ref_id,
	                            f06_respuesta,
                                f06_co,
                                f06_lista_precios,
                                f06_portafolio
                            )
                            values
                            (
	                            @ref_id,
	                            @respuesta,
                                @co,
                                @lista_precios,
                                @portafolio
                            )";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@ref_id", ref_id);
				cmd.Parameters.AddWithValue("@respuesta", result);
				cmd.Parameters.AddWithValue("@co", Configuracion.CentroOperacion);
				cmd.Parameters.AddWithValue("@lista_precios", Configuracion.ListaPrecios);
				cmd.Parameters.AddWithValue("@portafolio", Configuracion.Portafolio);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar resultado update base price: " + ex.Message);
			}
		}

		public static void SaveResultAddSkuSubcollection(string ref_id, string result)
		{
			string SQL = @"insert into
	                            t07_log_add_sku_subcollection
                            (
	                            f07_ref_id,
	                            f07_respuesta,
                                f07_co,
                                f07_portafolio
                            )
                            values
                            (
	                            @ref_id,
	                            @respuesta,
                                @co,
                                @portafolio
                            )";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@ref_id", ref_id);
				cmd.Parameters.AddWithValue("@respuesta", result);
				cmd.Parameters.AddWithValue("@co", Configuracion.CentroOperacion);
				cmd.Parameters.AddWithValue("@portafolio", Configuracion.Portafolio);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar resultado add sku subcollection: " + ex.Message);
			}
		}

		public static void SaveResultIntegrateOrder(string order, string detail)
		{
			string SQL = @"insert into
	                            t04_integrate_order
                            (
	                            f04_order,
	                            f04_detail
                            )
                            values
                            (
	                            @order,
	                            @detail
                            )";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@order", order);
				cmd.Parameters.AddWithValue("@detail", detail);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar resultado integrat order: " + ex.Message);
			}
		}

		public static void SaveErrorImport(string order, ErrorImport error)
		{
			string SQL = @"insert into
	                        t05_error_import
	                        (
		                        f05_order_id,
		                        f05_linea,
		                        f05_tipo_reg,
		                        f05_sub_tipo_reg,
		                        f05_version,
		                        f05_nivel,
		                        f05_error,
		                        f05_detalle
	                        )
	                        values
	                        (
		                        @order_id,
		                        @linea,
		                        @tipo_reg,
		                        @sub_tipo_reg,
		                        @version,
		                        @nivel,
		                        @error,
		                        @detalle
	                        )";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@order_id", order);
				cmd.Parameters.AddWithValue("@linea", error.Linea);
				cmd.Parameters.AddWithValue("@tipo_reg", error.TipoRegistro);
				cmd.Parameters.AddWithValue("@sub_tipo_reg", error.SubTipoRegistro);
				cmd.Parameters.AddWithValue("@version", error.Version);
				cmd.Parameters.AddWithValue("@nivel", error.Nivel);
				cmd.Parameters.AddWithValue("@error", error.Error);
				cmd.Parameters.AddWithValue("@detalle", error.Detalle);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar error import: " + ex.Message);
			}
		}

		public static void SaveResultUpdateProductSpecification(string ref_id, string result)
		{
			string SQL = @"insert into
	                            t10_log_update_product_specification
                            (
	                            f10_ref_id,
	                            f10_respuesta,
                                f10_co,
                                f10_portafolio
                            )
                            values
                            (
	                            @ref_id,
	                            @respuesta,
                                @co,
                                @portafolio
                            )";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@ref_id", ref_id);
				cmd.Parameters.AddWithValue("@respuesta", result);
				cmd.Parameters.AddWithValue("@co", Configuracion.CentroOperacion);
				cmd.Parameters.AddWithValue("@portafolio", Configuracion.Portafolio);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar resultado update product specification: " + ex.Message);
			}
		}

		public static void SaveResultDeleteSkuSubcollection(int coleccion, int sku, string result)
		{
			string SQL = @"insert into
	                            t11_log_delete_sku_subcollection
                            (
	                            f11_coleccion,
	                            f11_sku,
                                f11_respuesta
                            )
                            values
                            (
	                            @coleccion,
	                            @sku,
                                @respuesta
                            )";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@coleccion", coleccion);
				cmd.Parameters.AddWithValue("@sku", sku);
				cmd.Parameters.AddWithValue("@respuesta", result);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar resultado delete sku collection: " + ex.Message);
			}
		}

		public static void SaveResultCreatePromo(string promo_id, string result)
		{
			string SQL = @"insert into
	                            t12_log_create_promo
                            (
	                            f12_promo_id,
	                            f12_respuesta,
	                            f12_co,
                                f12_portafolio
                            )
                            values
                            (
	                            @promo_id,
	                            @respuesta,
	                            @co,
                                @portafolio
                            )";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@promo_id", promo_id);
				cmd.Parameters.AddWithValue("@respuesta", result);
				cmd.Parameters.AddWithValue("@co", Configuracion.CentroOperacion);
				cmd.Parameters.AddWithValue("@portafolio", Configuracion.Portafolio);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar resultado create promo: " + ex.Message);
			}
		}

		public static void SaveResultStartHandling(string orden, string result)
		{
			string SQL = @"insert into
								t14_log_start_handling
							(
								f14_orden,
								f14_respuesta,
								f14_fecha,
								f14_co
							)
							values
							(
								@orden,
								@respuesta,
								GETDATE(),
								@co
							)";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@orden", orden);
				cmd.Parameters.AddWithValue("@respuesta", result);
				cmd.Parameters.AddWithValue("@co", Configuracion.CentroOperacion);
				
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar resultado start-handling: " + ex.Message);
			}
		}

		public static void SaveTotalResult(string proceso, int result)
		{
			string SQL = @"insert into
	                            t13_resumen
                            (
	                            f13_proceso,
	                            f13_co,
	                            f13_resultado
                            )
                            values
                            (
	                            @proceso,
	                            @co,
	                            @resultado
                            )";
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@proceso", proceso);
				cmd.Parameters.AddWithValue("@co", Configuracion.CentroOperacion);
				cmd.Parameters.AddWithValue("@resultado", result);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al guardar resultado total: " + ex.Message);
			}
		}

		private int GetTotalResult(string proceso, string tiempo)
		{
			string SQL = @"select 
	                            sum(f13_resultado) total
                            from 
	                            t13_resumen
                            where
	                            f13_proceso=@proceso ";
			if (tiempo.Equals("M"))
				SQL += $" and f13_fecha between Convert(CHAR(8),GETDATE(),112) + ' 00:00:00' and Convert(CHAR(8),GETDATE(),112) + ' 08:00:00'";
			if (tiempo.Equals("T"))
				SQL += $" and f13_fecha between Convert(CHAR(8),GETDATE(),112) + ' 12:00:00' and Convert(CHAR(8),GETDATE(),112) + ' 16:00:00'";

			int res = 0;
			try
			{
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["integracion"].ConnectionString);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = SQL;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@proceso", proceso);
				int.TryParse(Convert.ToString(cmd.ExecuteScalar()), out res);
				conn.Close();
			}
			catch (Exception ex)
			{
				throw new Exception("Error al obtener resultado total: " + ex.Message);
			}
			return res;
		}

		public void EnviarCorreoMañana()
		{
			try
			{
				int precios = GetTotalResult("CreateUpdateBasePriceFixedPrices", "M");
				int existencias = GetTotalResult("UpdateInventory", "M");
				int colecciones = GetTotalResult("CreateSubcollectionSku", "M");
				int act_promos = GetTotalResult("UpdatePromotion", "M");
				int crea_promos = GetTotalResult("CreatePromotion", "M");
				int cambia_estado = GetTotalResult("StartHandling", "M");
				string body = $@"<!DOCTYPE html>
                            <html lang=""es"">
                            <body>
                              <h1>
                                Resultados Integración VTEX {DateTime.Now.ToShortDateString()}
                              </h1>
                              <p>
                                Se actualizaron {precios} precios de productos.
                              </p>
                              <p>
                                Se actualizaron {existencias} existencias de productos.
                              </p>
                              <p>
                                Se actualizaron {colecciones} colecciones.
                              </p>
                              <p>
                                Se actualizaron {act_promos} promociones.
                              </p>
                              <p>
                                Se crearon {crea_promos} promociones.
                              </p>
							 <p>
                                Se cambiaron {cambia_estado} estados.
                              </p>
                            </body>
                            </html>";

				Configuration config = ConfigurationManager.OpenExeConfiguration(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
				AppSettingsSection section = config.AppSettings;

				string smtp = section.Settings["smtp"].Value;
				int puerto = Convert.ToInt32(section.Settings["puerto"].Value);
				string from = section.Settings["from"].Value;
				string pwd = section.Settings["pwd"].Value;
				string[] to = section.Settings["to"].Value.Split(';');

				MailMessage mail = new MailMessage();
				SmtpClient SmtpServer = new SmtpClient(smtp);

				mail.From = new MailAddress(from, "Notificación");

				foreach (string s in to)
					mail.To.Add(s);

				mail.IsBodyHtml = true;
				mail.Subject = "Resultado Integración VTEX";
				mail.Body = body;
				//mail.Priority = MailPriority.High;
				
				SmtpServer.Port = puerto;
				SmtpServer.Credentials = new System.Net.NetworkCredential(from, pwd);
				SmtpServer.EnableSsl = false;//true para gmail

				ServicePointManager.ServerCertificateValidationCallback +=
							  delegate (
							  Object sender1,
							  X509Certificate certificate,
							  X509Chain chain,
							  SslPolicyErrors sslPolicyErrors)
							  {
								  return true;
							  };

				SmtpServer.Send(mail);

			}
			catch (Exception ex)
			{
				throw new Exception("Error al enviar correo: " + ex.Message);
			}
		}

		public void EnviarCorreoTarde()
		{
			try
			{
				int precios = GetTotalResult("CreateUpdateBasePriceFixedPrices", "T");
				int existencias = GetTotalResult("UpdateInventory", "T");
				int act_promos = GetTotalResult("UpdatePromotion", "T");
				int crea_promos = GetTotalResult("CreatePromotion", "T");

				string body = $@"<!DOCTYPE html>
                            <html lang=""es"">
                            <body>
                              <h1>
                                Resultados Integración VTEX {DateTime.Now.ToShortDateString()}
                              </h1>
                              <p>
                                Se actualizaron {precios} precios de productos.
                              </p>
                              <p>
                                Se actualizaron {existencias} existencias de productos.
                              </p>
							  <p>
                                Se actualizaron {act_promos} promociones.
                              </p>
                              <p>
                                Se crearon {crea_promos} promociones.
                              </p>
                            </body>
                            </html>";

				Configuration config = ConfigurationManager.OpenExeConfiguration(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
				AppSettingsSection section = config.AppSettings;

				string smtp = section.Settings["smtp"].Value;
				int puerto = Convert.ToInt32(section.Settings["puerto"].Value);
				string from = section.Settings["from"].Value;
				string pwd = section.Settings["pwd"].Value;
				string[] to = section.Settings["to"].Value.Split(';');

				MailMessage mail = new MailMessage();
				SmtpClient SmtpServer = new SmtpClient(smtp);

				mail.From = new MailAddress(from, "Notificación");

				foreach (string s in to)
					mail.To.Add(s);

				mail.IsBodyHtml = true;
				mail.Subject = "Resultado Integración VTEX";
				mail.Body = body;
				mail.Priority = MailPriority.High;

				SmtpServer.Port = puerto;
				SmtpServer.Credentials = new System.Net.NetworkCredential(from, pwd);
				SmtpServer.EnableSsl = false;//true en gmail

				ServicePointManager.ServerCertificateValidationCallback +=
							  delegate (
							  Object sender1,
							  X509Certificate certificate,
							  X509Chain chain,
							  SslPolicyErrors sslPolicyErrors)
							  {
								  return true;
							  };

				SmtpServer.Send(mail);

			}
			catch (Exception ex)
			{
				throw new Exception("Error al enviar correo: " + ex.Message);
			}
		}
	}

	public class ErrorImport
	{
		public int Linea { get; set; }
		public string TipoRegistro { get; set; }
		public string SubTipoRegistro { get; set; }
		public string Version { get; set; }
		public string Nivel { get; set; }
		public string Error { get; set; }
		public string Detalle { get; set; }
	}
}
