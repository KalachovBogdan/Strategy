using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;

    private List<Material> allMaterials = new List<Material>();
    private List<Color> originalEmissionColors = new List<Color>();

    private UnitController unitController;
    private TankTurretController turretController;
    private NavMeshAgent navAgent;

    public GameObject smokeEffectPrefab;
    private GameObject currentSmokeEffect;

    // 👇 Додано для Health Bar
    public MeshRenderer[] healthSegments; // 5 MeshRenderer об'єктів
    public Material greenMat;
    public Material redMat;

    void Start()
    {
        currentHP = maxHP;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            Material mat = rend.material;
            allMaterials.Add(mat);

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                originalEmissionColors.Add(mat.GetColor("_EmissionColor"));
            }
            else
            {
                originalEmissionColors.Add(Color.black);
            }
        }

        unitController = GetComponent<UnitController>();
        turretController = GetComponentInChildren<TankTurretController>();
        navAgent = GetComponent<NavMeshAgent>();

        UpdateHealthBar(); // оновити відображення з початку
    }

    public void TakeDamage(int damage)
    {
        if (currentHP <= 0) return;
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log($"{gameObject.name} отримав {damage} пошкодження, залишилось HP: {currentHP}");

        UpdateHealthBar();

        if (currentHP <= 0)
        {
            StartCoroutine(DieWithFlashAndSmoke());
        }
    }

    private void UpdateHealthBar()
    {
        int segmentsToShow = Mathf.CeilToInt(currentHP / 20f); // 1 сегмент = 20 HP

        for (int i = 0; i < healthSegments.Length; i++)
        {
            if (i < segmentsToShow)
                healthSegments[i].material = greenMat;
            else
                healthSegments[i].material = redMat;
        }
    }

    private IEnumerator DieWithFlashAndSmoke()
    {
        if (unitController != null)
            unitController.enabled = false;

        if (turretController != null)
            turretController.enabled = false;

        if (navAgent != null)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }

        if (currentSmokeEffect == null && smokeEffectPrefab != null)
        {
            currentSmokeEffect = Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity);
            currentSmokeEffect.transform.parent = transform;
        }

        float flashDuration = 1.5f;
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float intensity = Mathf.PingPong(elapsed * 5f, 1f);

            for (int i = 0; i < allMaterials.Count; i++)
            {
                Color baseColor = originalEmissionColors[i];
                Color emissionColor = baseColor + Color.red * intensity * 2f;
                allMaterials[i].SetColor("_EmissionColor", emissionColor);
            }

            yield return null;
        }

        if (currentSmokeEffect != null)
            Destroy(currentSmokeEffect);

        Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(999);
        }
    }
}
