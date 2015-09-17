using System;

namespace Wkx
{
    [Flags]
    public enum Dimensions
    {
        XY = 1,
        Z = 2,
        M = 4,
        XYZ = XY | Z,
        XYM = XY | M,
        XYZM = XY | Z | M
    }
}
