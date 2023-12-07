using System;
using UnityEditor.EditorTools;
using UnityEditor.Splines;
using UnityEditor;
using UnityEngine.Splines;
using UnityEngine;

[Icon("Packages/com.unity.splines/Editor/Resources/Icons/ToolHandleParent.png")]
[EditorTool("Track Speed Tool", typeof(SplineContainer))]
public class SpeedHandles : EditorTool
{
    Spline spline;
    SplineData<float> speedData;

    void OnEnable()
    {
        try
        {
            spline = (target as SplineContainer).Spline;
            speedData = spline.GetOrCreateFloatData("speed");
        }
        catch (Exception) { }
    }

    public override void OnToolGUI(EditorWindow _)
    {
        SplineDataHandles.DataPointHandles(spline, speedData);
    }
}

