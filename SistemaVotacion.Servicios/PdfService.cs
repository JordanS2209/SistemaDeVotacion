using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using SistemaVotacion.Servicios.Interfaces;
using iText.Layout.Properties;
using System.IO;

namespace SistemaVotacion.Servicios
{
    public class PdfService : IPdfService
    {
        public byte[] GenerarPdfResultadosGeneral(List<SistemaVotacion.Modelos.ResultadoGeneralDto> data, int? idProvincia)
        {
            if (data == null) data = new List<SistemaVotacion.Modelos.ResultadoGeneralDto>();

            using (var ms = new MemoryStream())
            {
                WriterProperties props = new WriterProperties();
                props.SetFullCompressionMode(false);

                using (var writer = new PdfWriter(ms, props))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        using (var document = new Document(pdf))
                        {
                            SetSafeFont(document);

                            document.Add(new Paragraph("Resultados Eleccion General").SetFontSize(18));

                            string filtroMsg = idProvincia.HasValue ? $"Filtro Provincia: {idProvincia}" : "Nivel: Nacional";
                            document.Add(new Paragraph(filtroMsg).SetFontSize(10));
                            document.Add(new Paragraph(" "));

                            var table = new Table(3); // Lista, Numero, Votos
                            table.SetWidth(UnitValue.CreatePercentValue(100));

                            table.AddHeaderCell("Lista");
                            table.AddHeaderCell("Numero");
                            table.AddHeaderCell("Votos");
                            
                            foreach (var item in data)
                            {
                                table.AddCell(new Paragraph(item.Lista ?? "N/A"));
                                table.AddCell(new Paragraph(item.Numero.ToString()));
                                table.AddCell(new Paragraph(item.Votos.ToString()));
                            }
                            document.Add(table);
                        }
                    }
                }
                return ms.ToArray();
            }
        }

        public byte[] GenerarPdfConsultaPopular(dynamic data)
        {
             using (var ms = new MemoryStream())
            {
         
                WriterProperties props = new WriterProperties();
                props.SetFullCompressionMode(false);

                using (var writer = new PdfWriter(ms, props))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        using (var document = new Document(pdf))
                        {
                            SetSafeFont(document);

                            document.Add(new Paragraph("Resultados Consulta Popular").SetFontSize(18));
                            document.Add(new Paragraph(" "));

                            
                            IEnumerable<dynamic> list = null;
                            try 
                            { 
                  
                                list = data as IEnumerable<dynamic>;
                                if(list == null && data != null)
                                {
                                   
                                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                                    list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);
                                }
                            } 
                            catch { }

                            if (list != null)
                            {
                                foreach (var preg in list)
                                {
                                    string txtPregunta = "Pregunta";
                                    try { txtPregunta = (string)preg.Pregunta ?? "Pregunta"; } catch { }

                                    document.Add(new Paragraph(txtPregunta));
                                    
                                    var table = new Table(2);
                                    table.SetWidth(UnitValue.CreatePercentValue(100));

                                    table.AddHeaderCell("Opcion");
                                    table.AddHeaderCell("Votos");
                                    
                                    IEnumerable<dynamic> resultados = null;
                                    try 
                                    { 
                                        // Safely get nested collection
                                        var resRaw = preg.Resultados;
                                        if(resRaw != null)
                                        {
                                             var jsonRes = Newtonsoft.Json.JsonConvert.SerializeObject(resRaw);
                                             resultados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(jsonRes);
                                        }
                                    } 
                                    catch { }

                                    if (resultados != null)
                                    {
                                        foreach (var res in resultados)
                                        {
                                            string opcion = "N/A";
                                            string votos = "0";

                                            try { opcion = (string)res.Opcion ?? "N/A"; } catch { }
                                            try { votos = ((object)res.Votos ?? 0).ToString(); } catch { }

                                            table.AddCell(new Paragraph(opcion));
                                            table.AddCell(new Paragraph(votos));
                                        }
                                    }
                                    document.Add(table);
                                    document.Add(new Paragraph(" "));
                                }
                            }
                        }
                    }
                }
                return ms.ToArray();
            }
        }

        private void SetSafeFont(Document document)
        {
             PdfFont font = null;
             try 
             { 
              
                font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA); 
             } 
             catch 
             {
                try 
                {
                  
                    font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                }
                catch { }
             }
            
             if (font != null) 
             {
                document.SetFont(font);
             }
        }
    }
}
