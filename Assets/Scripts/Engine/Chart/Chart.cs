using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class Chart
    {
        public Chart(Difficulty difficulty, Note[] notes)
        {
            Difficulty = difficulty;
            Notes = notes;
        }

        [field:SerializeField]
        public Difficulty Difficulty { get; private set; }
        [field:SerializeField]
        public Note[] Notes { get; private set; }
    }
}