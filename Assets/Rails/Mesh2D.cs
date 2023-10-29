using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Mesh2D : ScriptableObject
{
    [Serializable]
    public struct Vertex
    {
        public Vector2 position;
        public Vector2 normal;
        public float u; // UV without V
    }

    public Vertex[] vertices;
    public int[] lineIndices;
}
