using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageController : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private GameObject arSceneRoot;
    
    private bool scenePlaced = false;

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        if (scenePlaced) return;

        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            PlaceScene(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
                PlaceScene(trackedImage);
        }
    }

void PlaceScene(ARTrackedImage trackedImage)
{
    if (scenePlaced) return;
    scenePlaced = true;

    // Converte posição do marcador para espaço do mundo
    Vector3 worldPosition = trackedImage.transform.TransformPoint(Vector3.zero);
    Quaternion worldRotation = trackedImage.transform.rotation;

    arSceneRoot.transform.position = worldPosition;
    arSceneRoot.transform.rotation = worldRotation;
    arSceneRoot.SetActive(true);

    trackedImageManager.enabled = false;

    Debug.Log("Cena fixada: " + worldPosition);
}
}