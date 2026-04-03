using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

public class ARPlacementManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject placementIndicator;
    [SerializeField] private GameObject arSceneRoot;

    private bool scenePlaced = false;
    private Pose placementPose;
    private bool isPlacementValid = false;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Update()
    {
        if (scenePlaced) return;

        UpdatePlacementIndicator();
        HandleTouch();
    }

    private void UpdatePlacementIndicator()
{
    Vector2 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
    Debug.Log("Tentando raycast no centro: " + screenCenter);

    if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
    {
        placementPose = hits[0].pose;
        isPlacementValid = true;
        Debug.Log("Plano detectado em: " + placementPose.position);

        if (placementIndicator != null)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.position = placementPose.position;
            placementIndicator.transform.rotation = Quaternion.Euler(0, placementPose.rotation.eulerAngles.y, 0);
        }
    }
    else
    {
        isPlacementValid = false;
        Debug.Log("Nenhum plano detectado");
        if (placementIndicator != null)
            placementIndicator.SetActive(false);
    }
}

    private void HandleTouch()
{
    if (!isPlacementValid)
    {
        Debug.Log("HandleTouch: placement não é válido ainda");
        return;
    }

    if (Touchscreen.current == null)
    {
        Debug.LogError("HandleTouch: Touchscreen.current é NULL!");
        return;
    }

    var touch = Touchscreen.current.primaryTouch;
    
    if (touch.press.wasPressedThisFrame)
    {
        Debug.Log("Toque detectado! Chamando PlaceScene...");
        PlaceScene();
    }
}

    private void PlaceScene()
{
    scenePlaced = true;
    Debug.Log("PlaceScene chamado!");

    if (arSceneRoot != null)
    {
        arSceneRoot.SetActive(true);
        arSceneRoot.transform.position = placementPose.position;
        arSceneRoot.transform.rotation = Quaternion.Euler(0, placementPose.rotation.eulerAngles.y, 0);
        
        Debug.Log("Posição setada: " + arSceneRoot.transform.position);
    }

    if (placementIndicator != null)
        placementIndicator.SetActive(false);

    planeManager.enabled = false;

    foreach (var plane in planeManager.trackables)
        plane.gameObject.SetActive(false);
}
}