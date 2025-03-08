using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IInvoiceBusinesss
    {
        public InvoiceHeader Save(Invoice item);
        public bool Delete(long invoiceID);

        public Order Get(long invoiceID);
        public List<Order> MemberInvoices(long customerID);
        public List<Order> VendorInvoices(long customerID);

        public InvoiceReport PrintInvoice(Int64 orderID);
        public ReceiptReport PrintReceipt(Int64 vendorID);
        public bool SaveItem(InvoiceDetail item);
        public bool DeleteItem(short itemID,long invoiceID);

        public OrderDetail GetItem(short itemID, long invoiceID);
        public List<OrderDetail> ListItems(long invoiceID);
    }
}
