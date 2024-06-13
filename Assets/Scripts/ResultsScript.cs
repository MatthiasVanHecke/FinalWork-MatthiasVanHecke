using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultsScript : MonoBehaviour
{
    public Image photoImage;
    public TextMeshProUGUI resultText;

    void Start()
    {
        Debug.Log("Start method called in ResultsScript");

        if (photoImage == null)
        {
            Debug.LogError("photoImage is not assigned in the inspector.");
            return;
        }

        if (resultText == null)
        {
            Debug.LogError("resultText is not assigned in the inspector.");
            return;
        }

        string filePath = PlayerPrefs.GetString("photo");
        Debug.Log("Photo file path retrieved: " + filePath);
        
        if (System.IO.File.Exists(filePath))
        {
            Debug.Log("Photo file found at path: " + filePath);
            byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            photoImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError("Photo file not found at path: " + filePath);
            return;
        }

        // Check if the GoogleCloudVisionScript component is attached to the same GameObject
        var googleVisionScript = GetComponent<GoogleCloudVisionScript>();
        if (googleVisionScript == null)
        {
            Debug.LogError("GoogleCloudVisionScript is not attached to the GameObject.");
            return;
        }

        Debug.Log("Starting image analysis with GoogleCloudVisionScript");
        // Start the image analysis
        StartCoroutine(googleVisionScript.AnalyzeImage(filePath));
    }

    public void ProcessResults(string jsonResults)
    {
        Debug.Log("ProcessResults called with jsonResults: " + jsonResults);

        GoogleVisionResponse results = JsonUtility.FromJson<GoogleVisionResponse>(jsonResults);

        resultText.text = ""; // Clear the text first
        for (int i = 0; i < Mathf.Min(3, results.responses[0].labelAnnotations.Length); i++)
        {
            string label = results.responses[0].labelAnnotations[i].description;
            float score = results.responses[0].labelAnnotations[i].score;
            resultText.text += label + ": " + (score * 100).ToString("F0") + "%\n";
        }

        Debug.Log("Results text updated: " + resultText.text);
    }

    public void GoBack()
    {
        Debug.Log("GoBack was called, loading CameraScene");
        SceneManager.LoadScene("CameraScene");
    }

    [System.Serializable]
    public class LabelAnnotation
    {
        public string description;
        public float score;
    }

    [System.Serializable]
    public class Response
    {
        public LabelAnnotation[] labelAnnotations;
    }

    [System.Serializable]
    public class GoogleVisionResponse
    {
        public Response[] responses;
    }
}