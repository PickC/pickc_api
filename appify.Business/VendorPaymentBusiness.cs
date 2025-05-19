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
    public partial class VendorPaymentBusiness : IVendorPaymentBusiness
    {
        private IVendorPaymentRepository repository;
        public VendorPaymentBusiness(IVendorPaymentRepository vendorPaymentRepository) { 
            this.repository = vendorPaymentRepository;
        }

        public VendorPayment SaveVendorPayment(VendorPayment item)
        {
            return repository.SaveVendorPayment(item);
        }
        public bool RemoveVendorPayment(Int64 PaymentID)
        {
            return repository.RemoveVendorPayment(PaymentID);
        }
        public VendorPayment Get(long PaymentID)
        {

            return repository.Get(PaymentID);
        }
        public List<VendorPayment> GetAll()
        {
            return repository.GetAll();
        }

        public List<VendorPayment> PaymentListbyRows(int pageNo, int rows)
        {
            return this.repository.PaymentListbyRows(pageNo, rows);
        }
        public bool UpdateReferenceNo(VendorPaymentStatus item)
        { 
            return repository.UpdateReferenceNo(item);
        }
        public VendorPayment GetPaymentStatus(Int64 VendorID)
        {
            return repository.GetPaymentStatus(VendorID);
        }
        public List<VendorPayment> ListByVendor(Int64 VendorID)
        {
            return repository.ListByVendor(VendorID);
        }

        public VendorStatement GetStatement(Int64 VendorID, DateTime? dateFrom, DateTime? dateTo) => repository.GetStatement(VendorID, dateFrom, dateTo);
         

    }
}
