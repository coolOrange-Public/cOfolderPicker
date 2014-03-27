using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cOFolderPicker.Helper
{
	 internal static class IconHelper
	 {
		  /// <summary>
		  /// Icon to the image source.
		  /// </summary>
		  /// <param name="icon">The icon.</param>
		  /// <returns></returns>
		  /// <exception cref="System.ComponentModel.Win32Exception"></exception>
		  public static ImageSource ToImageSource(this Icon icon)
		  {
				Bitmap bitmap = icon.ToBitmap();
				IntPtr hBitmap = bitmap.GetHbitmap();

				ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
					hBitmap,
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
				return wpfBitmap;
		  }
	 }
}