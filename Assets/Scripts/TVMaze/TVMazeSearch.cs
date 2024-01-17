using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Michsky.UI.Heat;
using TVMaze.API_Calls;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class TVMazeSearch : MonoBehaviour
{
    #region Variables
    [SerializeField] private HorizontalSelector searchSelector; // 0 = shows, 1 = people, 2 = episodes
    [FormerlySerializedAs("showTitleInput")] [SerializeField] private TMP_InputField SearchTermInput;
    [SerializeField] private Texture2D defaultImage;
    [SerializeField] private float maxImageSizing = 625;
    [SerializeField] private PanelManager searchPanels; // 0 = results, 1 = details
    [SerializeField] private GameObject resultContent; // content area to hold result buttons
    [SerializeField] private TMP_Text detailText; // Text element to display details
    [SerializeField] private ButtonManager resultButtonPrefab; // Prefab for result buttons
    [SerializeField] private RawImage tvshowImage; // Assign this in the Inspector
    private Dictionary<int, IShowSearch.Show> showsDictionary = new Dictionary<int, IShowSearch.Show>();
    private Dictionary<int, IPeopleSearch.Person> peopleDictionary = new Dictionary<int, IPeopleSearch.Person>();
    // To prevent buttons from stacking on each other, this staggers their placement.
    private float buttonHeight = 50f; // Height of each button
    private float verticalSpacing = 10f; // Space between buttons
    private float initialOffset = -30f; // Offset from the top of the panel
    private float currentYPosition; // Start from the top
    #endregion
    
    void OnEnable() {        
        resultContent.SetActive(false);
        searchPanels.gameObject.SetActive(false);
    }

    public void OnSearchButtonClicked() {       
        searchPanels.gameObject.SetActive(true);
        ClearResultPanel();
        switch (searchSelector.index)
        {
            case 0:
                StartCoroutine(ShowSearchAPI.SearchShow(SearchTermInput.text, result =>
                {
                    DisplayShowResults(result);
                }));
                break;
            case 1:
                StartCoroutine(PeopleSearchAPI.SearchPeople(SearchTermInput.text, result =>
                {
                    DisplayPeopleResults(result);
                }));
                break;
            case 2:
                break;
            default:
                break;
        }
    }

    // Previous search results are removed
    void ClearResultPanel() {
        foreach (Transform child in resultContent.transform) {
            Destroy(child.gameObject);
        }
    }

    // The results of the initial show search are displayed as buttons 
    void DisplayShowResults(List<IShowSearch.ShowSearchRoot> shows) {
        currentYPosition = initialOffset; // Reset the position
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

    // The results of the initial people search are displayed as buttons 
    void DisplayPeopleResults(List<IPeopleSearch.PeopleRoot> people) {
        currentYPosition = initialOffset; // Reset the position
        // Each person is made into a button and placed on the results panel
        for (int i = 0; i < people.Count; i++) {
            IPeopleSearch.Person person = people[i].person;
            peopleDictionary[person.id] = person; // Store the show in the dictionary
            ButtonManager button = Instantiate(resultButtonPrefab, resultContent.transform);
            
            button.buttonText = $"{person.name}";  // Set the button text
            
            RectTransform buttonRectTransform = button.GetComponent<RectTransform>(); // Position the button
            buttonRectTransform.anchoredPosition = new Vector2(0, currentYPosition);
            
            currentYPosition -= (buttonHeight + verticalSpacing); // Update the position for the next button
            
            button.onClick.AddListener(() => DisplayShowDetails(person.id));// Add click listener
        }
        searchPanels.GameObject().SetActive(true); // Switch to the details panel
        resultContent.SetActive(true); // Show the results panel
    }
    
    // When one of the results buttons is clicked, this method is called.
    // There is no need to call the API again, all of these data was included 
    // in the initial search.
    void DisplayShowDetails(int showId) {
        var (str1, str2, str3) = ShowSearchAPI.SearchShowDetails(showsDictionary, showId);
        detailText.text = str1;
        if (!string.IsNullOrEmpty(str2)){ PopulateImage(str2); }
        else { DisplayImage(defaultImage); }

        if (str3 != null) {
            detailText.GetComponent<ClickableLink>().LinkUrl = str3;
            tvshowImage.GetComponent<ClickableLink>().LinkUrl = str3;
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
            StartCoroutine( DownloadImageAPI.LoadTexture(imageUrl, tempTexture => {
                DisplayImage(tempTexture); // Display the image in the space on the screen.;
            }));
        }  else {
            DisplayImage(defaultImage); // Image Space made blank if there is no image
        }
    }

    private void DisplayImage(Texture2D tempTexture) {
        // If there is an image, it is resized to fit in the space on the screen
        float tempWidth = tempTexture.width;
        float tempHeight = tempTexture.height;
        float ratio = tempWidth / tempHeight;
        if (tempWidth > tempHeight) {
            tempWidth = maxImageSizing;
            tempHeight = maxImageSizing / ratio;
        } else {
            tempHeight = maxImageSizing;
            tempWidth = maxImageSizing * ratio;
        }
        tvshowImage.rectTransform.sizeDelta = new Vector2(tempWidth, tempHeight);
        // Image is assigned to the space on the screen
        tvshowImage.texture = tempTexture;
    }
}