using System.Globalization;

namespace Rain_World_Drought
{
    /// <summary>
    /// Mainly for getting language settings
    /// </summary>
    public static class DroughtConfig
    {
        public static void Initialize(OptionInterface self)
        {
        }

        public static CultureInfo GetCultureInfo(OptionInterface self) => return self.GetCultureInfo();
    }
}
