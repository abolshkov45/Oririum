using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3.Accounts.Managed;

namespace Oririum.Model
{
    public class OririumAccount
    {
        public static string RpcUrl = $@"http://{OririumSettings.Default.IpAddress}:{OririumSettings.Default.Port}";
        private static Web3Geth Web3 { get; set; }
        private static ManagedAccount Account { get; set; }
        public static string AccountAddress { get; set; }

        public static async Task<bool> Login(string address, string password)
        {
            try
            {
                Account = new ManagedAccount(address, password);
                Web3 = new Web3Geth(Account, RpcUrl);

                if (await Web3.Personal.UnlockAccount.SendRequestAsync(Account.Address, Account.Password, 120))
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static async Task<bool> Login(string fileName)
        {
            try
            {
                Account restoredAccount;

                await using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    restoredAccount = await JsonSerializer.DeserializeAsync<Account>(fs);
                }

                Account = new ManagedAccount(restoredAccount.Address, restoredAccount.Password);
                Web3 = new Web3Geth(Account, RpcUrl);

                if (await Web3.Personal.UnlockAccount.SendRequestAsync(Account.Address, Account.Password, 120))
                {
                    AccountAddress = restoredAccount.Address;
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static async void Registration(string password)
        {
            try
            {
                Web3 = new Web3Geth(RpcUrl);

                var address = await Web3.Personal.NewAccount.SendRequestAsync(password);

                var account = new Account()
                {
                    Address = address,
                    Password = password
                };

                await using (var fs = new FileStream($@"{Directory.GetCurrentDirectory()}\{address}.json", FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync(fs, account);
                }

                MessageBox.Show(
                    "Регистрация прошла успешно. В папке программы создан файл с данными аккаунта. Вы можете выполнить вход используя данные из файла или при помощи загрузки файла в программу.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static async Task<string> GetBalance()
        {
            try
            {
                var balance = await Web3.Eth.GetBalance.SendRequestAsync(AccountAddress);
                return balance.Value.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static async Task<bool> SendTransaction(string accountAddressTo, string countSendToken)
        {
            try
            {
                if (Convert.ToInt32(await Task.Run(GetBalance)) >= Convert.ToInt64(countSendToken))
                {
                    var transaction = await Web3.TransactionManager.SendTransactionAsync(Account.Address, accountAddressTo, new HexBigInteger(Convert.ToInt64(countSendToken)));

                    await Web3.Miner.Start.SendRequestAsync(1);

                    var receipt = await Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transaction);

                    while (receipt == null)
                    {
                        await Task.Delay(1500);
                        receipt = await Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transaction);
                    }

                    await Web3.Miner.Stop.SendRequestAsync();

                    return true;
                }

                MessageBox.Show("Недостаточно средств!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
