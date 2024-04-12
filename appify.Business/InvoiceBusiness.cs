using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public class InvoiceBusiness : IInvoiceBusinesss
    {
        private IInvoiceHeaderRepository invoiceheaderrepository;
        private IInvoiceDetailRepository invoicedetailrepository;

        public InvoiceBusiness(IInvoiceHeaderRepository headerRepository, IInvoiceDetailRepository detailRepository) { 
            this.invoiceheaderrepository = headerRepository;
            this.invoicedetailrepository = detailRepository;

        
        }

        public bool Delete(long invoiceID)
        {
            throw new NotImplementedException();
        }

        public bool DeleteItem(short itemID, long invoiceID)
        {
            throw new NotImplementedException();
        }

        public Order Get(long invoiceID)
        {
            throw new NotImplementedException();
        }

        public OrderDetail GetItem(short itemID, long invoiceID)
        {
            throw new NotImplementedException();
        }

        public List<OrderDetail> ListItems(long invoiceID)
        {
            throw new NotImplementedException();
        }

        public List<Order> MemberInvoices(long customerID)
        {
            throw new NotImplementedException();
        }

        public InvoiceHeader Save(Invoice item)
        {
            InvoiceHeader invoiceHd = new InvoiceHeader
            {
                InvoiceID = item.InvoiceID,
                InvoiceNo = "", //string.Format("INV{0}{1}", item.SellerID.ToString(), DateTime.Now.ToString("ssyyyyyMMddHHmm")),
                SellerID = item.SellerID,
                MemberID = item.MemberID,
                InvoiceAmount = item.InvoiceAmount,
                TaxAmount = item.TaxAmount,
                TotalAmount = item.TotalAmount,
                OrderID = item.OrderID,
            };

            var returnInvocie = invoiceheaderrepository.Save(item);


            if (item.items?.Any() == true)
            {
                foreach (var dt in item.items)
                {
                    dt.InvoiceID = returnInvocie.InvoiceID;
                    invoicedetailrepository.Save(dt);
                }
            }

            return returnInvocie;


        }

        public bool SaveItem(InvoiceDetail item)
        {
            return invoicedetailrepository.Save(item);
        }

        public List<Order> VendorInvoices(long customerID)
        {
            throw new NotImplementedException();
        }

        public InvoiceReport PrintInvoice(Int64 orderID) {
            return invoiceheaderrepository.PrintInvoice(orderID);

        }
    }
}
