using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Payment.Model
{
    public enum TransaccionEstado
    {
        Exitoso=0,
        Creado=1,
        Pendiente=2,
        Desconocido = 3,
        Rechazado = 4,


        Error=99
    }
}