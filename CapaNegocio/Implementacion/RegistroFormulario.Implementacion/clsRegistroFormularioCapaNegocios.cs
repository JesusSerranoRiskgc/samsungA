using CapaDatos.Interfaz.auth.interfaz;
using CapaDatos.Interfaz.Listas.interfaz;
using CapaDatos.Interfaz.RegistroFormulario.Interface;
using CapaDatos.util;
using CapaDTO;
using CapaDTO.Common;
using CapaDTO.ERP;
using CapaDTO.Peticiones;
using CapaDTO.ReportesDTO;
using CapaDTO.Respuestas;
using CapaNegocio.Interfaz.Email.Interfaz;
using CapaNegocio.Interfaz.RegistroFormulario.Interface;
using CapaNegocio.Utils;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading.Tasks;
using static CapaNegocio.Utils.ConversoroOpciones;
using Border = iText.Layout.Borders.Border;
using Document = iText.Layout.Document;
using JsonException = Newtonsoft.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Paragraph = iText.Layout.Element.Paragraph;
using Style = iText.Layout.Style;
using Table = iText.Layout.Element.Table;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace CapaNegocio.Implementacion.RegistroFormulario.Implementacion
{
    public class clsRegistroFormularioCapaNegocios : IRegistroFormularioCapaNegocio
    {
        private cDataBase cDataBase;
        private readonly IConfiguration _configuration;
        private readonly IRegistroFormularioCapaDatos _RegistroForm;
        protected readonly IEMailService _iEMailService;

        public clsRegistroFormularioCapaNegocios(IConfiguration configuration, IRegistroFormularioCapaDatos RegistroForm, IEMailService iEMailService)
        {
            _configuration = configuration;
            cDataBase = new cDataBase(_configuration);
            _RegistroForm = RegistroForm;
            _iEMailService = iEMailService;
        }

        public async Task<FormularioDto> CrearNuenoFormulario(int IdUsuario)
        {
            return await _RegistroForm.CrearNuenoFormulario(IdUsuario);
        }



        public async Task<FormularioDto> ReplicaFormulario(int IdFormularioAnterior, int IdUsuario)
        {
            return await _RegistroForm.ReplicaFormulario(IdFormularioAnterior, IdUsuario);
        }



        public async Task<bool> CambiaEstadoFormulario(int IdFormulario, int IdEstado)
        {
           if (IdEstado != 5)
            {
                return await _RegistroForm.CambiaEstadoFormulario(IdFormulario, IdEstado);
            }

            return await ApruebaContabilidad(IdFormulario);

        }

        public async Task<bool> CalcularRiesgoFormulario(int IdFormulario)
        {
            return await _RegistroForm.CalcularRiesgoFormulario(IdFormulario);
        }

        public async Task<FormularioRiesgoCalculadoDto> ObtenerRiesgoFormulario(int IdFormulario)
        {
            return await _RegistroForm.ObtenerRiesgoFormulario(IdFormulario);
        }

        public async Task<bool> ApruebaContabilidad(int IdFormulario)
        {
            DatosGeneralesDto objdatosgenerarles = new DatosGeneralesDto();
            List<InformacionInspektorDto> listainspektor = new List<InformacionInspektorDto>();
            List<InformacionInspektorDto> listaRepresentantesInspektor = new List<InformacionInspektorDto>();
            List<InformacionInspektorDto> listaJuntainspektor = new List<InformacionInspektorDto>();
            List<InformacionInspektorDto> listaAccionistas = new List<InformacionInspektorDto>();
            objdatosgenerarles = await _RegistroForm.ConsultaDatosGenerales(IdFormulario);
            InformacionInspektorDto objTercero = new InformacionInspektorDto();
            objTercero.IdFomulario = IdFormulario;
            objTercero.Nombre = objdatosgenerarles.NombreRazonSocial;
            objTercero.Numero_Identificacion = objdatosgenerarles.NumeroIdentificacion;
            objTercero.Tipo_Identificacion = objdatosgenerarles.TipoIdentificacion;
            objTercero.Tipo_Tercero = "Tercero";
            listainspektor.Add(objTercero);

            if (objdatosgenerarles.CategoriaTercero != 3)
            {
                listaRepresentantesInspektor = await listainspektorRepresentanteLegal(IdFormulario);
                listaJuntainspektor = await listainspektorJuntaDirectiva(IdFormulario);
                listaAccionistas = await listainspektorAccinista(IdFormulario);
                listainspektor.AddRange(listaRepresentantesInspektor);
                listainspektor.AddRange(listaJuntainspektor);
                listainspektor.AddRange(listaAccionistas);
            }
            else
            {
                List<InformacionInspektorDto> ListaPregutnasPepDatosGenerales = new List<InformacionInspektorDto>();
                ListaPregutnasPepDatosGenerales = await listainspektorDatosGenerales(IdFormulario);

                if (ListaPregutnasPepDatosGenerales.Count != null || ListaPregutnasPepDatosGenerales.Count > 0)
                {
                    listainspektor.AddRange(ListaPregutnasPepDatosGenerales);
                }
            }

            int coincidencias = 0;
            string authToken = await ObtieneTockenConsultaInspektor();
            foreach (InformacionInspektorDto obj in listainspektor)
            {
                string respuestainpektor = await ConsultaInspektor(obj, authToken);
                
                // Validar que la respuesta no sea nula o vacía antes de deserializar
                if (string.IsNullOrEmpty(respuestainpektor))
                {
                    LogError($"ConsultaInspektor devolvió una respuesta vacía para {obj.Nombre} - {obj.Numero_Identificacion}");
                    // Continuar con el siguiente elemento o asignar valores por defecto
                    obj.Numero_Consulta = "0";
                    obj.Coincidencias = "0";
                    await GuardarConsultaInspektor(obj);
                    continue;
                }

                RespuestaConsulta respuesta = null;
                try
                {
                    respuesta = JsonConvert.DeserializeObject<RespuestaConsulta>(respuestainpektor);
                }
                catch (JsonException ex)
                {
                    LogError($"Error deserializando respuesta de Inspektor: {ex.Message}. Respuesta: {respuestainpektor}");
                    obj.Numero_Consulta = "0";
                    obj.Coincidencias = "0";
                    await GuardarConsultaInspektor(obj);
                    continue;
                }

                // Validar que la deserialización fue exitosa
                if (respuesta == null)
                {
                    LogError($"La deserialización de la respuesta de Inspektor devolvió null para {obj.Nombre}");
                    obj.Numero_Consulta = "0";
                    obj.Coincidencias = "0";
                    await GuardarConsultaInspektor(obj);
                    continue;
                }

                obj.Numero_Consulta = respuesta.NumConsulta.ToString();
                obj.Coincidencias = respuesta.CantCoincidencias.ToString();
                coincidencias += respuesta.CantCoincidencias;
                await GuardarConsultaInspektor(obj);
            }

    Task.Run(() => EnviarNotificacionCohincidenciasListasControlInterno(IdFormulario, coincidencias.ToString()));


            if (coincidencias > 0)
            {
                try
                {
                    Task.Run(() => CosnultaListasAdicionales(listainspektor, authToken));
                }
                catch (Exception ex)
                {

                }


                try
                {
                    EnviarNotificacionCohincidenciasListasControlInterno(IdFormulario, coincidencias.ToString());
                }
                catch (Exception ex)
                {

                }


                //Task.Run(() => EnviarNotificacionCohincidenciasListasControlInterno(IdFormulario, coincidencias.ToString()));

                try
                {
                    await ValidaPaisesRiesgo(IdFormulario);
                }
                catch (Exception EX)
                {

                }

                return await _RegistroForm.CambiaEstadoFormulario(IdFormulario, 7);

            }
            else
            {
                try
                {
                    await ValidaPaisesRiesgo(IdFormulario);
                }
                catch (Exception EX)
                {

                }
                return await ApruebaFormulario(IdFormulario, 5);
            }

        }


        public async Task<bool> ApruebaFormulario(int IdFormulario, int Estado)
        {

            bool Aprueba= await _RegistroForm.CambiaEstadoFormulario(IdFormulario, Estado);
            await InsertaInformacionERP(IdFormulario);
            Task.Run(() => EnviaNotificacionFormAprobadoERP(IdFormulario));
            return Aprueba;
        }

        private async Task<string> InsertaInformacionERP(int IdFormulario)
        {
            string Url = _configuration["ERPConnection:UrlConsulta"].ToString();
            string XAPIKEY = _configuration["ERPConnection:XAPIKEY"].ToString();


            ERPDTO obj = new ERPDTO();
            obj = await DevulvejsonErp(IdFormulario);



            string responseContent;
            string jsonContent = JsonSerializer.Serialize(obj);
            PeticionRespuestaERPDTO objPR = new PeticionRespuestaERPDTO();
            objPR.IdFormulario = IdFormulario;
            using (HttpClient client = new HttpClient())
            {
                // Configurar encabezados
                client.DefaultRequestHeaders.Add("X-Api-Key", XAPIKEY);

                // Crear el contenido de la solicitud
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    // Hacer la solicitud POST
                    HttpResponseMessage response = await client.PostAsync(Url, content);
                    objPR.Peticion = jsonContent;
                    // Validar la respuesta
                    if (response.IsSuccessStatusCode)
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                        objPR.Respuesta = responseContent;
                    }
                    else
                    {
                        Console.WriteLine($"Error en la solicitud: {response.StatusCode}");
                        responseContent = await response.Content.ReadAsStringAsync();
                        objPR.Respuesta = responseContent;
                    }
                }
                catch (Exception ex)
                {
                    responseContent = ($"Error al realizar la solicitud: {ex.Message}");
                    objPR.Respuesta = responseContent;
                }
            }

            _RegistroForm.GuardaPeticionRespuestaErp(objPR);
            return responseContent;

        }

        private async Task<ERPDTO> DevulvejsonErp(int IdFormulario)
        {
            DatosGeneralesDto objdatosgenerarles = new DatosGeneralesDto();
            objdatosgenerarles = await _RegistroForm.ConsultaDatosGenerales(IdFormulario);

            ERPDTO objERP = new ERPDTO();

            string Nit = string.Empty;

            if (string.IsNullOrEmpty(objdatosgenerarles.DigitoVerificacion))
            {
                Nit = objdatosgenerarles.NumeroIdentificacion;
            }
            else
            {
                Nit = objdatosgenerarles.NumeroIdentificacion + "-" + objdatosgenerarles.DigitoVerificacion;
            }


            objERP.Nit = Nit;
            objERP.RazonSocial = objdatosgenerarles.NombreRazonSocial;
            objERP.Tipo = ConversoroOpciones.DevuelveClienteProveedor(objdatosgenerarles.ClaseTercero.ToString());
            objERP.NombreComercial = objdatosgenerarles.NombreRazonSocial;
            DireccionDividida resultado = ConversoroOpciones.DividirDireccion(objdatosgenerarles.DireccionPrincipal);
            objERP.DireccionCobro1 = resultado.Direccion1;
            objERP.DireccionCobro2 = resultado.Direccion2;
            if (objdatosgenerarles.ClaseTercero == 1)
            {
                DespachoMercanciaDto objDespacho = new DespachoMercanciaDto();
                objDespacho = await _RegistroForm.ConsulataDespachoMercancia(IdFormulario);
                DireccionDividida resultado2 = ConversoroOpciones.DividirDireccion(objDespacho.DireccionDespacho);
                objERP.DireccionDespacho1 = resultado2.Direccion1;
                objERP.DireccionDespacho2 = resultado2.Direccion2;
                objERP.Ciudad = objDespacho.Cuidad;
            }
            else
            {
                objERP.DireccionDespacho1 = "";
                objERP.DireccionDespacho2 = "";
                objERP.Ciudad = "";
            }

            objERP.Pais = await _RegistroForm.DevulveNombrePais(objdatosgenerarles.Pais);
            objERP.CodPostal = objdatosgenerarles.CodigoPostal;
            objERP.Departamento = "";
            if (objERP.Pais == "COLOMBIA")
            {
                objERP.OrigenNE = "N";
            }
            else
            {
                objERP.OrigenNE = "E";
            }

            objERP.TipoContribuyente = "G";
            // objERP.ActEconomica= objdatosgenerarles.ActividadEconimoca.ToString().Substring(0,3);
            objERP.ActEconomica = objdatosgenerarles.ActividadEconimoca.ToString();
            objERP.TipoIdentificacion = DevuelveTipoDocumentoERP(objdatosgenerarles.TipoIdentificacion.ToString());
            objERP.Compania = objdatosgenerarles.Empresa.ToString();
            objERP.DirCorreo = objdatosgenerarles.CorreoElectronico;


            List<DatosContactoDto> objetocontactos = new List<DatosContactoDto>();


            List<Contacto> Contacto = new List<Contacto>();

            objetocontactos = await _RegistroForm.ListaDatosContacto(IdFormulario);

            int cont = 1;
            foreach (DatosContactoDto contacto in objetocontactos)
            {
                Contacto objcontacto = new Contacto();

                objcontacto.IdContacto = cont;
                objcontacto.NombreContacto = contacto.NombreContacto;
                objcontacto.TituloContacto = contacto.CargoContacto;
                objcontacto.Ciudad = contacto.Ciudad;
                objcontacto.Direccion = contacto.Direccion;
                objcontacto.TipoDocumento = "";
                objcontacto.Nit = "";
                objcontacto.DirecCorreo = contacto.CorreoElectronico;
                objcontacto.Prefijo1 = "";
                objcontacto.TipoTelef1 = "";
                objcontacto.NroTelefono1 = contacto.TelefonoContacto;
                objcontacto.Prefijo2 = "";
                objcontacto.TipoTelef2 = "";
                objcontacto.NroTelefono1 = "";
                cont++;
                Contacto.Add(objcontacto);
            }

            if (objdatosgenerarles.CategoriaTercero != 3)
            {
                object representantes = await _RegistroForm.ConsultaInfoAccionistas(IdFormulario);
                string json = representantes.ToString();

                RootAccionistas data = JsonConvert.DeserializeObject<RootAccionistas>(json);

                foreach (var representante in data.Accionista)
                {
                    Contacto objcontacto = new Contacto();
                    objcontacto.IdContacto = cont;
                    objcontacto.NombreContacto = representante.NombreCompleto;
                    objcontacto.TituloContacto = "Accionista";
                    objcontacto.TipoDocumento = DevuelveTipoDocumentoERP(representante.TipoDocumento.ToString());
                    objcontacto.Nit = representante.NumeroIdentificacion;
                    objcontacto.DirecCorreo = "";
                    objcontacto.Prefijo1 = "";
                    objcontacto.TipoTelef1 = "";
                    objcontacto.NroTelefono1 = "";
                    objcontacto.Prefijo2 = "";
                    objcontacto.TipoTelef2 = "";
                    objcontacto.NroTelefono1 = "";
                    cont++;
                    Contacto.Add(objcontacto);
                }

            }
            objERP.Contactos = Contacto;
            return objERP;
        }



        public async Task GuardarConsultaInspektor(InformacionInspektorDto obj)
        {
             await _RegistroForm.GuardarConsultaInspektor(obj);
        }




        public async Task<List<InformacionInspektorDto>> listainspektorRepresentanteLegal(int IdFormulario)
        {
            List<InformacionInspektorDto> listainspektorReprelegal = new List<InformacionInspektorDto>(); 

            object representantes = await _RegistroForm.ConsultaInfoRepresentanteslegales(IdFormulario);
            string json = representantes.ToString();

            RootRepresentante data = JsonConvert.DeserializeObject<RootRepresentante>(json);


            foreach (var representante in data.Representantes)
            {
                InformacionInspektorDto objPri =new InformacionInspektorDto();
                objPri.IdFomulario = IdFormulario;
                objPri.Nombre = representante.nombre;
                objPri.Numero_Identificacion = representante.NumeroIdentificacion;
                objPri.Tipo_Identificacion = Convert.ToInt32(representante.tipoDocumento);
                objPri.Tipo_Tercero = "Representante Legal";

                listainspektorReprelegal.Add(objPri);


                if (representante.Tienevinculosmas5 == "1" && representante.Vinculosmas != null)
                {
                    foreach (var vinculo in representante.Vinculosmas)
                    {
                        InformacionInspektorDto objvinculo = new InformacionInspektorDto();
                        objvinculo.IdFomulario = IdFormulario;
                        objvinculo.Nombre = vinculo.NombreCompleto;
                        objvinculo.Numero_Identificacion = vinculo.NumeroIdentificacion;
                        objvinculo.Tipo_Identificacion = Convert.ToInt32(vinculo.TipoIdentificacion);
                        objvinculo.Tipo_Tercero = "Representante Vinculado 5%";

                        listainspektorReprelegal.Add(objvinculo);
                      
                    }
                }

                if (representante.InfoFamiliaPep != null)
                {
                    foreach (var familia in representante.InfoFamiliaPep)
                    {
                        InformacionInspektorDto objfamiliao = new InformacionInspektorDto();
                        objfamiliao.IdFomulario = IdFormulario;
                        objfamiliao.Nombre = familia.NombreCompleto;
                        objfamiliao.Numero_Identificacion = familia.NumeroIdentificacion;
                        objfamiliao.Tipo_Identificacion = Convert.ToInt32(familia.TipoIdentificacion);
                        objfamiliao.Tipo_Tercero = "Representante Familiar";
                        listainspektorReprelegal.Add(objfamiliao);
                    }
                }
            }

            return listainspektorReprelegal;

        }


        public async Task<List<InformacionInspektorDto>> listainspektorJuntaDirectiva(int IdFormulario)
        {
            List<InformacionInspektorDto> listainspektorReprelegal = new List<InformacionInspektorDto>();

            object representantes = await _RegistroForm.ConsultaInfoJuntaDirectiva(IdFormulario);
            string json = representantes.ToString();

            RootDirectivo data = JsonConvert.DeserializeObject<RootDirectivo>(json);


            foreach (var representante in data.Directivos)
            {
                InformacionInspektorDto objPri = new InformacionInspektorDto();
                objPri.IdFomulario = IdFormulario;
                objPri.Nombre = representante.nombre;
                objPri.Numero_Identificacion = representante.NumeroIdentificacion;
                objPri.Tipo_Identificacion = Convert.ToInt32(representante.tipoDocumento);
                objPri.Tipo_Tercero = "Directivo";

                listainspektorReprelegal.Add(objPri);


                if (representante.Tienevinculosmas5 == "1" && representante.Vinculosmas != null)
                {
                    foreach (var vinculo in representante.Vinculosmas)
                    {
                        InformacionInspektorDto objvinculo = new InformacionInspektorDto();
                        objvinculo.IdFomulario = IdFormulario;
                        objvinculo.Nombre = vinculo.NombreCompleto;
                        objvinculo.Numero_Identificacion = vinculo.NumeroIdentificacion;
                        objvinculo.Tipo_Identificacion = Convert.ToInt32(vinculo.TipoIdentificacion);
                        objvinculo.Tipo_Tercero = "Directivo Vinculado 5%";

                        listainspektorReprelegal.Add(objvinculo);

                    }
                }

                if (representante.InfoFamiliaPep != null)
                {
                    foreach (var familia in representante.InfoFamiliaPep)
                    {
                        InformacionInspektorDto objfamiliao = new InformacionInspektorDto();
                        objfamiliao.IdFomulario = IdFormulario;
                        objfamiliao.Nombre = familia.NombreCompleto;
                        objfamiliao.Numero_Identificacion = familia.NumeroIdentificacion;
                        objfamiliao.Tipo_Identificacion = Convert.ToInt32(familia.TipoIdentificacion);
                        objfamiliao.Tipo_Tercero = "Directivo Familiar";
                        listainspektorReprelegal.Add(objfamiliao);
                    }
                }
            }

            return listainspektorReprelegal;

        }

        public async Task<List<InformacionInspektorDto>> listainspektorAccinista(int IdFormulario)
        {
            List<InformacionInspektorDto> listainspektorReprelegal = new List<InformacionInspektorDto>();

            object representantes = await _RegistroForm.ConsultaInfoAccionistas(IdFormulario);
            string json = representantes.ToString();

            RootAccionistas data = JsonConvert.DeserializeObject<RootAccionistas>(json);


            foreach (var representante in data.Accionista)
            {
                InformacionInspektorDto objPri = new InformacionInspektorDto();
                objPri.IdFomulario = IdFormulario;
                objPri.Nombre = representante.NombreCompleto;
                objPri.Numero_Identificacion = representante.NumeroIdentificacion;
                objPri.Tipo_Identificacion = Convert.ToInt32(representante.TipoDocumento);
                objPri.Tipo_Tercero = "Accionista";

                listainspektorReprelegal.Add(objPri);


                if (representante.Tienevinculosmas5 == "1" && representante.Vinculosmas != null)
                {
                    foreach (var vinculo in representante.Vinculosmas)
                    {
                        InformacionInspektorDto objvinculo = new InformacionInspektorDto();
                        objvinculo.IdFomulario = IdFormulario;
                        objvinculo.Nombre = vinculo.NombreCompleto;
                        objvinculo.Numero_Identificacion = vinculo.NumeroIdentificacion;
                        objvinculo.Tipo_Identificacion = Convert.ToInt32(vinculo.TipoIdentificacion);
                        objvinculo.Tipo_Tercero = "Accionista Vinculado 5%";

                        listainspektorReprelegal.Add(objvinculo);

                    }
                }

                if (representante.InfoFamiliaPep != null)
                {
                    foreach (var familia in representante.InfoFamiliaPep)
                    {
                        InformacionInspektorDto objfamiliao = new InformacionInspektorDto();
                        objfamiliao.IdFomulario = IdFormulario;
                        objfamiliao.Nombre = familia.NombreCompleto;
                        objfamiliao.Numero_Identificacion = familia.NumeroIdentificacion;
                        objfamiliao.Tipo_Identificacion = Convert.ToInt32(familia.TipoIdentificacion);
                        objfamiliao.Tipo_Tercero = "Accionista Familiar";
                        listainspektorReprelegal.Add(objfamiliao);
                    }
                }


                List<BeneficiarioFinal> beneficiariosFinales = new List<BeneficiarioFinal>();

                if (representante.BeneficiariosFinales != null && representante.BeneficiariosFinales.Count > 0)
                {
                    beneficiariosFinales.AddRange(representante.BeneficiariosFinales);
                }
                if ( (beneficiariosFinales !=null ) && (beneficiariosFinales.Count > 0))
                {
                    foreach (var beneficiario in beneficiariosFinales)
                    {
                        InformacionInspektorDto objPribene = new InformacionInspektorDto();
                        objPribene.IdFomulario = IdFormulario;
                        objPribene.Nombre = beneficiario.NombreCompleto;
                        objPribene.Numero_Identificacion = beneficiario.NumeroIdentificacion;
                        objPribene.Tipo_Identificacion = Convert.ToInt32(beneficiario.TipoDocumento);
                        objPribene.Tipo_Tercero = "Beneficiario Accionista";

                        listainspektorReprelegal.Add(objPribene);


                        if (beneficiario.Tienevinculosmas5 == "1" && beneficiario.Vinculosmas != null)
                        {
                            foreach (var vinculo in beneficiario.Vinculosmas)
                            {
                                InformacionInspektorDto objvinculo = new InformacionInspektorDto();
                                objvinculo.IdFomulario = IdFormulario;
                                objvinculo.Nombre = vinculo.NombreCompleto;
                                objvinculo.Numero_Identificacion = vinculo.NumeroIdentificacion;
                                objvinculo.Tipo_Identificacion = Convert.ToInt32(vinculo.TipoIdentificacion);
                                objvinculo.Tipo_Tercero = "Beneficiario Vinculado 5%";

                                listainspektorReprelegal.Add(objvinculo);

                            }
                        }

                        if (beneficiario.InfoFamiliaPep != null)
                        {
                            foreach (var familia in beneficiario.InfoFamiliaPep)
                            {
                                InformacionInspektorDto objfamiliao = new InformacionInspektorDto();
                                objfamiliao.IdFomulario = IdFormulario;
                                objfamiliao.Nombre = familia.NombreCompleto;
                                objfamiliao.Numero_Identificacion = familia.NumeroIdentificacion;
                                objfamiliao.Tipo_Identificacion = Convert.ToInt32(familia.TipoIdentificacion);
                                objfamiliao.Tipo_Tercero = "Beneficiario Familiar";
                                listainspektorReprelegal.Add(objfamiliao);
                            }
                        }
                    }

                }

              }
            return listainspektorReprelegal;

        }




        public async Task<List<InformacionInspektorDto>> listainspektorDatosGenerales(int IdFormulario)
        {
            List<InformacionInspektorDto> listainspektorReprelegal = new List<InformacionInspektorDto>();

            DatosGeneralesDto obj = await _RegistroForm.ConsultaDatosGenerales(IdFormulario);
            string json = obj.PreguntasAdicionales.ToString();

            RootDatosGenerarles data = JsonConvert.DeserializeObject<RootDatosGenerarles>(json);



            if (data.Tienevinculosmas5 == "1" && data.Vinculosmas != null)
            {
                foreach (var vinculo in data.Vinculosmas)
                {
                    InformacionInspektorDto objvinculo = new InformacionInspektorDto();
                    objvinculo.IdFomulario = IdFormulario;
                    objvinculo.Nombre = vinculo.NombreCompleto;
                    objvinculo.Numero_Identificacion = vinculo.NumeroIdentificacion;
                    objvinculo.Tipo_Identificacion = Convert.ToInt32(vinculo.TipoIdentificacion);
                    objvinculo.Tipo_Tercero = "Vinculado 5% Tercero Pep";

                    listainspektorReprelegal.Add(objvinculo);

                }
            }

            if (data.InfoFamiliaPep != null)
            {
                foreach (var familia in data.InfoFamiliaPep)
                {
                    InformacionInspektorDto objfamiliao = new InformacionInspektorDto();
                    objfamiliao.IdFomulario = IdFormulario;
                    objfamiliao.Nombre = familia.NombreCompleto;
                    objfamiliao.Numero_Identificacion = familia.NumeroIdentificacion;
                    objfamiliao.Tipo_Identificacion = Convert.ToInt32(familia.TipoIdentificacion);
                    objfamiliao.Tipo_Tercero = "Familiar Tercero Pep";
                    listainspektorReprelegal.Add(objfamiliao);
                }
            }


            return listainspektorReprelegal;

        }



        public async Task<List<FormularioDto>> ListaFormulariosCompradorVendedor(int IdUsuario)
        {
            return await _RegistroForm.ListaFormulariosCompradorVendedor(IdUsuario);
        }

        public async Task<List<FormularioDto>> ListaFormularios() {
            return await _RegistroForm.ListaFormularios();
        }

        public async Task<List<FormularioDto>> ListaFormulariosUsuarioOEA()
        {
            return await _RegistroForm.ListaFormulariosUsuarioOEA();
        }

        public async Task<List<FormularioDto>> ListaFormulariosContabilidad()
        {
            return await _RegistroForm.ListaFormulariosContabilidad();
        }


        public async Task<List<FormularioDto>> ListaFormulariosControlInterno()
        {
            return await _RegistroForm.ListaFormulariosControlInterno();
        }

        public async Task<List<FormularioDto>> ListaFormulariosOficialCumplimiento()
        {
            return await _RegistroForm.ListaFormulariosOficialCumplimiento();
        }



        public async Task<bool> GuardarDatosGenerales(DatosGeneralesDto objRegistro)
        {
            return await _RegistroForm.GuardarDatosGenerales(objRegistro);
        }

        public async Task<bool> GuardaInformacionContactos(List<DatosContactoDto> objRegistro)
        {
            return await _RegistroForm.GuardaInformacionContactos(objRegistro);
        }

        public async Task<bool> GuardaReferenciaComercialBanc(List<ReferenciaComercialesBancariasDtol> objRegistro)
        {
            return await _RegistroForm.GuardaReferenciaComercialBanc(objRegistro);
        }

        public async Task<bool> GuardaDatosPago(DatosPagosDto objRegistro) {
            return await _RegistroForm.GuardaDatosPago(objRegistro);
        }

        public async Task<bool> GuardaCumplimientoNormativo(CumplimientoNormativoDto objRegistro)
        {
            return await _RegistroForm.GuardaCumplimientoNormativo(objRegistro);
        }

        public async Task<bool> GuardaDespachoMercancia(DespachoMercanciaDto objRegistro)
        {
            return await _RegistroForm.GuardaDespachoMercancia(objRegistro);
        }


        public async Task<DatosGeneralesDto> ConsultaDatosGenerales(int IdFormulario)
        {
            return await _RegistroForm.ConsultaDatosGenerales(IdFormulario);
        }

        public async Task<List<DatosContactoDto>> ListaDatosContacto(int IdFormulario)
        {
            return await _RegistroForm.ListaDatosContacto(IdFormulario);
        }

        public async Task<List<ReferenciaComercialesBancariasDtol>> ListaReferenciasComercialesBan(int IdFormulario)
        {
            return await _RegistroForm.ListaReferenciasComercialesBan(IdFormulario);
        }

        public async Task<DatosPagosDto> ConsultaDatosPago(int IdFormulario) {
            return await _RegistroForm.ConsultaDatosPago(IdFormulario);
        }

        public async Task<CumplimientoNormativoDto> ConsultaCumplimientoNormativo(int IdFormulario)
        {
            return await _RegistroForm.ConsultaCumplimientoNormativo(IdFormulario);
        }

        public async Task<DespachoMercanciaDto> ConsulataDespachoMercancia(int IdFormulario)
        {
            return await _RegistroForm.ConsulataDespachoMercancia(IdFormulario);
        }

        public async Task<bool> GuardaInformacionRepresentantesLegales(int IdFormulario, object objrepresetantes)
        {


            return await _RegistroForm.GuardaInformacionRepresentantesLegales(IdFormulario, objrepresetantes);
        }

        public async Task<object> ConsultaInfoRepresentanteslegales(int IdFormulario) {
            return await _RegistroForm.ConsultaInfoRepresentanteslegales(IdFormulario);
        }



        public async Task<bool> GuardaInformacionAccionistas(int IdFormulario, object accionista)
        {
          

            return await _RegistroForm.GuardaInformacionAccionistas(IdFormulario, accionista);
        }


        public async Task<object> ConsultaInfoAccionistas(int IdFormulario)
        {
            return await _RegistroForm.ConsultaInfoAccionistas(IdFormulario);
        }


        public async Task<bool> GuardaInformacionJuntaDirectiva(int IdFormulario, object objjuntadirectiva)
        {
        

            return await _RegistroForm.GuardaInformacionJuntaDirectiva(IdFormulario, objjuntadirectiva);
        }


        public async Task<object> ConsultaInfoJuntaDirectiva(int IdFormulario)
        {
            return await _RegistroForm.ConsultaInfoJuntaDirectiva(IdFormulario);
        }


        public async Task<List<FormularioDto>> ListaFormulariosbyClienteProveedor(int IdUsuario,string Land)
        {
            return await _RegistroForm.ListaFormulariosbyClienteProveedor(IdUsuario, Land);
        }


        public async Task<bool> GuardaInfoAdjuntos(ArchivoDto objAdjunto)
        {
            return await _RegistroForm.GuardaInfoAdjuntos(objAdjunto);
        }

        public async Task<ArchivoDto> ConsultaInfoArchivo(int IdFormualrio, string Key)
        {
            return await _RegistroForm.ConsultaInfoArchivo(IdFormualrio, Key);
        }

        public async Task<bool> EliminaArchivoBasedatos(int idArchivo)
        {
            return await _RegistroForm.EliminaArchivoBasedatos(idArchivo);
        }

        public async Task<List<ArchivoDto>> ConsultaInfoArchivoCargados(int IdFormualrio)
        {
            return await _RegistroForm.ConsultaInfoArchivoCargados(IdFormualrio);
        }

        public async Task<bool> GuardaMotivoRechazoFormulario(RechazoFormularioDto objRechazo, int IdEstado, int IdUsuario)
        {
            return await _RegistroForm.GuardaMotivoRechazoFormulario(objRechazo, IdEstado, IdUsuario);
        }

        public async Task<RechazoFormularioDto> MuestraMotivoRechazo(int IdFormulario)
        {
            return await _RegistroForm.MuestraMotivoRechazo(IdFormulario);
        }

        public async Task<bool> GuardaConflictoInteres(ConflictoInteresDto objRegistro)
        {
            return await _RegistroForm.GuardaConflictoInteres(objRegistro);
        }
        public async Task<ConflictoInteresDto> ConsultaConflictoInteres(int IdFormulario)
        {
            return await _RegistroForm.ConsultaConflictoInteres(IdFormulario);
        }

        public async Task<bool> GuardaInformacionComplementaria(InformacionComplementariaDto objRegistro)
        {
            return await _RegistroForm.GuardaInformacionComplementaria(objRegistro);
        }

        public async Task<InformacionComplementariaDto> ConsultaInformacionComplementaria(int IdFormulario)
        {
            return await _RegistroForm.ConsultaInformacionComplementaria(IdFormulario);
        }

        public async Task<bool> GuardaInformacionFinanciera(InformacionFinancieraDto objRegistro)
        {
            return await _RegistroForm.GuardaInformacionFinanciera(objRegistro);
        }

        public async Task<InformacionFinancieraDto> ConsultaInformacionFinanciera(int IdFormulario)
        {
            return await _RegistroForm.ConsultaInformacionFinanciera(IdFormulario);
        }

        public async Task<bool> GuardaDatosRevisorFiscal(DatosRevisorFiscalDto objRegistro)
        {
            return await _RegistroForm.GuardaDatosRevisorFiscal(objRegistro);
        }
        public async Task<DatosRevisorFiscalDto> ConsultaDatosRevisorFiscal(int IdFormulario)
        {
            return await _RegistroForm.ConsultaDatosRevisorFiscal(IdFormulario);
        }


        private async Task<string> ConsultaInspektor(InformacionInspektorDto objInpektor,string authToken)
        {
            string Url = _configuration["Inspektor:UrlConsulta"].ToString();
           // string authToken = await ObtieneTockenConsultaInspektor();


            var peticion = new PeticionInspektorDTO
            {
                nombre = objInpektor.Nombre,
                tipoDocumento = objInpektor.Tipo_Identificacion,
                identificacion = objInpektor.Numero_Identificacion,
                cantidadPalabras = "1",
                tienePrioridad_4 = true,
                procuraduria=false,
                ramaJudicial = false,
                ramaJEPMS = false
            };

            string responseContent;
            string jsonContent = JsonSerializer.Serialize(peticion);

            using (HttpClient client = new HttpClient())
            {
                // Configurar encabezados
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                // Crear el contenido de la solicitud
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    // Hacer la solicitud POST
                    HttpResponseMessage response = await client.PostAsync(Url, content);

                    // Validar la respuesta
                    if (response.IsSuccessStatusCode)
                    {
                         responseContent = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        LogError($"Error consultando en inspektor linea 895 Negocios: {response.StatusCode}");
                         responseContent = await response.Content.ReadAsStringAsync();
                    }
                }
                catch (Exception ex)
                {
                    responseContent=($"Error al realizar la solicitud: {ex.Message}");
                    LogError($"Error al realizar la solicitud: {ex.Message}");
                }
            }
            return responseContent;

        }


        public async Task<string> ObtieneTockenConsultaInspektor()
        {
            string Url = _configuration["Inspektor:UrlConsultaCredencialesInspektor"].ToString();
            string authToken = "";

            string UsuarioInspektor = _configuration["Inspektor:userinspektor"].ToString();
            string PasswordInpektor = _configuration["Inspektor:PasswordInspektpr"].ToString();

            var peticion = new UserInspektorDto
            {
                user = UsuarioInspektor,
                password = PasswordInpektor,

            };

            string responseContent;
            string jsonContent = JsonSerializer.Serialize(peticion);

            using (HttpClient client = new HttpClient())
            {
                // Configurar encabezados

                // Crear el contenido de la solicitud
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    // Hacer la solicitud POST
                    HttpResponseMessage response = await client.PostAsync(Url, content);

                    // Validar la respuesta
                    if (response.IsSuccessStatusCode)
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                        // Extraer solo el `access_token` del JSON
                        var document = JsonDocument.Parse(responseContent);
                        authToken = document.RootElement.GetProperty("token").GetProperty("access_token").GetString();
                    }
                    else
                    {
                        LogError($"Error solicitado token para inspektor linea 951 Negocios: {response.StatusCode}");
                        responseContent = await response.Content.ReadAsStringAsync();
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error solicitado token para inspektor: {ex.Message}");

                }
            }
            return authToken;

        }

        public async Task CosnultaListasAdicionales(List<InformacionInspektorDto> ListaInspektor, string Tocken)
        {

            foreach (InformacionInspektorDto obj in ListaInspektor)
            {
                ListasAdicionalesDto objetofiltro = new ListasAdicionalesDto();
                objetofiltro.TipoIdentificacion = obj.Tipo_Identificacion.ToString();
                objetofiltro.NombreCompleto = obj.Nombre;
                objetofiltro.NumeroIdentificacion = obj.Numero_Identificacion;
                objetofiltro.Tipo_Identificacion = obj.Tipo_Identificacion;
                objetofiltro.NumeroConsulta = obj.Numero_Consulta;
                object jsonConsultaProcuraduria = await ConsultaInformacionProcuraduria(objetofiltro, Tocken);
                object jsonConsultaRamaJudicial = await ConsultaInformacionRamaJudicial(objetofiltro, Tocken);
                object jsonEjecucionPenas = await ConsultaInformacionEjecucionPenas(objetofiltro, Tocken);
                await GuardaInfoServiciosAdicionales(obj.IdFomulario, obj.Numero_Consulta, jsonConsultaProcuraduria, jsonConsultaRamaJudicial, jsonEjecucionPenas);
            }

        }

        public async Task GuardaInfoServiciosAdicionales(int IdFormulario, string NumeroConsulta, object jsonProcuraduria, object jsonRamaJudicial, object ejecucionPenas)
        {
            await _RegistroForm.GuardaInfoServiciosAdicionales(IdFormulario, NumeroConsulta, jsonProcuraduria, jsonRamaJudicial, ejecucionPenas);
        }


        public async Task<List<InformacionInspektorDto>> ListaResultadosInspektor(int IdFormulario)
        {
            return await _RegistroForm.ListaResultadosInspektor(IdFormulario);
        }



        public async Task EnviarNotificacionClienteEnviaForm(int IdFormulario,string path)
        {
            string CorreoVendedorComprador = string.Empty;
            string Usuario = string.Empty;
            EmailMessageRequestDto objetomail = new EmailMessageRequestDto();
            List<string> destinatarios = new List<string>();

            List<string> listaAdjuntos = new List<string>();
            listaAdjuntos.Add(path);
            destinatarios = new List<string>() { "m.gonzalez@riskgc.com" };

            //  destinatarios = await _RegistroForm.CorreosEnvioFormualrio(IdFormulario);

            UserFormInformationDTO usuarioform = new UserFormInformationDTO();
            usuarioform = await _RegistroForm.Userinfo(IdFormulario);


            HashSet<string> unicos = new HashSet<string>(destinatarios);

            // Convertir de nuevo a lista si es necesario
            destinatarios = unicos.ToList();

            string cuerpoCorreo = @"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            .email-body {
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333;
            }
            .email-header {
                background-color: #0715b0;
                color: white;
                padding: 10px;
                text-align: center;
            }
            .email-content {
                padding: 20px;
            }
            .email-footer {
                background-color: #f1f1f1;
                padding: 10px;
                text-align: center;
                color: #555;
            }
            .highlight {
                color: #0715b0;
                font-weight: bold;
            }
        </style>
    </head>
    <body class='email-body'>
        <div class='email-header'>
            <h1>Envio de Formulario para Validacion {Cliente} </h1>
        </div>
        <div class='email-content'>
            <p>Cordial saludo</p>
            <p>Se ha registrado con éxito la información en la plataforma para revisión y aprobación.</p>
            <p>Tercero:{Cliente} </p>
            <p>Identificacion:{Identificacion} </p>
               <br>
             <p>Por favor ingrese al aplicativo <span class='highlight'>{plataforma}</span> para verificarlas y gestionar el formulario.</p>
            <p>Muchas gracias.</p
        </div>
        <div class='email-footer'>
            <p>&copy; 2024 Risk. Todos los derechos reservados.</p>
        </div>
    </body>
    </html>";

            cuerpoCorreo = cuerpoCorreo.Replace("{IdFormulario}", IdFormulario.ToString())
                           .Replace("{plataforma}", "https://ambientetest.datalaft.com:2198/")
                           .Replace("{Cliente}", String.Concat(usuarioform.Nombre,' ', usuarioform.Apellido)).Replace("{Identificacion}", usuarioform.Identificacion);


            string Subject = "Alerta de Registro {Cliente}";
            Subject= Subject.Replace("{Cliente}", String.Concat(usuarioform.Nombre, ' ', usuarioform.Apellido));

            objetomail.To = destinatarios;
            objetomail.Subject = Subject;
            objetomail.Body = cuerpoCorreo;
            objetomail.AttachmentsRoutes = listaAdjuntos;

            try
            {
                var corro = await _iEMailService.SendMail(objetomail);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("error al enviar correo");
            }
        }



        public async Task EnviaNotificacionFormAprobadoERP(int IdFormulario)
        {
            string CorreoVendedorComprador = string.Empty;
            string Usuario = string.Empty;
            EmailMessageRequestDto objetomail = new EmailMessageRequestDto();
            List<string> destinatarios = new List<string>();
            destinatarios = new List<string>() { "m.gonzalez@riskgc.com" };


           // destinatarios = await _RegistroForm.CorreosEnvioFormualrio(IdFormulario);

            UserFormInformationDTO usuarioform = new UserFormInformationDTO();
            usuarioform = await _RegistroForm.Userinfo(IdFormulario);


            HashSet<string> unicos = new HashSet<string>(destinatarios);

            // Convertir de nuevo a lista si es necesario
            destinatarios = unicos.ToList();

            string cuerpoCorreo = @"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            .email-body {
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333;
            }
            .email-header {
                background-color: #0715b0;
                color: white;
                padding: 10px;
                text-align: center;
            }
            .email-content {
                padding: 20px;
            }
            .email-footer {
                background-color: #f1f1f1;
                padding: 10px;
                text-align: center;
                color: #555;
            }
            .highlight {
                color: #0715b0;
                font-weight: bold;
            }
        </style>
    </head>
    <body class='email-body'>
        <div class='email-header'>
            <h1>EL Formulario del {Cliente} Fue Aprobado</h1>
        </div>
        <div class='email-content'>
            <p>Cordial saludo</b></p>
            <p>El formulario #<span class='highlight'>{IdFormulario}</span> Aprobado </p>
            <p>Muchas gracias.</p>
        </div>
        <div class='email-footer'>
            <p>&copy; 2024 Risk. Todos los derechos reservados.</p>
        </div>
    </body>
    </html>";

            cuerpoCorreo = cuerpoCorreo.Replace("{IdFormulario}", IdFormulario.ToString())
                           .Replace("{plataforma}", "https://ambientetest.datalaft.com:2198/")
                           .Replace("{Cliente}", String.Concat(usuarioform.Nombre, ' ', usuarioform.Apellido));


            objetomail.To = destinatarios;
            objetomail.Subject = "Formulario Enviado";
            objetomail.Body = cuerpoCorreo;

            try
            {
                var corro = await _iEMailService.SendMail(objetomail);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("error al enviar correo");
            }
        }


        public async Task EnviarNotificacionPendienteCorrecciones(int IdFormulario, RechazoFormularioDto objRechazo)
        {
            string CorreoVendedorComprador = string.Empty;
            string Usuario = string.Empty;
            EmailMessageRequestDto objetomail = new EmailMessageRequestDto();
            List<string> destinatarios = new List<string>();


            destinatarios = await _RegistroForm.CorreosCorreccionFormulario(IdFormulario);


            UserFormInformationDTO usuarioform = new UserFormInformationDTO();
            usuarioform = await _RegistroForm.Userinfo(IdFormulario);


            HashSet<string> unicos = new HashSet<string>(destinatarios);

            // Convertir de nuevo a lista si es necesario
            destinatarios = unicos.ToList();

            string cuerpoCorreo = @"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            .email-body {
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333;
            }
            .email-header {
                background-color: #0715b0;
                color: white;
                padding: 10px;
                text-align: center;
            }
            .email-content {
                padding: 20px;
            }
            .email-footer {
                background-color: #f1f1f1;
                padding: 10px;
                text-align: center;
                color: #555;
            }
            .highlight {
                color: #0715b0;
                font-weight: bold;
            }
        </style>
    </head>
    <body class='email-body'>
        <div class='email-header'>
            <h1>Solicitud de Correccion de Formulario</h1>
        </div>
        <div class='email-content'>
            <p>Cordial saludo</b></p>
            <p>El formulario #<span class='highlight'>{IdFormulario}</span> se encuentra pendiente de correccion; Las siguentes son las novedades encontradas: </p><br>
             <p>{MotivoRechazo}</p>
               <br>
             <p>Por favor ingrese al aplicativo <span class='highlight'>{plataforma}</span> para verificarlas y gestionar el formulario.</p>
            <p>Muchas gracias.</p>
        </div>
        <div class='email-footer'>
            <p>&copy; 2024 Risk. Todos los derechos reservados.</p>
        </div>
    </body>
    </html>";

            cuerpoCorreo = cuerpoCorreo.Replace("{IdFormulario}", IdFormulario.ToString())
                           .Replace("{plataforma}", "https://ambientetest.datalaft.com:2198/")
                           .Replace("{MotivoRechazo}", objRechazo.MotivoRechazo);


            string Subject = "Alerta de Registro {Cliente}";
            Subject = Subject.Replace("{Cliente}", String.Concat(usuarioform.Nombre, ' ', usuarioform.Apellido));


            objetomail.To = destinatarios;
            objetomail.Subject = Subject;
            objetomail.Body = cuerpoCorreo;

            try
            {
                var corro = await _iEMailService.SendMail(objetomail);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("error al enviar correo");
            }
        }


        public async Task NotificacionFormularioCorregidoClienteForm(int IdFormulario, string path)
        {
            string CorreoVendedorComprador = string.Empty;
            string Usuario = string.Empty;
            EmailMessageRequestDto objetomail = new EmailMessageRequestDto();
            List<string> destinatarios = new List<string>();

            List<string> listaAdjuntos = new List<string>();
            listaAdjuntos.Add(path);

            destinatarios = await _RegistroForm.CorreosEnvioFormualrio(IdFormulario);

            UserFormInformationDTO usuarioform = new UserFormInformationDTO();
            usuarioform = await _RegistroForm.Userinfo(IdFormulario);

            HashSet<string> unicos = new HashSet<string>(destinatarios);

            // Convertir de nuevo a lista si es necesario
            destinatarios = unicos.ToList();

            string cuerpoCorreo = @"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            .email-body {
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333;
            }
            .email-header {
                background-color: #0715b0;
                color: white;
                padding: 10px;
                text-align: center;
            }
            .email-content {
                padding: 20px;
            }
            .email-footer {
                background-color: #f1f1f1;
                padding: 10px;
                text-align: center;
                color: #555;
            }
            .highlight {
                color: #0715b0;
                font-weight: bold;
            }
        </style>
    </head>
    <body class='email-body'>
        <div class='email-header'>
            <h1>Correccion de Formulario para Validacion</h1>
        </div>
        <div class='email-content'>
            <p>Cordial saludo</b></p>
            <p>El formulario #<span class='highlight'>{IdFormulario}</span> del cliente <span class='highlight'>{Cliente}</span> fue Corregido y Enviado a la plataforma <span class='highlight'>{plataforma}</span> para su respectiva validacion y aprobación</p>
            <p>Muchas gracias.</p>
        </div>
        <div class='email-footer'>
            <p>&copy; 2024 Risk. Todos los derechos reservados.</p>
        </div>
    </body>
    </html>";

            cuerpoCorreo = cuerpoCorreo.Replace("{IdFormulario}", IdFormulario.ToString())
                           .Replace("{plataforma}", "https://ambientetest.datalaft.com:2198/").Replace("{Cliente}", String.Concat(usuarioform.Nombre, ' ', usuarioform.Apellido)); ;

            string Subject = "Formulario Corregido y Enviado {Cliente}";
            Subject = Subject.Replace("{Cliente}", String.Concat(usuarioform.Nombre, ' ', usuarioform.Apellido));

            objetomail.To = destinatarios;
            objetomail.Subject = Subject;
            objetomail.Body = cuerpoCorreo;
            objetomail.AttachmentsRoutes = listaAdjuntos;

            try
            {
                var corro = await _iEMailService.SendMail(objetomail);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("error al enviar correo");
            }
        }


        public async Task EnviarNotificacionCohincidenciasListasControlInterno(int IdFormulario, string coincidencias)
        {
            string CorreoVendedorComprador = string.Empty;
            string Usuario = string.Empty;
            EmailMessageRequestDto objetomail = new EmailMessageRequestDto();
            List<string> destinatarios = new List<string>();


            destinatarios = await _RegistroForm.CorreosControlInterno();

            UserFormInformationDTO usuarioform = new UserFormInformationDTO();
            usuarioform = await _RegistroForm.Userinfo(IdFormulario);

            HashSet<string> unicos = new HashSet<string>(destinatarios);

            // Convertir de nuevo a lista si es necesario
            destinatarios = unicos.ToList();

            string cuerpoCorreo = @"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            .email-body {
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333;
            }
            .email-header {
                background-color: #0715b0;
                color: white;
                padding: 10px;
                text-align: center;
            }
            .email-content {
                padding: 20px;
            }
            .email-footer {
                background-color: #f1f1f1;
                padding: 10px;
                text-align: center;
                color: #555;
            }
            .highlight {
                color: #0715b0;
                font-weight: bold;
            }
        </style>
    </head>
    <body class='email-body'>
        <div class='email-header'>
            <h1>Coincidencias en consultas en listas en formulario  Cliente:{Cliente}</h1>
        </div>
        <div class='email-content'>
            <p>Cordial saludo</b></p>
            <p>En el Formulario #<span class='highlight'>{IdFormulario}</span> del cliente <span class='highlight'>{Cliente}</span>  se encontraron {coincidencias} coincidencias en listas,  se informa a control interno para la respectiva validacion</p><br>
             
             <p>Por favor ingrese a la plataforma <span class='highlight'>{plataforma}</span> y gestione el Fomulario.</p>
            <p>Muchas gracias.</p>
        </div>
        <div class='email-footer'>
            <p>&copy; 2024 Risk. Todos los derechos reservados.</p>
        </div>
    </body>
    </html>";

            cuerpoCorreo = cuerpoCorreo.Replace("{IdFormulario}", IdFormulario.ToString())
                           .Replace("{plataforma}", "https://ambientetest.datalaft.com:2198/")
                           .Replace("{coincidencias}", coincidencias).Replace("{Cliente}", String.Concat(usuarioform.Nombre, ' ', usuarioform.Apellido));

            string Subject = "Coincidencias Consulta en listas {Cliente}";
            Subject = Subject.Replace("{Cliente}", String.Concat(usuarioform.Nombre, ' ', usuarioform.Apellido));

            objetomail.To = destinatarios;
            objetomail.Subject = Subject;
            objetomail.Body = cuerpoCorreo;

            try
            {
                var corro = await _iEMailService.SendMail(objetomail);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("error al enviar correo");
            }
        }


        public async Task EnviarNotificacionRechazadoporControlInterno(int IdFormulario, RechazoFormularioDto objRechazo)
        {
            string CorreoVendedorComprador = string.Empty;
            string Usuario = string.Empty;
            EmailMessageRequestDto objetomail = new EmailMessageRequestDto();
            List<string> destinatarios = new List<string>();

            destinatarios = await _RegistroForm.CorreosOficialCumplimineto();
            UserFormInformationDTO usuarioform = new UserFormInformationDTO();
            usuarioform = await _RegistroForm.Userinfo(IdFormulario);

            HashSet<string> unicos = new HashSet<string>(destinatarios);

            // Convertir de nuevo a lista si es necesario
            destinatarios = unicos.ToList();

            string cuerpoCorreo = @"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            .email-body {
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333;
            }
            .email-header {
                background-color: #0715b0;
                color: white;
                padding: 10px;
                text-align: center;
            }
            .email-content {
                padding: 20px;
            }
            .email-footer {
                background-color: #f1f1f1;
                padding: 10px;
                text-align: center;
                color: #555;
            }
            .highlight {
                color: #0715b0;
                font-weight: bold;
            }
        </style>
    </head>
    <body class='email-body'>
        <div class='email-header'>
            <h1>Rechazo Formulario Control Interno cliente {Cliente}</h1>
        </div>
        <div class='email-content'>
            <p>Cordial saludo</b></p>
            <p>El formulario #<span class='highlight'>{IdFormulario}</span> del cliente <span class='highlight'>{Cliente}</span> fue rechazado por control interno; Los siguentes son los motivos del rechazo: </p><br>
             <p>{MotivoRechazo}</p>
               <br>
             <p>Por favor ingrese a la plataforma <span class='highlight'>{plataforma}</span> y gestione el Fomulario.</p>
            <p>Muchas gracias.</p>
        </div>
        <div class='email-footer'>
            <p>&copy; 2024 Risk. Todos los derechos reservados.</p>
        </div>
    </body>
    </html>";

            cuerpoCorreo = cuerpoCorreo.Replace("{IdFormulario}", IdFormulario.ToString())
                           .Replace("{plataforma}", "https://ambientetest.datalaft.com:2198/")    
                           .Replace("{MotivoRechazo}", objRechazo.MotivoRechazo).Replace("{Cliente}", String.Concat(usuarioform.Nombre, ' ', usuarioform.Apellido));

            string Subject = "Formulario Rechazado Cliente: {Cliente}";
            Subject = Subject.Replace("{Cliente}", String.Concat(usuarioform.Nombre, ' ', usuarioform.Apellido));

            objetomail.To = destinatarios;
            objetomail.Subject = Subject;
            objetomail.Body = cuerpoCorreo;

            try
            {
                var corro = await _iEMailService.SendMail(objetomail);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("error al enviar correo");
            }
        }

        public async Task EnviarNotificacionTerceroRechazadoOficialCumplimineto(int IdFormulario, RechazoFormularioDto objRechazo)
        {
            string CorreoVendedorComprador = string.Empty;
            string Usuario = string.Empty;
            EmailMessageRequestDto objetomail = new EmailMessageRequestDto();

            List<string> ControlInterno = new List<string>();
            ControlInterno = await _RegistroForm.CorreosControlInterno();

            List<string> ListaDestinatariosPrincipales = new List<string>();
            ListaDestinatariosPrincipales =  await _RegistroForm.CorreosEnvioFormualrio(IdFormulario);

            List<string> todosDestinatarios = new List<string>();
            todosDestinatarios.AddRange(ControlInterno);
            todosDestinatarios.AddRange(ListaDestinatariosPrincipales);

            UserFormInformationDTO usuarioform = new UserFormInformationDTO();
            usuarioform = await _RegistroForm.Userinfo(IdFormulario);

            HashSet<string> unicos = new HashSet<string>(todosDestinatarios);

            todosDestinatarios = unicos.ToList();

            string cuerpoCorreo = @"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            .email-body {
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333;
            }
            .email-header {
                background-color: #0715b0;
                color: white;
                padding: 10px;
                text-align: center;
            }
            .email-content {
                padding: 20px;
            }
            .email-footer {
                background-color: #f1f1f1;
                padding: 10px;
                text-align: center;
                color: #555;
            }
            .highlight {
                color: #0715b0;
                font-weight: bold;
            }
        </style>
    </head>
    <body class='email-body'>
        <div class='email-header'>
            <h1>El tercero fue rechazado para crearse en el ERP de la Compañía. cliente {Cliente}</h1>
        </div>
        <div class='email-content'>
            <p>Cordial saludo</b></p>
            <p>El tercero <span class='highlight'>{Cliente}</span> fue rechazado por el oficial de cumplimiento para crearse en el ERP; Los siguentes son los motivos del rechazo: </p><br>
             <p>{MotivoRechazo}</p>
               <br>
             <p>Por favor ingrese a la plataforma <span class='highlight'>{plataforma}</span> y gestione el Fomulario.</p>
            <p>Muchas gracias.</p>
        </div>
        <div class='email-footer'>
            <p>&copy; 2024 Risk. Todos los derechos reservados.</p>
        </div>
    </body>
    </html>";

            cuerpoCorreo = cuerpoCorreo.Replace("{IdFormulario}", IdFormulario.ToString())
                           .Replace("{plataforma}", "https://ambientetest.datalaft.com:2198/")
                           .Replace("{MotivoRechazo}", objRechazo.MotivoRechazo).Replace("{Cliente}", String.Concat(usuarioform.Nombre, ' ', usuarioform.Apellido));

            string Subject = "Vinculacion Rechazada  - {Cliente}";
            Subject = Subject.Replace("{Cliente}", String.Concat(usuarioform.Nombre, ' ', usuarioform.Apellido));


            objetomail.To = todosDestinatarios;
            objetomail.Subject = Subject;
            objetomail.Body = cuerpoCorreo;

            try
            {
                var corro = await _iEMailService.SendMail(objetomail);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("error al enviar correo");
            }
        }

        public async Task<FormularioDto> InfoFormulario(int IdFormualrio)
        {
            return await _RegistroForm.InfoFormulario(IdFormualrio);
        }

        public async Task<bool> GuardarInformacionOEA(FormularioModelDTO objRegistro, int IdUsuario)
        {
            return await _RegistroForm.GuardarInformacionOEA(objRegistro, IdUsuario);
        }

        public async Task<FormularioModelDTO> ConsultaDatosInformacionOEA(int IdFormulario)
        {
            return await _RegistroForm.ConsultaDatosInformacionOEA(IdFormulario);
        }


        public async Task<bool> GuardarDeclaraciones(DeclaracionesDto objRegistro)
        {
            return await _RegistroForm.GuardarDeclaraciones(objRegistro);
        }

        public async Task<DeclaracionesDto> ConsultaDeclaraciones(int IdFormulario)
        {
            return await _RegistroForm.ConsultaDeclaraciones(IdFormulario);
        }


        public async Task<bool> GuardarInformacionTriburaria(InformacionTributariaDTO objRegistro)
        {
            return await _RegistroForm.GuardarInformacionTriburaria(objRegistro);
        }


        public async Task<InformacionTributariaDTO> ConsultaInformacionTributaria(int IdFormulario)
        {
            return await _RegistroForm.ConsultaInformacionTributaria(IdFormulario);
        }



        public async Task<MemoryStream> ConsultaRamaJudicial(ListasAdicionalesDto objetofiltro)
        {
            MemoryStream memoryStream = new MemoryStream();


            object xxx = await _RegistroForm.ConsultaRamaJudicial(objetofiltro);

            if (xxx == null)
            {
                return await SinResultados(objetofiltro, "Rama Judicial");
            }

            List<ProcesosRamaJudicialDto> objeto = JsonSerializer.Deserialize<List<ProcesosRamaJudicialDto>>(xxx.ToString());


            PdfWriter writer = new PdfWriter(memoryStream);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, iText.Kernel.Geom.PageSize.A4, false);


            document.SetMargins(28.35f, 28.35f, 28.35f, 28.35f);

            PdfFont boldFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

            // Título del reporte
            document.Add(new Paragraph("Resultados del Proceso Rama Judicial").SetFontSize(18).SetFont(boldFont).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));

            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 30, 15, 25, 10, 10 })).UseAllAvailableWidth();

            Style headerStyle = new Style()
                .SetBackgroundColor(ColorConstants.BLUE)
                .SetFontColor(ColorConstants.WHITE)
                .SetFont(boldFont)
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER);

            // Estilo de las celdas de la tabla
            Style cellStyle = new Style()
                .SetFontSize(8) // Reducir el tamaño de la fuente para ajustar el contenido
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5)
                .SetBorder(new SolidBorder(0.5f))
                .SetMaxWidth(UnitValue.CreatePercentValue(100)); // Añadir ajuste de ancho máximo


            table.AddHeaderCell(new Cell().Add(new Paragraph("Id Proceso")).AddStyle(headerStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Sujetos Procesales")).AddStyle(headerStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Departamento")).AddStyle(headerStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Despacho")).AddStyle(headerStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha del Proceso")).AddStyle(headerStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha última actualización")).AddStyle(headerStyle));

            foreach (ProcesosRamaJudicialDto proceso in objeto)
            {
                try
                {

                    table.AddCell(new Cell().Add(new Paragraph(proceso.idProceso.ToString())).AddStyle(cellStyle));
                    table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(proceso.sujetosProcesales) ? proceso.sujetosProcesales : "")).AddStyle(cellStyle));
                    table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(proceso.departamento) ? proceso.departamento : "")).AddStyle(cellStyle));
                    table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(proceso.despacho) ? proceso.despacho : "")).AddStyle(cellStyle));
                    table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(proceso.fechaProceso) ? proceso.fechaProceso : "")).AddStyle(cellStyle));
                    table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(proceso.fechaUltimaActuacion) ? proceso.fechaUltimaActuacion : "")).AddStyle(cellStyle));

                }
                catch (Exception ex)
                {

                }

            }

            document.Add(table);

            document.Close();
            pdf.Close();
            writer.Close();



            return await Task.FromResult(memoryStream);
        }

        public async Task<object> ConsultaEjecucionPenas(ListasAdicionalesDto objRegistro)

        {
            string Consulta = string.Empty;
            object objetojson = null;
            string where = String.Format("Where IdFormulario={0} and [Numero_Consulta]='{1}' and Numero_Identificacion='{2}' ", objRegistro.IdFormulario, objRegistro.NumeroConsulta, objRegistro.NumeroIdentificacion);


            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[Tipo_Tercero] ,[Tipo_Identificacion] ,[Numero_Identificacion],[Nombre],[Numero_Consulta] ,[Coincidencias] ,[Fecha_Consulta],[JsonInfoProcuraduria] ,[JsonRamaJudicial],[JsonEjecucionPenas] FROM [dbo].[inspektor_respuesta_ws][Numero_Consulta] {0}", where);


            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                objetojson = !string.IsNullOrEmpty(dtInformacion.Rows[0]["JsonEjecucionPenas"].ToString()) ? dtInformacion.Rows[0]["JsonEjecucionPenas"] : null;

                return objetojson;
            }

            return objetojson;

        }

        public async Task<MemoryStream> SinResultados(ListasAdicionalesDto objetofiltro, string servicio)
        {
            MemoryStream memoryStream = new MemoryStream();

            PdfWriter writer = new PdfWriter(memoryStream);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, iText.Kernel.Geom.PageSize.A4, false);

            document.SetMargins(28.35f, 28.35f, 28.35f, 28.35f);

            PdfFont boldFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

            // Título del informe
            document.Add(new Paragraph($"Resultados de la búsqueda en {servicio}").SetFontSize(18).SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER));

            // Mensaje informativo
            document.Add(new Paragraph($"Usuario con Nombre {objetofiltro.NombreCompleto} y  número de identificación {objetofiltro.NumeroIdentificacion}: No se encontraron resultados en la búsqueda realizada con fecha {objetofiltro.FechaConsulta}.")
                .SetFontSize(14)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Close();
            pdf.Close();
            writer.Close();

            return await Task.FromResult(memoryStream);

        }


        public async Task<object> ConsultaInformacionRamaJudicial(ListasAdicionalesDto objetofiltro, string authToken)
        {
            string baseUrl = _configuration["ConsultasAdicionales:GLOBAL_WS_CONSULTA_INFORMACION_RAMA_JUDICIAL_URL"];

            var peticion = new PeticionServiciosAdicionalesDTO
            {
                name = objetofiltro.NombreCompleto,
                typeDocument = objetofiltro.Tipo_Identificacion,
                identification = objetofiltro.NumeroIdentificacion,
            };

            string responseContent;
            string jsonContent = JsonSerializer.Serialize(peticion);
            using (HttpClient client = new HttpClient())
            {
                // Configurar encabezados
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                // client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    // Hacer la solicitud POST
                    HttpResponseMessage response = await client.PostAsync(baseUrl, content);

                    // Validar la respuesta
                    if (response.IsSuccessStatusCode)
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                        var data = JsonSerializer.Deserialize<object>(responseContent);
                        return data;
                    }
                    else
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                        LogError($"Error ConsultaInformacionRamaJudicial(): {responseContent} para el cliente {peticion.name} con numero de documento {peticion.identification} este es el resultado: {responseContent}");
                        //var data = JsonSerializer.Deserialize<object>(responseContent);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al realizar la solicitud: {ex.Message}");
                    return null;
                }
            }
        }






        public async Task<MemoryStream> ConsultaInfoProcuraduria(ListasAdicionalesDto objetofiltro)
        {

            object json = await _RegistroForm.ConsultaInfoProcuraduria(objetofiltro);

            if (json == null)
            {
                return await SinResultados(objetofiltro, "Procuraduria");

            }


            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Hace que coincidan nombres sin importar mayúsculas/minúsculas
            };

            var jsonData = JsonSerializer.Deserialize<RootProcuraduria>((string)json, options);

            var data = jsonData.Data;


            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = new PdfWriter(memoryStream);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, iText.Kernel.Geom.PageSize.A4, false);

            // Configurar márgenes
            document.SetMargins(28.35f, 28.35f, 28.35f, 28.35f);

            // Crear una fuente negrita
            PdfFont boldFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

            // Título del reporte
            document.Add(new Paragraph("Resultados Procuraduria").SetFont(boldFont).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));

            // Información del ciudadano
            if (data != null && data.Data.Count > 0)
            {
                var ciudadano = data.Data[0];

                string numSiri = ciudadano.NumSiri != null ? ciudadano.NumSiri.Trim() : "";
                string Name = ciudadano.Name != null ? ciudadano.Name.Trim() : "";
                string Identification = ciudadano.Identification != null ? ciudadano.Identification.Trim() : "";
                document.Add(new Paragraph($"Nombre: {Name}"));
                document.Add(new Paragraph($"Identificación: {Identification}"));
                document.Add(new Paragraph($"Número SIRI: {numSiri}"));

                // Añadir sección de sanciones
                if (ciudadano.Sanciones != null && ciudadano.Sanciones.Count > 0)
                {
                    document.Add(new Paragraph("Sanciones").SetFont(boldFont).SetFontSize(14).SetMarginTop(10));
                    Table tableSanciones = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 })).UseAllAvailableWidth();

                    // Estilo de la cabecera de la tabla
                    Style headerStyle = new Style()
                        .SetBackgroundColor(ColorConstants.BLUE)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetFont(boldFont)
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.CENTER);

                    // Estilo de las celdas de la tabla
                    Style cellStyle = new Style()
                        .SetFontSize(8)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetPadding(5)
                        .SetBorder(new SolidBorder(0.5f));

                    // Agregar cabecera de la tabla con estilo
                    tableSanciones.AddHeaderCell(new Cell().Add(new Paragraph("Sanción")).AddStyle(headerStyle));
                    tableSanciones.AddHeaderCell(new Cell().Add(new Paragraph("Término")).AddStyle(headerStyle));
                    tableSanciones.AddHeaderCell(new Cell().Add(new Paragraph("Clase")).AddStyle(headerStyle));
                    tableSanciones.AddHeaderCell(new Cell().Add(new Paragraph("Suspendida")).AddStyle(headerStyle));

                    // Agregar datos de las sanciones
                    foreach (var sancion in ciudadano.Sanciones)
                    {
                        tableSanciones.AddCell(new Cell().Add(new Paragraph(sancion.sancion ?? "")).AddStyle(cellStyle));
                        tableSanciones.AddCell(new Cell().Add(new Paragraph(sancion.termino ?? "")).AddStyle(cellStyle));
                        tableSanciones.AddCell(new Cell().Add(new Paragraph(sancion.clase ?? "")).AddStyle(cellStyle));
                        tableSanciones.AddCell(new Cell().Add(new Paragraph(sancion.suspendida ?? "")).AddStyle(cellStyle));
                    }

                    document.Add(tableSanciones);
                }

                if (ciudadano.Delitos != null && ciudadano.Delitos.Count > 0)
                {
                    document.Add(new Paragraph("Delitos").SetFont(boldFont).SetFontSize(14).SetMarginTop(10));
                    Table tabledelitos = new Table(UnitValue.CreatePercentArray(new float[] { 25 })).UseAllAvailableWidth();

                    // Estilo de la cabecera de la tabla
                    Style headerStyle = new Style()
                        .SetBackgroundColor(ColorConstants.BLUE)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetFont(boldFont)
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.CENTER);

                    // Estilo de las celdas de la tabla
                    Style cellStyle = new Style()
                        .SetFontSize(8)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetPadding(5)
                        .SetBorder(new SolidBorder(0.5f));

                    // Agregar cabecera de la tabla con estilo
                    tabledelitos.AddHeaderCell(new Cell().Add(new Paragraph("Delitos")).AddStyle(headerStyle));


                    // Agregar datos de las sanciones
                    foreach (var delito in ciudadano.Delitos)
                    {
                        tabledelitos.AddCell(new Cell().Add(new Paragraph(delito.Descripcion)).AddStyle(cellStyle));

                    }

                    document.Add(tabledelitos);
                }

                if (ciudadano.Instancias != null && ciudadano.Instancias.Count > 0)
                {
                    document.Add(new Paragraph("Instancias").SetFont(boldFont).SetFontSize(14).SetMarginTop(10));
                    Table tablainstancias = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 })).UseAllAvailableWidth();

                    // Estilo de la cabecera de la tabla
                    Style headerStyle = new Style()
                        .SetBackgroundColor(ColorConstants.BLUE)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetFont(boldFont)
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.CENTER);

                    // Estilo de las celdas de la tabla
                    Style cellStyle = new Style()
                        .SetFontSize(8)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetPadding(5)
                        .SetBorder(new SolidBorder(0.5f));

                    // Agregar cabecera de la tabla con estilo
                    tablainstancias.AddHeaderCell(new Cell().Add(new Paragraph("nombre")).AddStyle(headerStyle));
                    tablainstancias.AddHeaderCell(new Cell().Add(new Paragraph("autoridad")).AddStyle(headerStyle));
                    tablainstancias.AddHeaderCell(new Cell().Add(new Paragraph("fecha_provincia")).AddStyle(headerStyle));
                    tablainstancias.AddHeaderCell(new Cell().Add(new Paragraph("fecha_efecto_juridicos")).AddStyle(headerStyle));

                    // Agregar datos de las sanciones
                    foreach (var Instancia in ciudadano.Instancias)
                    {
                        tablainstancias.AddCell(new Cell().Add(new Paragraph(Instancia.Nombre ?? "")).AddStyle(cellStyle));
                        tablainstancias.AddCell(new Cell().Add(new Paragraph(Instancia.Autoridad ?? "")).AddStyle(cellStyle));
                        tablainstancias.AddCell(new Cell().Add(new Paragraph(Instancia.fecha_provincia ?? "")).AddStyle(cellStyle));
                        tablainstancias.AddCell(new Cell().Add(new Paragraph(Instancia.fecha_efecto_juridicos ?? "")).AddStyle(cellStyle));

                    }

                    document.Add(tablainstancias);
                }


                if (ciudadano.Eventos != null && ciudadano.Eventos.Count > 0)
                {
                    document.Add(new Paragraph("Eventos").SetFont(boldFont).SetFontSize(14).SetMarginTop(10));
                    Table tablaEventos = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 })).UseAllAvailableWidth();

                    // Estilo de la cabecera de la tabla
                    Style headerStyle = new Style()
                        .SetBackgroundColor(ColorConstants.BLUE)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetFont(boldFont)
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.CENTER);

                    // Estilo de las celdas de la tabla
                    Style cellStyle = new Style()
                        .SetFontSize(8)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetPadding(5)
                        .SetBorder(new SolidBorder(0.5f));

                    // Agregar cabecera de la tabla con estilo
                    tablaEventos.AddHeaderCell(new Cell().Add(new Paragraph("nombre_causa")).AddStyle(headerStyle));
                    tablaEventos.AddHeaderCell(new Cell().Add(new Paragraph("entidad")).AddStyle(headerStyle));
                    tablaEventos.AddHeaderCell(new Cell().Add(new Paragraph("tipo_acto")).AddStyle(headerStyle));
                    tablaEventos.AddHeaderCell(new Cell().Add(new Paragraph("fecha_acto")).AddStyle(headerStyle));

                    // Agregar datos de las sanciones
                    foreach (var evento in ciudadano.Eventos)
                    {
                        tablaEventos.AddCell(new Cell().Add(new Paragraph(evento.nombre_causa ?? "")).AddStyle(cellStyle));
                        tablaEventos.AddCell(new Cell().Add(new Paragraph(evento.entidad ?? "")).AddStyle(cellStyle));
                        tablaEventos.AddCell(new Cell().Add(new Paragraph(evento.tipo_acto ?? "")).AddStyle(cellStyle));
                        tablaEventos.AddCell(new Cell().Add(new Paragraph(evento.fecha_acto ?? "")).AddStyle(cellStyle));
                    }

                    document.Add(tablaEventos);
                }

                if (ciudadano.Inhabilidades != null && ciudadano.Inhabilidades.Count > 0)
                {
                    document.Add(new Paragraph("Inhabilidades").SetFont(boldFont).SetFontSize(14).SetMarginTop(10));
                    Table tablaInhabilidad = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25, 25 })).UseAllAvailableWidth();

                    // Estilo de la cabecera de la tabla
                    Style headerStyle = new Style()
                        .SetBackgroundColor(ColorConstants.BLUE)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetFont(boldFont)
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.CENTER);

                    // Estilo de las celdas de la tabla
                    Style cellStyle = new Style()
                        .SetFontSize(8)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetPadding(5)
                        .SetBorder(new SolidBorder(0.5f));

                    // Agregar cabecera de la tabla con estilo
                    tablaInhabilidad.AddHeaderCell(new Cell().Add(new Paragraph("siri")).AddStyle(headerStyle));
                    tablaInhabilidad.AddHeaderCell(new Cell().Add(new Paragraph("modulo")).AddStyle(headerStyle));
                    tablaInhabilidad.AddHeaderCell(new Cell().Add(new Paragraph("inhabilidad_legal")).AddStyle(headerStyle));
                    tablaInhabilidad.AddHeaderCell(new Cell().Add(new Paragraph("fecha_inicio")).AddStyle(headerStyle));
                    tablaInhabilidad.AddHeaderCell(new Cell().Add(new Paragraph("fecha_fin")).AddStyle(headerStyle));

                    // Agregar datos de las sanciones
                    foreach (var inhabilidad in ciudadano.Inhabilidades)
                    {
                        tablaInhabilidad.AddCell(new Cell().Add(new Paragraph(inhabilidad.siri ?? "")).AddStyle(cellStyle));
                        tablaInhabilidad.AddCell(new Cell().Add(new Paragraph(inhabilidad.modulo ?? "")).AddStyle(cellStyle));
                        tablaInhabilidad.AddCell(new Cell().Add(new Paragraph(inhabilidad.inhabilidad_legal ?? "")).AddStyle(cellStyle));
                        tablaInhabilidad.AddCell(new Cell().Add(new Paragraph(inhabilidad.fecha_inicio ?? "")).AddStyle(cellStyle));
                        tablaInhabilidad.AddCell(new Cell().Add(new Paragraph(inhabilidad.fecha_fin ?? "")).AddStyle(cellStyle));

                    }

                    document.Add(tablaInhabilidad);
                }

                // Añadir otras secciones (Delitos, Instancias, Eventos, Inhabilidades) de manera similar...
            }

            document.Close();
            // memoryStream.Position = 0;

            return await Task.FromResult(memoryStream);
        }


        public async Task<object> ConsultaInformacionProcuraduria(ListasAdicionalesDto objetofiltro, string authToken)
        {
            string baseUrl = _configuration["ConsultasAdicionales:GLOBAL_WS_CONSULTA_INFORMACION_Procuraduria_URL"];
            var peticion = new PeticionServiciosAdicionalesDTO
            {
                name = objetofiltro.NombreCompleto,
                typeDocument = objetofiltro.Tipo_Identificacion,
                identification = objetofiltro.NumeroIdentificacion,
            };

            string responseContent;
            string jsonContent = JsonSerializer.Serialize(peticion);
            using (HttpClient client = new HttpClient())
            {
                // Configurar encabezados
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                // client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    // Hacer la solicitud POST
                    HttpResponseMessage response = await client.PostAsync(baseUrl, content);

                    // Validar la respuesta
                    if (response.IsSuccessStatusCode)
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                        var data = JsonSerializer.Deserialize<object>(responseContent);
                        return data;
                    }
                    else
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                        LogError($"Error ConsultaInformacionProcuraduria(): {responseContent} para el cliente {peticion.name} con numero de documento {peticion.identification}");
                        //var data = JsonSerializer.Deserialize<object>(responseContent);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al realizar la solicitud: {ex.Message}");
                    return null;
                }
            }
        }



        public async Task<MemoryStream> EjecucucionPenas(ListasAdicionalesDto objetofiltro)
        {

            object json = await _RegistroForm.ConsultaEjecucionPenas(objetofiltro);

            if (json == null)
            {
                return await SinResultados(objetofiltro, "Jusgado de ejecucion de penas");

            }


            var jsonData = JsonSerializer.Deserialize<List<EjecucionPenas>>(json.ToString());
            //var data = jsonData.Data;

            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = new PdfWriter(memoryStream);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, iText.Kernel.Geom.PageSize.A4, false);

            // Configurar márgenes
            document.SetMargins(28.35f, 28.35f, 28.35f, 28.35f);

            // Crear una fuente negrita
            PdfFont boldFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

            // Título del reporte
            document.Add(new Paragraph("Resultados Ejecución de penas").SetFont(boldFont).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));

            // Crear tabla con 6 columnas ajustadas a proporción del ancho total
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 15, 20, 20, 20, 25 })).UseAllAvailableWidth();

            // Estilo de la cabecera de la tabla
            Style headerStyle = new Style()
                .SetBackgroundColor(ColorConstants.BLUE)
                .SetFontColor(ColorConstants.WHITE)
                .SetFont(boldFont)
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER);

            // Estilo de las celdas de la tabla
            Style cellStyle = new Style()
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5)
                .SetBorder(new SolidBorder(0.5f)); // Añadir borde delgado para visibilidad

            // Agregar cabecera de la tabla con estilo

            table.AddHeaderCell(new Cell().Add(new Paragraph("Nombre")).AddStyle(headerStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Número de Identificación")).AddStyle(headerStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Ciudad")).AddStyle(headerStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha de Consulta")).AddStyle(headerStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Link")).AddStyle(headerStyle));

            // Agregar datos de los procesos
            foreach (EjecucionPenas data in jsonData)
            {
                try
                {
                    table.AddCell(new Cell().Add(new Paragraph(data.nameResult ?? "")).AddStyle(cellStyle));
                    table.AddCell(new Cell().Add(new Paragraph(data.identificationNumberResult ?? "")).AddStyle(cellStyle));
                    table.AddCell(new Cell().Add(new Paragraph(data.cityName ?? "")).AddStyle(cellStyle));
                    table.AddCell(new Cell().Add(new Paragraph(data.queryDate.ToString("yyyy-MM-dd HH:mm:ss") ?? "")).AddStyle(cellStyle));

                    // Ajustar el estilo del link para que haga wrapping
                    Link link = new Link("Ver Enlace", PdfAction.CreateURI(data.link ?? ""));
                    Paragraph linkParagraph = new Paragraph(link).SetFontSize(10).SetFontColor(ColorConstants.BLUE);

                    // Agregar el párrafo del enlace a la celda
                    Cell linkCell = new Cell().Add(linkParagraph).AddStyle(cellStyle);
                    table.AddCell(linkCell);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al añadir datos a la tabla: {ex.Message}");
                }
            }

            document.Add(table);
            document.Close();

            return await Task.FromResult(memoryStream);
        }


        public async Task<object> ConsultaInformacionEjecucionPenas(ListasAdicionalesDto objetofiltro, string authToken)
        {
            string baseUrl = _configuration["ConsultasAdicionales:GLOBAL_WS_CONSULTA_INFORMACION_EJEC_PENAS_URL"];
            var peticion = new PeticionServiciosAdicionalesDTO
            {
                name = objetofiltro.NombreCompleto,
                typeDocument = objetofiltro.Tipo_Identificacion,
                identification = objetofiltro.NumeroIdentificacion,
            };

            string responseContent;
            string jsonContent = JsonSerializer.Serialize(peticion);
            using (HttpClient client = new HttpClient())
            {
                // Configurar encabezados
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                // client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    // Hacer la solicitud POST
                    HttpResponseMessage response = await client.PostAsync(baseUrl, content);

                    // Validar la respuesta
                    if (response.IsSuccessStatusCode)
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                        var data = JsonSerializer.Deserialize<object>(responseContent);
                        return data;
                    }
                    else
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                        LogError($"Error ConsultaInformacionEjecucionPenas(): {responseContent} para el cliente {peticion.name} con numero de documento {peticion.identification} este es el resultado: {responseContent}");
                        //var data = JsonSerializer.Deserialize<object>(responseContent);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error ConsultaInformacionEjecucionPenas(): {ex.Message}");
                    return null;
                }

            }
        }



        public async Task ValidaPaisesRiesgo(int IdFormulario)
        {

            DatosGeneralesReporteDto objFormulariosInfo = new DatosGeneralesReporteDto();

            objFormulariosInfo = await _RegistroForm.ConsultaDatosGeneralesAlertaPaises(IdFormulario);


            bool paiose = ConversoroOpciones.ValidarPaisesProhibidos(objFormulariosInfo.Pais);

            bool otros = ConversoroOpciones.ValidarPaisesProhibidos(objFormulariosInfo.PaisesOtrasSucursales);



            string json = objFormulariosInfo.PreguntasAdicionales.ToString();
            try
            {
                RootDatosGenerarles data = JsonConvert.DeserializeObject<RootDatosGenerarles>(json);
                bool paisesobligaciontiburaria = ConversoroOpciones.ValidarPaisesProhibidos(ConversoroOpciones.ConvertirListaAString(data.PaisesObligacionTributaria));

                bool paisescuentaextranjera = ConversoroOpciones.ValidarPaisesProhibidos(ConversoroOpciones.ConvertirListaAString(data.PaisesCuentasExt));

                bool PAISEPODER = ConversoroOpciones.ValidarPaisesProhibidos(ConversoroOpciones.ConvertirListaAString(data.PaisesPoderCuentaExtranjera));
            }
            catch (Exception EX)
            { 
            
            }
            RepJunAccDTO representantes = new RepJunAccDTO();

            representantes = await _RegistroForm.ConsultaInfoRepresentanteslegalesAlertaPaises(IdFormulario);

        }

        public Task<bool> GuardarDatosGenerales(FormularioModelDTO objRegistro, int IdUsuario)
        {
            throw new NotImplementedException();
        }

        private void LogError(string message)
        {
            // Implementación para registrar el error, por ejemplo, escribir en un archivo de log o en la consola
            Console.WriteLine(message);
        }
    }
}
