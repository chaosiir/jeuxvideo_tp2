using System;

namespace Com.MyCompany.MyGame
{
    public class Util
    {
        public static double toRad(double degree)
        {
            return degree * 2 * Math.PI / 360;
        }

        public static double toDeg(double rad)
        {
            return rad * 360 / (2 * Math.PI);
        }

        public static double angleBetweenVec(double x1, double x2, double y1, double y2)
        {
            double angle;
            if (y2 - y1 == 0) {
                angle = 0;
                if (x2 - x1 < 0) {
                    angle += 180;
                }
            } else {
                angle = toDeg(Math.Atan((x2 - x1) / (y2 - y1)));
                if (y2 - y1 < 0) {
                    angle += 180;
                }
            }
            if (angle > 180) {
                angle -= 360;
            }
            return angle;
        }
    }
}