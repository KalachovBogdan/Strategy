using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FactoryMenuManager : MonoBehaviour
{
    public static FactoryMenuManager Instance;

    public GameObject menuUI;
    public Button spawnButton;
    public GameObject tankPrefab;

    private FactoryBuilding currentFactory;
    private CameraControler cameraControler;

    void Awake()
    {
        Instance = this;
        menuUI.SetActive(false);

        spawnButton.onClick.AddListener(OnSpawnTankClicked);

      
        GameObject cameraView = GameObject.Find("Camera View");
        if (cameraView != null)
        {
            cameraControler = cameraView.GetComponent<CameraControler>();
        }
    }

    void Update()
    {
        if (menuUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseMenu();
            }
           
        }
    }

    public void OpenMenu(FactoryBuilding factory)
    {
        currentFactory = factory;
        menuUI.SetActive(true);

        menuUI.transform.position = Camera.main.WorldToScreenPoint(factory.transform.position + Vector3.up * 2);

        if (cameraControler != null)
            cameraControler.SetCameraLock(true);
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);
        currentFactory = null;

        if (cameraControler != null)
            cameraControler.SetCameraLock(false);
    }

    public void OnSpawnTankClicked()
    {
        if (currentFactory != null)
        {
            StartCoroutine(SpawnAfterDelay(5f, currentFactory));
            CloseMenu();
        }
    }

    IEnumerator SpawnAfterDelay(float delay, FactoryBuilding factory)
    {
        yield return new WaitForSeconds(delay);
        factory.SpawnTank();
    }
}
