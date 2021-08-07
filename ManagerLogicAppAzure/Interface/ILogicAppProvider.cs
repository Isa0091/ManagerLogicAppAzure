using ManagerLogicAppAzure.Dtos;
using Microsoft.Azure.Management.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLogicAppAzure.Interface
{
    /// <summary>
    /// maneja los datos necesarios para la creacion y edicion de logic app especificas de azure
    /// </summary>
    public interface ILogicAppProvider
    {
        /// <summary>
        /// Crea una logic app de recurrencia <see cref="https://docs.microsoft.com/en-us/azure/connectors/connectors-native-recurrence"/>
        /// la cual tendra un mensaje listo para enviar aun topic definido por tiempo de recurrencia
        /// definido en <see cref="RecurreceVo"/> Mes,semana,Dia  y desde cuando se comenzara a ejecutar el logic app recurrente
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="Name"></param>
        /// <param name="resourceGroup"></param>
        /// <param name="message"></param>
        /// <param name="recurrece"></param>
        /// <param name="weekDays"></param>
        /// <returns></returns>
        Task<Workflow> CreateOrUpdateLogicAppRecurrenceToSendMessegeAsync(string WorflowLogicAppName, string topic, string resourceGroup, string message,RecurrenceDto recurrence, ConnectionData connectionData);

        /// <summary>
        /// Elimina una logic app
        /// </summary>
        /// <param name="WorflowLogicAppName"></param>
        /// <param name="resourceGroup"></param>
        /// <returns></returns>
        Task DeleteLogicAppAsync(string WorflowLogicAppName, string resourceGroup, ConnectionData connectionData);

        /// <summary>
        /// Obtneemos un logic app 
        /// </summary>
        /// <param name="worflowLogicAppName"></param>
        /// <param name="resourceGroup"></param>
        /// <returns></returns>
        Task<Workflow> GetLogicAppAsync(string worflowLogicAppName, string resourceGroup, ConnectionData connectionData);
    }
}
