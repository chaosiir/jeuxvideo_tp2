using System;

namespace Com.MyCompany.MyGame
{
    public class Util
    {
        public static double toRad(double angle)
        {
            return angle * 2 * Math.PI / 360;
        }

        public static double angleBetweenVec(double x1, double x2, double y1, double y2)
        {
            var angle = Math.Atan((y2 - y1) / (x2 - x1));
            if (x2 - x1 < 0)
            {
                angle += 180;
            }
            return angle;
        }
    }
}