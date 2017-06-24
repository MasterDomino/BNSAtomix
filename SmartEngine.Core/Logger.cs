﻿using log4net;
using System;
using System.Runtime.CompilerServices;

namespace SmartEngine.Core
{
    public static class Logger
    {
        #region Properties

        public static ILog Log { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Wraps up the message with the CallerMemberName
        /// </summary>
        /// <param name="Caller"></param>
        /// <param name="message"></param>
        /// <param name="memberName"></param>
        public static void Debug(string Caller, string message, [CallerMemberName]string memberName = "")
        {
            Log?.Debug($"{Caller} Method: {memberName} Packet: {message}");
        }

        /// <summary>
        /// Wraps up the error message with the CallerMemberName
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="innerException"></param>
        public static void Error(Exception innerException = null, [CallerMemberName]string memberName = "")
        {
            if (innerException != null)
            {
                Log?.Error($"{memberName}: {innerException.Message}", innerException);
            }
        }

        /// <summary>
        /// Wraps up the info message with the CallerMemberName
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="memberName"></param>
        public static void Info(string message, Exception innerException = null, [CallerMemberName]string memberName = "")
        {
            if (innerException != null)
            {
                Log?.Info($"Method: {memberName} Message: {message}", innerException);
            }
        }

        /// <summary>
        /// Wraps up the info message with the Logging Event
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="caller"></param>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        public static void LogEvent(string logEvent, string caller, string data, Exception ex = null)
        {
            if (ex != null)
            {
                Log?.Info($"[{logEvent}][{caller}]{data}");
            }
            else
            {
                Log?.Info($"[{logEvent}][{caller}]{data}", ex);
            }
        }

        public static void InitializeLogger(ILog log)
        {
            Log = log;
        }

        #endregion
    }
}