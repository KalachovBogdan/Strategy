using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public GameObject SelectionMarker;
    public MeshRenderer MyMeshRenderer;
    public Material Redmat, GreeMat;
    public bool isEnemy = false;

    public GameObject HealthBar;

    void Start()
    {
        if (HealthBar != null)
            HealthBar.SetActive(false);
    }

    public void SelectMe()
    {
        if (SelectionMarker != null)
            SelectionMarker.SetActive(true);

        if (!isEnemy && HealthBar != null)
            HealthBar.SetActive(true);

        Debug.Log($"✔ {name} обраний");
    }

    public void DeSelectMe()
    {
        if (SelectionMarker != null)
            SelectionMarker.SetActive(false);

        if (HealthBar != null)
            HealthBar.SetActive(false);
    }

    public void SetColorRed()
    {
        if (MyMeshRenderer != null && Redmat != null)
            MyMeshRenderer.material = Redmat;
    }
}
