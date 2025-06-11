using UnityEngine;
using System.Collections;

public class TankTurretController : MonoBehaviour
{
    public Transform turret;           
    public float rotationSpeed = 90f;  
    public float minWaitTime = 1f;     
    public float maxWaitTime = 5f;     
    public float maxRotationAngle = 90f; 
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool isRotating = false;

    void Start()
    {
        if (turret != null)
        {
            initialRotation = turret.localRotation;
            StartCoroutine(RotateTurretRoutine());
        }
    }

    IEnumerator RotateTurretRoutine()
    {
        while (true)
        {
         
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            
            float randomAngle = Random.Range(-maxRotationAngle, maxRotationAngle);

        
            targetRotation = initialRotation * Quaternion.Euler(0, randomAngle, 0);
            isRotating = true;

            while (isRotating)
            {
                turret.localRotation = Quaternion.RotateTowards(turret.localRotation, targetRotation, rotationSpeed * Time.deltaTime);

                if (Quaternion.Angle(turret.localRotation, targetRotation) < 0.1f)
                {
                    turret.localRotation = targetRotation;
                    isRotating = false;
                }
                yield return null;
            }
        }
    }
}
