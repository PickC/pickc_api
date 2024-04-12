using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IOrderDetailRepository
    {
        public bool Save(OrderItem item);
        public bool Delete(short itemID, Int64 orderID);

        public OrderDetail Get(short itemID, Int64 orderID);
        public List<OrderDetail> List(Int64 orderID);

        public List<OrderDetailDelivery> GetOrderItemForDelivery(Int64 orderID);
    }
}
