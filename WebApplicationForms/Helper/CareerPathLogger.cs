/*
 *
 * Version:
 *     $Id$
 *
 * Revisions:
 *     $Log$
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace WebApplicationForms.Helper
{
    /// <summary>
    /// Class representing the application logger
    /// 
    /// <list type="bullet">
    /// 
    /// <item>
    /// <term>Author</term>
    /// <description>Emmanuel Baguia</description>
    /// </item>
    /// 
    /// </list>
    /// 
    /// </summary>
    public static class CareerPathLogger
    {
        private static log4net.ILog Log { get; set; }       // logger object using log4net

        static CareerPathLogger()
        {
            Log = log4net.LogManager.GetLogger(typeof(CareerPathLogger));
        }

        /// <summary>
        /// Logger ERROR level
        /// </summary>
        /// <param name="msg">The message from the caller to be logged</param>
        public static void Error(object msg)
        {
            Log.Error(msg);
        }

        /// <summary>
        /// Logger ERROR level
        /// </summary>
        /// <param name="msg">The message from the caller to be logged</param>
        /// <param name="ex">With an exception information</param>
        public static void Error(object msg, Exception ex)
        {
            Log.Error(msg, ex);
        }

        /// <summary>
        /// Logger ERROR level
        /// </summary>
        /// <param name="ex">With an exception information</param>
        public static void Error(Exception ex)
        {
            Log.Error(ex.Message, ex);
        }

        /// <summary>
        /// Logger FATAL level
        /// </summary>
        /// <param name="msg">The message from the caller to be logged</param>
        public static void Fatal(object msg)
        {
            Log.Error(msg);
        }

        /// <summary>
        /// Logger FATAL level
        /// </summary>
        /// <param name="msg">The message from the caller to be logged</param>
        /// <param name="ex">With an exception information</param>
        public static void Fatal(object msg, Exception ex)
        {
            Log.Error(msg, ex);
        }

        /// <summary>
        /// Logger FATAL level
        /// </summary>
        /// <param name="ex">With an exception information</param>
        public static void Fatal(Exception ex)
        {
            Log.Error(ex.Message, ex);
        }

        /// <summary>
        /// Logger DEBUG level
        /// </summary>
        /// <param name="msg">The message from the caller to be logged</param>
        public static void Debug(object msg)
        {
            Log.Debug(msg);
        }

        /// <summary>
        /// Logger INFO level
        /// </summary>
        /// <param name="msg">The message from the caller to be logged</param>
        public static void Info(object msg)
        {
            Log.Info(msg);
        }

        /// <summary>
        /// Logger WARN level
        /// </summary>
        /// <param name="msg">The message from the caller to be logged</param>
        public static void Warn(object msg)
        {
            Log.Warn(msg);
        }
    }
}