using System.Globalization;
namespace RentalCheckIn.Utilities;
public static class CultureUtility
{
    public static CultureInfo CurCulture { get; set; } = CultureInfo.InvariantCulture;
    public static void SetCurrentCulture()

    {
        var curCulture = new CultureInfo(CurCulture.Name);
        CultureInfo.CurrentCulture = curCulture;
        CultureInfo.CurrentUICulture = curCulture;
    }
}
