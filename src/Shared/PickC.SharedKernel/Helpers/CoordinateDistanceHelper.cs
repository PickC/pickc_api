namespace PickC.SharedKernel.Helpers;

public static class CoordinateDistanceHelper
{
    private const double EarthRadiusKm = 6371.0;

    public static double CalculateDistanceKm(
        double lat1, double lon1, double lat2, double lon2)
    {
        var lat1Rad = lat1 * Math.PI / 180;
        var lon1Rad = lon1 * Math.PI / 180;
        var lat2Rad = lat2 * Math.PI / 180;
        var lon2Rad = lon2 * Math.PI / 180;

        return Math.Acos(
            Math.Cos(lat1Rad) * Math.Cos(lon1Rad) * Math.Cos(lat2Rad) * Math.Cos(lon2Rad) +
            Math.Cos(lat1Rad) * Math.Sin(lon1Rad) * Math.Cos(lat2Rad) * Math.Sin(lon2Rad) +
            Math.Sin(lat1Rad) * Math.Sin(lat2Rad)
        ) * EarthRadiusKm;
    }
}
