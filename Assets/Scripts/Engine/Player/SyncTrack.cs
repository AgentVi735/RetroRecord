namespace RetroRecord.Engine.Player
{
    public static class SyncTrack
    {
        public static double TickToSeconds(uint tick)
        {
            return tick / (NotesManager.s_CurrentTempo * EngineSettings.k_RESOLUTION);
        }

        public static float TimeToYPos(double time)
        {
            return (float)time * EngineSettings.s_HighwaySpeed;
        }

        public static float TimeToYPos(float time)
        {
            return time * EngineSettings.s_HighwaySpeed;
        }

        public static double YPosToTime(float pos)
        {
            return pos / EngineSettings.s_HighwaySpeed;
        }
    }
}