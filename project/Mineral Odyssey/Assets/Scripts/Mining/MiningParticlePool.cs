using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reuses particle systems for repeated mining feedback without allocating new objects every hit.
/// </summary>
public class MiningParticlePool : MonoBehaviour
{
    [Header("Pool Config")]
    [SerializeField] private ParticleSystem particlePrefab;
    [SerializeField] private int initialPoolSize = 8;
    [SerializeField] private int maxPoolSize = 24;

    private readonly Queue<ParticleSystem> availableParticles = new Queue<ParticleSystem>();
    private readonly List<ParticleSystem> allParticles = new List<ParticleSystem>();

    private void Awake()
    {
        initialPoolSize = Mathf.Max(0, initialPoolSize);
        if (maxPoolSize > 0)
        {
            initialPoolSize = Mathf.Min(initialPoolSize, maxPoolSize);
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            ParticleSystem particle = CreateParticle();
            if (particle != null)
            {
                availableParticles.Enqueue(particle);
            }
        }
    }

    public void Configure(ParticleSystem prefab, int initialSize, int maximumSize)
    {
        // Bootstrap scripts can configure pools after creating them at runtime.
        particlePrefab = prefab;
        initialPoolSize = Mathf.Max(0, initialSize);
        maxPoolSize = maximumSize;
        if (maxPoolSize > 0)
        {
            initialPoolSize = Mathf.Min(initialPoolSize, maxPoolSize);
        }

        while (allParticles.Count < initialPoolSize)
        {
            ParticleSystem particle = CreateParticle();
            if (particle != null)
            {
                availableParticles.Enqueue(particle);
            }
        }
    }

    public ParticleSystem Play(Vector3 position, Quaternion rotation)
    {
        // A missing prefab disables the effect cleanly instead of breaking mining logic.
        if (particlePrefab == null)
        {
            return null;
        }

        ParticleSystem particle = GetParticle();
        if (particle == null)
        {
            return null;
        }

        Transform particleTransform = particle.transform;
        particleTransform.SetParent(null);
        particleTransform.SetPositionAndRotation(position, rotation);
        particle.gameObject.SetActive(true);
        particle.Clear(true);
        particle.Play(true);

        StartCoroutine(ReturnAfterPlayback(particle));
        return particle;
    }

    private ParticleSystem GetParticle()
    {
        while (availableParticles.Count > 0)
        {
            ParticleSystem particle = availableParticles.Dequeue();
            if (particle != null)
            {
                return particle;
            }
        }

        if (maxPoolSize > 0 && allParticles.Count >= maxPoolSize)
        {
            return null;
        }

        return CreateParticle();
    }

    private ParticleSystem CreateParticle()
    {
        if (particlePrefab == null)
        {
            return null;
        }

        ParticleSystem particle = Instantiate(particlePrefab, transform);
        particle.gameObject.SetActive(false);
        allParticles.Add(particle);
        return particle;
    }

    private IEnumerator ReturnAfterPlayback(ParticleSystem particle)
    {
        // Wait for child particles too so bursts are not recycled before they finish.
        yield return new WaitWhile(() => particle != null && particle.IsAlive(true));

        if (particle == null)
        {
            yield break;
        }

        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.gameObject.SetActive(false);
        particle.transform.SetParent(transform);
        availableParticles.Enqueue(particle);
    }
}
