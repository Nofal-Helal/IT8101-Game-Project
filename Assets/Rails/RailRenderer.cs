using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(SplineContainer))]
[ExecuteInEditMode]
public class RailRenderer : MonoBehaviour
{
    [SerializeField] Mesh plankMesh;

    /// <summary>
    /// 2D Cross-section of a railing
    /// </summary>
    [SerializeField] Mesh2D rail2D;

    [Tooltip("Spacing between track planks")]
    [Range(0.4f, 2f)]
    [SerializeField] float plankSpacing = 0.1f;

    [Range(0.05f, 2f)]
    [Tooltip("Width of side railings")]
    [SerializeField] float railWidth = 0.5f;

    [Range(0.05f, 2f)]
    [Tooltip("Scale of side railings")]
    [SerializeField] float railScale = 0.5f;

    [Range(0.01f, 5f)]
    [Tooltip("Detail level of side railing")]
    [SerializeField] float railDetail = 2f;

    int RailSegments => Mathf.CeilToInt(railDetail * spline.GetLength());

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

    void Awake()
    {
        mesh = new Mesh();
        mesh.name = "Generated Track Mesh";
#if UNITY_EDITOR
        plankMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Rails/Plank.blend");
        rail2D = AssetDatabase.LoadAssetAtPath<Mesh2D>("Assets/Rails/Rail Cross Section.asset");
#else
        plankMesh = Resources.Load<Mesh>("Assets/Rails/Plank.blend");
        rail2D = Resources.Load<Mesh2D>("Assets/Rails/Rail Cross Section.asset");
#endif
    }

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
        if (plankMesh == null) return;

        mesh.Clear();

        CombineInstance[] combines;
        SubMeshDescriptor planksSubMesh;

        int plankCount = Mathf.CeilToInt(plankSpacing * spline.GetLength()) + 1;
        combines = new CombineInstance[plankCount + 2];
        // Place planks along the spline
        for (int i = 0; i < plankCount; i++)
        {
            float3 position, tangent, upVector;
            spline.Evaluate(i / (plankCount - 1f), out position, out tangent, out upVector);
            Quaternion rot = Quaternion.LookRotation(tangent, upVector);

            combines[i].mesh = new Mesh
            {
                vertices = plankMesh.vertices,
                triangles = plankMesh.triangles,
                normals = plankMesh.normals,
                uv = plankMesh.uv,
            };
            combines[i].transform = Matrix4x4.TRS(position, rot, Vector3.one);
        }

        planksSubMesh = new SubMeshDescriptor(0, plankCount * plankMesh.triangles.Length)
        {
            firstVertex = 0,
            vertexCount = plankCount * plankMesh.vertexCount,
        };


        // Side Rails
        SubMeshDescriptor railsSubMesh;
        {
            var (vertices, normals) = RailVerticesAndNormals(railWidth / 2f);
            combines[^2].mesh =
             new Mesh
             {
                 vertices = vertices.ToArray(),
                 normals = normals.ToArray(),
                 triangles = RailTriangles().ToArray(),
             };
            combines[^2].transform = Matrix4x4.identity;
        }

        {
            var (vertices, normals) = RailVerticesAndNormals(-railWidth / 2f);
            var triangles = RailTriangles().ToArray();
            combines[^1].mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                normals = normals.ToArray(),
                triangles = triangles,
            };
            combines[^1].transform = Matrix4x4.identity;

            railsSubMesh = new SubMeshDescriptor(planksSubMesh.indexCount, triangles.Length * 2)
            {
                firstVertex = planksSubMesh.vertexCount,
                vertexCount = vertices.Count() * 2
            };
        }

        mesh.CombineMeshes(combines);
        mesh.subMeshCount = 2;
        mesh.SetSubMesh(0, planksSubMesh);
        mesh.SetSubMesh(1, railsSubMesh);

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private (List<Vector3>, List<Vector3>) RailVerticesAndNormals(float offsetMagnitude)
    {
        List<Vector3> railVertices = new List<Vector3>();
        List<Vector3> railNormals = new List<Vector3>();
        for (int ring = 0; ring < RailSegments; ring++)
        {
            float3 position, tangent, upVector;
            spline.Evaluate(ring / (RailSegments - 1f), out position, out tangent, out upVector);
            position += railScale * upVector;
            Quaternion rot = Quaternion.LookRotation(tangent, upVector);
            Vector3 railOffset = Quaternion.AngleAxis(90, upVector) * tangent;
            railOffset.Normalize();
            railOffset *= offsetMagnitude;
            for (int i = 0; i < rail2D.vertices.Length; i++)
            {
                railVertices.Add(railOffset + (Vector3)position + rot * (railScale * rail2D.vertices[i].position));
                railNormals.Add(rot * rail2D.vertices[i].normal);
            }

        }
        // caps
        {
            railVertices.AddRange(railVertices.TakeLast(8).Where((_, i) => i % 2 == 0));
            railVertices.AddRange(railVertices.Take(8).Where((_, i) => i % 2 == 0));

            float3 position, tangent, upVector;
            spline.Evaluate(1, out position, out tangent, out upVector);
            Quaternion rot = Quaternion.LookRotation(tangent, upVector);
            railNormals.AddRange(Enumerable.Repeat(rot * new Vector3(0, 0, 1), 4));

            spline.Evaluate(0, out position, out tangent, out upVector);
            rot = Quaternion.LookRotation(tangent, upVector);
            railNormals.AddRange(Enumerable.Repeat(rot * new Vector3(0, 0, -1), 4));
        }

        return (railVertices, railNormals);
    }

    private List<int> RailTriangles()
    {
        List<int> railTriangles = new List<int>();
        for (int ring = 0; ring < RailSegments - 1; ring++)
        {
            int rootIndex = ring * rail2D.vertices.Length;
            int rootIndexNext = (ring + 1) * rail2D.vertices.Length;
            for (int line = 0; line < rail2D.lineIndices.Length; line += 2)
            {
                int lineIndexA = rail2D.lineIndices[line];
                int lineIndexB = rail2D.lineIndices[line + 1];
                int currentA = rootIndex + lineIndexA;
                int currentB = rootIndex + lineIndexB;
                int nextA = rootIndexNext + lineIndexA;
                int nextB = rootIndexNext + lineIndexB;

                railTriangles.Add(currentA);
                railTriangles.Add(nextA);
                railTriangles.Add(nextB);
                railTriangles.Add(currentA);
                railTriangles.Add(nextB);
                railTriangles.Add(currentB);
            }
        }

        // caps
        var capVertsIndex = RailSegments * rail2D.vertices.Length;
        railTriangles.Add(capVertsIndex + 1);
        railTriangles.Add(capVertsIndex + 0);
        railTriangles.Add(capVertsIndex + 2);
        railTriangles.Add(capVertsIndex + 2);
        railTriangles.Add(capVertsIndex + 0);
        railTriangles.Add(capVertsIndex + 3);

        capVertsIndex += 4;
        railTriangles.Add(capVertsIndex + 0);
        railTriangles.Add(capVertsIndex + 1);
        railTriangles.Add(capVertsIndex + 2);
        railTriangles.Add(capVertsIndex + 2);
        railTriangles.Add(capVertsIndex + 3);
        railTriangles.Add(capVertsIndex + 0);


        return railTriangles;
    }

}
