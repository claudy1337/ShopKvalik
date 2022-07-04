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
    /// Interaction logic for IntakesPage.xaml
    /// </summary>
    public partial class IntakesPage : Page
    {
        public static Worker currentWorker;
        public List<ProductIntake> Intakes { get; set; }
        public static ProductIntake currentIntake;
        public IntakesPage(Worker worker)
        {
            InitializeComponent();
            Intakes = DataAccess.GetProductIntakes().ToList();
            currentWorker = worker;
            DataContext = this;
            if (currentWorker.User.RoleId == 1)
                BtnAddIntake.Visibility = Visibility.Hidden;
            else
                BtnAddIntake.Visibility = Visibility.Visible;
        }

        private void BtnAddIntake_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddIntakePage(currentWorker.User));
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddIntakePage(currentIntake, currentWorker));
        }

        private void DGIntakes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedIntake = DGIntakes.SelectedItem as ProductIntake;
            currentIntake = selectedIntake;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductsListPage(currentWorker.User));
        }
    }
}
