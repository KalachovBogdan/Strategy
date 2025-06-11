using UnityEngine;

public class TankRegistration : MonoBehaviour
{
    private Unit unitComponent;

    void Awake()
    {
        unitComponent = GetComponent<Unit>();
    }

    void Start()
    {
        if (unitComponent != null && unitComponent.owner == FactoryBuilding.OwnerType.Enemy)
        {
            if (EnemyAIManager.Instance != null)
            {
                EnemyAIManager.Instance.RegisterEnemyTank(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        if (unitComponent != null && unitComponent.owner == FactoryBuilding.OwnerType.Enemy)
        {
            if (EnemyAIManager.Instance != null)
            {
                EnemyAIManager.Instance.UnregisterEnemyTank(gameObject);
            }
        }
    }
}