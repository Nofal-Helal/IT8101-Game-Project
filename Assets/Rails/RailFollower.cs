using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Splines.Interpolators;

/// <summary>
/// Component for following a rail track with varied speed
/// </summary>
public class RailFollower : MonoBehaviour
{
    public SplineContainer railTrack;
    private Spline spline;
    private SplineData<float> speedData;
    private SplineData<Object> obstacles;

    /// <summary>
    /// Current speed
    /// </summary>
    public float speed;

    /// <summary>
    /// Current linear distance along the rail track
    /// </summary>
    public float distance = 0f;

    /// <summary>
    /// Distance away from obstacles to stop at
    /// </summary>
    public float obstacleStopDistance = 1f;
    public Obstacle nextObstacle;

    private void Awake()
    {
        if (railTrack == null)
        {
            Debug.LogError(name + " is missing a reference to rail track");
        }
    }

    private void Start()
    {
        spline = railTrack.Spline;
        Debug.Assert(spline.TryGetFloatData("speed", out speedData));
        Debug.Assert(spline.TryGetObjectData("obstacles", out obstacles));

        nextObstacle = nextObstacle != null ? nextObstacle : NextObstacle();
    }

    // Update is called once per frame
    private void Update()
    {
        float t = spline.ConvertIndexUnit(
            distance,
            PathIndexUnit.Distance,
            PathIndexUnit.Normalized
        );
        _ = spline.Evaluate(t, out float3 position, out float3 tangent, out float3 upVector);
        Quaternion rotation = Quaternion.LookRotation(tangent, upVector);

        transform.SetPositionAndRotation(position, rotation);

        // if there is an obstacle and the cart is within obstacleStopDistance
        if (nextObstacle && (nextObstacle.Distance - distance) < obstacleStopDistance)
        {
            speed = 0;
        }
        else
        {
            // Get speed from spline data
            speed = speedData.Evaluate(spline, distance, new LerpFloat { });
        }

        distance += speed * Time.deltaTime;
    }

    // press o to remove obstacle
    public void OnRemoveObstacle()
    {
        if (nextObstacle)
        {
            nextObstacle.OnDestroy();
            Destroy(nextObstacle.gameObject);
            nextObstacle = NextObstacle();
        }
    }

    private Obstacle NextObstacle()
    {
        return obstacles.Count <= 0
            ? null
            : (Obstacle)obstacles.Evaluate(spline, distance, new NextObstacleInterpolator());
    }

    private class NextObstacleInterpolator : IInterpolator<Object>
    {
        public Object Interpolate(Object from, Object to, float t)
        {
            return t == 0 ? from : to;
        }
    }
}
