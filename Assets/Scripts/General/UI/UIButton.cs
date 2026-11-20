using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RetroRecord.General.UI
{
    public class UIButton : Button
    {
        [Header("Object References")]
        [SerializeField]
        private RectTransform m_rectTransform;

        [Header("Scale")]
        private Vector3 m_defaultScale;

        [SerializeField]
        private Vector3 m_hoverScale = Vector3.one;

        [SerializeField]
        private float m_scaleTime;

        private float m_scaleLerpIndex;
        private Coroutine m_scaleCoroutine;

        [Header("Options")]
        [SerializeField]
        private bool m_shouldScale = true;
        
        private bool CanPress() => IsActive() && IsInteractable();

        protected override void Awake()
        {
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

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            Press();

            if (!CanPress())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        private void Press()
        {
            if (!CanPress())
                return;

            onClick.Invoke();

            if (m_shouldScale)
                ChangeScale(false);
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
    }
}