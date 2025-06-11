using UnityEngine;
using UnityEngine.AI;

public class TankSmokeController : MonoBehaviour
{
    public ParticleSystem smokeParticleSystem; 
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (smokeParticleSystem == null)
            Debug.LogError("Smoke Particle System not assigned!");
    }

    void Update()
    {
        if (agent == null || smokeParticleSystem == null)
            return;

        
        if (agent.velocity.magnitude > 0.1f)
        {
            if (!smokeParticleSystem.isPlaying)
                smokeParticleSystem.Play();
        }
        else
        {
            if (smokeParticleSystem.isPlaying)
                smokeParticleSystem.Stop();
        }
    }
}
