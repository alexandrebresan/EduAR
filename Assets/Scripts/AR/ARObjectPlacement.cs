using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARObjectPlacement : MonoBehaviour
{
    [SerializeField] private GameObject objectToPlace;

    private ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool placementEnabled = false;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        if (objectToPlace != null)
        {
            objectToPlace.SetActive(false);
        }
    }

    public void EnablePlacement()
    {
        placementEnabled = true;

        Debug.Log("Posicionamento do objeto liberado.");
    }

    private void Update()
    {
        // O objeto só pode ser colocado depois que o desafio for liberado.
        if (!placementEnabled)
            return;

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        if (raycastManager.Raycast(
            touch.position,
            hits,
            TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            objectToPlace.transform.position = hitPose.position;

            objectToPlace.SetActive(true);

            placementEnabled = false;

            Debug.Log("Objeto colocado na superfície!");
        }
    }
}