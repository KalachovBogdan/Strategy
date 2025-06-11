using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;
    public GameObject impactEffectPrefab;

    private Transform target;

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        
        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;

        Vector3 direction = targetPosition - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        transform.LookAt(targetPosition);
    }

    void HitTarget()
    {
        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        }

        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
