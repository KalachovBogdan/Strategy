using UnityEngine;

[RequireComponent(typeof(Unit))]
public class TankController : MonoBehaviour
{
    [Header("Башта")]
    public Transform turret;

    [Header("Префаб снаряду та точка спавну")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    [Header("Налаштування виявлення")]
    public float detectionRadius = 15f;

    [Header("Налаштування повороту")]
    public float turretRotationSpeed = 5f;

    private string enemyTag;
    private Transform currentTarget;

    public float attackRange = 10f;
    public float attackCooldown = 1.5f;

    private float attackTimer = 0f;

    void Start()
    {
        Unit unit = GetComponent<Unit>();

        if (unit != null)
        {
            enemyTag = (unit.owner == FactoryBuilding.OwnerType.Player) ? "Enemy" : "PlayerTank";
        }
        else
        {
            Debug.LogWarning("Unit компонент не знайдено на танку!");
            enabled = false;
        }

        if (turret == null)
        {
            Debug.LogWarning("Не призначена башта у інспекторі!");
            enabled = false;
        }

        if (projectilePrefab == null)
        {
            Debug.LogWarning("Не призначено префаб снаряду!");
            enabled = false;
        }

        if (projectileSpawnPoint == null)
        {
            Debug.LogWarning("Не призначено точку спавну снаряду!");
            enabled = false;
        }
    }

    void Update()
    {
        UpdateTarget();

        if (currentTarget != null)
        {
            if (turret != null)
                RotateTurretTowards(currentTarget.position);

            float distance = Vector3.Distance(transform.position, currentTarget.position);
            if (distance <= attackRange)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    ShootProjectile(currentTarget);
                    attackTimer = attackCooldown;
                }
            }
        }
    }

    void ShootProjectile(Transform target)
    {
        Debug.Log($"Спавн снаряду у позиції: {projectileSpawnPoint.position}, ціль: {target.name} позиція: {target.position}");
        GameObject projectileGO = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetTarget(target);
        }
        else
        {
            Debug.LogWarning("Префаб снаряду не має компонента Projectile!");
        }
    }

    void UpdateTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
        }

        currentTarget = (closestEnemy != null) ? closestEnemy : null;
    }

    void RotateTurretTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - turret.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(direction);
        turret.rotation = Quaternion.Slerp(turret.rotation, targetRot, Time.deltaTime * turretRotationSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
