using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class Boid : MonoBehaviour
{
    [HideInInspector] 
    public BoidManager manager;

    [Header("Per-boid tunables (can be overridden by manager)")]
    public float perceptionRadius = 3f;
    public float maxSpeed = 5f;
    public float maxForce = 5f;

    [Header("State (public for debug/inspector)")]
    public Vector3 velocity;

    private Vector3 _acceleration = Vector3.zero;

    private void Update()
    {
        // 1) Получаем список соседей (наивная реализация O(n))
        List<Boid> neighbors = GetNeighbors();

        // 2) Вычисляем компоненты флокинга
        Vector3 alignment = ComputeAlignment(neighbors) * manager.alignmentWeight;
        Vector3 cohesion = ComputeCohesion(neighbors) * manager.cohesionWeight;
        Vector3 separation = ComputeSeparation(neighbors) * manager.separationWeight;

        // 3) Применяем веса и суммируем в acceleration
        _acceleration = Vector3.zero; // сбрасываем
        _acceleration += alignment;
        _acceleration += cohesion;
        _acceleration += separation;

        // 4) Обработка границ области симуляции
        HandleBounds();

        // Ограничиваем силу (чтобы маневры были гладкие)
        _acceleration = Vector3.ClampMagnitude(_acceleration, maxForce);

        // 5) Обновляем скорость и позицию (кинематическая интеграция)
        float dt = Time.deltaTime;
        velocity += _acceleration * dt;
        // Ограничение скорости
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Обновляем transform
        transform.position += velocity * dt;

        // Поворот "в сторону" направления движения, если скорость ненулевая
        if (velocity.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(velocity.normalized);
        }
    }

    private List<Boid> GetNeighbors()
    {
        var result = new List<Boid>();
        if (manager == null || manager.boids == null) 
            return result;

        var pos = transform.position;
        var r2 = perceptionRadius * perceptionRadius;

        foreach (var other in manager.boids)
        {
            if (other == this) 
                continue;

            if ((other.transform.position - pos).sqrMagnitude <= r2)
                result.Add(other);
        }
        return result;
    }

    private Vector3 ComputeAlignment(List<Boid> neighbors)
    {
        if (neighbors.Count == 0) 
            return Vector3.zero;

        var avgVel = Vector3.zero;
        foreach (var b in neighbors)
        {
            avgVel += b.velocity;
        }
        avgVel /= neighbors.Count;

        if (avgVel.sqrMagnitude < 0.00001f) 
            return Vector3.zero;

        var desired = avgVel.normalized * maxSpeed;
        var steering = desired - velocity;
        return steering;
    }

    private Vector3 ComputeCohesion(List<Boid> neighbors)
    {
        if (neighbors.Count == 0) 
            return Vector3.zero;

        var center = Vector3.zero;
        foreach (var b in neighbors)
        {
            center += b.transform.position;
        }
        center /= neighbors.Count;

        var desired = (center - transform.position);
        if (desired.sqrMagnitude < 0.00001f) 
            return Vector3.zero;

        desired = desired.normalized * maxSpeed;
        var steering = desired - velocity;
        return steering;
    }

    private Vector3 ComputeSeparation(List<Boid> neighbors)
    {
        if (neighbors.Count == 0) return Vector3.zero;

        var steer = Vector3.zero;
        var avoidCount = 0;
        var desiredSeparation = perceptionRadius * 0.5f; // можно вынести как отдельный параметр
        var desiredSeparation2 = desiredSeparation * desiredSeparation;

        foreach (var b in neighbors)
        {
            var diff = transform.position - b.transform.position;
            var d2 = diff.sqrMagnitude;
            if (d2 > 0 && d2 < desiredSeparation2)
            {
                steer += diff.normalized / Mathf.Sqrt(d2);
                avoidCount++;
            }
        }

        if (avoidCount == 0) 
            return Vector3.zero;

        steer /= avoidCount;

        if (steer.sqrMagnitude < 0.00001f) 
            return Vector3.zero;

        // вектор желаемой скорости
        var desired = steer.normalized * maxSpeed;
        var steering = desired - velocity;
        return steering;
    }
    
    private void HandleBounds()
    {
        if (manager == null) 
            return;

        var center = manager.transform.position;
        var radius = manager.boundsRadius;
        var pos = transform.position;

        var offset = pos - center;
        var dist = offset.magnitude;
        if (dist > radius * 0.9f)
        {
            var desired = (center - pos).normalized * maxSpeed;
            var steer = (desired - velocity) * manager.boundsAvoidanceWeight;
            _acceleration += steer; // прямо добавляем к acceleration (учтётся дальше)
        }
    }
}
