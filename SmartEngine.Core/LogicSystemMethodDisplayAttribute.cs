using System;

namespace SmartEngine.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LogicSystemMethodDisplayAttribute : Attribute
    {
        #region Instantiation

        public LogicSystemMethodDisplayAttribute(string displayText, string formatText)
        {
            DisplayText = displayText;
            FormatText = formatText;
        }

        #endregion

        #region Properties

        public string DisplayText { get; }

        public string FormatText { get; }

        #endregion
    }
}