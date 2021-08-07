using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLogicAppAzure.Dtos
{
    /// <summary>
    /// Configuracion necesaria para el provider de Logic App
    /// </summary>
    public class ConnectionData
    {
        /// Cliente provisto por azure
        /// </summary>
        public string AzureClientId { get; set; }
        /// <summary>
        /// Tenand provisto por azure
        /// </summary>
        public string AzureTenantId { get; set; }
        /// <summary>
        /// Cliente secret proviste por azure
        /// </summary>
        public string AzureClientSecret { get; set; }
        /// <summary>
        /// Identificador de la suscripcion
        /// </summary>
        public string SubscriptionId { get; set; }
    }
}
