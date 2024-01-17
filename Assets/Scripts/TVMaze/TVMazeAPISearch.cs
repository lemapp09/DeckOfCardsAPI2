using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Heat;
using Unity.VisualScripting;

public class TVMazeAPISearch : MonoBehaviour
{
    #region Variables
    public float maxImageSizing = 800;
    public TMP_InputField showTitleInput;
    [SerializeField] private PanelManager searchPanels; // 0 = results, 1 = details
    public GameObject resultContent; // content area to hold result buttons
    public GameObject detailContent; //content area to display show details
    public TMP_Text detailText; // Text element to display details
    public ButtonManager resultButtonPrefab; // Prefab for result buttons
    public RawImage tvshowImage; // Assign this in the Inspector
    private Dictionary<int, IShowSearch.Show> showsDictionary = new Dictionary<int, IShowSearch.Show>();
    #endregion
    
    void OnEnable() {        
        resultContent.SetActive(false);
        searchPanels.gameObject.SetActive(false);
        StartCoroutine(SelectInputField());
    }

    public void OnSearchButtonClicked() {       
        searchPanels.gameObject.SetActive(true);
        ClearResultPanel();
        string showTitle = showTitleInput.text;
        StartCoroutine(ShowSearchAPI.SearchShow(showTitle, result =>
        {
            DisplayResults(result);
        }));
    }

    // Previous search results are removed
    void ClearResultPanel() {
        foreach (Transform child in resultContent.transform) {
            Destroy(child.gameObject);
        }
    }

    // The results of the initial search are displayed as buttons 
    void DisplayResults(List<IShowSearch.ShowSearchRoot> shows) {
        // To prevent buttons from stacking on each other, this staggers their placement.
        float buttonHeight = 50f; // Height of each button
        float verticalSpacing = 10f; // Space between buttons
        float initialOffset = -30f; // Offset from the top of the panel
        float currentYPosition = initialOffset; // Start from the top

        // Each show is made into a button and placed on the results panel (left side)
        for (int i = 0; i < shows.Count; i++) {
            IShowSearch.Show show = shows[i].show;
            showsDictionary[show.id] = show; // Store the show in the dictionary
            ButtonManager button = Instantiate(resultButtonPrefab, resultContent.transform);

            // Set the button text
            button.buttonText = $"{show.name}";

            // Position the button
            RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
            buttonRectTransform.anchoredPosition = new Vector2(0, currentYPosition);

            // Update the position for the next button
            currentYPosition -= (buttonHeight + verticalSpacing);

            // Add click listener
            button.onClick.AddListener(() => DisplayShowDetails(show.id));
        }
        searchPanels.GameObject().SetActive(true); // Switch to the details panel
        resultContent.SetActive(true); // Show the results panel
    }


    // When one of the results buttons is clicked, this method is called.
    // There is no need to call the API again, all of these data was included 
    // in the initial search.
    void DisplayShowDetails(int showId) {
        // Get the show from the dictionary. 
        // If the show is not found, an error is logged. 
        // Note: The show ID is retrieved from the initial search
        // unlikely not to be found
        if (!showsDictionary.TryGetValue(showId, out IShowSearch.Show show)) {
            Debug.LogError("Show not found.");
            return;
        }
        // Remove <p> and </p> tags from the summary
        string cleanedSummary = show.summary.Replace("<p>", "").Replace("</p>", "");
        
        // populate the detail panel with the show image
        // Note: Two sizes of the image are provided. This example uses the smaller, medium sized version.
        if (show != null && show.image != null && !String.IsNullOrEmpty(show.image.medium)) {
            PopulateImage(show.image.medium);
        }
        
        // The supplied data is displayed. 
        // Note the special technique to display all of the Genres.
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if(!String.IsNullOrEmpty(show.officialSite)) {
            AppendLineIfNotEmpty(sb, "Official Site", "<link=\"MyLinkID1\">" + show.officialSite + "</link>");
        }
        AppendLineIfNotEmpty(sb, "Name", show.name);
        AppendLineIfNotEmpty(sb, "Type", show.type);
        AppendLineIfNotEmpty(sb, "Language", show.language);
        AppendLineIfNotEmpty(sb, "Genres", show.genres != null ? string.Join(", ", show.genres) : "");
        AppendLineIfNotEmpty(sb, "Status", show.status);
        AppendLineIfNotEmpty(sb, "Runtime", show.runtime.HasValue ? $"{show.runtime.Value} minutes" : "");
        AppendLineIfNotEmpty(sb, "Premiered", show.premiered);
        AppendLineIfNotEmpty(sb, "Rating", (bool)show.rating?.average.HasValue ? show.rating.average.Value.ToString() : "");
        AppendLineIfNotEmpty(sb, "Network", show.network?.name);
        AppendLineIfNotEmpty(sb, "Country", show.network?.country?.name);
        AppendLineIfNotEmpty(sb, "Summary", cleanedSummary); // Assuming cleanedSummary is already defined

        detailText.text = sb.ToString();
        if (show.officialSite != null) {
            detailText.GetComponent<ClickableLink>().LinkUrl = show.officialSite;
            tvshowImage.GetComponent<ClickableLink>().LinkUrl = show.officialSite;
        } else {
            detailText.GetComponent<ClickableLink>().LinkUrl = "";
            tvshowImage.GetComponent<ClickableLink>().LinkUrl = "";
        }
        searchPanels.OpenPanelByIndex(1); // Show the detail panel
    }

    private void PopulateImage(string imageUrl)
    { 
        // This coding grabs the image from the Internet. 
        if (!string.IsNullOrEmpty(imageUrl)) {
            StartCoroutine(DownloadImageAPI.LoadTexture(imageUrl, HandleTextureLoaded));
        }  else {
            // Image Space made blank if there is no image
            tvshowImage.texture = null; // or set to a default texture
        }
    }
    
    void HandleTextureLoaded(Texture2D loadedTexture) {
        if (loadedTexture != null) {
            DisplayImage(loadedTexture);
        } else {
            // if image load fails, display no image
            tvshowImage.texture = null;
        }
    }

    private void DisplayImage(Texture2D tempTexture) {
        // If there is an image, it is resized to fit in the space on the screen
        float tempWidth = tempTexture.width;
        float tempHeight = tempTexture.height;
        float ratio = tempWidth / tempHeight;
        if (tempWidth > tempHeight)
        {
            tempWidth = maxImageSizing;
            tempHeight = maxImageSizing / ratio;
        }
        else
        {
            tempHeight = maxImageSizing;
            tempWidth = maxImageSizing * ratio;
        }
        tvshowImage.rectTransform.sizeDelta = new Vector2(tempWidth, tempHeight);
        // Image is assigned to the space on the screen
        tvshowImage.texture = tempTexture;
    }
    
    // This method adds an item to the display if it contains data
    private void AppendLineIfNotEmpty(System.Text.StringBuilder sb, string label, string value) {
        if (!string.IsNullOrEmpty(value)) {
            // Check if the value starts with "http" (case insensitive) and format it with <link> tags
            if (value.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase)) {
                value = $"<color=\"blue\"><link>{value}</color></link>";
            }
            sb.AppendLine($"<color=\"black\"><b>{label}</b></color>: {value}"); // Append the label and value to the display
        }
    }
    
    IEnumerator SelectInputField()
    {
        yield return new WaitForEndOfFrame();
        showTitleInput.ActivateInputField();
    }
}