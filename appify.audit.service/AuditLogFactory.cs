using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using appify.models;

namespace appify.audit.service
{
    public static class AuditLogFactory
    {
        public static IAuditLog CreateAuditLog(
                    EntityType entityType,Int64 entityID,string eventType,
                    string changedBy,string source, string ipAddress,string payLoad) {

            return entityType switch
            {
                EntityType.Order => new OrderAuditLog
                {
                    OrderID = entityID,
                    ChangedBy = changedBy,
                    Source = source,
                    EventType = eventType,
                    IPAddress = ipAddress,
                    Payload = payLoad
                },
                EntityType.Product => new ProductAuditLog
                {
                    ProductID= entityID,
                    ChangedBy = changedBy,
                    Source = source,
                    EventType = eventType,
                    IPAddress = ipAddress,
                    Payload = payLoad

                },
                EntityType.Vendor => new VendorAuditLog
                {
                    VendorID = entityID,
                    ChangedBy = changedBy,
                    Source = source,
                    EventType = eventType,
                    IPAddress = ipAddress,
                    Payload = payLoad
                },
                _ => throw new ArgumentException($"Unsupported Entity Type {entityType}")


            };

        }
    }
}
