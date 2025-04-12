using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TomodachiCoffee.Models
{
    public class ViewModel
    {
        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        public Action LogoutAction { get; set; }

        public ViewModel()
        {
            NavigateCommand = new RelayCommand<string>(NavigateTo);
            LogoutCommand = new RelayCommand(Logout);
        }

        private void NavigateTo(string page)
        {
            Console.WriteLine($"Navegando a {page}");
            // Aquí puedes cambiar la vista o hacer alguna acción específica
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
