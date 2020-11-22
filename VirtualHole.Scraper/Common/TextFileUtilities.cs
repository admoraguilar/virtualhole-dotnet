using System;
using System.Collections.Generic;

namespace VirtualHole.Scraper
{
	public static class TextFileUtilities
	{
		/// <summary>
		/// Get comma separated values.
		/// </summary>
		public static IReadOnlyList<string> GetCSV(
			string content,
			StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries)
		{
			return content.Split(new string[] { "," }, splitOptions);
		}

		/// <summary>
		/// Get new line separated values.
		/// </summary>
		public static IReadOnlyList<string> GetNLSV(
			string content,
			StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries)
		{
			return content.Split(new string[] { Environment.NewLine }, splitOptions);
		}

		/// <summary>
		/// Get semi-colon separated values.
		/// </summary>
		public static IReadOnlyList<string> GetSCSV(
			string content,
			StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries)
		{
			return content.Split(new string[] { ";" }, splitOptions);
		}

		/// <summary>
		/// Get forward slash separated values;
		/// </summary>
		public static IReadOnlyList<string> GetFSSV(
			string content,
			StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries)
		{
			return content.Split(new string[] { "/" }, splitOptions);
		}
	}
}
