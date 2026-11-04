using System;

namespace RetroRecord.Engine.Chart
{
    [Serializable]
    public class Beatline
    {
        public BeatlineType Type;
        public double Time;
        public uint Tick;
    
        public Beatline(BeatlineType type, double time, uint tick)
        {
            Type = type;
            Time = time;
            Tick = tick;
        }
    }
}