using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace TVMaze.API_Calls
{
    public class PeopleSearchAPI : IPeopleSearch
    {
        // Search Buttons caused the initial contact with the search API.
        // if there are errors, they are recorded, otherwise result buttons are made
        public static IEnumerator SearchPeople(string personName, Action<List<IPeopleSearch.PeopleRoot>> callback) {
            string url = $"https://api.tvmaze.com/search/people?q={personName}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                    webRequest.result == UnityWebRequest.Result.ProtocolError) {
                    Debug.LogError("Error: " + webRequest.error);
                }  else  {
                    string jsonResponse = webRequest.downloadHandler.text;
                    List<IPeopleSearch.PeopleRoot> people = 
                        JsonConvert.DeserializeObject<List<IPeopleSearch.PeopleRoot>>(jsonResponse);
                    callback?.Invoke(people);
                }
            }
        }


    }
}