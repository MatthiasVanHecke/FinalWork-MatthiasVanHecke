// AIzaSyC_Eo_rsiIVwv-vEGNE4WzU6fs1veVNCXc

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleCloudVisionScript : MonoBehaviour
{
    public IEnumerator AnalyzeImage(string imagePath)
    {
        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
        string base64Image = System.Convert.ToBase64String(imageBytes);

        var visionRequest = new GoogleVisionRequest()
        {
            requests = new Request[]
            {
                new Request()
                {
                    image = new Image() { content = base64Image },
                    features = new Feature[]
                    {
                        new Feature() { type = "LABEL_DETECTION", maxResults = 3 }
                    }
                }
            }
        };

        string jsonRequest = JsonUtility.ToJson(visionRequest);

        using (UnityWebRequest webRequest = new UnityWebRequest("https://vision.googleapis.com/v1/images:annotate?key=AIzaSyC_Eo_rsiIVwv-vEGNE4WzU6fs1veVNCXc", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Sending request to Google Cloud Vision API");

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Received response: " + webRequest.downloadHandler.text);
                ResultsScript resultsScript = FindObjectOfType<ResultsScript>();
                if (resultsScript != null)
                {
                    resultsScript.ProcessResults(webRequest.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("ResultsScript not found");
                }
            }
        }
    }

    [System.Serializable]
    public class Image
    {
        public string content;
    }

    [System.Serializable]
    public class Feature
    {
        public string type;
        public int maxResults;
    }

    [System.Serializable]
    public class Request
    {
        public Image image;
        public Feature[] features;
    }

    [System.Serializable]
    public class GoogleVisionRequest
    {
        public Request[] requests;
    }
}
