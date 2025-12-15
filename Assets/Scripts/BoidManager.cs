using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [Header("Prefabs & Spawn")]
    public GameObject boidPrefab;
    public int initialCount = 100;
    public Vector3 spawnBounds = new(20, 5, 20);

    [Header("Global flocking parameters")]
    public float neighborRadius = 3f;
    [Range(0f, 5f)]
    public float cohesionWeight = 1f;
    [Range(0f, 5f)]
    public float alignmentWeight = 1f;
    [Range(0f, 5f)]
    public float separationWeight = 1.5f;

    [Header("Motion limits")]
    public float maxSpeed = 5f;
    public float maxForce = 5f;

    [Header("Bounds")]
    public float boundsRadius = 30f;
    public float boundsAvoidanceWeight = 10f;

    [HideInInspector]
    public List<Boid> boids = new();

    private void Start()
    {
        SpawnInitialBoids();
    }

    private void SpawnInitialBoids()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        boids.Clear();

        for (var i = 0; i < initialCount; i++)
        {
            var pos = transform.position + new Vector3(
                Random.Range(-spawnBounds.x, spawnBounds.x),
                Random.Range(-spawnBounds.y, spawnBounds.y),
                Random.Range(-spawnBounds.z, spawnBounds.z)
            );

            var go = Instantiate(boidPrefab, pos, Quaternion.identity, transform);
            var boid = go.GetComponent<Boid>();
            if (boid == null)
            {
                Debug.LogError("Boid prefab must have a Boid component!");
                Destroy(go);
                return;
            }

            boid.manager = this;
            boid.maxSpeed = maxSpeed;
            boid.maxForce = maxForce;
            boid.perceptionRadius = neighborRadius;
                
            boid.velocity = Random.insideUnitSphere.normalized * (maxSpeed * 0.5f);

            boids.Add(boid);
        }
    }

    private void OnValidate()
    {
        if (boids == null) 
            return;
        
        foreach (var boid in boids)
        {
            if (boid == null) 
                continue;
            
            boid.manager = this;
            boid.maxSpeed = maxSpeed;
            boid.maxForce = maxForce;
            boid.perceptionRadius = neighborRadius;
        }
    }
}
