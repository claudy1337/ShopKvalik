using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.my_ado;

namespace Shop.DataBase
{
    public class DB_Connection
    {
        public static ShopEntities connection = new ShopEntities();
    }
}
