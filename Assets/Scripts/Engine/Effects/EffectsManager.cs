using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Engine.Effects
{
    public class EffectsManager : MonoBehaviour
    {
        [SerializeField]
        private FlameEffectDisplay m_flameEffectDisplay;

        public void ShowEffect(EffectItem item)
        {
            switch (item.EffectType)
            {
                default:
                case EffectType.None:
                    Debug.LogWarning($"No effect set at tick {item.Tick} ({item.Time}s)");
                    break;
                case EffectType.Flame:
                    m_flameEffectDisplay.Enable(item.TimeDuration);
                    break;
            }
        }
    }
}