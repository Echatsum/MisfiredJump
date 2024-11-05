using System.Collections.Generic;
using UnityEngine;

namespace MisfiredJump
{
    public class MapObstacleMeshGenerator : MonoBehaviour
    {
        [SerializeField]
        private float _width; // thickness of the bars
        [SerializeField]
        private float _screenWidth = 1f;
        [SerializeField]
        private float _screenHeight = 1f;

        // Vertices
        private Vector2Int[] _obstacles;

        private Vector3 _backOffset;

        private Mesh _mesh;

        private Vector2Int _mapDimensions;

        private void Awake()
        {
            // Init offset, only constant here
            _backOffset = new Vector3(0f, 0f, _width);

            _obstacles = MisfiredJump.Instance.GetPodObstaclePositions();
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
            foreach (var obstacle in _obstacles)
            {
                float xCoord = (float)obstacle.x * _screenWidth / (float)_mapDimensions.x;
                float yCoord = (float)obstacle.y * _screenHeight / (float)_mapDimensions.y;
                verticeList.AddRange(MeshGeneratorTools.GenerateHex(new Vector3(xCoord, yCoord, 0f), _width));
            }
            int tot = verticeList.Count;
            verticeList.AddRange(MeshGeneratorTools.GenerateVerticeWithOffset(verticeList, _backOffset));

            for(int i=0; i<_obstacles.Length; i++)
            {
                int index = 7 * i;

                // Triangles from hexes
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(index, tot));
                // Triangles from depth
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 6+index, 5+index, 4+index, 3+index, 2+index, 1+index }, tot));
            }

            _mesh.Clear();
            _mesh.vertices = verticeList.ToArray();
            _mesh.triangles = triangleList.ToArray();
        }
    }
}
