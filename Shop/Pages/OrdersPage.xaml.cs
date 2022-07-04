using System;
using System.Collections.Generic;
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
using Shop.my_ado;
using Shop.DataBase;


namespace Shop.Pages
{
    /// <summary>
    /// Interaction logic for OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        public static User currentUser;
        public OrdersPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            if(user.RoleId == 1)
            {
                DGOrders.ItemsSource = DataAccess.GetOrders().Where(o => o.StatusOrderId == 1 || o.WorkerId == currentUser.Worker.Where(w => w.UserId == currentUser.Id).FirstOrDefault().Id);
            }
            else if(user.RoleId == 3)
            {
                CLnumber.Visibility = Visibility.Hidden;
                CLWorker.Visibility = Visibility.Hidden;
                DGOrders.ItemsSource = DataAccess.GetOrders().Where(o=> o.ClientId == user.Client.FirstOrDefault().Id);
            }
            
            DataContext = this;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrderPage(currentUser));
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            var order = DGOrders.SelectedItem as Order;
            if (order != null)
            {
                if(currentUser.RoleId == 1)
                {
                    if(order.StatusOrderId == 1)
                    {
                        order.StatusOrderId = 3;
                        order.WorkerId = currentUser.Worker.Where(w => w.UserId == currentUser.Id).FirstOrDefault().Id;
                        DB_Connection.connection.SaveChanges();
                        NavigationService.Navigate(new OrderPage(order, currentUser));
                    }
                    else
                    {
                        NavigationService.Navigate(new OrderPage(order, currentUser));
                        //MessageBox.Show($"Заказ в статусе {order.StatusOrder.Name}, вы можете просматривать лишь новые заказы") ;
                    }
                    
                }
                else if(currentUser.RoleId == 3)
                {
                    NavigationService.Navigate(new OrderPage(order, currentUser));
                }
                
            }
            else
                MessageBox.Show("Заказ не выбран");
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductsListPage(currentUser));
        }
    }
}
