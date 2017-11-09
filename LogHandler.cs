﻿using System;
using System.Deployment.Application;
using System.IO;
using System.Xml;

namespace Log_Handler
{
    class LogHandler
    {
        /// <summary>
        /// Creates a new log entry using the specified severity level
        /// </summary>
        /// <param name="e"></param>
        /// <param name="severity">
        /// Using SeverityLevel enumerator.
        /// </param>
        /// <param name="subject">A brief description of the event</param>
        public static async void CreateEntry(Exception e, SeverityLevel severity, string subject)
        {
            // Needs implementing
        }

        public static async void CreateEntry(SeverityLevel severity, string subject)
        {
            // Needs implementing
        }
    }

    class LogFile
    {
        private static string filePath;
        private static LogSession currentSession;

        public LogFile()
        {
            string applicationFolder = AppDomain.CurrentDomain.BaseDirectory;
            filePath = applicationFolder + @"\UpdateLog.xml";
        }

        private static void CreateLogFile()
        {
            XmlDocument defaultLogFile = new XmlDocument();
            XmlDeclaration xmlDeclaration = defaultLogFile.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = defaultLogFile.DocumentElement;
            defaultLogFile.InsertBefore(xmlDeclaration, root);

            XmlElement rootNode = defaultLogFile.CreateElement("UpdateLog");
            defaultLogFile.AppendChild(rootNode);

            try
            {
                defaultLogFile.Save(filePath);
            }
            catch
            {
                // Sorry, can't log your failure, we have no log file. ¯\_(ツ)_/¯
            }
        }

        private static void WriteSession()
        {
            XmlDocument logFile = new XmlDocument();
            try
            {
                logFile.Load(filePath);
            }
            catch
            {
                return;
            }

            XmlNode updateLog = logFile.SelectSingleNode("UpdateLog");

            updateLog.AppendChild(currentSession.CreateXmlElement(logFile));

            try
            {
                logFile.Save(filePath);
            }
            catch
            {

            }
        }

        public static void WriteLogEntry(LogEntry logEntry)
        {
            XmlDocument logFile = new XmlDocument;

            if (!File.Exists(filePath))
            {
                CreateLogFile();
            }
            else
            {
                try
                {
                    logFile.Load(filePath);
                }
                catch
                {
                    return;
                }

                if (logFile.SelectSingleNode("UpdateLog") == null)
                {
                    CreateLogFile();
                }
            }

            if (currentSession == null)
            {
                currentSession = new LogSession();
                WriteSession();
            }

            try
            {
                logFile.Load(filePath);
            }
            catch
            {

            }

            XmlNode sessionNode = logFile.SelectSingleNode("UpdateLog/LogSession[@StartTime='" + currentSession.startTime + "']");

            sessionNode.AppendChild(logEntry.CreateXmlElement(logFile));

            try
            {
                logFile.Save(filePath);
            }
            catch
            {

            }
        }
    }

    class LogEntry
    {
        public DateTime time { get; set; }
        public Exception exception { get; set; }
        public SeverityLevel severity { get; set; }
        public string subject { get; set; }
        public string detail { get; set; }

        public XmlElement CreateXmlElement(XmlDocument xmlDoc)
        {
            XmlElement element = xmlDoc.CreateElement("LogEntry");

            XmlAttribute timeAttribute = xmlDoc.CreateAttribute("Time");
            timeAttribute.InnerText = time.ToString("o");
            element.Attributes.Append(timeAttribute);

            XmlElement severityNode = xmlDoc.CreateElement("Severity");
            severityNode.InnerText = severity.ToString();
            element.AppendChild(severityNode);

            XmlElement subjectNode = xmlDoc.CreateElement("Subject");
            subjectNode.InnerText = subject;
            element.AppendChild(subjectNode);

            XmlElement detailNode = xmlDoc.CreateElement("Detail");
            detailNode.InnerText = detail;
            element.AppendChild(detailNode);

            XmlElement exceptionMessageNode = xmlDoc.CreateElement("ExceptionMessage");
            exceptionMessageNode.InnerText = exception.Message;
            element.AppendChild(exceptionMessageNode);

            XmlElement exceptionFullTextNode = xmlDoc.CreateElement("ExceptionFullText");
            exceptionFullTextNode.InnerText = exception.ToString();
            element.AppendChild(exceptionFullTextNode);

            return element;
        }
    }
    
    public enum SeverityLevel
    {
        Fatal,  // Data loss is likely to have occurred, or is likely to occur, as a result of this event
        Error,  // Application cannot function correctly following this event, and will likely terminate
        Warn,   // Application was stopped from doing something but can keep running, maybe switched to a backup or wasn't able to load a page
        Info,   // Useful information about what just happened, maybe a service started or a connection was established
        Debug,  // Information useful for technicians or sysadmins to troubleshoot an issue
        Trace   // Application has an itch on its nose that the developer might want to know about
    }
}
