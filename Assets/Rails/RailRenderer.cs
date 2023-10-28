using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(SplineContainer))]
[ExecuteInEditMode]
public class RailRenderer : MonoBehaviour
{
    public Mesh plankMesh;
    
    [HideInInspector]
    [Range(0.05f,1f)]
    public float spacing = 0.1f;

    Mesh mesh;

    SplineContainer _splines;
    SplineContainer splines
    {
        get
        {
            if (_splines != null) return _splines; else return GetComponent<SplineContainer>();
        }
    }

    Spline spline
    {
        get
        {
            return splines.Spline;
        }
        set
        {
            splines.Spline = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateMesh();
    }

    void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
    }
    void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline spline, int arg2, SplineModification modification)
    {
        if (splines != null && spline == this.spline)
            UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateMesh()
    {
        int length = (int)Math.Ceiling(1f / spacing) + 1;
        mesh = new Mesh();
        CombineInstance[] combines = new CombineInstance[length];
        for (int i = 0; i < length; i++)
        {
            float3 position, tangent, upVector;
            spline.Evaluate(i * spacing, out position, out tangent, out upVector);
            Quaternion rot = Quaternion.LookRotation(tangent);

            combines[i].mesh = new Mesh
            {
                vertices = plankMesh.vertices,
                triangles = plankMesh.triangles,
                normals = plankMesh.normals,
                uv = plankMesh.uv,
            };
            combines[i].transform = Matrix4x4.TRS(position, rot, Vector3.one);
        }
        mesh.CombineMeshes(combines);
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
