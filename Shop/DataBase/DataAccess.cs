using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shop.my_ado;

namespace Shop.DataBase
{
    public static class DataAccess
    {
        
        public static bool IsCorrectUser(string login, string password)
        {
            ObservableCollection<User> users = new ObservableCollection<User>(DB_Connection.connection.User);
            var currentUser = users.Where(u => u.Login == login && u.Password == password).ToList();
            return currentUser.Count == 1;
        }
        public static bool IsIncorrectUser(string login, string password)
        {
            ObservableCollection<User> users = new ObservableCollection<User>(DB_Connection.connection.User);
            var currentUser = users.Where(u => u.Login == login && u.Password == password).ToList();
            return currentUser.Count == 0;
        }
        public static bool StartBan(BanSession session)
        {
            try
            {
                DB_Connection.connection.BanSession.Add(session);
                DB_Connection.connection.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool AddProdCountry(ProductCountry productCountry)
        {
            try
            {
                DB_Connection.connection.ProductCountry.Add(productCountry);
                DB_Connection.connection.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool AddUser(User user)
        {
            try
            {
                user.RoleId = 3;
                DB_Connection.connection.User.Add(user);
                DB_Connection.connection.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Changeroduct()
        {
            try
            {
                DB_Connection.connection.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool DeleteProduct(Product product)
        {
            product.IsDeleted = true;
            try
            {
                DB_Connection.connection.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool DeleteProdCountry(ProductCountry productCountry)
        {
            try
            {
                DB_Connection.connection.ProductCountry.Remove(productCountry);
                DB_Connection.connection.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool AddClient(Client client)
        {
            try
            {
                DB_Connection.connection.Client.Add(client);
                DB_Connection.connection.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool AddProduct(Product product)
        {
            try
            {
                DB_Connection.connection.Product.Add(product);
                DB_Connection.connection.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }



        public static ObservableCollection<ProductIntake> GetProductIntakes()
        {
            return new ObservableCollection<ProductIntake>(DB_Connection.connection.ProductIntake);
        }

        public static void SaveProductIntake(ProductIntake productIntake)
        {
            if (GetProductIntakes().Where(p => p.Id == productIntake.Id).Count() == 0)
                DB_Connection.connection.ProductIntake.Add(productIntake);
            else
                DB_Connection.connection.ProductIntake.SingleOrDefault(p => p.Id == productIntake.Id);

            DB_Connection.connection.SaveChanges();
        }

        public static bool AddOrder(Order order, Client client)
        {
            if (GetOrders().Where(o => o.Id == order.Id).Count() == 0)
            {
                order.Date = DateTime.Now.Date;
                order.StatusOrderId = 1;
                order.ClientId = client.Id;
                DB_Connection.connection.Order.Add(order);
                DB_Connection.connection.SaveChanges();
            }
            else
                DB_Connection.connection.Order.SingleOrDefault(p => p.Id == order.Id);

            return true;

        }

        public static ObservableCollection<Product> GetProducts()
        {
            ObservableCollection<Product> products = new ObservableCollection<Product>(DB_Connection.connection.Product.Where(p => p.IsDeleted == false || p.IsDeleted == null));
            return products;
        }

        public static ObservableCollection<Order> GetOrders()
        {
            ObservableCollection<Order> orders = new ObservableCollection<Order>(DB_Connection.connection.Order);
            return orders;
        }

        public static ObservableCollection<StatusOrder> GetStatusOrder()
        {
            ObservableCollection<StatusOrder> statuses = new ObservableCollection<StatusOrder>(DB_Connection.connection.StatusOrder);
            return statuses;
        }

        public static ObservableCollection<ProductCountry> GetProdCountries()
        {
            ObservableCollection<ProductCountry> prodCountries = new ObservableCollection<ProductCountry>(DB_Connection.connection.ProductCountry);
            return prodCountries;
        }

        public static ObservableCollection<Product> GetProductsByNameOrDescription(string name_or_description)
        {
            ObservableCollection<Product> products = new ObservableCollection<Product>(DB_Connection.connection.Product.Where(n => (n.Name.Contains(name_or_description) || n.Description.Contains(name_or_description)) && n.IsDeleted == false));
            return products;
        }
        public static ObservableCollection<Unit> GetUnits()
        {
            ObservableCollection<Unit> units = new ObservableCollection<Unit>(DB_Connection.connection.Unit);
            return units;
        }
        public static ObservableCollection<Country> GetCountries()
        {
            ObservableCollection<Country> countries = new ObservableCollection<Country>(DB_Connection.connection.Country);
            return countries;
        }
        public static ObservableCollection<Supplier> GetSuppliers()
        {
            ObservableCollection<Supplier> suppliers = new ObservableCollection<Supplier>(DB_Connection.connection.Supplier);
            return suppliers;
        }

        public static User GetUser(int idUser)
        {
            ObservableCollection<User> users = new ObservableCollection<User>(DB_Connection.connection.User);
            var currentUser = users.Where(u => u.Id == idUser).FirstOrDefault();
            return currentUser;
        }

     

        public static User GetUser(string login, string password)
        {
            ObservableCollection<User> users = new ObservableCollection<User>(DB_Connection.connection.User);
            var currentUser = users.Where(u => u.Login == login && u.Password == password).FirstOrDefault();
            return currentUser;
        }
        public static BanSession GetLastBanSession()
        {
            ObservableCollection<BanSession> sessions = new ObservableCollection<BanSession>(DB_Connection.connection.BanSession);
            BanSession lastBanSession = sessions.Last();
            return lastBanSession;
        }
    }
}
