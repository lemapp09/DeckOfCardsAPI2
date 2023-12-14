using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BackgroundFade : MonoBehaviour
{
    public Image uiImage, titleGraphic;
    // public TextMeshProUGUI textMeshPro;

    public float fadeDuration = 5f;

    private void Start()
    {
        if (uiImage != null && titleGraphic != null)
        {
            StartCoroutine(FadeImage());
        }


    }

    IEnumerator FadeImage()
    {
        float currentTime = 0;
        Color startColor = uiImage.color;
        Color startColor2 = titleGraphic.color;

        while (currentTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, currentTime / fadeDuration);
            uiImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            titleGraphic.color = new Color(startColor2.r, startColor2.g, startColor2.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }

        uiImage.color = new Color(startColor.r, startColor.g, startColor.b, 0);
        titleGraphic.color = new Color(startColor2.r, startColor2.g, startColor2.b, 0);
    }
    
}