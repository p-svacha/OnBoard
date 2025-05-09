using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for creating spinning displays of 3D Gameobject as UI elements
/// </summary>
public static class ObjectPreviewManager
{
    private static List<GameObject> PreviewObjects = new List<GameObject>();
    private static List<GameObject> PreviewCameras = new List<GameObject>();

    public static GameObject ShowToken(RawImage rawImage, Token tokenData, float distance = 3f)
    {
        GameObject previewObject = TokenGenerator.GenerateTokenCopy(tokenData, hidden: false, frozen: true).gameObject;
        return ShowGameObject(rawImage, previewObject, distance);
    }

    public static GameObject ShowGameObject(RawImage rawImage, GameObject previewObject, float distance = 3f)
    {
        // Create render texture
        RenderTexture renderTexture = new RenderTexture(1024, 1024, 16);

        // Create preview camera
        GameObject cameraObject = new GameObject("PreviewCamera");
        float xPos = (100f * PreviewCameras.Count);
        cameraObject.transform.position = new Vector3(xPos, 200f + distance, 0f);
        cameraObject.transform.rotation = Quaternion.Euler(90, 0, 0);

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.clear;
        camera.targetTexture = renderTexture;
        camera.cullingMask = 1 << WorldManager.Layer_PreviewObject;

        PreviewCameras.Add(cameraObject);

        // Create preview object
        previewObject.SetActive(true);
        Rigidbody rb = previewObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        previewObject.layer = WorldManager.Layer_PreviewObject;
        previewObject.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        previewObject.AddComponent<SpinPreview>();
        previewObject.transform.position = new Vector3(xPos, 200f, 0f);
        PreviewObjects.Add(previewObject);

        // Draw rendered texture on raw image
        rawImage.texture = renderTexture;

        return previewObject;
    }

    public static void ClearPreview(GameObject previewObject)
    {
        int index = PreviewObjects.IndexOf(previewObject);

        GameObject.Destroy(PreviewObjects[index]);
        PreviewObjects.RemoveAt(index);

        GameObject.Destroy(PreviewCameras[index]);
        PreviewCameras.RemoveAt(index);
    }
}
