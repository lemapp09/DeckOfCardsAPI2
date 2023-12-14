using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BackgroundFade : MonoBehaviour
{
    public Image uiImage;
    public TextMeshProUGUI textMeshPro;

    public float fadeDuration = 5f;

    private void Start()
    {
        if (uiImage != null)
        {
            StartCoroutine(FadeImage());
        }

        if (textMeshPro != null)
        {
            StartCoroutine(FadeText());
        }
    }

    IEnumerator FadeImage()
    {
        float currentTime = 0;
        Color startColor = uiImage.color;

        while (currentTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, currentTime / fadeDuration);
            uiImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }

        uiImage.color = new Color(startColor.r, startColor.g, startColor.b, 0);
    }

    IEnumerator FadeText()
    {
        float currentTime = 0;
        Color startColor = textMeshPro.color;

        while (currentTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, currentTime / fadeDuration);
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }

        textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, 0);
    }
}