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
    [Tooltip("The obstacle that starts this wave")]
    public Obstacle obstacle;

    public List<WaveEnemies> enemies;

    private new BoxCollider collider;

    // Start is called before the first frame update
    private void Start()
    {
        if (obstacle == null) Debug.LogError("Wave has no Obstacle attached");

        collider = GetComponent<BoxCollider>();

        obstacle.OnStopAtObstacle = SpawnWave;
    }

    private void SpawnWave()
    {
        Debug.Log("Spawning Wave");

        foreach (var enemySpawns in enemies)
        {
            int spawned = 0;
            while (spawned < enemySpawns.count)
            {
                Vector3 position = new Vector3(
                    Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                    Random.Range(collider.bounds.min.y, collider.bounds.max.y),
                    Random.Range(collider.bounds.min.z, collider.bounds.max.z)
                );
                Instantiate(enemySpawns.enemy, position, Quaternion.identity);
                spawned += 1;
                Debug.Log("spawend " + spawned);
            }
        }
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
