using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLogicAppAzure
{
    /// <summary>
    /// Indicala frecuencia con la que se ejecutara
    /// una cargo recurrente
    /// </summary>
    public enum Frequency
    {
        /// <summary>
        /// Indica una frcuencia diaria
        /// </summary>
        [Description("Dia")]
        Day = 1,

        /// <summary>
        /// Indica una frcuencia semanal
        /// </summary>
        [Description("Semana")]
        Week = 2,

        /// <summary>
        /// Indica una frcuencia Mensual
        /// </summary>
        [Description("Mes")]
        Month = 3
    }

    /// <summary>
    /// Indetifica los dias de la semana
    /// </summary>
    public enum WeekDays
    {
        /// <summary>
        /// Lunes
        /// </summary>
        [Description("Lunes")]
        Monday = 1,
        /// <summary>
        /// Martes
        /// </summary>
        [Description("Martes")]
        Tuesday = 2,
        /// <summary>
        /// Miercoles
        /// </summary>
        [Description("Miercoles")]
        Wednesday = 3,
        /// <summary>
        /// Jueves
        /// </summary>
        [Description("Jueves")]
        Thursday = 4,
        /// <summary>
        /// Viernes
        /// </summary>
        [Description("Viernes")]
        Friday = 5,
        /// <summary>
        /// Sabado
        /// </summary>
        [Description("Sabado")]
        Saturday = 6,
        /// <summary>
        /// Domingo
        /// </summary>
        [Description("Domingo")]
        Sunday = 7

    }
}
