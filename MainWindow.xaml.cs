using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _12._5_HomeWork_WPFapp_clients_bank_base
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string PathBase = "База.txt";
        static string PathJournal = "Журнал.txt";
        static Repository data;
        static Journal log;
        static string postSelected = string.Empty;
        static int clientSelectedId;
        static Account accountSelected;
        static Account accountSource;
        static Account accountDestination;

        public MainWindow()
        {
            InitializeComponent();
            postSelector.ItemsSource = new string[] { "Консультант", "Менеджер" };

            data = Repository.CreateRepository(PathBase, PathJournal);
            clientsList.ItemsSource = data.Clients;

            log = Repository.CreateJournal();
            operationsList.ItemsSource = log.Lines;
        }
        //выбор должности
        private void postSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            postSelected = postSelector.SelectedValue.ToString();
        }
        //вывод счетов выбранного клиента
        private void clientsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clientSelectedId = (clientsList.SelectedItem as Client).clientId;
            listAccounts.ItemsSource = data.ClientsAccounts.Where(find);
        }
        //выбор счета
        private void listAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            accountSelected = listAccounts.SelectedItem as Account;
        }
        //метод поиска счетов клинта
        private bool find(Account arg)
        {
            return arg.clientId == (clientsList.SelectedItem as Client).clientId;
        }
        //открытие депозитного счета + запись в журнале
        private void openDepositBtn_Click(object sender, RoutedEventArgs e)
        {
            var temp = openDepositBtn.Content;
            string TypeAccount = Convert.ToString(temp);
            try
            {
                if (postSelected == string.Empty) throw new MyException("Должность не выбрана");
                resultOfOperation.Text = data.openAccount(clientSelectedId, TypeAccount, postSelected);
            }
            catch (MyException err)
            {
                resultOfOperation.Text = $"{err.Message}";
            }
        }
        //открытие текущего счета + запись в журнале
        private void openCurrentBtn_Click(object sender, RoutedEventArgs e)
        {
            var temp = openCurrentBtn.Content;
            string TypeAccount = Convert.ToString(temp);
            try
            {
                if (postSelected == string.Empty) throw new MyException("Должность не выбрана");
                resultOfOperation.Text = data.openAccount(clientSelectedId, TypeAccount, postSelected);
            }
            catch (MyException err)
            {
                resultOfOperation.Text = $"{err.Message}";
            }
        }
        //закрытие счета + запись в журнале
        private void closeAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (postSelected == string.Empty) throw new MyException("Должность не выбрана");
                resultOfOperation.Text = data.closeAccount(accountSelected, postSelected);
            }
            catch (MyException err)
            {
                resultOfOperation.Text = $"{err.Message}";
            }
        }
        //выбор счетов для перевода
        private void acceptAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (accountSource == null)
            {
                accountSource = accountSelected;
                accountIdToGetSum.Text = accountSource.accountId.ToString();
            }
            else
            {
                accountDestination = accountSelected;
                accountIdToPutSum.Text = accountDestination.accountId.ToString();
            }
        }
        //очистка выбранных счетов для перевода
        private void clearAccountsBtn_Click(object sender, RoutedEventArgs e)
        {
            accountSource = null;
            accountDestination = null;
            accountIdToGetSum.Text = null;
            accountIdToPutSum.Text = null;
        }
        //перевод между счетами + запись в журнале
        private void transferSumBtn_Click(object sender, RoutedEventArgs e)
        {
            double Sum;
            try
            {
                if (postSelected == string.Empty) throw new MyException("Должность не выбрана");
                if (!double.TryParse(sumToTransfer.Text, out Sum)) throw new MyException("Неверный формат суммы");

                resultOfOperation.Text = data.transferSumBetweenAccounts(accountSource, accountDestination, Sum, postSelected);
            }
            catch (MyException err)
            {
                resultOfOperation.Text = $"{err.Message}";
            }
        }
        //пополнение счета + запись в журнале
        private void additionAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            double Sum;
            try
            {
                if (postSelected == string.Empty) throw new MyException("Должность не выбрана");
                if (!double.TryParse(sumToAddition.Text, out Sum)) throw new MyException("Неверный формат суммы");

                resultOfOperation.Text = data.additionAccount(accountSelected, Sum, postSelected);
            }
            catch (MyException err)
            {
                resultOfOperation.Text = $"{err.Message}";
            }
        }
        //обновление списка счетов и журнала операций
        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            clientSelectedId = (clientsList.SelectedItem as Client).clientId;
            listAccounts.ItemsSource = data.ClientsAccounts.Where(find);

            operationsList.Items.Refresh();
        }
    }
}
