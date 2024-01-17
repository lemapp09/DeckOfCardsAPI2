using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadImageAPI : MonoBehaviour
{
    public static Texture2D tempTexture;

    public static IEnumerator LoadTexture(string url, Action<Texture2D> callback)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.LogError("Error: " + www.error);
        }  else   {
            tempTexture = DownloadHandlerTexture.GetContent(www);
            callback?.Invoke(tempTexture);
        }
    }
}
