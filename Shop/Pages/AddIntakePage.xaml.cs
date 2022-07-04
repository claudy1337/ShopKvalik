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
    /// Interaction logic for AddIntakePage.xaml
    /// </summary>
    public partial class AddIntakePage : Page
    {
        public static Worker currentWorker;
        public static User currentUser;
        public List<ProductIntakeProduct> ProductIntakes { get; set; }
        public static Supplier supplier;
        public List<ProductIntake> Intakes { get; set; }
        public List<Product> Products { get; set; }
        public List<Supplier> Suppliers { get; set; }

        public ProductIntake Intake { get; set; }
        public AddIntakePage(User user)
        {
            InitializeComponent();
            Intake = new ProductIntake();
            currentUser = user;
            currentWorker = user.Worker.Where(c => c.UserId == user.Id).FirstOrDefault();
            Products = DataAccess.GetProducts().ToList();
            ProductIntakes = new List<ProductIntakeProduct>();
            DPDate.SelectedDate = DateTime.Now;
            BtnAdd.Visibility = Visibility.Visible;
            DGProducts.SelectionMode = DataGridSelectionMode.Extended;
            BtnAccept.Visibility = Visibility.Hidden;
            Suppliers = DataAccess.GetSuppliers().ToList();
            CBSupplier.SelectedIndex = 0;
            DataContext = this;
        }
        public AddIntakePage(ProductIntake intake, Worker worker)
        {
            InitializeComponent();
            Intake = intake;
            currentWorker = worker;
            DPDate.SelectedDate = Intake.Data;
            ProductIntakes = Intake.ProductIntakeProduct.ToList();
            DGProducts.ItemsSource = ProductIntakes;
            decimal sum = 0;
            if (currentWorker.User.RoleId == 1)
                BtnAccept.Visibility = Visibility.Visible;
            else
                BtnAccept.Visibility = Visibility.Hidden;
            if(Intake.StatusIntakeId == 1)
                BtnAccept.Visibility = Visibility.Hidden;
            else
                BtnAccept.Visibility = Visibility.Visible;
            foreach (ProductIntakeProduct productOrder in ProductIntakes)
            {
                sum += productOrder.Sum;
            }
            TbSum.Text = sum.ToString();
            BtnAdd.Visibility = Visibility.Hidden;
            BtnCreate.Visibility = Visibility.Hidden;
            CBSupplier.Visibility = Visibility.Hidden;
            CBSupplier.SelectedItem = intake.Supplier.Name;
            CBProduct.Visibility = Visibility.Hidden;
            Tbl.Visibility = Visibility.Hidden;
            BtnCreate.Visibility = Visibility.Hidden;
            DataContext = this;

        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (DGProducts.Items.Count != 0)
            {
                Intake.SupplierId = supplier.Id;
                Intake.TotalAmount = decimal.Parse(TbSum.Text);
                Intake.Data = (DateTime)DPDate.SelectedDate;
                Intake.ProductIntakeProduct = ProductIntakes;
                Intake.StatusIntakeId = 2;
                MessageBox.Show("Документ на поставку составлен");
                DataAccess.SaveProductIntake(Intake);
            }
            else
                MessageBox.Show("Выберите поставленные продукты");
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var product = CBProduct.SelectedItem as Product;
            ProductIntakes.Add(new ProductIntakeProduct() { ProductId = product.Id, Product = product, PriceUnit = Convert.ToDecimal(product.Price)});

            Products.Remove(product);

            DGProducts.Items.Refresh();
        }

        private void DGProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DGProducts_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (DGProducts.SelectedItem != null)
            {
                (sender as DataGrid).RowEditEnding -= DGProducts_RowEditEnding;
                (sender as DataGrid).CommitEdit();
                (sender as DataGrid).Items.Refresh();

                decimal sum = 0;
                foreach (ProductIntakeProduct product in DGProducts.ItemsSource)
                {
                    sum += product.Sum;
                }
                TbSum.Text = sum.ToString();
                (sender as DataGrid).RowEditEnding += DGProducts_RowEditEnding;
            }
            return;
        }

        private void CBSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            supplier = CBSupplier.SelectedItem as Supplier;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductsListPage(currentUser));
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            Intake.StatusIntakeId = 1;
            DB_Connection.connection.SaveChanges();
            MessageBox.Show("Поставка принята");
            NavigationService.Navigate(new IntakesPage(currentWorker));
        }
    }
}
