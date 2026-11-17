using UnityEngine;

namespace RetroRecord.Library.Crates
{
    public class Divider : MonoBehaviour
    {
        [SerializeField]
        private char m_character;
        
        public void Load(DividerData data)
        {
            m_character = data.Character;
        }
    }
}