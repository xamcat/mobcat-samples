using System;
using Microsoft.MobCAT;
using Xamarin.Forms;

namespace News.Helpers
{
    public static class ColorHelper
    {
        public static Color ToColor(this string hexString)
        {
            try
            {
                return Color.FromHex(hexString);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return default(Color);
            }
        }
    }
}
