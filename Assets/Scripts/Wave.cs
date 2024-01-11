using System;
using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// instances of the spawned enemies
    /// </summary>
    [NonSerialized]
    public List<GameObject> enemies;
}

[Serializable]
public struct EnemyActivator
{
    [Tooltip("Enemies to activate")]
    [NaughtyAttributes.AllowNesting]
    public GameObject[] enemies;
}

[Serializable]
public class SubWave
{
    [Tooltip(
        "Start this subwave after all enemies in the previous wave are defeated. If false start after certain time."
    )]
    [NaughtyAttributes.AllowNesting]
    public bool startAfterPrevious = true;

    [Tooltip("Time to wait to start this sub-wave after the previous wave started.")]
    [NaughtyAttributes.HideIf("startAfterPrevious")]
    [NaughtyAttributes.AllowNesting]
    public float waitTime;

    [NaughtyAttributes.AllowNesting]
    public WaveType type = WaveType.EnemyActivator;

    [NaughtyAttributes.ShowIf("IsSpawner")]
    [NaughtyAttributes.AllowNesting]
    public EnemySpawner spawner;

    [NaughtyAttributes.ShowIf("IsActivator")]
    [NaughtyAttributes.AllowNesting]
    public EnemyActivator activator;

    public bool IsSpawner => type == WaveType.EnemySpawner;
    public bool IsActivator => type == WaveType.EnemyActivator;
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
    public float waitTime = 0f;
    private int currentSubWave = 0;
    private bool isCountingDown = true;
    private bool HasNextSubWave => subWaves != null && currentSubWave < subWaves.Length;

    private new BoxCollider collider;
    private DataPoint<UnityEngine.Object> dataPoint;
    public float Distance => dataPoint.Index;
    private Vector3 _spline_position;
    private bool inWave = false;

    // Start is called before the first frame update
    private void Start()
    {
        if (obstacle == null)
        {
            Debug.LogError("Wave has no Obstacle attached");
        }
        collider = GetComponent<BoxCollider>();
        isCountingDown = waitTime == 0f ? false : true;
        AttachToRailTrack();
    }

    private void AttachToRailTrack()
    {
        SplineContainer railTrack = obstacle.railTrack;
        SplineData<UnityEngine.Object> waveData = railTrack.Spline.GetOrCreateObjectData("waves");
        dataPoint = new(obstacle.Distance + offset, this);
        _ = waveData.Add(dataPoint);
    }

    public void SpawnNextSubWave()
    {
        if (HasNextSubWave)
        {
            SubWave subWave = subWaves[currentSubWave];
            switch (subWave.type)
            {
                case WaveType.EnemySpawner:
                    RandomSpawnEnemies(subWave.spawner);
                    break;
                case WaveType.EnemyActivator:
                    ActivateEnemies(subWave.activator);
                    break;
            }
            currentSubWave += 1;

            // if next wave needs waiting time, start the countdown
            if (currentSubWave < subWaves.Length)
            {
                SubWave nextSubwave = subWaves[currentSubWave];
                if (!nextSubwave.startAfterPrevious)
                {
                    isCountingDown = true;
                    waitTime = nextSubwave.waitTime;
                }
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (isCountingDown) // assumes there is a next sub-wave
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0f)
            {
                isCountingDown = false;
                SpawnNextSubWave();
                inWave = true;
            }
        }
        else if (currentSubWave != 0 && HasNextSubWave)
        {
            // assumes the next wave needs to check for enemies from the previous wave
            SubWave nextSubwave = subWaves[currentSubWave - 1];
            IEnumerable<GameObject> enemies =
                nextSubwave.type == WaveType.EnemyActivator
                    ? nextSubwave.activator.enemies
                    : nextSubwave.type == WaveType.EnemySpawner
                        ? nextSubwave.spawner.enemies
                        : null;
        }
    }

    private void ActivateEnemies(EnemyActivator activator, bool active = true)
    {
        foreach (GameObject enemy in activator.enemies)
        {
            if (enemy)
            {
                BaseUniversal enemyData = enemy.GetComponent<BaseUniversal>();
                enemyData.isActivated = true;
            }
        }
    }
    private void RandomSpawnEnemies(EnemySpawner spawner)
    {
        int spawned = 0;
        spawner.enemies = new List<GameObject>();
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
                Debug.Log("am i even here?");
                GameObject enemy = Instantiate(spawner.enemy, position.Value, Quaternion.identity);
                spawner.enemies.Add(enemy);
                spawned += 1;
            }
            else
            {
                Debug.Log("no position :(");
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
            100f,
            layerMask
        )
            ? raycastHit.point
            : null;
    }

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
