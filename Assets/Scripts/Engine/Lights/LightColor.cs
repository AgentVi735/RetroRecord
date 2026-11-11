using UnityEngine;

namespace RetroRecord.Engine.Lights
{
    [CreateAssetMenu(menuName = "Data/Light Color")]
    public class LightColor : ScriptableObject
    {
        [field:SerializeField]
        public Color[] Colors { get; private set; }
    }
}