using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using TomodachiCoffee.Models;

namespace TomodachiCoffee.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        /*private ObservableCollection<string> _productos;

        public ObservableCollection<string> Productos
        {
            get => _productos;
            set => SetProperty(ref _productos, value);
        }*/

        public SeriesCollection MesSeries { get; set; }
        public string[] MesLabels { get; set; }

        public HomeViewModel()
        {
            var datos = new List<(string Mes, double Total)>();

            var dt = Database.ExecuteQuery(@"
                SELECT month(Fecha), left(monthname(Fecha), 3) as Mes, SUM(Total) AS Total 
                FROM bd_cafeteria.ventas
                WHERE Fecha >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)
                GROUP BY month(Fecha), left(monthname(Fecha), 3)
                ORDER BY month(Fecha) ASC
            ");

            foreach (DataRow row in dt.Rows)
            {
                string medioPago = row["Mes"].ToString();
                double total = Convert.ToDouble(row["Total"]);
                datos.Add((medioPago, total));
            }

            MesLabels = datos.Select(d => d.Mes).ToArray();
            MesSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Ventas x Mes",
                    Values = new ChartValues<double>(datos.Select(d => (double)d.Total)),
                    Fill = (Brush)new BrushConverter().ConvertFromString("#7166f9")
                }
            };
        }
    }
}