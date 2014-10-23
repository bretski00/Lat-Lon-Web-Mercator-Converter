using System;

namespace LatLonAndWebMercatorConverter
{
    class Program
    {
        private const double LargestX = 20037508.3427892;

        static void Main(string[] args)
        {
            Console.WriteLine("1) Geographic(Lat, Lon) to Web Mercator(X, Y)");
            Console.WriteLine("2) Web Mercator(X, Y) to Geographic(Lat, Lon)");
            Console.WriteLine("3) Normalize X for extents. This will take an extent and convert to an extent that is within bounds");
            Console.WriteLine("exit) Exits the application");
            Console.Write("1, 2, 3, or exit? -> ");
            var method = Console.ReadLine();

            while (method != null && method.ToLower() != "exit")
            {
                if (method == "1")
                {
                    Console.Write("Lat (Y): ");
                    var yLat = Convert.ToDouble(Console.ReadLine());
                    Console.Write("Lon (X): ");
                    var xLon = Convert.ToDouble(Console.ReadLine());
                    double mercatorX, mercatorY;
                    GeographicToWebMercator(xLon, yLat, out mercatorX, out mercatorY);
                    Console.WriteLine("X: " + mercatorX);
                    Console.WriteLine("Y:" + mercatorY);
                }
                else if (method == "2")
                {
                    Console.Write("X: ");
                    var xLon = Convert.ToDouble(Console.ReadLine());
                    Console.Write("Y: ");
                    var yLat = Convert.ToDouble(Console.ReadLine());
                    double outLat, outLon;
                    WebMercatorToGeographic(xLon, yLat, out outLon, out outLat);
                    Console.WriteLine("Lat:" + outLat);
                    Console.WriteLine("Lon:" + outLon);
                }
                else if (method == "3")
                {
                    Console.Write("X to Normalize: ");
                    var newX = NormalizeX(Convert.ToDouble(Console.ReadLine()));
                    Console.WriteLine("New X: " + newX);
                    Console.WriteLine("Lon: " + WebMercatorXToGeographicLon(newX));
                }
                Console.Write("1, 2, 3, or exit? -> ");
                method = Console.ReadLine();
            } 
        }

        private static void WebMercatorToGeographic(double mercatorX, double mercatorY, out double xLon, out double yLat)
        {
            xLon = WebMercatorXToGeographicLon(mercatorX);
            yLat = WebMercatorYToGeographicLat(mercatorY);
        }

        private static double WebMercatorXToGeographicLon(double mercatorX)
        {
            var x = (NormalizeX(mercatorX) / 6378137.0) * 57.295779513082323;
            return x - ((Math.Floor(((x + 180.0) / 360.0))) * 360.0);
        }

        private static double WebMercatorYToGeographicLat(double mercatorY)
        {
            return (1.5707963267948966 - (2.0 * Math.Atan(Math.Exp((-1.0 * mercatorY) / 6378137.0)))) * 57.295779513082323;
        }

        private static void GeographicToWebMercator(double xLon, double yLat, out double mercatorX, out double mercatorY)
        {
            var y = yLat*0.017453292519943295;

            mercatorX = 6378137.0 * (xLon * 0.017453292519943295);
            mercatorY = 3189068.5*Math.Log((1.0 + Math.Sin(y))/(1.0 - Math.Sin(y)));
        }

        private static double NormalizeX(double x)
        {
            var absX = Math.Abs(x);
            var returnX = absX;
            var isNegative = (x < 0 ? -1 : 1);
            var multiple = Convert.ToInt32(Math.Floor(absX/LargestX));
            var isEven = (multiple%2) == 0;

            // return x no need to go any further
            if (absX <= LargestX) return x;

            // If is odd number of divisions then need to take largest - x
            // If is event number. then just take the remainder, cause it is coming from 0 then

            while (returnX > LargestX)
            {
                returnX = returnX - LargestX;
                isNegative = isNegative * -1;
            }

            if (isEven)
            {
                return returnX*isNegative;
            }

            // Is not even so need to take largest - x
            return ((LargestX - returnX)*isNegative);
        }

        /* Notes X Lon values and the normalized returns
        12 - 41627611 = 1552594.3144216
        -81 - 30982685 = -9092331.6855784
        -167 - 61508577 = -18641456.3711568
        -4 - 79667568 = -482465.371156804
        102 - 91564839 = 11414805.62884322
         */
    }
}
