using System;

namespace RetroRecord.Engine.Lights
{
    public enum LightTime
    {
        WholeNote,          // 1/1 LiFW
        HalfNote,           // 1/2 LiFH
        QuarterNote,        // 1/4 LiFQ
        QuaverNote,         // 1/8 LiFV
        SemiQuaverNote,     // 1/16 LiFS
        ThirtySecondNote,   // 1/32 LiFT
        SixtyFourthNote     // 1/64 LiFF
    }

    public static class LightTimeExtensions
    {
        public static float GetDivisor(this LightTime time)
        {
            return time switch
            {
                LightTime.WholeNote => 4,
                LightTime.HalfNote => 2,
                LightTime.QuarterNote => 1,
                LightTime.QuaverNote => 0.5f,
                LightTime.SemiQuaverNote => 0.25f,
                LightTime.ThirtySecondNote => 0.125f,
                LightTime.SixtyFourthNote => 0.0625f,
                _ => 1
            };
        }
    }
}