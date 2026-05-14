using System.Globalization;

namespace WebApp.Helpers;

public static class UiText
{
    public static string T(string key)
    {
        return App.Resources.Views.Home.ResourceManager.GetString(key, CultureInfo.CurrentUICulture)
               ?? App.Resources.Views.Home.ResourceManager.GetString(key, CultureInfo.InvariantCulture)
               ?? key;
    }
}
