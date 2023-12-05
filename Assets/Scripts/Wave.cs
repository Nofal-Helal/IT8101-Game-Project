using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct WaveEnemies
{
    [Tooltip("Type of enemy to spawn")]
    public GameObject enemy;

    [Tooltip("Number of enemies of that type to spawn in this wave")]
    public int count;
}

public class Wave : MonoBehaviour
{
    [NaughtyAttributes.Required]
    [Tooltip("The obstacle that starts this wave")]
    public Obstacle obstacle;

    public List<WaveEnemies> enemies;

    private new BoxCollider collider;

    // Start is called before the first frame update
    private void Start()
    {
        if (obstacle == null)
        {
            Debug.LogError("Wave has no Obstacle attached");
        }

        collider = GetComponent<BoxCollider>();

        obstacle.OnStopAtObstacle = SpawnWave;
    }

    private void SpawnWave()
    {
        foreach (WaveEnemies enemySpawns in enemies)
        {
            int spawned = 0;
            while (spawned < enemySpawns.count)
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
                    _ = Instantiate(enemySpawns.enemy, position.Value, Quaternion.identity);
                    spawned += 1;
                }
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

    private void OnDrawGizmos()
    {
        if (obstacle)
        {
            obstacle.Nearest(out float3 nearest, out _);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                transform.position,
                (Vector3)nearest + obstacle.railTrack.transform.position
            );
        }
    }
}
