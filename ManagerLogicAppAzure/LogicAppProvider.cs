using ManagerLogicAppAzure.Dtos;
using ManagerLogicAppAzure.Interface;
using Microsoft.Azure.Management.Logic;
using Microsoft.Azure.Management.Logic.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManagerLogicAppAzure
{
    public class LogicAppProvider : ILogicAppProvider
    {
        private LogicManagementClient _logicManagementClient;
        private ILogger<LogicAppProvider> _log;

        public LogicAppProvider(
            ILogger<LogicAppProvider> log)
        {
            _log = log;
        }

        public async Task<Workflow> CreateOrUpdateLogicAppRecurrenceToSendMessegeAsync(string worflowLogicAppName, string topic, string resourceGroup, string message,string location, RecurrenceDto recurrence, ConnectionData connectionData)
        {
            ConfigureLogicManagementClient(connectionData);
            string WorkflowDefinition = "";
            try
            {
                Workflow workflow = new Workflow(worflowLogicAppName)
                {
                    State = WorkflowState.Enabled,
                    Location = location
                };
                workflow.Parameters = new Dictionary<string, WorkflowParameter>();

                workflow.Parameters.Add("$connections", new WorkflowParameter("Object", new ServieBusData()
                {
                    Servicebus = new Servicebus()
                    {
                        ConnectionId = $"/subscriptions/{connectionData.SubscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.Web/connections/servicebus",
                        ConnectionName = "servicebus",
                        Id = $"/subscriptions/{connectionData.SubscriptionId}/providers/Microsoft.Web/locations/westus/managedApis/servicebus"
                    }

                }));

                WorkflowDefinition = GetDefinitionWorkflow(recurrence, message, topic);
                workflow.Definition = JToken.Parse(WorkflowDefinition);

                Workflow workflowLogicApp =
                    await _logicManagementClient.Workflows.CreateOrUpdateAsync(resourceGroup, worflowLogicAppName, workflow);

                return workflowLogicApp;

            }
            catch (ValidationException ex)
            {
                string mensajeError = string.Format("Error de validacion al crear logic app Contenido:{0}", WorkflowDefinition ?? "");
                _log.LogError(ex, mensajeError);
                throw;

            }
            catch (SerializationException ex)
            {
                string mensajeError = string.Format("Error de serializacion crear logic app Contenido:{0}", WorkflowDefinition ?? "");
                _log.LogError(ex, mensajeError);
                throw;
            }
            catch (CloudException ex)
            {
                string mensajeError = string.Format("Error de azure cloud al crear logic app Contenido:{0}", WorkflowDefinition ?? "");
                _log.LogError(ex, mensajeError);
                throw;
            }

        }

        public async Task DeleteLogicAppAsync(string worflowLogicAppName, string resourceGroup, ConnectionData connectionData)
        {
            try
            {
                ConfigureLogicManagementClient(connectionData);
                await _logicManagementClient.Workflows.DeleteAsync(resourceGroup, worflowLogicAppName);
            }
            catch (ValidationException ex)
            {
                string mensajeError = string.Format("Error de validacion al eliminar logic app worflowLogicAppName:{0} , resourcegroup : {1}",
                    worflowLogicAppName, resourceGroup);
                _log.LogError(ex, mensajeError);
                throw;

            }
            catch (CloudException ex)
            {
                string mensajeError = string.Format("Error de azure cloud al eliminar logic app worflowLogicAppName:{0} , resourcegroup : {1}",
                    worflowLogicAppName, resourceGroup);
                _log.LogError(ex, mensajeError);
                throw;
            }
        }

        public async Task<Workflow> GetLogicAppAsync(string worflowLogicAppName, string resourceGroup, ConnectionData connectionData)
        {
            try
            {
                ConfigureLogicManagementClient(connectionData);
                Workflow workflow = await _logicManagementClient.Workflows.GetAsync(resourceGroup, worflowLogicAppName);
                return workflow;
            }
            catch (ValidationException ex)
            {
                string mensajeError = string.Format("Error al obtener logic app worflowLogicAppName:{0} , resourcegroup : {1}",
                    worflowLogicAppName, resourceGroup);
                _log.LogError(ex, mensajeError);
                throw;

            }
            catch (CloudException ex)
            {
                string mensajeError = string.Format("Error de azure cloud al obtener logic app worflowLogicAppName:{0} , resourcegroup : {1}",
                    worflowLogicAppName, resourceGroup);
                _log.LogError(ex, mensajeError);
                throw;
            }
        }

        /// <summary>
        /// Configura los datos de conexion del client de logic app
        /// </summary>
        /// <param name="conexionData"></param>
        private void ConfigureLogicManagementClient(ConnectionData conexionData)
        {
            _logicManagementClient =
             new LogicManagementClient(new CustomLoginCredentials(conexionData.AzureClientId, conexionData.AzureTenantId, conexionData.AzureClientSecret));
            _logicManagementClient.SubscriptionId = conexionData.SubscriptionId;
        }
        /// <summary>
        /// Obtengo la definicion especifica de datos que requiere el json del flujo de la logic app
        /// <see cref="https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#"/>
        /// ya que azure deserializa la data enviada en base a dicho archivo
        /// </summary>
        /// <param name="recurrence"></param>
        /// <param name="data"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        private string GetDefinitionWorkflow(RecurrenceDto recurrence, string data, string topic)
        {

            byte[] plainTextDataBytes = System.Text.Encoding.UTF8.GetBytes(data);
            string Database64 = System.Convert.ToBase64String(plainTextDataBytes);

            LogicAppData logicAppData = new LogicAppData()
            {

                Schema = new Uri("https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#"),

                Actions = new Actions()
                {
                    SendMessage = new SendMessage()
                    {
                        Inputs = new Inputs()
                        {
                            Body = new Body()
                            {
                                ContentData = Database64
                            },
                            Host = new Host()
                            {
                                Connection = new Connection()
                                {
                                    Name = "@parameters('$connections')['servicebus']['connectionId']"
                                },

                            },
                            Method = "post",
                            Path = "/" + topic + "/messages",
                            Queries = new Queries()
                            {
                                SystemProperties = "None"
                            }
                        },
                        Type = "ApiConnection",
                        RunAfter = new Outputs()
                    },
                },
                Outputs = new Outputs(),
                ContentVersion = "1.0.0.0",
                Parameters = new Parameters()
                {
                    Connections = new Connections()
                    {
                        DefaultValue = new Outputs(),
                        Type = "Object"
                    }
                },
                Triggers = new Triggers()
                {
                    Recurrence = new Recurrence()
                    {
                        RecurrenceRecurrence = new RecurrenceClass()
                        {
                            Frequency = recurrence.Frequency.ToString(),
                            Interval = recurrence.Interval,
                            StartTime = recurrence.StartTime,
                            TimeZone = recurrence.TimeZone
                        },
                        Type = "Recurrence",
                    },
                }
            };

            //TODO faltaria definir las horas y minutos
            if (recurrence.WeekDays != null && recurrence.WeekDays.Any())
                logicAppData.Triggers.Recurrence.RecurrenceRecurrence.Schedule = new Schedule()
                {
                    WeekDays = recurrence.WeekDays.Select(z => z.ToString()).ToList(),
                    Minutes = new List<string> { "0" },
                    Hours = new List<string> { "0" }
                };

            string logigAppData = JsonConvert.SerializeObject(logicAppData);
            return logigAppData;
        }

        #region Clases para la creacion del Workflow logic app en azure

        private class ServieBusData
        {
            [JsonProperty("servicebus")]
            public Servicebus Servicebus { get; set; }
        }
        private class Servicebus
        {
            [JsonProperty("connectionId")]
            public string ConnectionId { get; set; }

            [JsonProperty("connectionName")]
            public string ConnectionName { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }
        private class LogicAppData
        {
            [JsonProperty("$schema")]
            public Uri Schema { get; set; }

            [JsonProperty("actions")]
            public Actions Actions { get; set; }

            [JsonProperty("contentVersion")]
            public string ContentVersion { get; set; }

            [JsonProperty("outputs")]
            public Outputs Outputs { get; set; }

            [JsonProperty("parameters")]
            public Parameters Parameters { get; set; }

            [JsonProperty("triggers")]
            public Triggers Triggers { get; set; }
        }
        private class Actions
        {
            [JsonProperty("Send_message")]
            public SendMessage SendMessage { get; set; }
        }
        private class SendMessage
        {
            [JsonProperty("inputs")]
            public Inputs Inputs { get; set; }

            [JsonProperty("runAfter")]
            public Outputs RunAfter { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }
        private class Schedule
        {
            [JsonProperty("hours")]
            public List<string> Hours { get; set; }

            [JsonProperty("minutes")]
            public List<string> Minutes { get; set; }

            [JsonProperty("weekDays")]
            public List<string> WeekDays { get; set; }
        }
        private class Inputs
        {
            [JsonProperty("body")]
            public Body Body { get; set; }

            [JsonProperty("host")]
            public Host Host { get; set; }

            [JsonProperty("method")]
            public string Method { get; set; }

            [JsonProperty("path")]
            public string Path { get; set; }

            [JsonProperty("queries")]
            public Queries Queries { get; set; }
        }
        private class Body
        {
            [JsonProperty("ContentData")]
            public string ContentData { get; set; }
        }
        private class Host
        {
            [JsonProperty("connection")]
            public Connection Connection { get; set; }
        }
        private class Connection
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }
        private class Queries
        {
            [JsonProperty("systemProperties")]
            public string SystemProperties { get; set; }
        }
        private class Outputs
        {
        }
        private class Parameters
        {
            [JsonProperty("$connections")]
            public Connections Connections { get; set; }
        }
        private class Connections
        {
            [JsonProperty("defaultValue")]
            public Outputs DefaultValue { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }
        private class Triggers
        {
            [JsonProperty("Recurrence")]
            public Recurrence Recurrence { get; set; }
        }
        private class Recurrence
        {
            [JsonProperty("recurrence")]
            public RecurrenceClass RecurrenceRecurrence { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }
        private class RecurrenceClass
        {
            [JsonProperty("frequency")]
            public string Frequency { get; set; }

            [JsonProperty("interval")]
            public long Interval { get; set; }

            [JsonProperty("startTime")]
            public DateTimeOffset StartTime { get; set; }

            [JsonProperty("timeZone")]
            public string TimeZone { get; set; }

            [JsonProperty("schedule")]
            public Schedule Schedule { get; set; }
        }
        #endregion

    }

    /// <summary>
    /// Clase interna que hereda de la clase abstracta  ServiceClientCredentials, se utiliza para inicializar el 
    /// LogicManagementClient y al moemnto que este realice las peticiones al api de logic app se obtenga el token
    /// </summary>
    internal class CustomLoginCredentials : ServiceClientCredentials
    {
        private string _azureTenantId;
        private string _azureClientId;
        private string _azureClientSecret;
        public CustomLoginCredentials(
             string azureClientId,
             string azureTenantId,
             string azureClientSecret)
        {
            _azureTenantId = azureTenantId;
            _azureClientId = azureClientId;
            _azureClientSecret = azureClientSecret;
        }

        private string AuthenticationToken { get; set; }
        public override void InitializeServiceClient<T>(ServiceClient<T> client)
        {
            var authenticationContext = new AuthenticationContext("https://login.windows.net/" + _azureTenantId);
            var credential = new ClientCredential(clientId: _azureClientId, clientSecret: _azureClientSecret);

            var result = authenticationContext.AcquireTokenAsync(resource: "https://management.core.windows.net/", clientCredential: credential).GetAwaiter().GetResult();

            if (result == null) throw new InvalidOperationException("Failed to obtain the JWT token");

            AuthenticationToken = result.AccessToken;
        }
        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException("request");

            if (AuthenticationToken == null) throw new InvalidOperationException("Token Provider Cannot Be Null");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticationToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            await base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}

