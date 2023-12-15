using System;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class TVMazeAPISearch : MonoBehaviour
{
    public TMP_InputField showTitleInput;
    public GameObject resultPanel; // Panel to hold result buttons
    public GameObject detailPanel; // Panel to display show details
    public TMP_Text detailText; // Text element to display details
    public Button resultButtonPrefab; // Prefab for result buttons
    public RawImage tvshowImage; // Assign this in the Inspector
    private Dictionary<int, Show> showsDictionary = new Dictionary<int, Show>();

    public void OnSearchButtonClicked() {
        string showTitle = showTitleInput.text;
        StartCoroutine(SearchShow(showTitle));
    }

    // Search Buttons caused the initial contact with the search API.
    // if there are errors, they are recorded, otherwise result buttons are made
    IEnumerator SearchShow(string showTitle) {
        string url = $"https://api.tvmaze.com/search/shows?q={showTitle}";
        ClearResultPanel();

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("Error: " + webRequest.error);
            }  else  {
                string jsonResponse = webRequest.downloadHandler.text;
                List<Root> shows = JsonConvert.DeserializeObject<List<Root>>(jsonResponse);
                DisplayResults(shows);
            }
        }
    }

    // Previous search results are removed
    void ClearResultPanel() {
        foreach (Transform child in resultPanel.transform) {
            Destroy(child.gameObject);
        }
    }

    // The results of the initial search are displayed as buttons 
    void DisplayResults(List<Root> shows) {
        // To prevent buttons from stacking on each other, this staggers their placement.
        float buttonHeight = 50f; // Height of each button
        float verticalSpacing = 10f; // Space between buttons
        float initialOffset = -30f; // Offset from the top of the panel
        float currentYPosition = initialOffset; // Start from the top

        // Each show is made into a button and placed on the results panel (left side)
        for (int i = 0; i < shows.Count; i++) {
            Show show = shows[i].show;
            showsDictionary[show.id] = show; // Store the show in the dictionary
            Button button = Instantiate(resultButtonPrefab, resultPanel.transform);

            // Set the button text
            button.GetComponentInChildren<TMP_Text>().text = $"{show.name}";

            // Position the button
            RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
            buttonRectTransform.anchoredPosition = new Vector2(0, currentYPosition);

            // Update the position for the next button
            currentYPosition -= (buttonHeight + verticalSpacing);

            // Add click listener
            button.onClick.AddListener(() => DisplayShowDetails(show.id));

        }
    }


    // When one of the results buttons is clicked, this method is called.
    // There is no need to call the API again, all of these data was included 
    // in the initial search.
    void DisplayShowDetails(int showId) {
        if (!showsDictionary.TryGetValue(showId, out Show show)) {
            Debug.LogError("Show not found.");
            return;
        }
        // Remove <p> and </p> tags from the summary
        string cleanedSummary = show.summary.Replace("<p>", "").Replace("</p>", "");

        // This coding grabs the image from the Internet. 
        // Note: Two sizes of the image are provided. This example uses the smaller, medium sized version.
        if (!string.IsNullOrEmpty(show.image?.medium)) {
            StartCoroutine(DownloadImage(show.image.medium));
        }  else {
            // Image Space made blank if there is no image
            tvshowImage.texture = null; // or set to a default texture
        }
        // The supplied data is displayed. 
        // Note the special technique to display all of the Genres.
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

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
        AppendLineIfNotEmpty(sb, "Official Site", show.officialSite);
        AppendLineIfNotEmpty(sb, "Summary", cleanedSummary); // Assuming cleanedSummary is already defined

        detailText.text = sb.ToString();

        detailPanel.SetActive(true); // Show the detail panel
    }
    
    IEnumerator DownloadImage(string MediaUrl) {
        // This Method makes a call out on the web to grab an image
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        // wait until the image is received
        yield return request.SendWebRequest();

        // Check for errors. If any, they are recorded in the Error Log and the display image is made blank
        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError) {
            Debug.Log(request.error);
            tvshowImage.texture = null; // or set to a default texture
        }  else  {
            // If there is an image, it is resized to fit in the space on the screen
            Texture2D tempTexture = DownloadHandlerTexture.GetContent(request);
            float tempWidth = tempTexture.width;
            float tempHeight = tempTexture.height;
            float ratio = tempWidth / tempHeight;
            if (tempWidth > tempHeight)
            {
                tempWidth = 435;
                tempHeight = 435 / ratio;
            }
            else
            {
                tempHeight = 435;
                tempWidth = 435 * ratio;
            }
            tvshowImage.rectTransform.sizeDelta = new Vector2(tempWidth, tempHeight);
            // Image is assigned to the space on the screen
            tvshowImage.texture = tempTexture;
        }
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
    
    // Below is the coding used to Parse the API data into something useful.
    //
    // Generated at https://json2csharp.com/ 
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Country {
        public string name { get; set; }
        public string code { get; set; }
        public string timezone { get; set; }
    }

    public class Externals {
        public int? tvrage { get; set; }
        public int? thetvdb { get; set; }
        public string imdb { get; set; }
    }

    public class Image {
        public string medium { get; set; }
        public string original { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
        public Previousepisode previousepisode { get; set; }
    }

    public class Network {
        public int id { get; set; }
        public string name { get; set; }
        public Country country { get; set; }
        public string officialSite { get; set; }
    }

    public class Previousepisode {
        public string href { get; set; }
    }

    public class Rating {
        public double? average { get; set; }
    }

    public class Root {
        public double score { get; set; }
        public Show show { get; set; }
    }

    public class Schedule {
        public string time { get; set; }
        public List<string> days { get; set; }
    }

    public class Self {
        public string href { get; set; }
    }

    public class Show {
        public int id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string language { get; set; }
        public List<string> genres { get; set; }
        public string status { get; set; }
        public int? runtime { get; set; }
        public int? averageRuntime { get; set; }
        public string premiered { get; set; }
        public string ended { get; set; }
        public string officialSite { get; set; }
        public Schedule schedule { get; set; }
        public Rating rating { get; set; }
        public int weight { get; set; }
        public Network network { get; set; }
        public WebChannel webChannel { get; set; }
        public object dvdCountry { get; set; }
        public Externals externals { get; set; }
        public Image image { get; set; }
        public string summary { get; set; }
        public int updated { get; set; }
        public Links _links { get; set; }
    }

    public class WebChannel {
        public int id { get; set; }
        public string name { get; set; }
        public Country country { get; set; }
        public string officialSite { get; set; }
    }
}