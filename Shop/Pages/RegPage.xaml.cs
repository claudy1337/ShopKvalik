using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Shop.my_ado;
using System.Windows;
using Shop.DataBase;
using System;

namespace Shop.Pages
{
    /// <summary>
    /// Interaction logic for RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        private ObservableCollection<User> users = new ObservableCollection<User>(DB_Connection.connection.User);
        public RegPage()
        {
            InitializeComponent();
            if (!RBtnFemale.IsPressed || !RBtnMale.IsPressed)
                SexError.Text = "Пол не выбран";
            PhoneError.Text = "Введите телефон";
            LoginError.Text = "Придумайте логин";
            PasswordError.Text = "Придумайте пароль";
            FIOError.Text = "Введите ФИО";
            EmailError.Text = "Введите адрес эл. почты";
        }


        private void TBLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            var sameUser = users.Where(u => u.Login == TBLogin.Text).ToList();
            Regex syms = new Regex(@"\s");
            if (sameUser.Count == 1)
                LoginError.Text = "Пользователь с таким именем сущесвует";
            else if (TBLogin.Text.Length == 0 || syms.IsMatch(TBLogin.Text))
                LoginError.Text = "Придумайте логин";
            else
                LoginError.Text = "";
        }

        private void TBPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nyms = new Regex(@"[0-9]");
            var symbols = new Regex(@"[!@#$%^]");
            var latinLetters = new Regex(@"[A-Z]");
            var cyrillicLetters = new Regex(@"[А-Я]");
            if (TBPassword.Text == "")
                PasswordError.Text = "Придумайте пароль";
            else if (nyms.IsMatch(TBPassword.Text) && symbols.IsMatch(TBPassword.Text) && (cyrillicLetters.IsMatch(TBPassword.Text) || latinLetters.IsMatch(TBPassword.Text)) && TBPassword.Text.Length >= 6)
            {
                PasswordError.Text = "";
                Password.Text = "";
            }
            else
            {
                Password.Text = $"Пароль должен содержать: \nХотя бы одну цифру,\nХотя бы одну букву верхнего регистра,\nХотя бы один символ из !@#$%^";
                PasswordError.Text = "Пароль не соответствует требованиям";
            }
        }

        private void TBFIO_TextChanged(object sender, TextChangedEventArgs e)
        {   
            Regex syms = new Regex(@"[А-Я][а-я]+\s[А-Я][а-я]+");
            if (TBFIO.Text.Length == 0 )
                FIOError.Text = "Введите ФИО";
            else if(syms.IsMatch(TBFIO.Text))
                FIOError.Text = "";
            else
                FIOError.Text = "Введите ФИО";

        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private void TBEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsValidEmail(TBEmail.Text))
                EmailError.Text = "";
            else
                EmailError.Text = "Некорректный адрес эл. почты";
        }


        private void BtnRegistrate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (LoginError.Text.Length == 0 && PasswordError.Text.Length == 0 && FIOError.Text.Length == 0 && EmailError.Text.Length == 0  && SexError.Text.Length == 0 && PhoneError.Text.Length == 0)
            {
                User userToAdd = new User();
                userToAdd.Password = TBPassword.Text;
                userToAdd.Login = TBLogin.Text;
                DataAccess.AddUser(userToAdd);
                var lastUser = users.Last();
                Client clientToAdd = new Client();
                clientToAdd.FIO = TBFIO.Text;
                clientToAdd.UserId = lastUser.Id+1;
                if (RBtnMale.IsPressed)
                    clientToAdd.GenderId = 1;
                else
                    clientToAdd.GenderId = 2;
                clientToAdd.Email = TBEmail.Text;
                clientToAdd.AddDate = DateTime.Now.Date;
                clientToAdd.NumberPhone = TBPhone.Text;
                DataAccess.AddClient(clientToAdd);
                NavigationService.GoBack();
            }
            else
                MessageBox.Show("Введённые данные некорректны");
        }

        private void RBtnMale_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            SexError.Text = "";
        }

        private void RBtnFemale_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            SexError.Text = "";
        }

        private void TBPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nums = new Regex(@"[1{10} - 9{10}]");
            if (TBPhone.Text.Length != 11 )
                PhoneError.Text = "Номер телефона болжен содержать 11 символов";
            else if(nums.IsMatch(TBPhone.Text) && TBPhone.Text.Length == 11 )
                PhoneError.Text = "";
        }
    }
}
