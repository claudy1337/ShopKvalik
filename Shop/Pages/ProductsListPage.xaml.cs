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
using Shop.DataBase;
using System.Collections.ObjectModel;
using Shop.my_ado;

namespace Shop.Pages
{
    /// <summary>
    /// Interaction logic for ProductsListPage.xaml
    /// </summary>
    public partial class ProductsListPage : Page
    {
        private static ObservableCollection<Product> products { get; set; }
        public static User currentUser;
        int actualPage;
        public ProductsListPage(User user)
        {
            InitializeComponent();
            products = DataAccess.GetProducts();
            LVProducts.ItemsSource = products;
            DataContext = this;
            currentUser = user;
            if (user.RoleId == 3)
            {
                BtnOrders.Visibility = Visibility.Hidden;
                BtnAdd.Visibility = Visibility.Hidden;
                BtnIntake.Visibility = Visibility.Hidden;
                BtnIntakes.Visibility = Visibility.Hidden;
            }
            else if(user.RoleId == 1)
            {
                BtnOrder.Visibility = Visibility.Hidden;
                BtnMyOrders.Visibility = Visibility.Hidden;
                BtnIntake.Visibility = Visibility.Hidden;
            }
            else if(user.RoleId == 2)
            {
                BtnOrders.Visibility = Visibility.Hidden;
                BtnAdd.Visibility = Visibility.Hidden;
                BtnOrder.Visibility = Visibility.Hidden;
                BtnMyOrders.Visibility = Visibility.Hidden;
                BtnIntakes.Visibility = Visibility.Hidden;

            }
            var allUnits = new ObservableCollection<Unit>(DB_Connection.connection.Unit.ToList());
            allUnits.Insert(0, new Unit() { Id = -1, Name = "Все" });
            CBUnit.ItemsSource = allUnits;
            CBUnit.DisplayMemberPath = "Name";
        }
        private void Filter()
        {
            var filterProd = (IEnumerable<Product>)DB_Connection.connection.Product.Where(a => a.IsDeleted == false).ToList();

            if (TBSearch.Text != "")
            {
                filterProd = DataAccess.GetProductsByNameOrDescription(TBSearch.Text);
            }
            if(CBUnit.SelectedIndex == 0)
            {
                filterProd = DataAccess.GetProductsByNameOrDescription(TBSearch.Text);
            }

            else if (CBUnit.SelectedIndex > 0)
            {
                filterProd = filterProd.Where(c => c.UnitId == (CBUnit.SelectedItem as Unit).Id || c.UnitId == -1);
            }

            if (CBAlphabet.SelectedIndex == 1)
            {
                filterProd = filterProd.OrderBy(c => c.Name);
            }
            else if(CBAlphabet.SelectedIndex == 2)
            {
                filterProd = filterProd.OrderByDescending(c => c.Name);
            }

            if (CBDate.SelectedIndex == 1)
            {
                filterProd = filterProd.OrderBy(c => c.AddDate);
            }
            else if (CBDate.SelectedIndex == 2)
            {
                filterProd = filterProd.OrderByDescending(c => c.AddDate);
            }

            if (CBMonth.IsChecked.GetValueOrDefault())
            {
                var date = DateTime.Now.Month;
                filterProd = filterProd.Where(c => c.AddDate.Month == date);
            }

            if (SortCount.SelectedIndex > 0 && filterProd.Count() > 0)
            {
                var cbb = SortCount.SelectedItem as ComboBoxItem;
                int SelCount = Convert.ToInt32(cbb.Content);
                filterProd = filterProd.Skip(SelCount * actualPage).Take(SelCount);
                if (filterProd.Count() == 0)
                {
                    actualPage--;
                    return;
                }
                LVProducts.ItemsSource = filterProd;
            }
            LVProducts.ItemsSource = filterProd;
        }

        private void TBSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            actualPage = 0;
            Filter();
        }

        private void LVProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentUser.RoleId == 1)
            {
                var selectedProduct = LVProducts.SelectedItem as Product;
                NavigationService.Navigate(new ProductEditPage(selectedProduct));
            }
            
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProductPage(new Product()));   
        }

        private void CBUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            actualPage = 0; 
            Filter();
        }

        private void CBAlphabet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            actualPage = 0;
            Filter();
        }

        private void CBDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            actualPage = 0;
            Filter();
        }

        private void CBMonth_Click(object sender, RoutedEventArgs e)
        {
            actualPage = 0;
            Filter();
        }

        private void SortCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            actualPage = 0;
            Filter();
        }

        private void BackListBtn_Click(object sender, RoutedEventArgs e)
        {
            actualPage--;
            if (actualPage < 0)
                actualPage = 0;
            Filter();
        }
        
        private void ForwardListBtn_Click(object sender, RoutedEventArgs e)
        {
            actualPage++;
            Filter();
        }
        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            actualPage = 0;
            Filter();
        }

        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrderPage(currentUser));
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        { 
            NavigationService.Navigate(new OrdersPage(currentUser));
        }

        private void BtnMyOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersPage(currentUser));
        }

        private void BtnNewIntake_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddIntakePage(currentUser));
        }

        private void BtnIntakes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddIntakePage(currentUser));
        }

        private void BtnIntakes_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new IntakesPage(currentUser.Worker.Where(w => w.UserId == currentUser.Id).FirstOrDefault()));
        }
    }
}
