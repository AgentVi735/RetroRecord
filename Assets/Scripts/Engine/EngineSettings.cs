using UnityEngine;

namespace RetroRecord.Engine
{
    [CreateAssetMenu(menuName = "Settings/Engine", fileName = "Engine Settings")]
    public class EngineSettings : ScriptableObject
    {
        [field:SerializeField]
        public float HighwaySpeed { get; private set; }
    }
}