using ScottPlot;
using SistemaVotacion.Servicios.Interfaces;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SistemaVotacion.Servicios
{
    public class ChartService : IChartService
    {
        public byte[] GeneratePieChart(dynamic data)
        {
            try
            {
                var myPlot = new ScottPlot.Plot();
                
                List<double> values = new List<double>();
                List<string> labels = new List<string>();

                // Data processing
                IEnumerable<dynamic> items = (IEnumerable<dynamic>)data;
                foreach (var item in items)
                {
                    // Safe parsing for JValue/JObject with case-insensitive fallback
                    double votos = 0;
                    string nombre = "N/A";

                    // Try PascalCase then camelCase
                    try { votos = (double)item.Votos; } catch { 
                        try { votos = (double)item.votos; } catch { 
                             try { votos = (double)item["Votos"]; } catch { 
                                 try { votos = (double)item["votos"]; } catch { } 
                             }
                        } 
                    }

                    try { nombre = (string)item.Lista; } catch { 
                        try { nombre = (string)item.lista; } catch { 
                             try { nombre = (string)item["Lista"]; } catch { 
                                 try { nombre = (string)item["lista"]; } catch { }
                             }
                        } 
                    }

                    if (votos > 0)
                    {
                        values.Add(votos);
                        labels.Add(nombre);
                    }
                }

                if (values.Count == 0 || values.All(x => x == 0))
                {
                     myPlot.Title("Sin Votos Registrados");
                     myPlot.HideGrid();
                     myPlot.Axes.Margins(0, 0);
                     myPlot.Axes.Bottom.IsVisible = false;
                     myPlot.Axes.Left.IsVisible = false;
                     myPlot.Axes.Right.IsVisible = false;
                     myPlot.Axes.Top.IsVisible = false;
                }
                else
                {
                    var pie = myPlot.Add.Pie(values.ToArray());
                    pie.ExplodeFraction = 0.05;
                    // pie.DonutFraction = 0.6; // Comentado por error de versión (API v5 cambia rápido)
                    pie.ShowSliceLabels = true;
                    pie.SliceLabelDistance = 1.2; 
                    
                    // Configuración de estilo global para etiquetas
                    // Dependiendo de la versión exacta de ScottPlot 5, puede ser SliceLabelStyle o similar.
                    // Probaremos lo más estándar. Si falla, las etiquetas default son legibles.
                    
                    double total = values.Sum();

                    for (int i = 0; i < pie.Slices.Count; i++)
                    {
                         double p = (values[i] / total) * 100;
                         pie.Slices[i].Label = $"{labels[i]}\n({p:0.0}%)";
                         // pie.Slices[i].LabelFontSize = 12; // Propiedad no existente en PieSlice
                    }

                    myPlot.Title("Resultados Generales");
                    
                    // Hide everything for a clean look
                    myPlot.HideGrid();
                    
                    // Manual hiding for ScottPlot 5 compatibility
                    myPlot.Axes.Margins(0, 0);
                    myPlot.Axes.Bottom.IsVisible = false;
                    myPlot.Axes.Left.IsVisible = false;
                    myPlot.Axes.Right.IsVisible = false;
                    myPlot.Axes.Top.IsVisible = false;
                    
                    myPlot.ShowLegend();
                }

                return myPlot.GetImage(600, 400).GetImageBytes();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Chart Error: " + ex.Message);
                return null;
            }
        }
    }
}
