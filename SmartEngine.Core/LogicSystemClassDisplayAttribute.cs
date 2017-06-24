using System;

namespace SmartEngine.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LogicSystemClassDisplayAttribute : Attribute
    {
        #region Instantiation

        public LogicSystemClassDisplayAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        #endregion

        #region Properties

        public string DisplayName { get; }

        #endregion
    }
}