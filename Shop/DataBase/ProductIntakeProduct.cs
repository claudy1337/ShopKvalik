using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.my_ado
{
    public partial class ProductIntakeProduct
    {
        public decimal Sum => Count * PriceUnit;
    }
}
