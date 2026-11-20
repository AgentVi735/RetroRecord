using UnityEngine;

namespace RetroRecord.Library.Crates
{
    public class Divider : CrateObject
    {
        public override CrateObjectType Type => CrateObjectType.Divider;
        
        [SerializeField]
        private char m_character;
        
        public void Load(DividerData data, bool startFlipped)
        {
            transform.localRotation = Quaternion.Euler(startFlipped ? m_settings.FlippedRotation : m_settings.DefaultRotation);
            m_character = data.Character;
        }
    }
}