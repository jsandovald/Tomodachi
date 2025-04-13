using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TomodachiCoffee.UserControls;

namespace TomodachiCoffee.Models
{
    public class ViewModel : ViewModelBase
    {
        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        public Action LogoutAction { get; set; }

        private UserControl _currentView;

        public UserControl CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ViewModel()
        {
            NavigateCommand = new RelayCommand<string>(NavigateTo);
            LogoutCommand = new RelayCommand(Logout);

            // Establecer vista por defecto al iniciar
            CurrentView = new HomeView();
        }

        private void NavigateTo(string page)
        {
            Console.WriteLine($"Navegando a {page}");
            // Aquí puedes cambiar la vista o hacer alguna acción específica

            switch (page)
            {
                case "Home":
                    CurrentView = new HomeView();
                    break;
                case "Finance":
                    // Agrega más vistas según las tengas
                    // CurrentView = new FinanceView();
                    break;
                case "Upload":
                    CurrentView = new UploadView();
                    break;
                default:
                    break;
            }
        }

        private void Logout()
        {
            Console.WriteLine("Cerrando sesión...");
            // Lógica para cerrar sesión

            // Ejecutar la acción que viene desde la View
            LogoutAction?.Invoke();

            // 1️⃣ Cerrar la ventana actual (asumiendo que es la principal)
            //Application.Current.MainWindow.Hide();

            // 2️⃣ Mostrar la ventana de inicio de sesión
            //LoginWindow loginWindow = new LoginWindow();
            //loginWindow.Show();
        }
    }


}
