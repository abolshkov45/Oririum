using System.Threading.Tasks;
using System.Windows;
using Oririum.Model;

namespace Oririum.View
{
    /// <summary>
    /// Логика взаимодействия для RegistrationDialog.xaml
    /// </summary>
    public partial class RegistrationDialog : Window
    {
        public RegistrationDialog()
        {
            InitializeComponent();
        }

        private async void CreateAccount_OnClick(object sender, RoutedEventArgs e)
        {
            CreateAccount.IsEnabled = false;
            RegistrationPassword.IsEnabled = false;

            if (RegistrationPassword.Password != "")
            {
                string password = RegistrationPassword.Password;
                await Task.Run(() => OririumAccount.Registration(password));
            }
            else
            {
                MessageBox.Show("Заполните все поля!", "Внимание", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            RegistrationPassword.Clear();
            CreateAccount.IsEnabled = true;
            RegistrationPassword.IsEnabled = true;
        }
    }
}
