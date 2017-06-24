﻿using System;
using System.ComponentModel;
using System.Globalization;

namespace SmartEngine.Core.Math
{
    public class _GeneralTypeConverter<MathType> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                try
                {
                    Type type = typeof(MathType);
                    return type.GetMethod("Parse").Invoke(null, new object[] { value });
                }
                catch (Exception)
                {
                    return value;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
