using RetroRecord.Engine.Chart;
using RetroRecord.Engine.Settings;

namespace RetroRecord.Engine.Player
{
    public static class SyncTrack
    {
        public static TempoChange s_CurrentTempoChange;
        public static TimeSignatureChange s_CurrentTimeSignature;
        
        public static double TickToSeconds(uint tick)
        {
            return tick / (NotesManager.s_CurrentTempo * EngineSettings.RESOLUTION);
        }

        public static uint TimeToTick(double time)
        {
            return TimeToTick(time, s_CurrentTempoChange);
        }
        
        public static uint TimeToTick(double time, TempoChange currentTempo)
        {
            return currentTempo.TimeToTick(time);
        }

        public static float TimeToYPos(double time)
        {
            return (float)time * EngineSettings.HighwaySpeed;
        }

        public static float TimeToYPos(float time)
        {
            return time * EngineSettings.HighwaySpeed;
        }

        public static double YPosToTime(float pos)
        {
            return pos / EngineSettings.HighwaySpeed;
        }
        
        public static double GetQuarterNotePosition(uint quarterTick)
        {
            return s_CurrentTimeSignature.QuarterNoteCount + s_CurrentTimeSignature.GetQuarterNoteProgress(quarterTick);
        }
    }
}