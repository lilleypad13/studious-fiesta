using UnityEngine;
using System.Collections.Generic;

public class CubeDrawer : MonoBehaviour
{
    public Material cubeMaterial;
    private const float CubeRadius = 0.25f;
    private int cubesMade;

    static readonly List<Vector3> batchedVertices = new List<Vector3>(24);
    static readonly List<int> batchedTriangles = new List<int>(36);
    static readonly List<Color> batchedColors = new List<Color>(24);

    static readonly List<MeshWrapper> batches = new List<MeshWrapper>();

    static readonly Vector3[] verts = {
        new Vector3 (-CubeRadius, CubeRadius, CubeRadius),
        new Vector3 (-CubeRadius, -CubeRadius, CubeRadius),
        new Vector3 (-CubeRadius, -CubeRadius, -CubeRadius),
        new Vector3 (-CubeRadius, CubeRadius, -CubeRadius),

        new Vector3 (CubeRadius, CubeRadius, CubeRadius),
        new Vector3 (CubeRadius, -CubeRadius, CubeRadius),
        new Vector3 (CubeRadius, -CubeRadius, -CubeRadius),
        new Vector3 (CubeRadius, CubeRadius, -CubeRadius),

        new Vector3 (CubeRadius, -CubeRadius, CubeRadius),
        new Vector3 (-CubeRadius, -CubeRadius, CubeRadius),
        new Vector3 (-CubeRadius, -CubeRadius, -CubeRadius),
        new Vector3 (CubeRadius, -CubeRadius, -CubeRadius),

        new Vector3 (CubeRadius, CubeRadius, CubeRadius),
        new Vector3 (CubeRadius, CubeRadius, -CubeRadius),
        new Vector3 (-CubeRadius, CubeRadius, -CubeRadius),
        new Vector3 (-CubeRadius, CubeRadius, CubeRadius),

        new Vector3 (CubeRadius, CubeRadius, -CubeRadius),
        new Vector3 (CubeRadius, -CubeRadius, -CubeRadius),
        new Vector3 (-CubeRadius, -CubeRadius, -CubeRadius),
        new Vector3 (-CubeRadius, CubeRadius, -CubeRadius),

        new Vector3 (CubeRadius, CubeRadius, CubeRadius),
        new Vector3 (CubeRadius, -CubeRadius, CubeRadius),
        new Vector3 (-CubeRadius, -CubeRadius, CubeRadius),
        new Vector3 (-CubeRadius, CubeRadius, CubeRadius)
    };

    static readonly int[] tris = {
        2, 1, 0, 0, 3, 2,
        4, 5, 6, 6, 7, 4,
        8, 9, 10, 10, 11, 8,
        12, 13, 14, 14, 15, 12,
        16, 17, 18, 18, 19, 16,
        22, 21, 20, 20, 23, 22
    };

    struct MeshWrapper
    {
        public Mesh mesh;
        public Vector3 location;
    }

    void Start()
    {
        cubesMade = 0;

        //for (int i = 0; i < 500; i++)
        //{
        //    Vector3 offset = new Vector3(i * 2.0f, 0.0f, 0.0f);
        //    AddCubeToBatch(offset, Color.red);
        //}

        //AddCubeToBatch(new Vector3(3.0f, 3.0f, 3.0f), Color.blue);
    }

    void Update()
    {
        for (int i = 0; i < batches.Count; i++)
        {
            MeshWrapper wrapper = batches[i];
            Graphics.DrawMesh(wrapper.mesh, wrapper.location, Quaternion.identity, cubeMaterial, 0);
        }
    }

    public void AddCubeToBatch(Vector3 centralPosition, Color color)
    {
        for (int vertIndex = 0; vertIndex < verts.Length; vertIndex++)
        {
            batchedVertices.Add(verts[vertIndex] + centralPosition);
            batchedColors.Add(color);
        }

        for (int triIndex = 0; triIndex < tris.Length; triIndex++)
        {
            //batchedTriangles.Add(tris[triIndex] + cubesMade * verts.Length);
            batchedTriangles.Add(tris[triIndex]);
        }
    
        // Set necessary mesh parameters
        Mesh batchedMesh = new Mesh();
        batchedMesh.SetVertices(batchedVertices);
        batchedMesh.SetColors(batchedColors);
        batchedMesh.SetTriangles(batchedTriangles, 0);
        batchedMesh.Optimize();
        batchedMesh.UploadMeshData(true);

        // Add current mesh wrapper to total list for rendering
        batches.Add(new MeshWrapper { mesh = batchedMesh, location = Vector3.zero });

        // Clear out lists to prepare for next cube
        batchedVertices.Clear();
        batchedColors.Clear();
        batchedTriangles.Clear();

        cubesMade += 1;
    }

    public void ResetCubeDrawer()
    {
        batches.Clear();
        cubesMade = 0;
    }
    
}
