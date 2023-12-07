using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Object = UnityEngine.Object;

public class Obstacle : MonoBehaviour
{
    [NaughtyAttributes.Required]
    public SplineContainer railTrack;

    private Spline spline;
    private SplineData<Object> splineData;
    private DataPoint<Object> dataPoint;

    // is the obstacle attached to a rail track
    private bool attached = false;

    /// <summary>
    /// Distance along the spline
    /// </summary>
    public float Distance => dataPoint.Index;

    public Action OnStopAtObstacle = () => { };

    private void OnEnable()
    {
        if (railTrack == null)
        {
            Debug.LogError("Obstacle has no reference to a rail track.");
            return;
        }

        spline = railTrack.Spline;
        splineData = spline.GetOrCreateObjectData("obstacles");
        AddDataPoint();
        attached = true;
    }

    // Start is called before the first frame update
    public void Start()
    {
        Debug.Assert(attached, "Obstacle has not attached to a rail track");
    }

    public void OnDestroy()
    {
        _ = splineData.RemoveDataPoint(dataPoint.Index);
    }

    // Update is called once per frame
    // void Update() { }

    public void Nearest(out float3 nearest, out float t)
    {
        float3 point = transform.position - railTrack.transform.position;
        _ = SplineUtility.GetNearestPoint(spline, point, out nearest, out t);
    }

    private void AddDataPoint()
    {
        Nearest(out _, out float t);
        float t_dist = SplineUtility.ConvertIndexUnit(spline, t, PathIndexUnit.Distance);
        dataPoint = new DataPoint<Object>(t_dist, this);
        _ = splineData.Add(dataPoint);
    }

    private void OnDrawGizmos()
    {
        if (railTrack)
        {
            spline ??= railTrack.Spline;
            Nearest(out float3 nearest, out _);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, (Vector3)nearest + railTrack.transform.position);
        }
    }
}
