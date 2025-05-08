using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using appify.Business;
using appify.Business.Contract;
using appify.models;
using System.Net;
using System.Net.Sockets; 

namespace appify.audit.service
{

    public interface IAuditService {
        Task LogAsync(EntityType entityType, Int64 entityID, string eventType, string changedBy, string source, string ipAddress, object payLoad);

        Task<IEnumerable<IAuditLog>> GetLogsByEntityAsync(EntityType entityType, Int64 entityID);
    }
    public class AuditService : IAuditService
    {

        private IOrderLogBusiness orderLog;
        private IProductLogBusiness productLog;
        private IVendorLogBusiness vendorLog;


        public AuditService(IOrderLogBusiness order,IProductLogBusiness product, IVendorLogBusiness vendor)
        {
            this.orderLog = order;
            this.productLog = product;
            this.vendorLog = vendor;
        }

        public async Task LogAsync(EntityType entityType, long entityID, string eventType, string changedBy, string source, string ipAddress, object payLoad)
        {

            string payLoadJSON = System.Text.Json.JsonSerializer.Serialize(payLoad);

            var auditLog = AuditLogFactory.CreateAuditLog(entityType,entityID, eventType, changedBy, source, ipAddress, payLoadJSON);

            switch (entityType)
            {   
                case EntityType.Order:
                    await orderLog.SaveOrderLog((IOrderAuditLog)auditLog);
                    break;
                case EntityType.Product:
                    await productLog.SaveProductLog((IProductAuditLog)auditLog);
                    break;
                case EntityType.Vendor:
                    await vendorLog.SaveVendorLog((IVendorAuditLog)auditLog);
                    break;
                default:
                    break;
            }

        }

        public async Task<IEnumerable<IAuditLog>> GetLogsByEntityAsync(EntityType entityType, Int64 entityID)
        {

            return entityType
            switch
            {
                EntityType.Order => (await orderLog.ListOrderLog(entityID)).Cast<IAuditLog>(),
                EntityType.Product => (await productLog.ListProductLog(entityID)).Cast<IAuditLog>(),
                EntityType.Vendor => (await vendorLog.ListVendorLog(entityID)).Cast<IAuditLog>(),
                _ => throw new ArgumentException($"Unsupported Entity Type {entityType}")
            };


        }

    }
}
