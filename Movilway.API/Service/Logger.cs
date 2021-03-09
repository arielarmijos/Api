using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Xml;
using log4net;

namespace Movilway.API.Service
{
    public class Logger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

        public Logger()
        {
            InitializeLog(ConfigurationManager.AppSettings["LogFilePath"], ConfigurationManager.AppSettings["LogFileNameOldApi"]);
        }

        /// <summary>
        /// Enumerable que contiene los tipos de mensajes para logs que pueden ocurrir durante la ejecución del proceso
        /// </summary>
        public enum LogMessageType { Warning, Error, Info }

        /// <summary>
        /// Enumerable que contiene los tipos de mensajes para logs que pueden ocurrir durante la ejecución del proceso
        /// </summary>
        public enum LoggingLevelType { Low = 1, Medium = 2, High = 3 }

        /// <summary>
        /// Método que se encarga de configurar el archivo donde se realizará el registro de lo que suceda durante la ejecución  
        /// </summary>
        /// <param name="path">ruta donde se guardará el log</param>
        /// <param name="fileName">nombre del archivo log</param>
        public static void InitializeLog(string path, string fileName)
        {
            XmlDocument miXMLDoc = new XmlDocument();
            miXMLDoc.LoadXml(
                @"<log4net>
                    <appender name='RollingFileAppender' type='log4net.Appender.RollingFileAppender'>
                        <file type='log4net.Util.PatternString' value='" + (!String.IsNullOrEmpty(path) ? path + (!path.EndsWith(@"\") ? @"\" : "") : @"c:\Logs\") + fileName.Replace(".log", "") + @".log"" />
                        <lockingModel type='log4net.Appender.FileAppender+MinimalLock' />
                        <rollingStyle value='Composite' />
                        <datePattern value='-yyyyMMdd-HH.log' />
                        <maxSizeRollBackups value='100' />
                        <maximumFileSize value='100MB' />
                        <appendToFile value='true' />
                        <layout type='log4net.Layout.PatternLayout'>
                           <conversionPattern value='%date{yyyy-MM-dd HH:mm:ss.fff} %-40% [%-4.5p] %m%n' /> 
                        </layout>
                    </appender>
                    <root>
                        <level value='ALL' />
                        <appender-ref ref='RollingFileAppender' />
                    </root>
                </log4net>");
            
            XmlElement miXMLConfig = miXMLDoc.DocumentElement;
            log4net.Config.XmlConfigurator.Configure(miXMLConfig);
        }

        /// <summary>
        /// Procedimiento que permite realizar el registro de los mensajes según el nivel en el archivo Log
        /// </summary>
        /// <param name="logType">tipo de mensaje</param>
        /// <param name="message">mensaje a registrar</param>
        /// <param name="loggingLevel">nivel de registro en el log</param>
        public void Message(LogMessageType logType, string message, LoggingLevelType loggingLevel)
        {
            if (Convert.ToInt32(loggingLevel) <= Convert.ToInt32(ConfigurationManager.AppSettings["LoggingLevelType"]))
            {
                if (logType == LogMessageType.Info)
                    log.Info(message);
                else if (logType == LogMessageType.Warning)
                    log.Warn(message);
                else if (logType == LogMessageType.Error)
                    log.Error(message);
                else
                    log.Fatal(message);
            }
        }
    }
}