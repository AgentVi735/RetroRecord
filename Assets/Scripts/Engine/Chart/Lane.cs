using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public struct Lanes
    {
        public Lanes(byte leftLanes, byte rightLanes)
        {
            LeftLanes = leftLanes;
            RightLanes = rightLanes;
        }

        [field: SerializeField]
        public byte LeftLanes { get; private set; }
        [field: SerializeField]
        public byte RightLanes { get; private set; }
    }
}