using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Splines.Interpolators;

/// <summary>
/// Component for following a rail track with varied speed
/// </summary>
public class RailFollower : MonoBehaviour
{
    public SplineContainer railTrack;
    Spline spline;
    SplineData<float> speedData;

    /// <summary>
    /// Current speed
    /// </summary>
    public float speed;
    /// <summary>
    /// Current linear distance along the rail track
    /// </summary>
    float distance;

    void Awake()
    {
        if (railTrack == null)
        {
            Debug.LogError(name + " is missing a reference to rail track");
        }
    }

    void Start()
    {
        spline = railTrack.Spline;
        spline.TryGetFloatData("speed", out speedData);

    }

    // Update is called once per frame
    void Update()
    {
        var t = spline.ConvertIndexUnit(distance, PathIndexUnit.Distance, PathIndexUnit.Normalized);
        spline.Evaluate(t, out var position, out var tangent, out var upVector);
        var rotation = Quaternion.LookRotation(tangent, upVector);

        transform.SetPositionAndRotation(position, rotation);

        // Get speed from spline data
        speed = speedData.Evaluate(spline, distance, new LerpFloat { });
        distance += speed * Time.deltaTime;
    }
}
