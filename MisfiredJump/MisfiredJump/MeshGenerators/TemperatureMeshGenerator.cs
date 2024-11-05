using OWML.Common;
using System.Collections.Generic;
using UnityEngine;

namespace MisfiredJump
{
    public class TemperatureMeshGenerator : MonoBehaviour
    {
        [SerializeField]
        private float _width; // thickness of the bars
        [SerializeField]
        private float _screenWidth = 1f;

        // Vertices
        private Vector3[] _mainHex;
        private Vector3[] _pointHex;

        private Vector3 _backOffset;

        private Mesh _mesh;

        private float _targetTemperature = 0f;
        private float _maxTemperature = 10f;

        public void SetTemperature(float currentTemperature, float maxTemperature)
        {
            _targetTemperature = currentTemperature;
            _maxTemperature = maxTemperature;

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
            float temperaturePercent = _targetTemperature / _maxTemperature;
            float angle = temperaturePercent * Mathf.PI;

            _mainHex = MeshGeneratorTools.GenerateHex(Vector3.zero, _width);
            _pointHex = MeshGeneratorTools.GenerateHex(new Vector3(-Mathf.Cos(angle)*_screenWidth/2f, Mathf.Sin(angle)*_screenWidth/2f, 0f), _width);

            // Start from clean slates
            List<Vector3> verticeList = new List<Vector3>();
            List<int> triangleList = new List<int>();

            // Vertice
            verticeList.AddRange(_mainHex); // 0-6
            verticeList.AddRange(_pointHex); // 7-13
            int tot = verticeList.Count;
            verticeList.AddRange(MeshGeneratorTools.GenerateVerticeWithOffset(verticeList, _backOffset));

            // Triangles from hexes
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(0, tot));
            triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(7, tot));

            if(temperaturePercent <= (1f / 3f))
            {
                // Triangles from links
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(8, 13, 6, 5, tot)); // left, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(8, 5, 4, 9, tot)); // left, middle
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(9, 4, 3, 10, tot)); // left, lower
                // Triangles from depth
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 13, 12, 11, 10, 3, 2, 1, 6 }, tot));
            }
            else if(temperaturePercent <= (2f / 3f))
            {
                // Triangles from links
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(9, 8, 1, 6, tot)); // top, right
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(9, 6, 5, 10, tot)); // top, middle
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(10, 5, 4, 11, tot)); // top, left
                // Triangles from depth
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 8, 13, 12, 11, 4, 3, 2, 1 }, tot));
            }
            else
            {
                // Triangles from links
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(6, 5, 12, 11, tot)); // right, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(6, 11, 10, 1, tot)); // right, middle
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(1, 10, 9, 2, tot)); // right, lower
                // Triangles from depth
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 5, 4, 3, 2, 9, 8, 13, 12 }, tot));
            }

            _mesh.Clear();
            _mesh.vertices = verticeList.ToArray();
            _mesh.triangles = triangleList.ToArray();
        }
    }
}
