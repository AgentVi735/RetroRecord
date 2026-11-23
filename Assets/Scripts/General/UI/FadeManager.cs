using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRecord.General.UI
{
    public class FadeManager : MonoBehaviour
    {
        [SerializeField]
        private Image fadeImage;

        [SerializeField]
        private float fadeTime;

        public bool IsOn { get; private set; }

        private Coroutine fadeCoroutine;

        [SerializeField]
        private bool startOn = true;

        [SerializeField]
        private Color startColor = Color.black;

        private void Awake()
        {
            fadeImage.color = startColor;
            if (startOn)
                Show(startColor);
        }

        /// <summary>
        /// Start a fade in or out
        /// </summary>
        /// <param name="fadeIn">
        /// Set to true if the fade image should disappear
        /// </param>
        public void StartFade(bool fadeIn) => StartFade(fadeIn, null, Color.black);
        /// <summary>
        /// Start a fade in or out
        /// </summary>
        /// <param name="fadeIn">
        /// Set to true if the fade image should disappear
        /// </param>
        /// <param name="callback">
        /// Method to run after the fade has finished
        /// </param>
        public void StartFade(bool fadeIn, Action callback) => StartFade(fadeIn, callback, Color.black);

        /// <summary>
        /// Start a fade in or out
        /// </summary>
        /// <param name="fadeIn">
        /// Set to true if the fade image should disappear
        /// </param>
        /// <param name="callback">
        /// Method to run after the fade has finished
        /// </param>
        /// <param name="fadeColor">
        /// Colour that the fade should be
        /// </param>
        public void StartFade(bool fadeIn, Action callback, Color fadeColor)
        {
            if (!gameObject.activeSelf || !IsOn)
                Show(fadeIn ? Color.clear : fadeColor);
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(Fade(fadeIn, callback, fadeColor));
        }

        /// <summary>
        /// Show the fade image
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            fadeImage.gameObject.SetActive(true);
            IsOn = true;
        }
        
        /// <summary>
        /// Show the fade image
        /// </summary>
        /// <param name="color">
        /// Colour that the fade image should be
        /// </param>
        public void Show(Color color)
        {
            fadeImage.color = color;
            gameObject.SetActive(true);
            fadeImage.gameObject.SetActive(true);
            IsOn = true;
        }

        /// <summary>
        /// Hide the fade image
        /// </summary>
        public void Hide()
        {
            fadeImage.gameObject.SetActive(false);
            IsOn = false;
        }

        private IEnumerator Fade(bool fadeIn, Action callback, Color fadeColor)
        {
            IsOn = !fadeIn;
            Color fadeStartColor = fadeIn ? fadeColor : Color.clear;
            Color fadeEndColor = fadeIn ? Color.clear : fadeColor;
            fadeImage.color = fadeStartColor;
            fadeImage.gameObject.SetActive(true);

            for (float i = 0; i <= fadeTime + Time.deltaTime; i += Time.deltaTime)
            {
                if (i > fadeTime) i = fadeTime;

                float fillAmount = i / fadeTime;

                fadeImage.color = Color.Lerp(fadeStartColor, fadeEndColor, fillAmount);

                yield return null;
            }

            fadeImage.color = fadeEndColor;
            if (fadeIn)
                fadeImage.gameObject.SetActive(false);

            callback?.Invoke();
        }
    }
}