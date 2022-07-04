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
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Shop.Pages
{
    /// <summary>
    /// Interaction logic for AddProductPage.xaml
    /// </summary>
    public partial class AddProductPage : Page
    {
        Product productToAdd;
        public AddProductPage(Product product)
        {
            InitializeComponent();
            UnitCb.ItemsSource = DataAccess.GetUnits();
            CountryCb.ItemsSource = DataAccess.GetCountries();
            DataContext = productToAdd;
            productToAdd = product;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductsListPage(ProductsListPage.currentUser));
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(InvalidName.Text.Length == 0)
                {
                    productToAdd.Description = TBDescription.Text;
                    productToAdd.IsDeleted = false;
                    var unit = UnitCb.SelectedItem as Unit;
                    productToAdd.UnitId = unit.Id;
                    productToAdd.Name = TBName.Text;
                    productToAdd.Photo = productToAdd.Photo;
                    productToAdd.AddDate = DateTime.Now.Date;
                    productToAdd.Price = Int32.Parse(TBPrice.Text);
                    DataAccess.AddProduct(productToAdd);
                }
                else
                {
                    MessageBox.Show("Введите корректное название");
                }
                
            }
            catch (FormatException)
            {
                MessageBox.Show("Цена в цифрах!");
            }
            BtnAddCountry.Visibility = Visibility.Visible;
            BtnRemoveCountry.Visibility = Visibility.Visible;
            CountryCb.Visibility = Visibility.Visible;
            CountryLv.Visibility = Visibility.Visible;
        }

        private void BtnAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "*.png|*.png|*.jpeg|*.jpeg|*.jpg|*.jpg"
            };
            if (openFile.ShowDialog().GetValueOrDefault())
            {
                productToAdd.Photo = File.ReadAllBytes(openFile.FileName);
                ProductPhoto.Source = new BitmapImage(new Uri(openFile.FileName));
            }
        }

        private void BtnAddCountry_Click(object sender, RoutedEventArgs e)
        {
            if (CountryCb.SelectedIndex >= 0)
            {
                var ProdCountry = new ProductCountry();
                var sel = CountryCb.SelectedItem as Country;
                ProdCountry.ProductId = productToAdd.Id;
                ProdCountry.CountryId = sel.Id;
                var isCountry = DataAccess.GetProdCountries().Where(c => c.CountryId == sel.Id && c.ProductId == productToAdd.Id).Count();
                if (isCountry == 0)
                {
                    DataAccess.AddProdCountry(ProdCountry);
                    UpdateCountryList();
                }
            }
        }
        private void UpdateCountryList()
        {
            CountryLv.ItemsSource = DataAccess.GetProdCountries().Where(e => e.ProductId == productToAdd.Id).ToList();
        }

        private void BtnRemoveCountry_Click(object sender, RoutedEventArgs e)
        {
            if (CountryLv.SelectedItem != null)
            {
                var selProductCountry = DataAccess.GetProdCountries().ToList().Find(c => c.ProductId == productToAdd.Id && c.CountryId == (CountryLv.SelectedItem as ProductCountry).CountryId);
                DataAccess.DeleteProdCountry(selProductCountry);
                UpdateCountryList();
            }
        }

        private void TBName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsLetter(e.Text, 0) && e.Text != "-")
            {
                e.Handled = true;
            }
        }

        private void TBDescription_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsLetter(e.Text, 0) && e.Text != "-")
            {
                e.Handled = true;
            }
        }
    }
}
