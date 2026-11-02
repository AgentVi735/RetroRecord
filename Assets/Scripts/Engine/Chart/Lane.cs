using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public struct Lanes
    {
        [field: SerializeField]
        public byte LeftLanes { get; private set; }
        [field: SerializeField]
        public byte RightLanes { get; private set; }
    }
}