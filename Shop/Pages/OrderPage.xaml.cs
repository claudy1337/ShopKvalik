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
using System.Collections.ObjectModel;
using Shop.DataBase;

namespace Shop.Pages
{
    /// <summary>
    /// Interaction logic for OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        public static User currentUser;

        public static Client currentClient;
        public List<Product> Products { get; set; }
        public Order Order { get; set; }
        public List<StatusOrder> StatusOrders { get; set; }
        public List<ProductOrder> ProductOrders { get; set; }
        public OrderPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            currentClient = user.Client.Where(c => c.UserId == user.Id).FirstOrDefault();
            DPDate.SelectedDate = DateTime.Now.Date;
            Products = DataAccess.GetProducts().ToList();
            StatusOrders = DataAccess.GetStatusOrder().ToList();
            Order = new Order
            {
                StatusOrder = StatusOrders[0]
            };
            ProductOrders = Order.ProductOrder.ToList();
           
            BtnDecline.Visibility = Visibility.Hidden;
            BtnPay.Visibility = Visibility.Hidden;
            BtnAccept.Visibility = Visibility.Hidden;
            DGProducts.SelectionMode = DataGridSelectionMode.Extended;
            DataContext = this;
        }
        public OrderPage(Order order, User user)
        {
            InitializeComponent();
            Order = order;
            currentUser = user;
            currentClient = user.Client.Where(c => c.UserId == user.Id).FirstOrDefault();
            DPDate.SelectedDate = Order.Date;
            Products = DataAccess.GetProducts().ToList();
            ProductOrders = Order.ProductOrder.ToList();
            DGProducts.ItemsSource = ProductOrders;
            decimal sum = 0;
            foreach (ProductOrder productOrder in ProductOrders)
            {
                sum += productOrder.Sum;
            }
            TBSum.Text = sum.ToString();
            BtnCreate.Visibility = Visibility.Hidden;
            DataContext = this;

            if (user.RoleId == 1)
            {
                if(order.StatusOrderId == 3)
                {
                    BtnDecline.Visibility = Visibility.Visible;
                    BtnAccept.Visibility = Visibility.Visible;
                }
                else if(order.StatusOrderId == 6 || order.StatusOrderId == 5 || order.StatusOrderId == 4 || order.StatusOrderId == 2)
                {
                    BtnDecline.Visibility = Visibility.Hidden;
                    BtnAccept.Visibility = Visibility.Hidden;
                }
                CBProduct.Visibility = Visibility.Hidden;
                BtnCreate.Visibility = Visibility.Hidden;
                BtnPay.Visibility = Visibility.Hidden;
                BtnAdd.Visibility = Visibility.Hidden;
                BtnRemove.Visibility = Visibility.Hidden;
                DPDate.Visibility = Visibility.Hidden;
                clCount.IsReadOnly = true;
            }

            else if (user.RoleId == 3)
            {
                
                if (order.StatusOrderId == 1)
                {
                    DPDate.Visibility = Visibility.Visible;
                    CBProduct.Visibility = Visibility.Visible;
                    BtnAdd.Visibility = Visibility.Visible;
                    BtnPay.Visibility = Visibility.Hidden;
                    BtnCreate.Visibility = Visibility.Visible;
                    BtnRemove.Visibility = Visibility.Visible;
                    BtnDecline.Visibility = Visibility.Hidden;
                    BtnAccept.Visibility = Visibility.Hidden;
                    clCount.IsReadOnly = false;

                }
               
                else if (order.StatusOrderId == 2)
                {
                    BtnPay.Visibility = Visibility.Visible;
                    DPDate.Visibility = Visibility.Hidden;
                    CBProduct.Visibility = Visibility.Hidden;
                    BtnAdd.Visibility = Visibility.Hidden;
                    BtnCreate.Visibility = Visibility.Hidden;
                    BtnRemove.Visibility = Visibility.Hidden;
                    BtnDecline.Visibility = Visibility.Hidden;
                    BtnAccept.Visibility = Visibility.Hidden;
                }

                else if (order.StatusOrderId != 2 && order.StatusOrderId !=1)
                {
                    BtnPay.Visibility = Visibility.Hidden;
                    DPDate.Visibility = Visibility.Hidden;
                    CBProduct.Visibility = Visibility.Hidden;
                    BtnAdd.Visibility = Visibility.Hidden;
                    BtnCreate.Visibility = Visibility.Hidden;
                    BtnRemove.Visibility = Visibility.Hidden;
                    BtnDecline.Visibility = Visibility.Hidden;
                    BtnAccept.Visibility = Visibility.Hidden;
                }
            }            
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            foreach (var productOrder in ProductOrders)
            {
                Order.ProductOrder.Add(productOrder);
            }
            if (DGProducts.Items.Count != 0)
            {
                if (DataAccess.GetOrders().Where(o => o.Id == Order.Id).Count() == 1)
                {
                    DB_Connection.connection.SaveChanges();
                    MessageBox.Show("Заказ переоформлен");
                }
                else
                {
                    DataAccess.AddOrder(Order, currentClient);
                    MessageBox.Show("Заказ оформлен");
                }

                NavigationService.Navigate(new ProductsListPage(currentUser));
            }
            
            else
                MessageBox.Show("Выберите продукты для заказа!");
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var product = CBProduct.SelectedItem as Product;
            ProductOrders.Add(new ProductOrder { Product = product, ProductId = product.Id});
            Products.Remove(product);
            DGProducts.Items.Refresh();
        }

        private void DGProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var t = (sender as Product);
        }
        private void SetEnable()
        {
            if (Order.StatusOrder.Name != "Новый")
            {
                grid.IsEnabled = false;
            }
        }

        private void DGProducts_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (this.DGProducts.SelectedItem != null)
            {
                (sender as DataGrid).RowEditEnding -= DGProducts_RowEditEnding;
                (sender as DataGrid).CommitEdit();
                (sender as DataGrid).Items.Refresh();

                decimal sum = 0;
                foreach (ProductOrder product in DGProducts.ItemsSource)
                {
                    sum += product.Sum;
                }
                TBSum.Text = sum.ToString();
                (sender as DataGrid).RowEditEnding += DGProducts_RowEditEnding;
                
            }
            return;
        }
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var index = DGProducts.SelectedIndex;
            ProductOrders.RemoveAt(index);
            DGProducts.Items.Refresh();
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            Order.StatusOrderId = 2;
            DB_Connection.connection.SaveChanges();
            MessageBox.Show("Заказ принят");
            NavigationService.Navigate(new OrdersPage(currentUser));
        }

        private void BtnDecline_Click(object sender, RoutedEventArgs e)
        {
            Order.StatusOrderId = 5;
            DB_Connection.connection.SaveChanges();
            MessageBox.Show("Заказ отклонён");
            NavigationService.Navigate(new OrdersPage(currentUser));
        }

        private void Btnback_Click(object sender, RoutedEventArgs e)
        { 
            NavigationService.Navigate(new OrdersPage(currentUser));
        }

        private void BtnPay_Click(object sender, RoutedEventArgs e)
        {
            Order.StatusOrderId = 6;
            DB_Connection.connection.SaveChanges();
            MessageBox.Show("Заказ оплачен");
            NavigationService.Navigate(new OrdersPage(currentUser));
        }
    }
}
