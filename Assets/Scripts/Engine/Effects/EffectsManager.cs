using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Engine.Effects
{
    public class EffectsManager : MonoBehaviour
    {
        [SerializeField]
        private FlameEffectDisplay m_flameEffectDisplay;
        
        [SerializeField]
        private SparkEffectDisplay m_sparkEffectDisplay;
        
        [SerializeField]
        private SmokeEffectDisplay m_smokeEffectDisplay;

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
                case EffectType.Spark:
                    m_sparkEffectDisplay.Enable(item.TimeDuration);
                    break;
                case EffectType.Smoke:
                    m_smokeEffectDisplay.Enable(item);
                    break;
            }
        }
    }
}