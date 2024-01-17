using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ShowSearchAPI : IShowSearch
{
    // Search Buttons caused the initial contact with the search API.
    // if there are errors, they are recorded, otherwise result buttons are made
    public static IEnumerator SearchShow(string showTitle, Action<List<IShowSearch.ShowSearchRoot>> callback) {
        string url = $"https://api.tvmaze.com/search/shows?q={showTitle}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("Error: " + webRequest.error);
            }  else  {
                string jsonResponse = webRequest.downloadHandler.text;
                List<IShowSearch.ShowSearchRoot> shows = 
                    JsonConvert.DeserializeObject<List<IShowSearch.ShowSearchRoot>>(jsonResponse);
                callback?.Invoke(shows);
            }
        }
    }
    
    // When one of the results buttons is clicked, this method is called.
    // There is no need to call the API again, all of these data was included 
    // in the initial search.
    public static (string, string, string) SearchShowDetails(Dictionary<int, IShowSearch.Show> showsDictionary,
        int showId)
    {
        // Get the show from the dictionary. 
        // If the show is not found, an error is logged. 
        // Note: The show ID is retrieved from the initial search
        // unlikely not to be found
        if (!showsDictionary.TryGetValue(showId, out IShowSearch.Show show)) {
            Debug.LogError("Show not found.");
            return ("", "", "");
        }
        // Remove <p> and </p> tags from the summary
        string cleanedSummary = show.summary.Replace("<p>", "").Replace("</p>", "");
        
        // The supplied data is displayed. 
        // Note the special technique to display all of the Genres.
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if (!String.IsNullOrEmpty(show.officialSite)) {
            AppendLineIfNotEmpty(sb, "Official Site", "<link=\"MyLinkID1\">" + show.officialSite + "</link>");
        }

        AppendLineIfNotEmpty(sb, "Name", show.name);
        AppendLineIfNotEmpty(sb, "Type", show.type);
        AppendLineIfNotEmpty(sb, "Language", show.language);
        AppendLineIfNotEmpty(sb, "Genres", show.genres != null ? string.Join(", ", show.genres) : "");
        AppendLineIfNotEmpty(sb, "Status", show.status);
        AppendLineIfNotEmpty(sb, "Runtime", show.runtime.HasValue ? $"{show.runtime.Value} minutes" : "");
        AppendLineIfNotEmpty(sb, "Premiered", show.premiered);
        AppendLineIfNotEmpty(sb, "Rating",
            (bool)show.rating?.average.HasValue ? show.rating.average.Value.ToString() : "");
        AppendLineIfNotEmpty(sb, "Network", show.network?.name);
        AppendLineIfNotEmpty(sb, "Country", show.network?.country?.name);
        AppendLineIfNotEmpty(sb, "Summary", cleanedSummary); // Assuming cleanedSummary is already defined

        return (sb.ToString(), show.image?.medium, show.officialSite);
    }

    // This method adds an item to the display if it contains data
    static void AppendLineIfNotEmpty(System.Text.StringBuilder sb, string label, string value) {
        if (!string.IsNullOrEmpty(value)) {
            // Check if the value starts with "http" (case insensitive) and format it with <link> tags
            if (value.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase)) {
                value = $"<color=\"blue\"><link>{value}</color></link>";
            }
            sb.AppendLine($"<color=\"black\"><b>{label}</b></color>: {value}"); // Append the label and value to the display
        }
    }
    
}
