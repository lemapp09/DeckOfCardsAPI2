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

    public void OnSearchButtonClicked()
    {
        string showTitle = showTitleInput.text;
        StartCoroutine(SearchShow(showTitle));
    }

    IEnumerator SearchShow(string showTitle)
    {
        string url = $"https://api.tvmaze.com/search/shows?q={showTitle}";
        ClearResultPanel();

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                List<Root> shows = JsonConvert.DeserializeObject<List<Root>>(jsonResponse);
                DisplayResults(shows);
            }
        }
    }

    void ClearResultPanel()
    {
        foreach (Transform child in resultPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void DisplayResults(List<Root> shows)
    {
        float buttonHeight = 50f; // Height of each button
        float verticalSpacing = 10f; // Space between buttons
        float initialOffset = -30f; // Offset from the top of the panel
        float currentYPosition = initialOffset; // Start from the top

        for (int i = 0; i < shows.Count; i++)
        {
            Show show = shows[i].show;
            Button button = Instantiate(resultButtonPrefab, resultPanel.transform);

            // Set the button text
            button.GetComponentInChildren<TMP_Text>().text = $"{show.name}";

            // Position the button
            RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
            buttonRectTransform.anchoredPosition = new Vector2(0, currentYPosition);

            // Update the position for the next button
            currentYPosition -= (buttonHeight + verticalSpacing);

            // Add click listener
            button.onClick.AddListener(() => DisplayShowDetails(show));
        }
    }


    void DisplayShowDetails(Show show)
    {
        detailText.text = $"Name: {show.name}\n" +
                          $"Genres: {string.Join(", ", show.genres)}\n" +
                          $"Rating: {show.rating?.average}\n" +
                          $"Summary: {show.summary}";

        detailPanel.SetActive(true); // Show the detail panel
    }

    // Include all the nested classes (Country, Externals, Image, Links, etc.) here...
    
    // Generated at https://json2csharp.com/ 
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Country
    {
        public string name { get; set; }
        public string code { get; set; }
        public string timezone { get; set; }
    }

    public class Externals
    {
        public int? tvrage { get; set; }
        public int? thetvdb { get; set; }
        public string imdb { get; set; }
    }

    public class Image
    {
        public string medium { get; set; }
        public string original { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
        public Previousepisode previousepisode { get; set; }
    }

    public class Network
    {
        public int id { get; set; }
        public string name { get; set; }
        public Country country { get; set; }
        public string officialSite { get; set; }
    }

    public class Previousepisode
    {
        public string href { get; set; }
    }

    public class Rating
    {
        public double? average { get; set; }
    }

    public class Root
    {
        public double score { get; set; }
        public Show show { get; set; }
    }

    public class Schedule
    {
        public string time { get; set; }
        public List<string> days { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Show
    {
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

    public class WebChannel
    {
        public int id { get; set; }
        public string name { get; set; }
        public Country country { get; set; }
        public string officialSite { get; set; }
    }


}