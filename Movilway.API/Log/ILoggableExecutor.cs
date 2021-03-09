using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movilway.API.Log
{
    interface ILoggableExecutor
    {
        void LogPreExcetution(Object excecutionParameter);
        void LodPostExcecution(Object excecutionResult);
    }
}
