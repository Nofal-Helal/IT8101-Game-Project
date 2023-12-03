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
        spline.TryGetFloatData("speed", out speedData);
        spline.TryGetObjectData("obstacles", out obstacles);

        nextObstacle = nextObstacle != null ? nextObstacle : NextObstacle();
    }

    // Update is called once per frame
    void Update()
    {
        var t = spline.ConvertIndexUnit(distance, PathIndexUnit.Distance, PathIndexUnit.Normalized);
        spline.Evaluate(t, out var position, out var tangent, out var upVector);
        var rotation = Quaternion.LookRotation(tangent, upVector);

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

    Obstacle NextObstacle()
    {
        if (obstacles.Count <= 0)
            return null;
        return (Obstacle)obstacles.Evaluate(spline, distance, new NextObstacleInterpolator());
    }

    class NextObstacleInterpolator : IInterpolator<Object>
    {
        public Object Interpolate(Object from, Object to, float t)
        {
            if (t == 0)
                return from;
            else
                return to;
        }
    }
}
