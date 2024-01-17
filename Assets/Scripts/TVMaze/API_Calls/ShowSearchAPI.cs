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
                List<IShowSearch.ShowSearchRoot> shows = JsonConvert.DeserializeObject<List<IShowSearch.ShowSearchRoot>>(jsonResponse);
                callback?.Invoke(shows);
            }
        }
    }
}
