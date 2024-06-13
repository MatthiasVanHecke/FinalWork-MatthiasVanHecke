using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour
{
    private WebCamTexture webCamTexture;

    void Start()
    {
        webCamTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = webCamTexture;
        
        if (webCamTexture != null)
        {
            webCamTexture.Play();
            Debug.Log("WebCamTexture started successfully");
        }
        else
        {
            Debug.LogError("Failed to start WebCamTexture");
        }
    }

    public void TakePhoto()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
            photo.SetPixels(webCamTexture.GetPixels());
            photo.Apply();

            // Save the photo to the device and get the file path
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, "photo.png");
            System.IO.File.WriteAllBytes(filePath, photo.EncodeToPNG());

            // Pass the file path to the next scene
            PlayerPrefs.SetString("photo", filePath);

            Debug.Log("TakePhoto was called, loading ResultsScene");
            SceneManager.LoadScene("ResultsScene");
        }
        else
        {
            Debug.LogError("WebCamTexture is not playing. Cannot take photo.");
        }
    }
}