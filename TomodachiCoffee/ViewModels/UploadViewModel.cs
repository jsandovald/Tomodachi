using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows;
using TomodachiCoffee.Models;
using System.IO;
using System.Data;
using TomodachiCoffee.Models;
using ExcelDataReader;

namespace TomodachiCoffee.ViewModels
{
    public class UploadViewModel : ViewModelBase
    {
        private string _rutaArchivo;
        public string RutaArchivo
        {
            get => _rutaArchivo;
            set
            {
                if (SetProperty(ref _rutaArchivo, value))
                {
                    // Notificar que la condición del comando puede haber cambiado
                    (CargarArchivoCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand SeleccionarArchivoCommand { get; }
        public ICommand CargarArchivoCommand { get; }

        public ObservableCollection<ReporteItem> ReporteItems { get; set; } = new ObservableCollection<ReporteItem>();

        public ICommand CargarReporteCommand { get; }

        public UploadViewModel()
        {
            SeleccionarArchivoCommand = new RelayCommand(SeleccionarArchivo);
            CargarArchivoCommand = new RelayCommand(CargarArchivo, PuedeCargar);
            CargarReporteCommand = new RelayCommand(CargarReporte);
        }

        private void SeleccionarArchivo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos de Excel (*.xls;*.xlsx)|*.xls;*.xlsx",
                Title = "Seleccionar archivo Excel"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RutaArchivo = openFileDialog.FileName;
            }
        }

        private bool PuedeCargar()
        {
            return !string.IsNullOrEmpty(RutaArchivo) && File.Exists(RutaArchivo);
        }

        private void CargarArchivo()
        {
            try
            {
                var reporte = LeerHojaExcel(RutaArchivo, 3, "Ventas");
                var detalleReporte = LeerHojaExcel(RutaArchivo, 0, "Adiciones");

                //Database.InsertDataTable(reporte, "reporte");
                Database.InsertDataTable(detalleReporte, "detallereporte");
                

                MessageBox.Show($"Archivo cargado correctamente:\n{RutaArchivo}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar archivo:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static DataTable LeerHojaExcel(string rutaArchivo, int filaEncabezado, string hojaNombreOIndice)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = File.Open(rutaArchivo, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = false
                }
            });

            // Obtener la hoja específica por nombre o índice
            DataTable hoja = null;

            if (int.TryParse(hojaNombreOIndice, out int indice))
            {
                if (indice >= 0 && indice < dataSet.Tables.Count)
                    hoja = dataSet.Tables[indice];
            }
            else
            {
                hoja = dataSet.Tables.Cast<DataTable>().FirstOrDefault(t => t.TableName.Equals(hojaNombreOIndice, StringComparison.OrdinalIgnoreCase));
            }

            if (hoja == null || filaEncabezado >= hoja.Rows.Count)
                throw new ArgumentException("Hoja no encontrada o fila de encabezado fuera de rango.");

            // Crear DataTable con nombres de columnas desde la fila de encabezado
            var columnas = hoja.Rows[filaEncabezado]
                .ItemArray
                .Select((c, i) => new { Nombre = c?.ToString() ?? $"Col{i}", Index = i })
                .ToList();

            var tabla = new DataTable();
            columnas.ForEach(c => tabla.Columns.Add(c.Nombre));

            for (int i = filaEncabezado + 1; i < hoja.Rows.Count; i++)
            {
                var fila = hoja.Rows[i];
                if (fila.ItemArray.All(item => string.IsNullOrWhiteSpace(item?.ToString()))) continue;

                var nuevaFila = tabla.NewRow();
                for (int j = 0; j < columnas.Count; j++)
                {
                    nuevaFila[j] = fila[columnas[j].Index];
                }
                tabla.Rows.Add(nuevaFila);
            }

            return tabla;
        }

        private void CargarReporte()
        {
            ReporteItems.Clear();
            DataTable dt = Database.ExecuteQuery("SELECT * FROM bd_cafeteria.reporte ORDER BY Fecha DESC LIMIT 10");

            foreach (DataRow row in dt.Rows)
            {
                ReporteItems.Add(new ReporteItem
                {
                    Id = row["IdReporte"]?.ToString(),
                    Fecha = row["Fecha"]?.ToString(),
                    Total = row["Total"]?.ToString(),
                    MedioPago = row["MedioPago"]?.ToString()
                    // Agrega más columnas según tu tabla
                });
            }
        }
    }
}