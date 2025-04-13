using System.Collections.ObjectModel;
using TomodachiCoffee.Models;

namespace TomodachiCoffee.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private ObservableCollection<string> _productos;

        public ObservableCollection<string> Productos
        {
            get => _productos;
            set => SetProperty(ref _productos, value);
        }

        public HomeViewModel()
        {
            CargarProductos();
        }

        private void CargarProductos()
        {
            var productos = new ObservableCollection<string>();
            var dt = Database.ExecuteQuery("SELECT Total FROM reporte");

            foreach (System.Data.DataRow row in dt.Rows)
            {
                productos.Add(row["Total"].ToString());
            }

            Productos = productos;
        }
    }
}