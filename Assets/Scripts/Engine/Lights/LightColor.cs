using UnityEngine;

namespace RetroRecord.Engine.Lights
{
    [CreateAssetMenu(menuName = "Data/Light Color")]
    public class LightColor : ScriptableObject
    {
        [field:SerializeField]
        public Color[] Colors { get; private set; }
        
        [field:SerializeField]
        public LightType Type { get; private set; }
        
        [field:SerializeField]
        public LightSide Side { get; private set; }
    }
}