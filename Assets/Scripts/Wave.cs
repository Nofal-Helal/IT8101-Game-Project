using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

[Serializable]
public enum WaveType
{
    EnemyActivator,
    EnemySpawner,
}

[Serializable]
public struct EnemySpawner
{
    [Tooltip("Type of enemy to spawn")]
    [NaughtyAttributes.AllowNesting]
    public GameObject enemy;

    [Tooltip("Number of enemies of that type to spawn")]
    [NaughtyAttributes.AllowNesting]
    public int count;
}

[Serializable]
public struct EnemyActivator
{
    [Tooltip("Enemies to activate")]
    [NaughtyAttributes.AllowNesting]
    public GameObject[] enemies;
}

[Serializable]
public struct SubWave
{
    [NaughtyAttributes.AllowNesting]
    public WaveType type;

    [NaughtyAttributes.ShowIf("IsSpawner")]
    [NaughtyAttributes.AllowNesting]
    public EnemySpawner spawner;

    [NaughtyAttributes.ShowIf("IsActivator")]
    [NaughtyAttributes.AllowNesting]
    public EnemyActivator activator;

    public readonly bool IsSpawner => type == WaveType.EnemySpawner;
    public readonly bool IsActivator => type == WaveType.EnemyActivator;
}

public class Wave : MonoBehaviour
{
    [NaughtyAttributes.Required]
    [Tooltip("The obstacle that starts this wave")]
    public Obstacle obstacle;

    [NaughtyAttributes.MaxValue(0f)]
    [NaughtyAttributes.OnValueChanged("UpdateSplinePosition")]
    [Tooltip("Offset from the obstacle at which the wave activates")]
    public float offset = -10f;

    public SubWave[] subWaves;

    private new BoxCollider collider;
    private DataPoint<UnityEngine.Object> dataPoint;
    public float Distance => dataPoint.Index;

    private Vector3 _spline_position;

    // Start is called before the first frame update
    private void Start()
    {
        if (obstacle == null)
        {
            Debug.LogError("Wave has no Obstacle attached");
        }

        collider = GetComponent<BoxCollider>();

        AttachToRailTrack();
        /* obstacle.OnStopAtObstacle = () => SpawnWave(); */

        foreach (SubWave subWave in subWaves)
            if (subWave.type == WaveType.EnemyActivator)
            {
                ActivateEnemies(subWave.activator, false);
            }
    }

    private void AttachToRailTrack()
    {
        SplineContainer railTrack = obstacle.railTrack;
        SplineData<UnityEngine.Object> waveData = railTrack.Spline.GetOrCreateObjectData("waves");
        dataPoint = new(obstacle.Distance + offset, this);
        _ = waveData.Add(dataPoint);
    }

    public void SpawnWave()
    {
        foreach (SubWave subWave in subWaves)
        {
            switch (subWave.type)
            {
                case WaveType.EnemySpawner:
                    RandomSpawnEnemies(subWave.spawner);
                    break;
                case WaveType.EnemyActivator:
                    ActivateEnemies(subWave.activator);
                    break;
            }
        }
    }

    private void ActivateEnemies(EnemyActivator activator, bool active = true)
    {
        foreach (GameObject enemy in activator.enemies)
        {
            enemy.SetActive(active);
        }
    }

    private void RandomSpawnEnemies(EnemySpawner spawner)
    {
        int spawned = 0;
        while (spawned < spawner.count)
        {
            Vector3 initialPosition =
                new(
                    Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                    Random.Range(collider.bounds.min.y, collider.bounds.max.y),
                    Random.Range(collider.bounds.min.z, collider.bounds.max.z)
                );

            Vector3? position = PlaceEnemy(initialPosition);
            if (position.HasValue)
            {
                _ = Instantiate(spawner.enemy, position.Value, Quaternion.identity);
                spawned += 1;
            }
        }
    }

    /// <summary>
    /// Places an enemy on the ground by ray tracing from an initial spawning
    /// position.
    /// </summary>
    /// <param name="initialPosition">Initial position of spawning</param>
    /// <returns>
    /// Position on the ground or null if somehow the ray didn't hit
    /// </returns>
    private Vector3? PlaceEnemy(Vector3 initialPosition)
    {
        const int layerMask = 1 << 7;
        return Physics.Raycast(
            initialPosition,
            Vector3.down,
            out RaycastHit raycastHit,
            10f,
            layerMask
        )
            ? raycastHit.point
            : null;
    }

    // Update is called once per frame
    /* private void Update() { } */

    // Editor only functions
    private void UpdateSplinePosition()
    {
        if (obstacle != null)
        {
            try
            {
                Spline spline = obstacle.railTrack.Spline;
                obstacle.Nearest(out _, out float t);
                float offset_distance = spline.ConvertIndexUnit(t, PathIndexUnit.Distance) + offset;
                float offset_t = spline.ConvertIndexUnit(
                    offset_distance,
                    PathIndexUnit.Distance,
                    PathIndexUnit.Normalized
                );
                _spline_position = spline.EvaluatePosition(offset_t);
            }
            catch (Exception) { }
        }
        else
        {
            _spline_position = float3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        if (_spline_position != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                transform.position,
                _spline_position + obstacle.railTrack.transform.position
            );
        }
        else
        {
            UpdateSplinePosition();
        }
    }
}
