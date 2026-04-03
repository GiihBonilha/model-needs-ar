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

    // Desparentar do XR Origin e colocar na raiz da cena
    arSceneRoot.transform.SetParent(null);
    
    arSceneRoot.SetActive(true);
    arSceneRoot.transform.position = trackedImage.transform.position;
    arSceneRoot.transform.rotation = trackedImage.transform.rotation;

    // Desativa o rastreamento da imagem para liberar processamento
    trackedImageManager.enabled = false;

    Debug.Log("Cena fixada no mundo real: " + arSceneRoot.transform.position);
}
}