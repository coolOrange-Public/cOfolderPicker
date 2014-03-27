namespace cOFolderPicker.Helper
{
	 internal static class StringTrim
	 {
		  /// <summary>
		  /// Trims the start.
		  /// </summary>
		  /// <param name="target">The target.</param>
		  /// <param name="trimString">The trim string.</param>
		  /// <returns>The trimmed string</returns>
		  public static string TrimStart(this string target, string trimString)
		  {
				string result = target;
				while (result.StartsWith(trimString))
					 result = result.Substring(trimString.Length);
				return result;
		  }

		  /// <summary>
		  /// Trims the end.
		  /// </summary>
		  /// <param name="target">The target.</param>
		  /// <param name="trimString">The trim string.</param>
		  /// <returns>The trimmed string</returns>
		  public static string TrimEnd(this string target, string trimString)
		  {
				string result = target;
				while (result.EndsWith(trimString))
					 result = result.Substring(0, result.Length - trimString.Length);
				return result;
		  }
	 }
}