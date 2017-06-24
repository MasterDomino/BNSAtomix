using System;

namespace SmartEngine.Core
{
    public class LogicSystemBrowsableAttribute : Attribute
    {
        #region Instantiation

        public LogicSystemBrowsableAttribute(bool browsable)
        {
            Browsable = browsable;
        }

        #endregion

        #region Properties

        public bool Browsable { get; }

        #endregion
    }
}