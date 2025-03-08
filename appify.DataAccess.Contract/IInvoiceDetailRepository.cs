using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IInvoiceDetailRepository
    {
        public List<InvoiceDetail> GetAll(Int64 invoiceID);
        public InvoiceDetail Get(short itemID, Int64 invoiceID);
        public bool Save(InvoiceDetail item);
        public void Remove(short itemID, Int64 invoiceID);

    }
}
