using CapaDatos.Interfaz.Reporte.Interface;
using CapaDatos.util;
using CapaDTO.Peticiones;
using CapaDTO.ReportesDTO;
using CapaDTO.Respuestas;
using CapaNegocio.Interfaz.Reporte.Interzas;
using CapaNegocio.Utils;
using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Implementacion.Reporte.Implementacion
{
    public class clsReporteCapaNegocios: IReporteCapaNegocios
    {
        private readonly IReporteCapaDatos InterfaceReporteoTiemposCapaDatos;
        private cDataBase cDataBase;
        private readonly IConfiguration _configuration;



        public clsReporteCapaNegocios(IConfiguration configuration, IReporteCapaDatos _interfaceReporteTimpoCapaDatos)
        {
            _configuration = configuration;
            cDataBase = new cDataBase(configuration);
            this.InterfaceReporteoTiemposCapaDatos = _interfaceReporteTimpoCapaDatos;
        }

        public async Task<List<defaulchartsDto>> ReporteRegistroTiemposxarea(int IdUsuario)
        {
            return await InterfaceReporteoTiemposCapaDatos.ReporteRegistroTiemposxarea(IdUsuario);
        }


        public async Task<List<defaulchartsDto>> ReporteRegistroTiemposxUsuario(int IdUsuario)
        {
            return await InterfaceReporteoTiemposCapaDatos.ReporteRegistroTiemposxUsuario(IdUsuario);
        }

        public async Task<List<defaulchartsDto>> ReporteRegistroTiemposxCliente(int IdUsuario)
        {
            return await InterfaceReporteoTiemposCapaDatos.ReporteRegistroTiemposxCliente(IdUsuario);
        }

        public async Task<List<defaulchartsDto>> ReporteRegistroTiemposxServicio(int IdUsuario)
        {
            return await InterfaceReporteoTiemposCapaDatos.ReporteRegistroTiemposxServicio(IdUsuario);
        }





        public async Task<MemoryStream> ReporteFormulario(ReporteDto objetofiltro)
        {
            var workbook = new XLWorkbook();
            var Formularios = workbook.Worksheets.Add("Formularios");
            await AddInformaciongenerarlSheet(Formularios, objetofiltro);
            var DatosGenerarles = workbook.Worksheets.Add("Datos Generales");
            await AddDatosGenerarst(DatosGenerarles, objetofiltro);
            var Representantes = workbook.Worksheets.Add("Representantes");  
            await AddDatosRepresntantes(Representantes, objetofiltro);
            var Jutadirectiva = workbook.Worksheets.Add("Junta Directiva");
            await AddDatoJuantaDirectiva(Jutadirectiva,objetofiltro);
            var Accionistas = workbook.Worksheets.Add("Accionistas");
            await AddDatoAccionistas(Accionistas, objetofiltro);



            var InformacionTributaria = workbook.Worksheets.Add("Informacion Tributaria");
            await AddInformacionTributaria(InformacionTributaria, objetofiltro);


             var DatosContacto = workbook.Worksheets.Add("Datos Contacto");
            await AddDatosContactoSheet(DatosContacto, objetofiltro);
            var Referencias = workbook.Worksheets.Add("Referencias Com");
            await AddReferenciascombanSheet(Referencias, objetofiltro);
            var datosPago = workbook.Worksheets.Add("Datos de Pago");
            await AddDatosPagoSheet(datosPago, objetofiltro);
            var DespachoMercancia = workbook.Worksheets.Add("Despacho Mercancia");
            await AddDespachoMercanciaSheet(DespachoMercancia, objetofiltro);
            var Cumplimiento = workbook.Worksheets.Add("Cumplimiento Norm");
            await AddCumplimeintoNormativoSheet(Cumplimiento, objetofiltro);
            var Adjuntos = workbook.Worksheets.Add("Adjuntos");
            await AddAdjuntosSheet(Adjuntos, objetofiltro);
            var InfomracionOEA = workbook.Worksheets.Add("Informacion OEA");
            await AddInfoOEASheet(InfomracionOEA, objetofiltro);
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }



        private async Task AddInformaciongenerarlSheet(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<FormularioDto> objFormulariosInfo = new List<FormularioDto>();

            objFormulariosInfo = await InterfaceReporteoTiemposCapaDatos.ContultaInfobasicaFormularioList(objetofiltro);

            worksheet.Cell(1, 1).Value = "IdFormulario";
            worksheet.Cell(1, 2).Value = "Nombre Usuario";
            worksheet.Cell(1, 3).Value = "Estado Formulario";
            worksheet.Cell(1, 4).Value = "Fecha Formulario";

            for (int i = 1; i <= 4; i++)
            {
                var cell = worksheet.Cell(1, i);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }


            int row = 2;
            foreach (FormularioDto obj in objFormulariosInfo)
            {
                worksheet.Cell(row, 1).Value = obj.Id;
                worksheet.Cell(row, 2).Value = obj.NombreUsuario;
                worksheet.Cell(row, 3).Value = obj.Estado;
                worksheet.Cell(row, 4).Value = obj.FechaFormulario;
                row++;
            }

        }


        private async Task AddDatosGenerarst(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<DatosGeneralesReporteDto> objFormulariosInfo = new List<DatosGeneralesReporteDto>();

            objFormulariosInfo = await InterfaceReporteoTiemposCapaDatos.ConsultaDatosGenerales(objetofiltro);


            worksheet.Cell(1, 1).Value = "Tercero";
            worksheet.Range("A1:U1").Merge().Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.Blue)
                .Font.SetFontColor(XLColor.White)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Font.FontSize = 16;

            worksheet.Cell(1, 22).Value = "Informaicon PEP";
            worksheet.Range("V1:AV1").Merge().Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.BlueGray)
                .Font.SetFontColor(XLColor.White)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Font.FontSize = 16;


            worksheet.Cell(2, 1).Value = "IdFormulario";
            worksheet.Cell(2, 2).Value = "Empresa";
            worksheet.Cell(2, 3).Value = "Tipo Solicitud";
            worksheet.Cell(2, 4).Value = "Clase Tercero";
            worksheet.Cell(2, 5).Value = "Categoria Tercero";
            worksheet.Cell(2, 6).Value = "Nombre Razon Social";
            worksheet.Cell(2, 7).Value = "Tipo Identificacion";
            worksheet.Cell(2, 8).Value = "Numero Identificacion";
            worksheet.Cell(2, 9).Value = "Digito Verificacion";
            worksheet.Cell(2, 10).Value = "Pais";
            worksheet.Cell(2, 11).Value = "Cuidad";
            worksheet.Cell(2, 12).Value = "TamañoTercero";
            worksheet.Cell(2, 13).Value = "Actividad Econimica";
            worksheet.Cell(2, 14).Value = "Direccion Principal";
            worksheet.Cell(2, 15).Value = "Codigo Postal";
            worksheet.Cell(2, 16).Value = "Correo Electronico";
            worksheet.Cell(2, 17).Value = "Telefono";
            worksheet.Cell(2, 18).Value = "Obligado FE";
            worksheet.Cell(2, 19).Value = "Correo FE";
            worksheet.Cell(2, 20).Value = "Sucursales Otros Paises";
            worksheet.Cell(2, 21).Value = "Otro Pais";

            worksheet.Cell(2, 22).Value = "Es Pep";

            worksheet.Cell(2, 23).Value = "ManejaRecursos";
            worksheet.Cell(2, 24).Value = "CualesRecursos";
            worksheet.Cell(2, 25).Value = "PoderPolitico";
            worksheet.Cell(2, 26).Value = "RamaPoderPublico";
            worksheet.Cell(2, 27).Value = "CargoPublico";
            worksheet.Cell(2, 28).Value = "CualCargoPublico";
            worksheet.Cell(2, 29).Value = "ObligacionTributarial";
            worksheet.Cell(2, 30).Value = "PaisesObligacionTributaria";
            worksheet.Cell(2, 31).Value = "CuentasFinancierasExt";
            worksheet.Cell(2, 32).Value = "PaisesCuentasExt";
            worksheet.Cell(2, 33).Value = "TienePoderCuentaExtranjera";
            worksheet.Cell(2, 34).Value = "PaisesPoderCuentaExtranjera";
            worksheet.Cell(2, 35).Value = "hasidoPep2";
            worksheet.Cell(2, 36).Value = "Tienevinculosmas5";
                              
                              
            worksheet.Cell(2, 37).Value = "Nombre de la Entidad:";
            worksheet.Cell(2, 38).Value = "Fecha de Ingreso:";
            worksheet.Cell(2, 39).Value = "Fecha de desvinculación:";
                              
                              
            worksheet.Cell(2, 40).Value = "Nombre completo o Razón social";
            worksheet.Cell(2, 41).Value = "Tipo de Identificación (ID)";
            worksheet.Cell(2, 42).Value = "Pais:";
            worksheet.Cell(2, 43).Value = "% Participación";


            worksheet.Cell(2, 44).Value = "Nombre completo";
            worksheet.Cell(2, 45).Value = "Tipo de Identificación (ID))";
            worksheet.Cell(2, 46).Value = "Número de identificación (ID)";
            worksheet.Cell(2, 47).Value = "Nacionalidad";
            worksheet.Cell(2, 48).Value = "Vínculo familiar";






            int row = 3;
            foreach (DatosGeneralesReporteDto obj in objFormulariosInfo)
            {
                worksheet.Cell(row, 1).Value = obj.IdFormulario;
                worksheet.Cell(row, 2).Value = obj.Empresa;
                worksheet.Cell(row, 3).Value = obj.TipoSolicitud;
                worksheet.Cell(row, 4).Value = obj.ClaseTercero;
                worksheet.Cell(row, 5).Value = obj.CategoriaTercero;
                worksheet.Cell(row, 6).Value = obj.NombreRazonSocial;
                worksheet.Cell(row, 7).Value = obj.TipoIdentificacion;
                worksheet.Cell(row, 8).Value = obj.NumeroIdentificacion;
                worksheet.Cell(row, 9).Value = obj.DigitoVerificacion;
                worksheet.Cell(row, 10).Value = obj.Pais;
                worksheet.Cell(row, 11).Value = obj.Ciudad;
                worksheet.Cell(row, 12).Value = obj.TamanoTercero;
                worksheet.Cell(row, 13).Value = obj.ActividadEconimoca;
                worksheet.Cell(row, 14).Value = obj.DireccionPrincipal;
                worksheet.Cell(row, 15).Value = obj.CodigoPostal;
                worksheet.Cell(row, 16).Value = obj.CorreoElectronico;
                worksheet.Cell(row, 17).Value = obj.Telefono;
                worksheet.Cell(row, 18).Value = obj.ObligadoFE;
                worksheet.Cell(row, 19).Value = obj.CorreoElectronicoFE;
                worksheet.Cell(row, 20).Value = obj.TieneSucursalesOtrosPaises;
                worksheet.Cell(row, 21).Value = obj.PaisesOtrasSucursales;



                string json = obj.PreguntasAdicionales.ToString();

                RootDatosGenerarles data = JsonConvert.DeserializeObject<RootDatosGenerarles>(json);


                worksheet.Cell(row, 22).Value = ConversoroOpciones.ConvertirEnteroARespuesta(data.VinculadoPep);
                worksheet.Cell(row, 23).Value = ConversoroOpciones.ConvertirEnteroARespuesta(data.ManejaRecursos);
                worksheet.Cell(row, 24).Value = data.CualesRecursos;
                worksheet.Cell(row, 25).Value = ConversoroOpciones.ConvertirEnteroARespuesta(data.PoderPolitico);
                worksheet.Cell(row, 26).Value = data.RamaPoderPublico;
                worksheet.Cell(row, 27).Value = ConversoroOpciones.ConvertirEnteroARespuesta(data.CargoPublico);
                worksheet.Cell(row, 28).Value = data.CualCargoPublico;
                worksheet.Cell(row, 29).Value = ConversoroOpciones.ConvertirEnteroARespuesta(data.ObligacionTributaria); 
                worksheet.Cell(row, 30).Value = ConversoroOpciones.ConvertirListaAString(data.PaisesObligacionTributaria);
                worksheet.Cell(row, 31).Value = ConversoroOpciones.ConvertirEnteroARespuesta(data.CuentasFinancierasExt);
                worksheet.Cell(row, 32).Value = ConversoroOpciones.ConvertirListaAString(data.PaisesCuentasExt);
                worksheet.Cell(row, 33).Value = ConversoroOpciones.ConvertirEnteroARespuesta(data.TienePoderCuentaExtranjera);
                worksheet.Cell(row, 34).Value = ConversoroOpciones.ConvertirListaAString(data.PaisesPoderCuentaExtranjera);
                worksheet.Cell(row, 35).Value = ConversoroOpciones.ConvertirEnteroARespuesta(data.HasidoPep2);
                worksheet.Cell(row, 36).Value = ConversoroOpciones.ConvertirEnteroARespuesta(data.Tienevinculosmas5);


                int rowfilaheadcargos = row;
                int rowVinculos = row;
                int rowFamiliares = row;
                foreach (var cargos in data.cargosPublicos)
                {
                    worksheet.Cell(rowfilaheadcargos, 23).Value = cargos.NombreEntidad;
                    worksheet.Cell(rowfilaheadcargos, 24).Value = cargos.FechaIngreso;
                    worksheet.Cell(rowfilaheadcargos, 25).Value = cargos.FechaDesvinculacion;
                    rowfilaheadcargos++;
                }

                foreach (var vinculos in data.Vinculosmas)
                {
                    worksheet.Cell(rowVinculos, 26).Value = vinculos.NombreCompleto;
                    worksheet.Cell(rowVinculos, 27).Value = ConversoroOpciones.DevuelveTipoDocumento(vinculos.TipoIdentificacion);
                    worksheet.Cell(rowVinculos, 28).Value = ConversoroOpciones.DevuelvePais(vinculos.Pais);
                    worksheet.Cell(rowVinculos, 29).Value = vinculos.PorcentajeParticipacion;
                    rowVinculos++;
                }

                foreach (var familia in data.InfoFamiliaPep)
                {
                    worksheet.Cell(rowFamiliares, 30).Value = familia.NombreCompleto;
                    worksheet.Cell(rowFamiliares, 31).Value = ConversoroOpciones.DevuelveTipoDocumento(familia.TipoIdentificacion);
                    worksheet.Cell(rowFamiliares, 32).Value = familia.NumeroIdentificacion;
                    worksheet.Cell(rowFamiliares, 33).Value = ConversoroOpciones.DevuelvePais(familia.Nacionalidad);
                    worksheet.Cell(rowFamiliares, 34).Value = familia.VinculoFamiliar;
                    rowFamiliares++;
                }

                if (rowfilaheadcargos > rowVinculos && rowfilaheadcargos > rowFamiliares)
                {
                    row = rowfilaheadcargos;
                }
                else if (rowVinculos > rowfilaheadcargos && rowVinculos > rowFamiliares)
                {
                    row = rowVinculos;
                }
                else
                {
                    row = rowFamiliares;
                }

                row++;
            }

        }


        private async Task AddDatosRepresntantes(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<RepJunAccDTO> objRepresentanteInfo = new List<RepJunAccDTO>();

            objRepresentanteInfo = await InterfaceReporteoTiemposCapaDatos.ConsultaInfoRepresentanteslegales(objetofiltro);

            worksheet.Cell(1, 1).Value = "Representantes";
            worksheet.Range("A1:H1").Merge().Style
                .Font.SetBold(true)              
                .Fill.SetBackgroundColor(XLColor.Blue)
                .Font.SetFontColor(XLColor.White)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Font.FontSize = 16;

            worksheet.Cell(1, 9).Value = "Preguntas Pep";
            worksheet.Range("I1:V1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Red)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;


            worksheet.Cell(1, 23).Value = "Cargos Publicos";
            worksheet.Range("W1:Y1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Gray)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;


            worksheet.Cell(1, 26).Value = "Vinculos";
            worksheet.Range("Z1:AC1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Yellow)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;

            worksheet.Cell(1, 30).Value = "Familia";
            worksheet.Range("AD1:AH1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.BlizzardBlue)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;


            worksheet.Cell(2, 1).Value = "IdFormulario";
            worksheet.Cell(2, 2).Value = "Nombre Rrepresentante";
            worksheet.Cell(2, 3).Value = "TipoDocumento";
            worksheet.Cell(2, 4).Value = "Numero Identificacion";
            worksheet.Cell(2, 5).Value = "Nacionalidad";
            worksheet.Cell(2, 6).Value = "Telefono";
            worksheet.Cell(2, 7).Value = "CorreoElectronico";
            worksheet.Cell(2, 8).Value = "vinculadoPep";
            worksheet.Cell(2, 9).Value = "ManejaRecursos";
            worksheet.Cell(2, 10).Value = "CualesRecursos";
            worksheet.Cell(2, 11).Value = "PoderPolitico";
            worksheet.Cell(2, 12).Value = "RamaPoderPublico";
            worksheet.Cell(2, 13).Value = "CargoPublico";
            worksheet.Cell(2, 14).Value = "CualCargoPublico";
            worksheet.Cell(2, 15).Value = "ObligacionTributarial";
            worksheet.Cell(2, 16).Value = "PaisesObligacionTributaria";
            worksheet.Cell(2, 17).Value = "CuentasFinancierasExt";
            worksheet.Cell(2, 18).Value = "PaisesCuentasExt";
            worksheet.Cell(2, 19).Value = "TienePoderCuentaExtranjera";
            worksheet.Cell(2, 20).Value = "PaisesPoderCuentaExtranjera";
            worksheet.Cell(2, 21).Value = "hasidoPep2";
            worksheet.Cell(2, 22).Value = "Tienevinculosmas5";


            worksheet.Cell(2, 23).Value = "Nombre de la Entidad:";
            worksheet.Cell(2, 24).Value = "Fecha de Ingreso:";
            worksheet.Cell(2, 25).Value = "Fecha de desvinculación:";


            worksheet.Cell(2, 26).Value = "Nombre completo o Razón social";
            worksheet.Cell(2, 27).Value = "Tipo de Identificación (ID)";
            worksheet.Cell(2, 28).Value = "Pais:";
            worksheet.Cell(2, 29).Value = "% Participación";


            worksheet.Cell(2, 30).Value = "Nombre completo";
            worksheet.Cell(2, 31).Value = "Tipo de Identificación (ID))";
            worksheet.Cell(2, 32).Value = "Número de identificación (ID)";
            worksheet.Cell(2, 33).Value = "Nacionalidad";
            worksheet.Cell(2, 34).Value = "Vínculo familiar";


            int row = 3;

            foreach (RepJunAccDTO objRepresentante in objRepresentanteInfo)
            {
                int IdFormulario = objRepresentante.IdFomrulario;
                object representantes = objRepresentante.Persona;
                string json = representantes.ToString();

                RootRepresentante data = JsonConvert.DeserializeObject<RootRepresentante>(json);


                foreach (var representante in data.Representantes)
                {
                    worksheet.Cell(row, 1).Value = IdFormulario;
                    worksheet.Cell(row, 2).Value = representante.nombre;
                    worksheet.Cell(row, 3).Value = ConversoroOpciones.DevuelveTipoDocumento(representante.tipoDocumento);
                    worksheet.Cell(row, 4).Value = representante.NumeroIdentificacion;
                    worksheet.Cell(row, 5).Value = ConversoroOpciones.DevuelvePais(representante.Nacionalidad);
                    worksheet.Cell(row, 6).Value = representante.Telefono;
                    worksheet.Cell(row, 7).Value = representante.CorreoElectronico;
                    worksheet.Cell(row, 8).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.vinculadoPep);
                    worksheet.Cell(row, 9).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.ManejaRecursos); 
                    worksheet.Cell(row, 10).Value = representante.CualesRecursos;
                    worksheet.Cell(row, 11).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.PoderPolitico);
                    worksheet.Cell(row, 12).Value = representante.RamaPoderPublico;
                    worksheet.Cell(row, 13).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.CargoPublico); 
                    worksheet.Cell(row, 14).Value = representante.CualCargoPublico;
                    worksheet.Cell(row, 15).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.ObligacionTributaria);
                    worksheet.Cell(row, 16).Value = representante.PaisesObligacionTributaria;
                    worksheet.Cell(row, 17).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.CuentasFinancierasExt);
                    worksheet.Cell(row, 18).Value = representante.PaisesCuentasExt;
                    worksheet.Cell(row, 19).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.TienePoderCuentaExtranjera);
                    worksheet.Cell(row, 20).Value = representante.PaisesPoderCuentaExtranjera;
                    worksheet.Cell(row, 21).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.hasidoPep2); 
                    worksheet.Cell(row, 22).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.Tienevinculosmas5);

                    int rowfilaheadcargos = row;
                    int rowVinculos = row;
                    int rowFamiliares = row;
                    foreach (var cargos in representante.cargosPublicos)
                    {
                        worksheet.Cell(rowfilaheadcargos, 23).Value = cargos.NombreEntidad;
                        worksheet.Cell(rowfilaheadcargos, 24).Value = cargos.FechaIngreso;
                        worksheet.Cell(rowfilaheadcargos, 25).Value = cargos.FechaDesvinculacion;
                        rowfilaheadcargos++;
                    }

                    foreach (var vinculos in representante.Vinculosmas)
                    {
                        worksheet.Cell(rowVinculos, 26).Value = vinculos.NombreCompleto;
                        worksheet.Cell(rowVinculos, 27).Value = ConversoroOpciones.DevuelveTipoDocumento(vinculos.TipoIdentificacion);
                        worksheet.Cell(rowVinculos, 28).Value = ConversoroOpciones.DevuelvePais(vinculos.Pais);
                        worksheet.Cell(rowVinculos, 29).Value = vinculos.PorcentajeParticipacion;
                        rowVinculos++;
                    }

                    foreach (var familia in representante.InfoFamiliaPep)
                    {
                        worksheet.Cell(rowFamiliares, 30).Value = familia.NombreCompleto;
                        worksheet.Cell(rowFamiliares, 31).Value = ConversoroOpciones.DevuelveTipoDocumento(familia.TipoIdentificacion);
                        worksheet.Cell(rowFamiliares, 32).Value = familia.NumeroIdentificacion;
                        worksheet.Cell(rowFamiliares, 33).Value = ConversoroOpciones.DevuelvePais(familia.Nacionalidad);
                        worksheet.Cell(rowFamiliares, 34).Value = familia.VinculoFamiliar;
                        rowFamiliares++;
                    }

                    if (rowfilaheadcargos > rowVinculos && rowfilaheadcargos > rowFamiliares)
                    {
                        row = rowfilaheadcargos;
                    }
                    else if (rowVinculos > rowfilaheadcargos && rowVinculos > rowFamiliares)
                    {
                        row = rowVinculos;
                    }
                    else
                    {
                        row = rowFamiliares;
                    }

                    row++;
                }


                }



        }


        private async Task AddDatoJuantaDirectiva(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<RepJunAccDTO> objRepresentanteInfo = new List<RepJunAccDTO>();

            objRepresentanteInfo = await InterfaceReporteoTiemposCapaDatos.ConsultaInfoJuntaDirectivalegales(objetofiltro);

            worksheet.Cell(1, 1).Value = "Directivos";
            worksheet.Range("A1:E1").Merge().Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.Blue)
                .Font.SetFontColor(XLColor.White)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Font.FontSize = 16;

            worksheet.Cell(1, 6).Value = "Preguntas Pep";
            worksheet.Range("F1:S1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Red)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;


            worksheet.Cell(1, 20).Value = "Cargos Publicos";
            worksheet.Range("T1:V1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Gray)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;


            worksheet.Cell(1, 23).Value = "Vinculos";
            worksheet.Range("W1:Z1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Yellow)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;

            worksheet.Cell(1, 27).Value = "Familia";
            worksheet.Range("AA1:AE1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.BlizzardBlue)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;


            worksheet.Cell(2, 1).Value = "IdFormulario";
            worksheet.Cell(2, 2).Value = "Nombre Directivo";
            worksheet.Cell(2, 3).Value = "Tipo Identificacion";
            worksheet.Cell(2, 4).Value = "Numero Identificacion";
            worksheet.Cell(2, 5).Value = "Nacionalidad";
            worksheet.Cell(2, 6).Value = "vinculadoPep";
            worksheet.Cell(2, 7).Value = "ManejaRecursos";
            worksheet.Cell(2, 8).Value = "CualesRecursos";
            worksheet.Cell(2, 9).Value = "PoderPolitico";
            worksheet.Cell(2, 10).Value = "RamaPoderPublico";
            worksheet.Cell(2, 11).Value = "CargoPublico";
            worksheet.Cell(2, 12).Value = "CualCargoPublico";
            worksheet.Cell(2, 13).Value = "ObligacionTributarial";
            worksheet.Cell(2, 14).Value = "PaisesObligacionTributaria";
            worksheet.Cell(2, 15).Value = "CuentasFinancierasExt";
            worksheet.Cell(2, 16).Value = "PaisesCuentasExt";
            worksheet.Cell(2, 17).Value = "TienePoderCuentaExtranjera";
            worksheet.Cell(2, 18).Value = "PaisesPoderCuentaExtranjera";
            worksheet.Cell(2, 19).Value = "hasidoPep2";
            worksheet.Cell(2, 20).Value = "Tienevinculosmas5";


            worksheet.Cell(2, 21).Value = "Nombre de la Entidad:";
            worksheet.Cell(2, 22).Value = "Fecha de Ingreso:";
            worksheet.Cell(2, 23).Value = "Fecha de desvinculación:";


            worksheet.Cell(2, 24).Value = "Nombre completo o Razón social";
            worksheet.Cell(2, 25).Value = "Tipo de Identificación (ID)";
            worksheet.Cell(2, 26).Value = "Pais:";
            worksheet.Cell(2, 27).Value = "% Participación";


            worksheet.Cell(2, 28).Value = "Nombre completo";
            worksheet.Cell(2, 29).Value = "Tipo de Identificación (ID))";
            worksheet.Cell(2, 30).Value = "Número de identificación (ID)";
            worksheet.Cell(2, 31).Value = "Nacionalidad";
            worksheet.Cell(2, 32).Value = "Vínculo familiar";


            int row = 3;

            foreach (RepJunAccDTO objRepresentante in objRepresentanteInfo)
            {
                int IdFormulario = objRepresentante.IdFomrulario;
                object representantes = objRepresentante.Persona;
                string json = representantes.ToString();

                RootDirectivo data = JsonConvert.DeserializeObject<RootDirectivo>(json);


                foreach (var representante in data.Directivos)
                {
                    worksheet.Cell(row, 1).Value = IdFormulario;
                    worksheet.Cell(row, 2).Value = representante.nombre;
                    worksheet.Cell(row, 3).Value = ConversoroOpciones.DevuelveTipoDocumento(representante.tipoDocumento);
                    worksheet.Cell(row, 4).Value = representante.NumeroIdentificacion;
                    worksheet.Cell(row, 5).Value = ConversoroOpciones.DevuelvePais(representante.Nacionalidad);
                    worksheet.Cell(row, 6).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.vinculadoPep);
                    worksheet.Cell(row, 7).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.ManejaRecursos);
                    worksheet.Cell(row, 8).Value = representante.CualesRecursos;
                    worksheet.Cell(row, 9).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.PoderPolitico);
                    worksheet.Cell(row, 10).Value = representante.RamaPoderPublico;
                    worksheet.Cell(row, 11).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.CargoPublico);
                    worksheet.Cell(row, 12).Value = representante.CualCargoPublico;
                    worksheet.Cell(row, 13).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.ObligacionTributaria);
                    worksheet.Cell(row, 14).Value = representante.PaisesObligacionTributaria;
                    worksheet.Cell(row, 15).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.CuentasFinancierasExt);
                    worksheet.Cell(row, 16).Value = representante.PaisesCuentasExt;
                    worksheet.Cell(row, 17).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.TienePoderCuentaExtranjera);
                    worksheet.Cell(row, 18).Value = representante.PaisesPoderCuentaExtranjera;
                    worksheet.Cell(row, 19).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.hasidoPep2);
                    worksheet.Cell(row, 20).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.Tienevinculosmas5);

                    int rowfilaheadcargos = row;
                    int rowVinculos = row;
                    int rowFamiliares = row;
                    foreach (var cargos in representante.cargosPublicos)
                    {
                        worksheet.Cell(rowfilaheadcargos, 21).Value = cargos.NombreEntidad;
                        worksheet.Cell(rowfilaheadcargos, 22).Value = cargos.FechaIngreso;
                        worksheet.Cell(rowfilaheadcargos, 23).Value = cargos.FechaDesvinculacion;
                        rowfilaheadcargos++;
                    }

                    foreach (var vinculos in representante.Vinculosmas)
                    {
                        worksheet.Cell(rowVinculos, 24).Value = vinculos.NombreCompleto;
                        worksheet.Cell(rowVinculos, 25).Value = ConversoroOpciones.DevuelveTipoDocumento(vinculos.TipoIdentificacion);
                        worksheet.Cell(rowVinculos, 26).Value = ConversoroOpciones.DevuelvePais(vinculos.Pais);
                        worksheet.Cell(rowVinculos, 27).Value = vinculos.PorcentajeParticipacion;
                        rowVinculos++;
                    }

                    foreach (var familia in representante.InfoFamiliaPep)
                    {
                        worksheet.Cell(rowFamiliares, 28).Value = familia.NombreCompleto;
                        worksheet.Cell(rowFamiliares, 29).Value = ConversoroOpciones.DevuelveTipoDocumento(familia.TipoIdentificacion);
                        worksheet.Cell(rowFamiliares, 30).Value = familia.NumeroIdentificacion;
                        worksheet.Cell(rowFamiliares, 31).Value = ConversoroOpciones.DevuelvePais(familia.Nacionalidad);
                        worksheet.Cell(rowFamiliares, 32).Value = familia.VinculoFamiliar;
                        rowFamiliares++;
                    }

                    if (rowfilaheadcargos > rowVinculos && rowfilaheadcargos > rowFamiliares)
                    {
                        row = rowfilaheadcargos;
                    }
                    else if (rowVinculos > rowfilaheadcargos && rowVinculos > rowFamiliares)
                    {
                        row = rowVinculos;
                    }
                    else
                    {
                        row = rowFamiliares;
                    }

                    row++;
                }


            }



        }


        private async Task AddDatoAccionistas(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<RepJunAccDTO> objRepresentanteInfo = new List<RepJunAccDTO>();

            objRepresentanteInfo = await InterfaceReporteoTiemposCapaDatos.ConsultaInfoAccionistas(objetofiltro);

            worksheet.Cell(1, 1).Value = "Accionista";
            worksheet.Range("A1:G1").Merge().Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.Blue)
                .Font.SetFontColor(XLColor.White)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Font.FontSize = 16;

            worksheet.Cell(1, 8).Value = "Preguntas Pep";
            worksheet.Range("H1:U1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Red)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;


            worksheet.Cell(1, 22).Value = "Cargos Publicos";
            worksheet.Range("V1:X1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Gray)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;


            worksheet.Cell(1, 25).Value = "Vinculos";
            worksheet.Range("Y1:AB1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Yellow)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;

            worksheet.Cell(1, 29).Value = "Familia";
            worksheet.Range("AC1:AG1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.BlizzardBlue)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;


            worksheet.Cell(2, 1).Value = "IdFormulario";
            worksheet.Cell(2, 2).Value = "Nombre Directivo";
            worksheet.Cell(2, 3).Value = "Tipo Identificacion";
            worksheet.Cell(2, 4).Value = "Numero Identificacion";
            worksheet.Cell(2, 5).Value = "Nacionalidad";
            worksheet.Cell(2, 6).Value = "% de participación:";
            worksheet.Cell(2, 7).Value = "vinculadoPep";
            worksheet.Cell(2, 8).Value = "ManejaRecursos";
            worksheet.Cell(2, 9).Value = "CualesRecursos";
            worksheet.Cell(2, 10).Value = "PoderPolitico";
            worksheet.Cell(2, 11).Value = "RamaPoderPublico";
            worksheet.Cell(2, 12).Value = "CargoPublico";
            worksheet.Cell(2, 13).Value = "CualCargoPublico";
            worksheet.Cell(2, 14).Value = "ObligacionTributarial";
            worksheet.Cell(2, 15).Value = "PaisesObligacionTributaria";
            worksheet.Cell(2, 16).Value = "CuentasFinancierasExt";
            worksheet.Cell(2, 17).Value = "PaisesCuentasExt";
            worksheet.Cell(2, 18).Value = "TienePoderCuentaExtranjera";
            worksheet.Cell(2, 19).Value = "PaisesPoderCuentaExtranjera";
            worksheet.Cell(2, 20).Value = "hasidoPep2";
            worksheet.Cell(2, 21).Value = "Tienevinculosmas5";


            worksheet.Cell(2, 22).Value = "Nombre de la Entidad:";
            worksheet.Cell(2, 23).Value = "Fecha de Ingreso:";
            worksheet.Cell(2, 24).Value = "Fecha de desvinculación:";


            worksheet.Cell(2, 25).Value = "Nombre completo o Razón social";
            worksheet.Cell(2, 26).Value = "Tipo de Identificación (ID)";
            worksheet.Cell(2, 27).Value = "Pais:";
            worksheet.Cell(2, 28).Value = "% Participación";


            worksheet.Cell(2, 29).Value = "Nombre completo";
            worksheet.Cell(2, 30).Value = "Tipo de Identificación (ID))";
            worksheet.Cell(2, 31).Value = "Número de identificación (ID)";
            worksheet.Cell(2, 32).Value = "Nacionalidad";
            worksheet.Cell(2, 33).Value = "Vínculo familiar";


            worksheet.Cell(1, 34).Value = "Beneficiarios"; 
            worksheet.Range("AH1:AM1").Merge().Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.Blue)
                .Font.SetFontColor(XLColor.White)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Font.FontSize = 16;

            worksheet.Cell(1, 40).Value = "Preguntas Pep";
            worksheet.Range("AN1:BA1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Red)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;



            worksheet.Cell(1, 54).Value = "Cargos Publicos";
            worksheet.Range("BB1:BD1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Gray)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;


            worksheet.Cell(1, 57).Value = "Vinculos";
            worksheet.Range("BE1:BH1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.Yellow)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;

            worksheet.Cell(1, 61).Value = "Familia";
            worksheet.Range("BI1:BM1").Merge().Style
    .Font.SetBold(true)
    .Fill.SetBackgroundColor(XLColor.BlizzardBlue)
    .Font.SetFontColor(XLColor.White)
    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    .Font.FontSize = 16;



            worksheet.Cell(2, 34).Value = "Nombre beneficiario";
            worksheet.Cell(2, 35).Value = "Tipo Identificacion";
            worksheet.Cell(2, 36).Value = "Numero Identificacion";
            worksheet.Cell(2, 37).Value = "Nacionalidad";
            worksheet.Cell(2, 38).Value = "% de participación:";
            worksheet.Cell(2, 39).Value = "vinculadoPep";
            worksheet.Cell(2, 40).Value = "ManejaRecursos";
            worksheet.Cell(2, 41).Value = "CualesRecursos";
            worksheet.Cell(2, 42).Value = "PoderPolitico";
            worksheet.Cell(2, 43).Value = "RamaPoderPublico";
            worksheet.Cell(2, 44).Value = "CargoPublico";
            worksheet.Cell(2, 45).Value = "CualCargoPublico";
            worksheet.Cell(2, 46).Value = "ObligacionTributarial";
            worksheet.Cell(2, 47).Value = "PaisesObligacionTributaria";
            worksheet.Cell(2, 48).Value = "CuentasFinancierasExt";
            worksheet.Cell(2, 49).Value = "PaisesCuentasExt";
            worksheet.Cell(2, 50).Value = "TienePoderCuentaExtranjera";
            worksheet.Cell(2, 51).Value = "PaisesPoderCuentaExtranjera";
            worksheet.Cell(2, 52).Value = "hasidoPep2";
            worksheet.Cell(2, 53).Value = "Tienevinculosmas5";


            worksheet.Cell(2, 54).Value = "Nombre de la Entidad:";
            worksheet.Cell(2, 55).Value = "Fecha de Ingreso:";
            worksheet.Cell(2, 56).Value = "Fecha de desvinculación:";


            worksheet.Cell(2, 57).Value = "Nombre completo o Razón social";
            worksheet.Cell(2, 58).Value = "Tipo de Identificación (ID)";
            worksheet.Cell(2, 59).Value = "Pais:";
            worksheet.Cell(2, 60).Value = "% Participación";


            worksheet.Cell(2, 61).Value = "Nombre completo";
            worksheet.Cell(2, 62).Value = "Tipo de Identificación (ID))";
            worksheet.Cell(2, 63).Value = "Número de identificación (ID)";
            worksheet.Cell(2, 64).Value = "Nacionalidad";
            worksheet.Cell(2, 65).Value = "Vínculo familiar";






            int row = 3;

            foreach (RepJunAccDTO objRepresentante in objRepresentanteInfo)
            {
                int IdFormulario = objRepresentante.IdFomrulario;
                object representantes = objRepresentante.Persona;
                string json = representantes.ToString();

                RootAccionistas data = JsonConvert.DeserializeObject<RootAccionistas>(json);


                foreach (var representante in data.Accionista)
                {
                    worksheet.Cell(row, 1).Value = IdFormulario;
                    worksheet.Cell(row, 2).Value = representante.NombreCompleto;
                    worksheet.Cell(row, 3).Value = ConversoroOpciones.DevuelveTipoDocumento(representante.TipoDocumento);
                    worksheet.Cell(row, 4).Value = representante.NumeroIdentificacion;
                    worksheet.Cell(row, 5).Value = ConversoroOpciones.DevuelvePais(representante.Nacionalidad);
                    worksheet.Cell(row, 6).Value = representante.Porcentajeparticipacion;
                    worksheet.Cell(row, 7).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.VinculadoPep);
                    worksheet.Cell(row, 8).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.ManejaRecursos);
                    worksheet.Cell(row, 9).Value = representante.CualesRecursos;
                    worksheet.Cell(row, 10).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.PoderPolitico);
                    worksheet.Cell(row, 11).Value = representante.RamaPoderPublico;
                    worksheet.Cell(row, 12).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.CargoPublico);
                    worksheet.Cell(row, 13).Value = representante.CualCargoPublico;
                    worksheet.Cell(row, 14).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.ObligacionTributaria);
                    worksheet.Cell(row, 15).Value = representante.PaisesObligacionTributaria;
                    worksheet.Cell(row, 16).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.CuentasFinancierasExt);
                    worksheet.Cell(row, 17).Value = representante.PaisesCuentasExt;
                    worksheet.Cell(row, 18).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.TienePoderCuentaExtranjera);
                    worksheet.Cell(row, 19).Value = representante.PaisesPoderCuentaExtranjera;
                    worksheet.Cell(row, 20).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.HasidoPep2);
                    worksheet.Cell(row, 21).Value = ConversoroOpciones.ConvertirEnteroARespuesta(representante.Tienevinculosmas5);

                    int rowfilaheadcargos = row;
                    int rowVinculos = row;
                    int rowFamiliares = row;
                    foreach (var cargos in representante.cargosPublicos)
                    {
                        worksheet.Cell(rowfilaheadcargos, 22).Value = cargos.NombreEntidad;
                        worksheet.Cell(rowfilaheadcargos, 23).Value = cargos.FechaIngreso;
                        worksheet.Cell(rowfilaheadcargos, 24).Value = cargos.FechaDesvinculacion;
                        rowfilaheadcargos++;
                    }

                    foreach (var vinculos in representante.Vinculosmas)
                    {
                        worksheet.Cell(rowVinculos, 25).Value = vinculos.NombreCompleto;
                        worksheet.Cell(rowVinculos, 26).Value = ConversoroOpciones.DevuelveTipoDocumento(vinculos.TipoIdentificacion);
                        worksheet.Cell(rowVinculos, 27).Value = ConversoroOpciones.DevuelvePais(vinculos.Pais);
                        worksheet.Cell(rowVinculos, 28).Value = vinculos.PorcentajeParticipacion;
                        rowVinculos++;
                    }

                    foreach (var familia in representante.InfoFamiliaPep)
                    {
                        worksheet.Cell(rowFamiliares, 29).Value = familia.NombreCompleto;
                        worksheet.Cell(rowFamiliares, 30).Value = ConversoroOpciones.DevuelveTipoDocumento(familia.TipoIdentificacion);
                        worksheet.Cell(rowFamiliares, 31).Value = familia.NumeroIdentificacion;
                        worksheet.Cell(rowFamiliares, 32).Value = ConversoroOpciones.DevuelvePais(familia.Nacionalidad);
                        worksheet.Cell(rowFamiliares, 33).Value = familia.VinculoFamiliar;
                        rowFamiliares++;
                    }

                    if (rowfilaheadcargos > rowVinculos && rowfilaheadcargos > rowFamiliares)
                    {
                        row = rowfilaheadcargos;
                    }
                    else if (rowVinculos > rowfilaheadcargos && rowVinculos > rowFamiliares)
                    {
                        row = rowVinculos;
                    }
                    else
                    {
                        row = rowFamiliares;
                    }


                    List<BeneficiarioFinal> beneficiariosFinales = new List<BeneficiarioFinal>();

                    if (representante.BeneficiariosFinales != null && representante.BeneficiariosFinales.Count > 0)
                    {
                        beneficiariosFinales.AddRange(representante.BeneficiariosFinales);
                    }

                    if ((beneficiariosFinales != null) && (beneficiariosFinales.Count > 0))
                    {
                        foreach (var beneficiario in beneficiariosFinales)
                        {
                            worksheet.Cell(row, 34).Value = beneficiario.NombreCompleto;
                            worksheet.Cell(row, 35).Value = beneficiario.NumeroIdentificacion;
                            worksheet.Cell(row, 36).Value = ConversoroOpciones.DevuelvePais(beneficiario.Nacionalidad);
                            worksheet.Cell(row, 37).Value = beneficiario.Porcentajeparticipacion;
                            worksheet.Cell(row, 38).Value = ConversoroOpciones.ConvertirEnteroARespuesta(beneficiario.VinculadoPep);
                            worksheet.Cell(row, 39).Value = ConversoroOpciones.ConvertirEnteroARespuesta(beneficiario.ManejaRecursos);
                            worksheet.Cell(row, 40).Value = beneficiario.CualesRecursos;
                            worksheet.Cell(row, 41).Value = ConversoroOpciones.ConvertirEnteroARespuesta(beneficiario.PoderPolitico);
                            worksheet.Cell(row, 42).Value = beneficiario.RamaPoderPublico;
                            worksheet.Cell(row, 43).Value = ConversoroOpciones.ConvertirEnteroARespuesta(beneficiario.CargoPublico);
                            worksheet.Cell(row, 44).Value = beneficiario.CualCargoPublico;
                            worksheet.Cell(row, 46).Value = ConversoroOpciones.ConvertirEnteroARespuesta(beneficiario.ObligacionTributaria);
                            worksheet.Cell(row, 47).Value = beneficiario.PaisesObligacionTributaria;
                            worksheet.Cell(row, 48).Value = ConversoroOpciones.ConvertirEnteroARespuesta(beneficiario.CuentasFinancierasExt);
                            worksheet.Cell(row, 49).Value = beneficiario.PaisesCuentasExt;
                            worksheet.Cell(row, 50).Value = ConversoroOpciones.ConvertirEnteroARespuesta(beneficiario.TienePoderCuentaExtranjera);
                            worksheet.Cell(row, 51).Value = beneficiario.PaisesPoderCuentaExtranjera;
                            worksheet.Cell(row, 52).Value = ConversoroOpciones.ConvertirEnteroARespuesta(beneficiario.HasidoPep2);
                            worksheet.Cell(row, 53).Value = ConversoroOpciones.ConvertirEnteroARespuesta(beneficiario.Tienevinculosmas5);



                            int rowfilaheadcargos2 = row;
                            int rowVinculos2 = row;
                            int rowFamiliares2 = row;


                            foreach (var cargos in beneficiario.cargosPublicos)
                            {
                                worksheet.Cell(rowfilaheadcargos, 54).Value = cargos.NombreEntidad;
                                worksheet.Cell(rowfilaheadcargos, 55).Value = cargos.FechaIngreso;
                                worksheet.Cell(rowfilaheadcargos, 56).Value = cargos.FechaDesvinculacion;
                                rowfilaheadcargos2++;
                            }


                            foreach (var vinculos in beneficiario.Vinculosmas)
                            {
                                worksheet.Cell(rowVinculos, 57).Value = vinculos.NombreCompleto;
                                worksheet.Cell(rowVinculos, 58).Value = ConversoroOpciones.DevuelveTipoDocumento(vinculos.TipoIdentificacion);
                                worksheet.Cell(rowVinculos, 59).Value = ConversoroOpciones.DevuelvePais(vinculos.Pais);
                                worksheet.Cell(rowVinculos, 60).Value = vinculos.PorcentajeParticipacion;
                                rowVinculos2++;
                            }

                            foreach (var familia in beneficiario.InfoFamiliaPep)
                            {
                                worksheet.Cell(rowFamiliares, 61).Value = familia.NombreCompleto;
                                worksheet.Cell(rowFamiliares, 62).Value = ConversoroOpciones.DevuelveTipoDocumento(familia.TipoIdentificacion);
                                worksheet.Cell(rowFamiliares, 63).Value = familia.NumeroIdentificacion;
                                worksheet.Cell(rowFamiliares, 64).Value = ConversoroOpciones.DevuelvePais(familia.Nacionalidad);
                                worksheet.Cell(rowFamiliares, 65).Value = familia.VinculoFamiliar;
                                rowFamiliares2++;
                            }

                            if (rowfilaheadcargos2 > rowVinculos2 && rowfilaheadcargos2 > rowFamiliares2)
                            {
                                row = rowfilaheadcargos2;
                            }
                            else if (rowVinculos2 > rowfilaheadcargos2 && rowVinculos2 > rowFamiliares2)
                            {
                                row = rowVinculos2;
                            }
                            else
                            {
                                row = rowFamiliares2;
                            } 
                            row++;
                        }
                     }

                    row++;

                    }


            }

        }


        private async Task AddDatosContactoSheet(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<DatosContactoDto> lsitaContactos = new List<DatosContactoDto>();

            lsitaContactos = await InterfaceReporteoTiemposCapaDatos.ListaDatosContacto(objetofiltro);
            worksheet.Cell(1, 1).Value = "IdFormulario";
            worksheet.Cell(1, 2).Value = "Nombre de Contacto";
            worksheet.Cell(1, 3).Value = "Cargo";
            worksheet.Cell(1, 4).Value = "Area";
            worksheet.Cell(1, 5).Value = "Teléfono";
            worksheet.Cell(1, 6).Value = "Correo Electronico";
            worksheet.Cell(1, 7).Value = "Ciudad";
            worksheet.Cell(1, 8).Value = "Direccion";



            for (int i = 1; i <= 6; i++)
            {
                var cell = worksheet.Cell(1, i);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }


            int row = 2;
            foreach (DatosContactoDto obj in lsitaContactos)
            {
                worksheet.Cell(row, 1).Value = obj.IdFormulario;
                worksheet.Cell(row, 2).Value = obj.NombreContacto;
                worksheet.Cell(row, 3).Value = obj.CargoContacto;
                worksheet.Cell(row, 4).Value = obj.AreaContacto;
                worksheet.Cell(row, 5).Value = obj.TelefonoContacto;
                worksheet.Cell(row, 6).Value = obj.CorreoElectronico;
                worksheet.Cell(row, 7).Value = obj.Ciudad;
                worksheet.Cell(row, 8).Value = obj.Direccion;

                row++;
            }

        }


        private async Task AddReferenciascombanSheet(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<ReferenciaComercialesBancariasReporteDto> objRegistro = new List<ReferenciaComercialesBancariasReporteDto>();

            objRegistro = await InterfaceReporteoTiemposCapaDatos.ListaReferenciasComercialesBan(objetofiltro);
            worksheet.Cell(1, 1).Value = "IdFormulario";
            worksheet.Cell(1, 2).Value = "Tipo Referencia";
            worksheet.Cell(1, 3).Value = "Nombre Completo";
            worksheet.Cell(1, 4).Value = "Cuidad";
            worksheet.Cell(1, 5).Value = "Teléfono";

            for (int i = 1; i <= 5; i++)
            {
                var cell = worksheet.Cell(1, i);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }



            int row = 2;
            foreach (ReferenciaComercialesBancariasReporteDto obj in objRegistro)
            {
                worksheet.Cell(row, 1).Value = obj.IdFormulario;
                worksheet.Cell(row, 2).Value = obj.TipoReferencia;
                worksheet.Cell(row, 3).Value = obj.NombreCompleto;
                worksheet.Cell(row, 4).Value = obj.Ciudad;
                worksheet.Cell(row, 5).Value = obj.Telefono;
                row++;
            }

        }//Task<List<DatosPagosDto>> ConsultaDatosPago();

        private async Task AddDatosPagoSheet(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<DatosPagosReporteDto> objRegistro = new List<DatosPagosReporteDto>();

            objRegistro = await InterfaceReporteoTiemposCapaDatos.ConsultaDatosPago(objetofiltro);
            worksheet.Cell(1, 1).Value = "IdFormulario";
            worksheet.Cell(1, 2).Value = "Nombre Banco";
            worksheet.Cell(1, 3).Value = "Numero Cuenta";
            worksheet.Cell(1, 4).Value = "Tipo Cuenta";
            worksheet.Cell(1, 5).Value = "Codigo Swift";
            worksheet.Cell(1, 6).Value = "Cuidad";
            worksheet.Cell(1, 7).Value = "Pias";
            worksheet.Cell(1, 8).Value = "Correo Electronico";

            for (int i = 1; i <= 8; i++)
            {
                var cell = worksheet.Cell(1, i);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }


            int row = 2;
            foreach (DatosPagosReporteDto obj in objRegistro)
            {
                worksheet.Cell(row, 1).Value = obj.IdFormulario;
                worksheet.Cell(row, 2).Value = obj.NombreBanco;
                worksheet.Cell(row, 3).Value = obj.NumeroCuenta;
                worksheet.Cell(row, 4).Value = obj.TipoCuenta;
                worksheet.Cell(row, 5).Value = obj.CodigoSwift;
                worksheet.Cell(row, 6).Value = obj.Ciudad; 
                worksheet.Cell(row, 7).Value = obj.Pais;
                worksheet.Cell(row, 8).Value = obj.CorreoElectronico;
                row++;
            }

        }


        private async Task AddDespachoMercanciaSheet(IXLWorksheet worksheet,ReporteDto objetofiltro)
        { 

            List<DespachoMercanciaReporteDto> objRegistro = new List<DespachoMercanciaReporteDto>();
            objRegistro = await InterfaceReporteoTiemposCapaDatos.ConsulataDespachoMercancia(objetofiltro);
            worksheet.Cell(1, 1).Value = "IdFormulario";
            worksheet.Cell(1, 2).Value = "Direccion Mercancia";
            worksheet.Cell(1, 3).Value = "Pais";
            worksheet.Cell(1, 4).Value = "Cuidad";
            worksheet.Cell(1, 5).Value = "Codigo Postal";
            worksheet.Cell(1, 6).Value = "Telefono";

            for (int i = 1; i <= 6; i++)
            {
                var cell = worksheet.Cell(1, i);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            int row = 2;
            foreach (DespachoMercanciaReporteDto obj in objRegistro)
            {
                worksheet.Cell(row, 1).Value = obj.IdFormulario;
                worksheet.Cell(row, 2).Value = obj.DireccionDespacho;
                worksheet.Cell(row, 3).Value = obj.Pais;
                worksheet.Cell(row, 4).Value = obj.Cuidad;
                worksheet.Cell(row, 5).Value = obj.CodigoPostalEnvio;
                worksheet.Cell(row, 5).Value = obj.Telefono;
                row++;
            }

        }//Task<List<CumplimientoNormativoDto>> ConsultaCumplimientoNormativo()

        private async Task AddCumplimeintoNormativoSheet(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<CumplimientoNormativoDto> objRegistro = new List<CumplimientoNormativoDto>();
            objRegistro = await InterfaceReporteoTiemposCapaDatos.ConsultaCumplimientoNormativo(objetofiltro);
            worksheet.Cell(1, 1).Value = "IdFormulario";
            worksheet.Cell(1, 2).Value = "¿Conforme a la normativa que le aplica, está obligado a implementar un programa de prevención de lavado de activos/financiación del terrorismo o proliferación de armas de destrucción masiva?";
            worksheet.Cell(1, 3).Value = "¿Conforme a la normativa que le aplica, está obligado a implementar un programa de transparencia y ética empresarial o anticorrupción?";
            worksheet.Cell(1, 4).Value = "¿Actualmente tiene contratos o transacciones con entidades del sector público?";
            worksheet.Cell(1, 5).Value = "¿Realiza actividades con activos virtuales y/o criptoactivos?";
            worksheet.Cell(1, 6).Value = "Intercambio entre Activos Virtuales y monedas fiat";
            worksheet.Cell(1, 7).Value = "Intercambio entre una o más formas de Activos Virtuales";
            worksheet.Cell(1, 8).Value = "Transferencia de Activos Virtuales";
            worksheet.Cell(1, 9).Value = "Custodia o administración de Activos Virtuales o instrumentos que permitan el control sobre Activos Virtuales";
            worksheet.Cell(1, 10).Value = "Participación y provisión de servicios financieros relacionados con la oferta de un emisor o venta de un Activo Virtual";
            worksheet.Cell(1, 11).Value = "En general, servicios relacionados con Activos Virtuales";
            worksheet.Cell(1, 12).Value = "31 de diciembre del año inmediatamente anterior, obtuvo Ingresos Totales iguales o superiores a tres mil (3.000) SMLMV o tuvo Activos iguales o superiores a cinco mil (5.000) SMLMV";


            for (int i = 1; i <= 12; i++)
            {
                var cell = worksheet.Cell(1, i);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }



            int row = 2;
            foreach (CumplimientoNormativoDto obj in objRegistro)
            {
                //worksheet.Cell(row, 1).Value = obj.IdFormulario;
                //worksheet.Cell(row, 2).Value =ConversoroOpciones.ConvertirEnteroARespuesta(obj.ObligadoProgramaLA.ToString());
                //worksheet.Cell(row, 3).Value =ConversoroOpciones.ConvertirEnteroARespuesta(obj.ObligadoProgramaEE.ToString());
                //worksheet.Cell(row, 4).Value =ConversoroOpciones.ConvertirEnteroARespuesta(obj.TieneContratosSP.ToString());
                //worksheet.Cell(row, 5).Value =ConversoroOpciones.ConvertirEnteroARespuesta(obj.ActividadesActivosVirtuales.ToString());
                //worksheet.Cell(row, 6).Value = ConversoroOpciones.ConvertirEnteroARespuesta(obj.IntercambiosActivosMoneda.ToString());
                //worksheet.Cell(row, 7).Value = ConversoroOpciones.ConvertirEnteroARespuesta(obj.IntercambioActivosVirtuales.ToString());
                //worksheet.Cell(row, 8).Value = ConversoroOpciones.ConvertirEnteroARespuesta(obj.TransferenciasActivosVirtuales.ToString());
                //worksheet.Cell(row, 9).Value = ConversoroOpciones.ConvertirEnteroARespuesta(obj.CustodiaAdministraAC.ToString());
                //worksheet.Cell(row, 10).Value = ConversoroOpciones.ConvertirEnteroARespuesta(obj.ParticipacionSFAV.ToString());
                //worksheet.Cell(row, 11).Value = ConversoroOpciones.ConvertirEnteroARespuesta(obj.ServiciosAV.ToString());
                //worksheet.Cell(row, 12).Value = ConversoroOpciones.ConvertirEnteroARespuesta(obj.IngresosYearAnterior.ToString());
                row++;
            }

        }//77Task<List<ArchivoDto>> ConsultaInfoArchivoCargados()


         private async Task AddAdjuntosSheet(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<ArchivoDto> objRegistro = new List<ArchivoDto>();
            objRegistro = await InterfaceReporteoTiemposCapaDatos.ConsultaInfoArchivoCargados(objetofiltro);
            worksheet.Cell(1, 1).Value = "IdFormulario";
            worksheet.Cell(1, 2).Value = "Nombre Archivo";
            worksheet.Cell(1, 3).Value = "Extencion";
            worksheet.Cell(1, 4).Value = "Clase Archivo";

            for (int i = 1; i <= 4; i++)
            {
                var cell = worksheet.Cell(1, i);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            int row = 2;
            foreach (ArchivoDto obj in objRegistro)
            {
                worksheet.Cell(row, 1).Value = obj.IdFormulario;
                worksheet.Cell(row, 2).Value = obj.NombreArchivo;
                worksheet.Cell(row, 3).Value = obj.extencion;
                worksheet.Cell(row, 4).Value = obj.Key;
                row++;
            }

        }


        private async Task AddInfoOEASheet(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<FormularioModelDTO> objRegistro = new List<FormularioModelDTO>();
            objRegistro = await InterfaceReporteoTiemposCapaDatos.ConsultaDatosInformacionOEA(objetofiltro);
            worksheet.Cell(1, 1).Value = "IdFormulario";
            worksheet.Cell(1, 2).Value = "Uen";
            worksheet.Cell(1, 3).Value = "ResponsableVenta";
            worksheet.Cell(1, 4).Value = "CorreoElectronico";
            worksheet.Cell(1, 5).Value = "ResponsableCartera";
            worksheet.Cell(1, 6).Value = "ResponsableTecnico";
            worksheet.Cell(1, 7).Value = "Moneda";
            worksheet.Cell(1, 8).Value = "FormaPago";
            worksheet.Cell(1, 9).Value = "NumeroDias";
            worksheet.Cell(1, 10).Value = "CadenaLogistica";
            worksheet.Cell(1, 11).Value = "ListasRiesgo";
            worksheet.Cell(1, 12).Value = "SustanciasNarcoticos";
            worksheet.Cell(1, 13).Value = "Certificaciones";
            worksheet.Cell(1, 14).Value = "ProveedorCadenaLogistica";
            worksheet.Cell(1, 15).Value = "RiesgoPais";
            worksheet.Cell(1, 16).Value = "AntiguedadEmpresa";
            worksheet.Cell(1, 17).Value = "RiesgoSeguridad";
            worksheet.Cell(1, 18).Value = "Valoracion";
            worksheet.Cell(1, 19).Value = "ListasRiesgoCliente";
            worksheet.Cell(1, 20).Value = "TipoNegociacion";
            worksheet.Cell(1, 21).Value = "VistoBuenoAseguradora";
            worksheet.Cell(1, 22).Value = "RiesgoPaisCliente";
            worksheet.Cell(1, 23).Value = "CertificacionesInstitucionalidad";
            worksheet.Cell(1, 24).Value = "RiesgoSeguridadCliente";
            worksheet.Cell(1, 25).Value = "ValoracionCliente";
            worksheet.Cell(1, 26).Value = "SegmentacionRiesgo";
            worksheet.Cell(1, 27).Value = "Usuario";

            for (int i = 1; i <= 27; i++)
            {
                var cell = worksheet.Cell(1, i);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }


            int row = 2;
            foreach (FormularioModelDTO obj in objRegistro)
            {
                worksheet.Cell(row, 1).Value = obj.IdFormulario;
                worksheet.Cell(row, 2).Value = obj.Uen;
                worksheet.Cell(row, 3).Value = obj.ResponsableVenta;
                worksheet.Cell(row, 4).Value = obj.CorreoElectronico;
                worksheet.Cell(row, 5).Value = obj.ResponsableCartera;
                worksheet.Cell(row, 6).Value = obj.ResponsableTecnico;
                worksheet.Cell(row, 7).Value = obj.Moneda;
                worksheet.Cell(row, 8).Value = obj.FormaPago;
                worksheet.Cell(row, 9).Value = obj.NumeroDias;
                worksheet.Cell(row, 10).Value = obj.CadenaLogistica;
                worksheet.Cell(row, 11).Value = obj.ListasRiesgo;
                worksheet.Cell(row, 12).Value = obj.SustanciasNarcoticos;
                worksheet.Cell(row, 13).Value = obj.Certificaciones;
                worksheet.Cell(row, 14).Value = obj.ProveedorCadenaLogistica;
                worksheet.Cell(row, 15).Value = obj.RiesgoPais;
                worksheet.Cell(row, 16).Value = obj.AntiguedadEmpresa;
                worksheet.Cell(row, 17).Value = obj.RiesgoSeguridad;
                worksheet.Cell(row, 18).Value = obj.Valoracion;
                worksheet.Cell(row, 19).Value = obj.ListasRiesgoCliente;
                worksheet.Cell(row, 20).Value = obj.TipoNegociacion;
                worksheet.Cell(row, 21).Value = obj.VistoBuenoAseguradora;
                worksheet.Cell(row, 22).Value = obj.RiesgoPaisCliente;
                worksheet.Cell(row, 23).Value = obj.CertificacionesInstitucionalidad;
                worksheet.Cell(row, 24).Value = obj.RiesgoSeguridadCliente;
                worksheet.Cell(row, 25).Value = obj.ValoracionCliente;
                worksheet.Cell(row, 26).Value = obj.SegmentacionRiesgo;
                worksheet.Cell(row, 27).Value = obj.IdFormulario;               
                row++;
            }

        }



        private async Task AddInformacionTributaria(IXLWorksheet worksheet, ReporteDto objetofiltro)
        {
            List<InformacionTributariaDTO> objRegistro = new List<InformacionTributariaDTO>();
            objRegistro = await InterfaceReporteoTiemposCapaDatos.ConsultaInformacionTributaria(objetofiltro);
            worksheet.Cell(1, 1).Value = "IdFormulario";
            worksheet.Cell(1, 2).Value = "¿Es gran contribuyente?";
            worksheet.Cell(1, 3).Value = "Número de resolución";
            worksheet.Cell(1, 4).Value = "Fecha de la resolución";
            worksheet.Cell(1, 5).Value = "¿Es Autorretenedor?";
            worksheet.Cell(1, 6).Value = "Número de resolución";
            worksheet.Cell(1, 7).Value = "Fecha de la resolución";
            worksheet.Cell(1, 8).Value = "¿Es Responsable de ICA?";
            worksheet.Cell(1, 9).Value = "Municipio a retener";
            worksheet.Cell(1, 10).Value = "Tarifa";
            worksheet.Cell(1, 11).Value = "¿Es responsable de IVA?";
            worksheet.Cell(1, 12).Value = "Agente Retenedor IVA";
           

            for (int i = 1; i <= 12; i++)
            {
                var cell = worksheet.Cell(1, i);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }


            int row = 2;
            foreach (InformacionTributariaDTO obj in objRegistro)
            {
                worksheet.Cell(row, 1).Value = obj.IdFormulario;
                worksheet.Cell(row, 2).Value = ConversoroOpciones.ConvertirEnteroARespuesta(obj.GranContribuyente.ToString());
                worksheet.Cell(row, 3).Value = obj.NumResolucionGranContribuyente;
                worksheet.Cell(row, 4).Value = obj.FechaResolucionGranContribuyente;
                worksheet.Cell(row, 5).Value = ConversoroOpciones.ConvertirEnteroARespuesta(obj.Autorretenedor.ToString());
                worksheet.Cell(row, 6).Value = obj.NumResolucionAutorretenedor;
                worksheet.Cell(row, 7).Value = obj.FechaResolucionAutorretenedor;
                worksheet.Cell(row, 8).Value = ConversoroOpciones.ConvertirEnteroARespuesta(obj.ResponsableICA.ToString());
                worksheet.Cell(row, 9).Value = obj.MunicipioRetener;
                worksheet.Cell(row, 10).Value = obj.Tarifa;
                worksheet.Cell(row, 11).Value = ConversoroOpciones.ConvertirEnteroARespuesta(obj.ResponsableIVA.ToString());
                worksheet.Cell(row, 12).Value = ConversoroOpciones.ConvertirEnteroARespuesta( obj.AgenteRetenedorIVA.ToString());
               
                row++;
            }

        }


    }
}
