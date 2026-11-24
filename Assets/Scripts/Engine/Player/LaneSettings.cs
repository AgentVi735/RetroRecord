using UnityEngine;

namespace RetroRecord.Engine.Player
{
    [CreateAssetMenu(menuName = "Settings/Lane")]
    public class LaneSettings : ScriptableObject
    {
        [System.Serializable]
        public class LaneData
        {
            [field: SerializeField]
            public Material PrimaryMaterial { get; private set; }
            
            [field: SerializeField]
            public Material SecondaryMaterial { get; private set; }

            [field: SerializeField]
            public Color DefaultColor { get; private set; }

            [field: SerializeField]
            public Color SecondaryColor { get; private set; }
        }

        [field: SerializeField]
        public LaneData LaneSprite { get; private set; }

        [field: SerializeField]
        public LaneData StrikelineSprite { get; private set; }
        
        [field: SerializeField]
        public LaneData StrikelineHitSprite { get; private set; }
    }
}