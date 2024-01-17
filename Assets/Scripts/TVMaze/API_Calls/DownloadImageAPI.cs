using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadImageAPI : MonoBehaviour, IShowSearch
{
    public delegate void TextureCallback(Texture2D texture);

    public static IEnumerator LoadTexture(string url, TextureCallback callback)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
            callback?.Invoke(null); // Invoke the callback with null in case of an error
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            callback?.Invoke(texture); // Invoke the callback with the loaded texture
        }
    }

}
