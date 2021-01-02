using System;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;

namespace VirtualHole.API
{
	/// <summary>
	/// Converts string that's comma-separated into List<string>
	/// </summary>
	public class CSVToListStringTypeCoverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType == typeof(string)) {
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context,
			CultureInfo culture, object value)
		{
			if(value is string) {
				string raw = (string)value;
				List<string> listString = new List<string>(raw.Split(','));
				return listString;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
