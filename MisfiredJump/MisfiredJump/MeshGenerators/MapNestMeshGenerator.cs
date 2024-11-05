using System.Collections.Generic;
using UnityEngine;

namespace MisfiredJump
{
    public class MapNestMeshGenerator : MonoBehaviour
    {
        [SerializeField]
        private float _width; // thickness of the bars
        [SerializeField]
        private float _screenWidth = 1f;
        [SerializeField]
        private float _screenHeight = 1f;

        // Vertices
        private Vector2Int _nest;

        private Vector3 _backOffset;

        private Mesh _mesh;

        private Vector2Int _mapDimensions;

        private void Awake()
        {
            // Init offset, only constant here
            _backOffset = new Vector3(0f, 0f, _width);

            _nest = MisfiredJump.Instance.GetPodNestPosition();
            _mapDimensions = MisfiredJump.Instance.GetPodMapDimensions();

            // init mesh, and update once
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
            UpdateMesh();
            base.enabled = false;
        }

        private void UpdateMesh()
        {
            // Start from clean slates
            List<Vector3> verticeList = new List<Vector3>();
            List<int> triangleList = new List<int>();

            // Vertice
            float xCoord = (float)_nest.x * _screenWidth / (float)_mapDimensions.x;
            float yCoord = (float)_nest.y * _screenHeight / (float)_mapDimensions.y;
            verticeList.AddRange(MeshGeneratorTools.GenerateHex(new Vector3(xCoord, yCoord, 0f), _width));            
            int tot = verticeList.Count;
            verticeList.AddRange(MeshGeneratorTools.GenerateVerticeWithOffset(verticeList, _backOffset));

            // Triangles from hexes
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(0, tot));
            // Triangles from depth
            triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 6, 5, 4, 3, 2, 1 }, tot));

            _mesh.Clear();
            _mesh.vertices = verticeList.ToArray();
            _mesh.triangles = triangleList.ToArray();
        }
    }
}
