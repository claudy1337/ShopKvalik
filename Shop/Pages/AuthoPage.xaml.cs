using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Shop.DataBase;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shop.my_ado;

namespace Shop.Pages
{
    /// <summary>
    /// Interaction logic for AuthoPage.xaml
    /// </summary>
    public partial class AuthoPage : Page
    {
        private int IncorrectTry = 0;
        public AuthoPage()
        {
            InitializeComponent();
            TBLogin.Text = Properties.Settings.Default.Login;
        }

        private void BtnRegistrate_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }
        
        private void BtnAuthorize_Click(object sender, RoutedEventArgs e)
        {
            if (DataAccess.IsCorrectUser(TBLogin.Text, TBPassword.Password) && DateTime.Now > DataAccess.GetLastBanSession().DateEnd)
            {
                if (RememberUser.IsChecked.GetValueOrDefault())
                    Properties.Settings.Default.Login = TBLogin.Text;
                else
                    Properties.Settings.Default.Login = null;
                Properties.Settings.Default.Save();
                MessageBox.Show("WELCUM");
                NavigationService.Navigate(new ProductsListPage(DataAccess.GetUser(TBLogin.Text, TBPassword.Password)));
            }
            else if(DateTime.Now < DataAccess.GetLastBanSession().DateEnd)
                MessageBox.Show($"Бан закончится {DataAccess.GetLastBanSession().DateEnd}");
            else if(DataAccess.IsIncorrectUser(TBLogin.Text, TBPassword.Password) && DataAccess.GetLastBanSession().DateEnd < DateTime.Now)
            {
                MessageBox.Show("who da fuck r' u? identify yo'self, nigga");
                IncorrectTry++;
                if(IncorrectTry == 3)
                {
                    BanSession session = new BanSession();
                    session.DateStart = DateTime.Now;
                    session.DateEnd = DateTime.Now.AddMinutes(1);
                    DataAccess.StartBan(session);
                    MessageBox.Show("Блокировка на 1 минуту");
                    IncorrectTry = 0;
                }
            }
        }
    }
}
