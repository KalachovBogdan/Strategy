using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    public Camera mainCamera;
    private GameObject buildingToPlace;
    private GameObject currentGhost;

    public LayerMask groundLayer;

    void Update()
    {
        if (buildingToPlace != null)
        {
            MoveGhostToMouse();

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceBuilding();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
            }
        }
    }

    public void SetBuildingToPlace(string buildingName)
    {
        GameObject prefab = Resources.Load<GameObject>("Buildings/" + buildingName);
        if (prefab != null)
        {
            buildingToPlace = prefab;
            currentGhost = Instantiate(buildingToPlace);

            // ✅ Перевірка чи є Collider
            Collider col = currentGhost.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
            else
            {
                Debug.LogWarning("Building prefab is missing a Collider: " + buildingName);
            }

            SetTransparent(currentGhost, 0.5f);
        }
        else
        {
            Debug.LogError("Building prefab not found: Buildings/" + buildingName);
        }
    }

    void MoveGhostToMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            currentGhost.transform.position = hit.point;
        }
    }

    void PlaceBuilding()
    {
        Vector3 placePosition = currentGhost.transform.position;
        Debug.Log("Placing at: " + placePosition);

        GameObject newBuilding = Instantiate(buildingToPlace, placePosition, Quaternion.identity);
        Debug.Log("New building created: " + newBuilding.name);

        Destroy(currentGhost);
        buildingToPlace = null;
    }

    void CancelPlacement()
    {
        Destroy(currentGhost);
        buildingToPlace = null;
    }

    void SetTransparent(GameObject obj, float alpha)
    {
        foreach (var rend in obj.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in rend.materials)
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
                mat.SetFloat("_Mode", 2);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }
    }
}
