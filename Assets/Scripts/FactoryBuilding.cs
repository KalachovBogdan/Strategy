using UnityEngine;
using System.Collections;

public class FactoryBuilding : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject tankPrefab;

    public enum OwnerType { Player, Enemy }
    public OwnerType owner = OwnerType.Player;

    public bool isBuilding = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    if (owner == OwnerType.Player)
                    {
                        FactoryMenuManager.Instance.OpenMenu(this);
                    }
                    else
                    {
                        Debug.Log("Це ворожий завод. Меню не відкривається.");
                    }
                }
            }
        }
    }

    public void StartBuildingTank()
    {
        if (!isBuilding && (owner == OwnerType.Player || owner == OwnerType.Enemy))
        {
            isBuilding = true;
            StartCoroutine(BuildTankCoroutine());
        }
    }

    private IEnumerator BuildTankCoroutine()
    {
        float buildTime = 5f;
        yield return new WaitForSeconds(buildTime);

        SpawnTank();

        isBuilding = false;
    }

    public void SpawnTank()
    {
        Vector2 randomOffset = Random.insideUnitCircle * 2f;
        Vector3 spawnPosition = spawnPoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);

        GameObject newTank = Instantiate(tankPrefab, spawnPosition, Quaternion.identity);

        Unit tankUnit = newTank.GetComponent<Unit>();
        if (tankUnit != null)
        {
            tankUnit.owner = owner;
        }

        if (owner == OwnerType.Enemy)
        {
            newTank.tag = "Enemy";
        }
        else if (owner == OwnerType.Player)
        {
            newTank.tag = "PlayerTank";
        }

        if (owner == OwnerType.Enemy)
        {
            EnemyAIManager enemyManager = Object.FindFirstObjectByType<EnemyAIManager>();
            if (enemyManager != null)
            {
                enemyManager.RegisterEnemyTank(newTank);
            }
        }
        else if (owner == OwnerType.Player)
        {
            SelectionManager selectionManager = Object.FindFirstObjectByType<SelectionManager>();
            if (selectionManager != null)
            {
                SelectableObject so = newTank.GetComponent<SelectableObject>();
                if (so != null)
                {
                    selectionManager.AllSelectableObjects.Add(so);
                }
            }
        }
    }

}
