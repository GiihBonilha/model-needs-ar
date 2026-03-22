using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject placementIndicator; // ícone pulsante na mesa
    [SerializeField] private GameObject arSceneRoot;        // raiz de toda a cena 3D

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
        // Faz um raycast do centro da tela para detectar superfície
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            placementPose = hits[0].pose;
            isPlacementValid = true;

            if (placementIndicator != null)
            {
                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(
                    placementPose.position,
                    placementPose.rotation
                );
            }
        }
        else
        {
            isPlacementValid = false;
            if (placementIndicator != null)
                placementIndicator.SetActive(false);
        }
    }

    private void HandleTouch()
    {
        if (!isPlacementValid) return;
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        PlaceScene();
    }

    private void PlaceScene()
    {
        scenePlaced = true;

        // Posiciona a cena 3D no ponto tocado
        if (arSceneRoot != null)
        {
            arSceneRoot.SetActive(true);
            arSceneRoot.transform.SetPositionAndRotation(
                placementPose.position,
                placementPose.rotation
            );
        }

        // Esconde o indicador e para de detectar novos planos
        if (placementIndicator != null)
            placementIndicator.SetActive(false);

        planeManager.enabled = false;

        // Esconde os planos detectados
        foreach (var plane in planeManager.trackables)
            plane.gameObject.SetActive(false);
    }
}