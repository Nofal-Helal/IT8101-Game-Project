using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Splines.Interpolators;

/// <summary>
/// Component for following a rail track with varied speed
/// </summary>
public class RailFollower : MonoBehaviour
{
    [NaughtyAttributes.Required]
    public SplineContainer railTrack;
    private Spline spline;
    private SplineData<float> speedData;
    private SplineData<Object> obstacles;
    private SplineData<Object> waves;

    /// <summary>
    /// Current speed
    /// </summary>
    public float speed;

    /// <summary>
    /// Current linear distance along the rail track
    /// </summary>
    [NaughtyAttributes.OnValueChanged("ResetPosition")]
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
        Debug.Assert(spline.TryGetObjectData("waves", out waves));

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
            speed = Mathf.MoveTowards(speed, 0, Time.deltaTime * 5);
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

            // add speed to start moving again
            if (speed == 0f)
                speed = 0.1f;
        }
    }

    private Obstacle NextObstacle()
    {
        return obstacles.Count <= 0
            ? null
            : (Obstacle)obstacles.Evaluate(spline, distance, new NextObjectInterpolator());
    }

    public Wave NextWave =>
        waves.Count <= 0
            ? null
            : (Wave)waves.Evaluate(spline, distance, new NextObjectInterpolator());

    public class NextObjectInterpolator : IInterpolator<Object>
    {
        public Object Interpolate(Object from, Object to, float t)
        {
            return t == 0 ? from : to;
        }
    }

    // inspector button to reset the cart to the set distance along the track
    [NaughtyAttributes.Button]
    public void ResetPosition()
    {
        spline = railTrack.Spline;
        float t = spline.ConvertIndexUnit(
            distance,
            PathIndexUnit.Distance,
            PathIndexUnit.Normalized
        );

        _ = spline.Evaluate(t, out float3 position, out float3 tangent, out float3 upVector);
        Quaternion rotation = Quaternion.LookRotation(tangent, upVector);

        transform.SetPositionAndRotation(position, rotation);
    }
}
