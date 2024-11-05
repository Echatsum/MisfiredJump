using OWML.Common;
using System.Collections.Generic;
using UnityEngine;

namespace MisfiredJump
{
    public class MapCrossMeshGenerator : MonoBehaviour
    {
        [SerializeField]
        private float _width; // thickness of the bars
        [SerializeField]
        private float _screenWidth = 1f;
        [SerializeField]
        private float _screenHeight = 1f;

        // Vertices
        private Vector3[] _leftHex;
        private Vector3[] _topHex;
        private Vector3[] _rightHex;
        private Vector3[] _bottomHex;
        private Vector3[] _middleHex;

        private Vector3 _backOffset;

        private Mesh _mesh;

        private Vector2Int _targetPosition;
        private Vector2Int _mapDimensions;

        public void SetCoordinates(Vector2Int position, Vector2Int mapDimensions)
        {
            _targetPosition = position;
            _mapDimensions = mapDimensions;

            UpdateMesh();
        }

        private void Awake()
        {
            // Init offset, only constant here
            _backOffset = new Vector3(0f, 0f, _width);

            _targetPosition = new Vector2Int(1, 1); // safety init value
            _mapDimensions = new Vector2Int(2, 2); // safety init value

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

            _bottomHex = MeshGeneratorTools.GenerateQuadlikeHex(new Vector3(xCoord, 0f, 0f), _width, quadLikeBottom: false, quadLikeTop: true);
            _topHex = MeshGeneratorTools.GenerateQuadlikeHex(new Vector3(xCoord, _screenHeight, 0f), _width, quadLikeBottom: true, quadLikeTop: false);
            _leftHex = MeshGeneratorTools.GenerateHex(new Vector3(0f, yCoord, 0f), _width);
            _rightHex = MeshGeneratorTools.GenerateHex(new Vector3(_screenWidth, yCoord, 0f), _width);
            _middleHex = MeshGeneratorTools.GenerateQuadlikeHex(new Vector3(xCoord, yCoord, 0f), _width, quadLikeBottom: true, quadLikeTop: true);

            // Start from clean slates
            List<Vector3> verticeList = new List<Vector3>();
            List<int> triangleList = new List<int>(); // Note for better perfs later: for the cross, triangles never change, only vertice positions

            // Vertice
            verticeList.AddRange(_leftHex); // 0-6
            verticeList.AddRange(_topHex); // 7-13
            verticeList.AddRange(_rightHex); // 14-20
            verticeList.AddRange(_bottomHex); // 21-27
            verticeList.AddRange(_middleHex); // 28-34
            int tot = verticeList.Count;
            verticeList.AddRange(MeshGeneratorTools.GenerateVerticeWithOffset(verticeList, _backOffset));

            // Triangles from hexes
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(0, tot));
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(7, tot));
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(14, tot));
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(21, tot));
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(28, tot));
            // Triangles from links
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(1, 6, 33, 32, tot)); // left link, upper
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(1, 32, 31, 2, tot)); // left link, lower
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(34, 33, 10, 9, tot)); // top link
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(29, 34, 19, 18, tot)); // right link, upper
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(29, 18, 17, 30, tot)); // right link, lower
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(31, 30, 27, 26, tot)); // bottom link
            // Triangles from depth
            triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 6, 5, 4, 3, 2, 31, 26, 25, 24, 23, 22, 21, 27, 30, 17, 16, 15, 20, 19, 34, 9, 8, 13, 12, 11, 10, 33 }, tot));

            _mesh.Clear();
            _mesh.vertices = verticeList.ToArray();
            _mesh.triangles = triangleList.ToArray();
        }
    }
}
