using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.External;
using Movilway.API.Service;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Reflection;

namespace Movilway.API.Log
{
    public class LogUtils
    {
        private static ThreadLocal<String> _methodInvocationID = null;

        public static void LogMethodInvocationStart(params Object[] parameters)
        {
            LogMethodInvocationStart(false, parameters);
        }

        public static void LogMethodInvocationStart(Boolean clearThreadData, params Object[] parameters)
        {
            if (clearThreadData)
                _methodInvocationID = null;
            else
                _methodInvocationID = new ThreadLocal<String>(() => Thread.CurrentThread.ManagedThreadId.ToString() + DateTime.Now.ToString("HHmmssfffff"));

            StackTrace stack = new StackTrace();
            String methodFQN = stack.GetFrames()[1].GetMethod().DeclaringType.ToString() + "."+ stack.GetFrames()[1].GetMethod().Name;

            StringBuilder sb = new StringBuilder();
            foreach (Object parameter in parameters)
            {
                Type parameterType = parameter.GetType();

                foreach (PropertyInfo propertyInfo in parameterType.GetProperties())
                {
                    if (propertyInfo.GetCustomAttributes(typeof(NotLoggableAttribute), false).Count()==0)
                    {
                        sb.Append(propertyInfo.Name).Append("=").Append(propertyInfo.GetValue(parameter, null));
                        sb.Append("|");
                    }
                }
            }
            ApiServiceBase.Log(Logger.LogMessageType.Info, "MethodInvocation: " + methodFQN + "Data: "+sb.ToString(), Logger.LoggingLevelType.Low);
        }

        public static void LogMethodInvocationEnd()
        {
        }
    }
}