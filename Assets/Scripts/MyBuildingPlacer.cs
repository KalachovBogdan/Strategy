using UnityEngine;
using UnityEngine.AI;

public class MyBuildingPlacer : MonoBehaviour
{
    public Camera mainCamera;
    public string groundLayerName = "Ground";
    public Material previewMaterial;
    public Material invalidPlacementMaterial;


    private GameObject selectedBuildingPrefab;
    private GameObject previewInstance;
    private bool isPlacing = false;

    private Renderer[] previewRenderers;
    private Collider[] previewColliders;
    private bool canPlace = true;

    public float rotationSpeed = 100f;

    void Update()
    {
        if (isPlacing && previewInstance != null)
        {
            UpdatePreviewPosition();

            

            if (Input.GetKey(KeyCode.Z))
            {
                previewInstance.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.X))
            {
                previewInstance.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceBuilding();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

    public void StartPlacing(GameObject buildingPrefab)
    {
        selectedBuildingPrefab = buildingPrefab;
        previewInstance = Instantiate(selectedBuildingPrefab);

        
        NavMeshObstacle obstacle = previewInstance.GetComponentInChildren<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.enabled = false;
        }

        
        previewColliders = previewInstance.GetComponentsInChildren<Collider>();
        foreach (Collider col in previewColliders)
        {
            col.enabled = true;
            col.isTrigger = true;
        }

        previewRenderers = previewInstance.GetComponentsInChildren<Renderer>();
        ApplyPreviewMaterial(previewMaterial);

        isPlacing = true;
        canPlace = true;
    }

    private void UpdatePreviewPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask(groundLayerName);

        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            Vector3 targetPos = hit.point;

            bool canPlaceHere = CanPlaceAtPosition(targetPos);

            if (canPlaceHere)
            {
                previewInstance.transform.position = targetPos;
                canPlace = true;
            }
            else
            {
                
                previewInstance.transform.position = targetPos;
                canPlace = false;
            }

            ApplyPreviewMaterial(canPlace ? previewMaterial : invalidPlacementMaterial);

            Debug.Log($"Preview position: {targetPos}, Can place: {canPlace}");
        }
    }

    private bool CanPlaceAtPosition(Vector3 position)
    {
        if (previewColliders == null || previewColliders.Length == 0)
            return false;

        int collisionMask = ~LayerMask.GetMask(groundLayerName);

        foreach (Collider col in previewColliders)
        {
            
            Vector3 localCenterOffset = col.bounds.center - previewInstance.transform.position;

            
            Vector3 checkCenter = position + localCenterOffset;

            Vector3 halfExtents = col.bounds.extents;
            Quaternion rotation = previewInstance.transform.rotation;

            Collider[] overlaps = Physics.OverlapBox(checkCenter, halfExtents, rotation, collisionMask);

            foreach (var overlap in overlaps)
            {
                if (overlap.gameObject != previewInstance)
                {
                    Debug.Log($"Cannot place: overlaps with {overlap.gameObject.name}");
                    return false;
                }
            }
        }

        return true;
    }

    private void PlaceBuilding()
    {
        Instantiate(selectedBuildingPrefab, previewInstance.transform.position, previewInstance.transform.rotation);
        Destroy(previewInstance);
        ResetPlacement();
    }

    private void CancelPlacement()
    {
        Destroy(previewInstance);
        ResetPlacement();
    }

    private void ResetPlacement()
    {
        previewInstance = null;
        selectedBuildingPrefab = null;
        isPlacing = false;
    }

    private void ApplyPreviewMaterial(Material mat)
    {
        foreach (Renderer renderer in previewRenderers)
        {
            Material[] mats = new Material[renderer.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = mat;
            }
            renderer.materials = mats;
        }
    }
}
