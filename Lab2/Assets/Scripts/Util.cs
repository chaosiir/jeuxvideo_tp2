using System;

namespace Com.MyCompany.MyGame
{
    public class Util
    {
        public static double toRad(double angle)
        {
            return angle * 2 * Math.PI / 360;
        }
    }
}