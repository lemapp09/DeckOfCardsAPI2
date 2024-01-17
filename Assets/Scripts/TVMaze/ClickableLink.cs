using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Add IPointerClickHandler interface to let Unity know you want to
// catch and handle clicks (or taps on Mobile)
public class ClickableLink : MonoBehaviour, IPointerClickHandler
{
    // URLs to open when links clicked
    public string LinkUrl ;

    // Callback for handling clicks.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(LinkUrl))
        {
            // Let's see that web page!
            Application.OpenURL(LinkUrl);
        }
    }
}