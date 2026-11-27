using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RetroRecord.General.UI
{
    public class UISlider : Slider
    {
        [Header("Object References")]
        [SerializeField]
        private RectTransform m_rectTransform;
        
        [SerializeField]
        private TMP_Text m_text;
        
        [SerializeField]
        private TMP_Text m_valueText;

        [Header("Scaling")]
        private Vector3 m_defaultScale;

        [SerializeField]
        private Vector3 m_hoverScale = new(1.1f, 1.1f, 1.1f);

        [SerializeField]
        private float m_scaleTime = 0.1f;

        private float m_scaleLerpIndex;
        private Coroutine m_scaleCoroutine;

        [Header("Options")]
        [SerializeField]
        private bool m_shouldScale = true;

        [SerializeField]
        private float m_valueTextMultiplier = 1;
        
        protected override void Awake()
        {
#if UNITY_EDITOR
            if (m_rectTransform == null)
                m_rectTransform = GetComponent<RectTransform>();
            
            if (!Application.isPlaying)
                return;
#endif
            base.Awake();
            if (m_shouldScale)
                m_defaultScale = m_rectTransform.localScale;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (m_shouldScale)
                ChangeScale(true);
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (m_shouldScale)
                ChangeScale(false);
            base.OnPointerExit(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (m_shouldScale)
                ChangeScale(true);
            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if (m_shouldScale)
                ChangeScale(false);
            base.OnDeselect(eventData);
        }

        private void ChangeScale(bool toggle)
        {
            if (!m_shouldScale)
                return;
            if (m_scaleCoroutine != null)
                StopCoroutine(m_scaleCoroutine);
            if (gameObject.activeInHierarchy)
                m_scaleCoroutine = StartCoroutine(LerpScaleCoroutine(toggle));
        }

        private IEnumerator LerpScaleCoroutine(bool toggle)
        {
            Vector3 startScale = m_rectTransform.localScale;
            Vector3 endScale = toggle ? m_hoverScale : m_defaultScale;

            if (m_scaleLerpIndex > 0)
                m_scaleLerpIndex = m_scaleTime - m_scaleLerpIndex;

            for (; m_scaleLerpIndex < m_scaleTime; m_scaleLerpIndex += Time.unscaledDeltaTime)
            {
                m_rectTransform.localScale = Vector3.Lerp(startScale, endScale, m_scaleLerpIndex / m_scaleTime);
                yield return null;
            }

            m_scaleLerpIndex = 0;

            m_rectTransform.localScale = endScale;
        }

        public void SetText(string text)
        {
            m_text.text = text;
        }

        protected override void Set(float input, bool sendCallback = true)
        {
            base.Set(input, sendCallback);
            if (m_valueText == null) return;
            string currentValue = (value * m_valueTextMultiplier).ToString("F0");
            string max = (maxValue * m_valueTextMultiplier).ToString("F0");
            m_valueText.text = $"{currentValue}/{max}";
        }
    }
}