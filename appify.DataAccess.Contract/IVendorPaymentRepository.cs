using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IVendorPaymentRepository
    {
        public bool SaveVendorPayment(VendorPayment item);
        public bool RemoveVendorPayment(Int64 PaymentID);
        public VendorPayment Get(Int64 PaymentID);
        public List<VendorPayment> GetAll();
        public List<VendorPayment> PaymentListbyRows(int pageNo, int rows);
        public bool UpdateReferenceNo(VendorPaymentStatus item);
        public decimal GetPaymentStatus(Int64 VendorID);
        public List<VendorPayment> ListByVendor(Int64 VendorID);
    }
}
