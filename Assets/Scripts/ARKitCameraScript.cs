using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class ARKitCameraScript : MonoBehaviour
{
    public ARCameraManager arCameraManager;

    void Start()
    {
        if (arCameraManager == null)
        {
            Debug.LogError("ARCameraManager is not assigned.");
            return;
        }

        StartCoroutine(StartARSession());
    }

    IEnumerator StartARSession()
    {
        if (ARSession.state != ARSessionState.SessionInitializing && ARSession.state != ARSessionState.SessionTracking)
        {
            yield return ARSession.CheckAvailability();
            ARSession.stateChanged += (ARSessionStateChangedEventArgs args) =>
            {
                if (args.state == ARSessionState.SessionInitializing || args.state == ARSessionState.SessionTracking)
                {
                    Debug.Log("AR session started");
                }
            };
            yield return new WaitForSeconds(1);
        }
    }

    public void TakePhoto()
    {
        Debug.Log("TakePhoto called");
        StartCoroutine(CapturePhoto());
    }

    IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        if (arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            var texture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
            XRCpuImage.ConversionParams conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, image.width, image.height),
                outputDimensions = new Vector2Int(image.width, image.height),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.MirrorY
            };

            var rawTextureData = texture.GetRawTextureData<byte>();
            image.Convert(conversionParams, rawTextureData);
            texture.Apply();

            string filePath = System.IO.Path.Combine(Application.persistentDataPath, "photo.png");
            System.IO.File.WriteAllBytes(filePath, texture.EncodeToPNG());

            PlayerPrefs.SetString("photo", filePath);
            PlayerPrefs.Save();

            image.Dispose();

            SceneManager.LoadScene("ResultScene");
        }
        else
        {
            Debug.LogError("Failed to acquire AR camera image.");
        }
    }
}