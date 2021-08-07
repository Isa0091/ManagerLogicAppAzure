using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLogicAppAzure.Dtos
{
    /// <summary>
    /// Maneja los datos necesarios para crear recurrencias
    /// </summary>
    public class RecurrenceDto
    {
        /// <summary>
        /// 
        /// </summary>
        public RecurrenceDto()
        {
            WeekDays = new List<WeekDays>();
        }
        /// <summary>
        /// Indica el intervalo de tiempo segun la frecuencia
        ///- Mes: 1-16 meses
        ///- Semana: 1-71 semanas
        ///- Día: 1-500 días
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Indica la frecuencia con que se ejecutara la recurrencia
        /// </summary>
        public Frequency Frequency { get; set; }

        /// <summary>
        /// Indica la fecha de incio de la primera recurrencia desde el momento que se define
        /// la recurrencia seejecutara el dia y hora indicado en la primera recurrencia
        /// </summary>
        public DateTimeOffset StartTime { get; set; }
        /// <summary>
        /// Dias de la semana especificados para la recurrencia
        /// </summary>
        public List<WeekDays> WeekDays { get; set; }

        /// <summary>
        /// Identificadro de la zona horaria a usar
        /// en este caso puedeser "Central America Standard Time"
        /// </summary>
        public string TimeZone { get; set; }
    }
}
