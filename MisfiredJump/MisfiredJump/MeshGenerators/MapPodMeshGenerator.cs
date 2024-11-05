using OWML.Common;
using System.Collections.Generic;
using UnityEngine;

namespace MisfiredJump
{
    public class MapPodMeshGenerator : MonoBehaviour
    {
        [SerializeField]
        private float _width = 1f; // thickness of the bars
        [SerializeField]
        private float _screenWidth = 1f;
        [SerializeField]
        private float _screenHeight = 1f;

        // Vertices
        private Vector3[] _middleHex;

        private Vector3 _backOffset;

        private Mesh _mesh;

        private Vector2Int _targetPosition = Vector2Int.zero;
        private Direction _facing;
        private Vector2Int _mapDimensions = Vector2Int.one;

        public void SetCoordinates(Vector2Int position, Direction facing, Vector2Int mapDimensions)
        {
            _targetPosition = position;
            _facing = facing;
            _mapDimensions = mapDimensions;

            UpdateMesh();
        }

        private void Awake()
        {
            // Init offset, only constant here
            _backOffset = new Vector3(0f, 0f, _width);

            // init mesh, and update once
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
            UpdateMesh();
            base.enabled = false;
        }

        private void UpdateMesh()
        {
            // Generate hexes
            float xCoord = (float)_targetPosition.x * _screenWidth / (float)_mapDimensions.x;
            float yCoord = (float)_targetPosition.y * _screenHeight / (float)_mapDimensions.y;

            _middleHex = MeshGeneratorTools.GeneratePodHex(new Vector3(xCoord, yCoord, 0f), _width, _facing);

            // Start from clean slates
            List<Vector3> verticeList = new List<Vector3>();
            List<int> triangleList = new List<int>(); // Note for better perfs later: for the cross, triangles never change, only vertice positions

            // Vertice
            verticeList.AddRange(_middleHex); // 0-6
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
