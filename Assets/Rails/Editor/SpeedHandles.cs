using System;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR

[Icon("Packages/com.unity.splines/Editor/Resources/Icons/ToolHandleParent.png")]
[EditorTool("Track Speed Tool", typeof(SplineContainer))]
public class SpeedHandles : EditorTool
{
    Spline spline;
    SplineData<float> speedData;

    GUIStyle style;

    void OnEnable()
    {
        try
        {
            spline = (target as SplineContainer).Spline;
            speedData = spline.GetOrCreateFloatData("speed");
        }
        catch (Exception) { }

        style = new();
        style.normal.textColor = Color.black;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 16;
    }

    public override void OnToolGUI(EditorWindow _)
    {
        SplineDataHandles.DataPointHandles(spline, speedData);

        // Display labels above each data point
        foreach (
            var dataPoint in speedData.Select(
                (dp, i) =>
                    new
                    {
                        Distance = dp.Index,
                        Speed = dp.Value,
                        Index = i
                    }
            )
        )
        {
            float t = spline.ConvertIndexUnit(
                dataPoint.Distance,
                PathIndexUnit.Distance,
                PathIndexUnit.Normalized
            );
            float3 position = spline.EvaluatePosition(t);
            position.y += 1f;
            Handles.Label(position, $"[{dataPoint.Index}] = {dataPoint.Speed}", style);
        }
    }
}

#endif