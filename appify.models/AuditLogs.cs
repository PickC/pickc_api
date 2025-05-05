using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appify.models
{

    public interface IAuditLog
    {
        Int64 AuditId { get; set; }
        string EventType { get; set; }
        string ChangedBy { get; set; }
        DateTime ChangedOn { get; set; }
        string Payload { get; set; }
        string Source { get; set; }
        string IPAddress { get; set; }
    }


    public interface IOrderAuditLog : IAuditLog
    {
        Int64 OrderID { get; set; }
    }
    public interface IProductAuditLog : IAuditLog
    {
        Int64 ProductID { get; set; }
    }

    public interface IVendorAuditLog : IAuditLog
    {
        Int64 VendorID { get; set; }
    }


    public abstract class AuditLogBase : IAuditLog
    {
        public Int64 AuditId { get; set; }
        public string EventType { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public string Payload { get; set; }
        public string Source { get; set; }
        public string IPAddress { get; set; }
    }

    public class OrderAuditLog : AuditLogBase, IOrderAuditLog 
    {
        public Int64 OrderID { get; set; }
    }
    public class ProductAuditLog : AuditLogBase, IProductAuditLog
    {
        public Int64 ProductID { get; set; }
    }

    public class VendorAuditLog : AuditLogBase,IVendorAuditLog
    {
        public Int64 VendorID { get; set; }
    }
}






