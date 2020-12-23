using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Oririum.Annotations;
using Oririum.Model;
using Oririum.View;

namespace Oririum.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            EnabledLoginControl = true;
            RunningTransactionVisibility = Visibility.Collapsed;

            GetSettings();

            SetVisibilityLogin();
        }

        private string accountTo;

        public string AccountTo
        {
            get => accountTo;
            set
            {
                accountTo = value;
                OnPropertyChanged("AccountTo");
            }
        }

        private string countCoin;

        public string CountCoin
        {
            get => countCoin;
            set
            {
                countCoin = value;
                OnPropertyChanged("CountCoin");
            }
        }

        private string accountAddress;

        public string AccountAddress
        {
            get => accountAddress;
            set
            {
                accountAddress = value;
                OnPropertyChanged("AccountAddress");
            }
        }

        private string accountPassword;

        public string AccountPassword
        {
            get => accountPassword;
            set
            {
                accountPassword = value;
                OnPropertyChanged("AccountPassword");
            }
        }


        private string accountBalance;

        public string AccountBalance
        {
            get => accountBalance;
            set
            {
                accountBalance = value;
                OnPropertyChanged("AccountBalance");
            }
        }
        private string pathToAccountFile;

        public string PathToAccountFile
        {
            get => pathToAccountFile;
            set
            {
                pathToAccountFile = value;
                OnPropertyChanged("PathToAccountFile");
            }
        }

        private bool enabledLoginControl;

        public bool EnabledLoginControl
        {
            get => enabledLoginControl;
            set
            {
                enabledLoginControl = value;
                OnPropertyChanged("EnabledLoginControl");
            }
        }

        private Visibility runningTransactionVisibility;

        public Visibility RunningTransactionVisibility
        {
            get => runningTransactionVisibility;
            set
            {
                runningTransactionVisibility = value;
                OnPropertyChanged("RunningTransactionVisibility");
            }
        }

        private Visibility loginVisibility;

        public Visibility LoginVisibility
        {
            get => loginVisibility;
            set
            {
                loginVisibility = value;
                OnPropertyChanged("LoginVisibility");
            }
        }

        private Visibility accountVisibility;

        public Visibility AccountVisibility
        {
            get => accountVisibility;
            set
            {
                accountVisibility = value;
                OnPropertyChanged("AccountVisibility");
            }
        }

        private string ipAddress;

        public string IpAddress
        {
            get => ipAddress;
            set
            {
                ipAddress = value;
                OnPropertyChanged("IpAddress");
            }
        }

        private string port;

        public string Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged("Port");
            }
        }

        private RelayCommand loginCommand;

        public RelayCommand LoginCommand
        {
            get
            {
                return loginCommand ??= new RelayCommand(async obj =>
                {
                    AccountPassword = GetPassword(obj);

                    if (AccountAddress != "" && AccountPassword != "")
                    {
                        EnabledLoginControl = false;

                        if (await OririumAccount.Login(AccountAddress, AccountPassword))
                        {
                            OririumAccount.AccountAddress = AccountAddress;
                            SetVisibilityAccount();
                        }

                        EnabledLoginControl = true;
                    }
                    else
                    {
                        MessageBox.Show("Заполните все поля!", "Внимание", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                });
            }
        }

        private RelayCommand loginFromFileCommand;

        public RelayCommand LoginFromFileCommand
        {
            get
            {
                return loginFromFileCommand ??= new RelayCommand(async obj =>
                {
                    if (PathToAccountFile != "")
                    {
                        EnabledLoginControl = false;

                        if (await OririumAccount.Login(PathToAccountFile))
                        {
                            SetVisibilityAccount();
                        }

                        EnabledLoginControl = true;
                    }
                    else
                    {
                        MessageBox.Show("Заполните все поля!", "Внимание", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                });
            }
        }

        private RelayCommand selectedAccountFromFileCommand;

        public RelayCommand SelectedAccountFromFileCommand
        {
            get
            {
                return selectedAccountFromFileCommand ??= new RelayCommand(async obj =>
                {
                    var open = new OpenFileDialog()
                    {
                        Filter = "JSON File | *.json"
                    };

                    if (open.ShowDialog() == true)
                    {
                        PathToAccountFile = open.FileName;
                    }
                });
            }
        }

        private RelayCommand registrationCommand;

        public RelayCommand RegistrationCommand
        {
            get
            {
                return registrationCommand ??= new RelayCommand(obj =>
                {
                    var registrationDialog = new RegistrationDialog();
                    registrationDialog.ShowDialog();
                });
            }
        }

        private RelayCommand saveSettingsCommand;

        public RelayCommand SaveSettingsCommand
        {
            get
            {
                return saveSettingsCommand ??= new RelayCommand(async obj =>
                {
                    SaveSettings();
                });
            }
        }

        private RelayCommand getBalanceCommand;

        public RelayCommand GetBalanceCommand
        {
            get
            {
                return getBalanceCommand ??= new RelayCommand(async obj =>
                {
                    GetAccountData();
                });
            }
        }

        private RelayCommand sendTokenCommand;

        public RelayCommand SendTokenCommand
        {
            get
            {
                return sendTokenCommand ??= new RelayCommand(async obj =>
                {
                    if (AccountTo != null && CountCoin != null && long.TryParse(CountCoin, out _))
                    {
                        var transactionConfirm =
                            MessageBox.Show(
                                $"Вы действительно хотите перевести {CountCoin} Oririum на кошелек {AccountTo}?",
                                "Транзакция", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (transactionConfirm == MessageBoxResult.Yes)
                        {
                            RunningTransactionVisibility = Visibility.Visible;

                            if (await Task.Run(() => OririumAccount.SendTransaction(AccountTo, CountCoin)))
                            {
                                await Task.Run(GetAccountData);

                                AccountTo = null;
                                CountCoin = null;

                                MessageBox.Show("Транзакция завершена успешно!", "Информация", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                            }

                            RunningTransactionVisibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Заполните все поля!", "Внимание", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                });
            }
        }

        private async void GetAccountData()
        {
            AccountAddress = OririumAccount.AccountAddress;
            AccountBalance = await OririumAccount.GetBalance();
        }

        private string GetPassword(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            return password;
        }

        private void SetVisibilityLogin()
        {
            LoginVisibility = Visibility.Visible;
            AccountVisibility = Visibility.Collapsed;
        }

        private async void SetVisibilityAccount()
        {
            LoginVisibility = Visibility.Collapsed;
            AccountVisibility = Visibility.Visible;

            await Task.Run(GetAccountData);
        }

        private void GetSettings()
        {
            IpAddress = OririumSettings.Default.IpAddress;
            Port = OririumSettings.Default.Port;
        }

        private void SaveSettings()
        {
            if (IpAddress != "" && Port != "")
            {
                OririumSettings.Default.IpAddress = IpAddress;
                OririumSettings.Default.Port = Port;
                OririumSettings.Default.Save();

                MessageBox.Show("Настройки сохранены!", "Информация", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Введите адрес сервера и порт!", "Внимание", MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                GetSettings();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
