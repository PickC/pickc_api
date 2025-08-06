using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IInvoiceHeaderRepository
    {
        public List<InvoiceHeader> GetAll();
        public InvoiceHeader Get(Int64 invoiceID);
        public InvoiceHeader Save(InvoiceHeader item);
        public void Remove(Int64 invoiceID);

        public InvoiceReport PrintInvoice(Int64 orderID);
        public ReceiptReport PrintReceipt(Int64 vendorID);
    }
}
