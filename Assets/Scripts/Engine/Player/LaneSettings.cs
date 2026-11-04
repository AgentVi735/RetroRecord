using UnityEngine;

namespace RetroRecord.Engine.Player
{
    [CreateAssetMenu(menuName = "Settings/Lane")]
    public class LaneSettings : ScriptableObject
    {
        [System.Serializable]
        public class SpriteData
        {
            [field: SerializeField]
            public Sprite Sprite { get; private set; }

            [field: SerializeField]
            public Color DefaultColor { get; private set; }

            [field: SerializeField]
            public Color SecondaryColor { get; private set; }
        }

        [field: SerializeField]
        public SpriteData LaneSprite { get; private set; }

        [field: SerializeField]
        public SpriteData StrikelineSprite { get; private set; }
        
        [field: SerializeField]
        public SpriteData StrikelineHitSprite { get; private set; }
    }
}