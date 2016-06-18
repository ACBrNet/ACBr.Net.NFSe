using System;
using System.Globalization;

namespace ACBr.Net.NFSe.Util
{
	internal static class Extensions
	{
		public static string ToInvariant(this IFormattable formattable)
		{
			if (formattable == null)
				throw new ArgumentNullException(nameof(formattable));

			return formattable.ToString(null, CultureInfo.InvariantCulture);
		}
	}
}