using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TomodachiCoffee.ViewModels;

namespace TomodachiCoffee.UserControls
{
    /// <summary>
    /// Lógica de interacción para HomeView.xaml
    /// </summary>
    public partial class ProveedorView : UserControl
    {
        private ObservableCollection<ProveedorProducto> listaProveedorProducto;
        private ObservableCollection<Producto> listaProducto;
        
        public ProveedorView()
        {
            InitializeComponent();
            listaProveedorProducto = new ObservableCollection<ProveedorProducto>();
            listaProducto = new ObservableCollection<Producto>();
        }

        private void cmbTabla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)cmbTabla.SelectedItem)?.Content.ToString() == "ProveedorProducto")
            {
                // Aquí cargarías desde BD si es necesario
                dgDatos.ItemsSource = listaProveedorProducto;
            }
            else if (((ComboBoxItem)cmbTabla.SelectedItem)?.Content.ToString() == "Producto")
            {
                // Aquí cargarías desde BD si es necesario
                dgDatos.ItemsSource = listaProducto;
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (((ComboBoxItem)cmbTabla.SelectedItem)?.Content.ToString() == "ProveedorProducto")
            {
                listaProveedorProducto.Add(new ProveedorProducto());
            }
            else if (((ComboBoxItem)cmbTabla.SelectedItem)?.Content.ToString() == "Producto")
            {
                listaProducto.Add(new Producto());
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgDatos.SelectedItem;
            if (selected != null)
            {
                if (selected is ProveedorProducto pp)
                    listaProveedorProducto.Remove(pp);
                else if (selected is Producto p)
                    listaProducto.Remove(p);
            }
        }
    
    }

    public class ProveedorProducto
    {
        public string Proveedor { get; set; }
        public string Producto { get; set; }
        public int UnidadPack { get; set; }
        public int PrecioNeto { get; set; }

        public double PrecioNetoUnidad => UnidadPack != 0 ? (double)PrecioNeto / UnidadPack : 0;
        public double IVA => Math.Round(PrecioNetoUnidad * 0.19, 0);
        public double Total => Math.Round(PrecioNetoUnidad + IVA, 0);
    }

    public class Producto
    {
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public int PrecioVenta { get; set; }
    }
}
