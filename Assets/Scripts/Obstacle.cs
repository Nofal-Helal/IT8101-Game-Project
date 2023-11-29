using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Object = UnityEngine.Object;

public class Obstacle : MonoBehaviour
{
    public SplineContainer railTrack;
    Spline spline;
    SplineData<Object> splineData;
    DataPoint<Object> dataPoint;

    /// <summary>
    /// Distance along the spline
    /// </summary>
    public float Distance => dataPoint.Index;

    // Start is called before the first frame update
    public void Start()
    {
        if (railTrack == null)
        {
            Debug.LogError("Obstacle has no reference to a rail track.");
            return;
        }

        spline = railTrack.Spline;
        splineData = spline.GetOrCreateObjectData("obstacles");
        AddDataPoint();
    }

    public void OnDestroy()
    {
        splineData.RemoveDataPoint(dataPoint.Index);
    }


    // Update is called once per frame
    void Update()
    {
    }

    public void Nearest(out float3 nearest, out float t)
    {
        float3 point = transform.position - railTrack.transform.position;
        SplineUtility.GetNearestPoint(spline, point, out nearest, out t);
    }

    private void AddDataPoint()
    {
        Nearest(out _, out var t);
        var t_dist = SplineUtility.ConvertIndexUnit(spline, t, PathIndexUnit.Distance);
        dataPoint = new DataPoint<Object>(t_dist, this);
        splineData.Add(dataPoint);
    }

    void OnDrawGizmos()
    {
        if (railTrack)
        {
            spline ??= railTrack.Spline;
            Nearest(out var nearest, out _);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, (Vector3)nearest + railTrack.transform.position);
        }
    }

}
