using OWML.Common;
using System.Collections.Generic;
using UnityEngine;

namespace MisfiredJump
{
    public class SymbolMeshGenerator : MonoBehaviour
    {
        [SerializeField]
        private float _width; // thickness of the bars

        // Vertice of front face
        private Vector3[] _middleHexFront;
        private Vector3[] _rightHexFront;
        private Vector3[] _leftHexFront;
        private Vector3[] _topHexFront;
        private Vector3[] _bottomHexFront;
        private Vector3 _extraTopRightFront;
        private Vector3 _extraBottomRightFront;
        private Vector3 _extraBottomLeftFront;
        private Vector3 _extraTopLeftFront;
        private Vector3 _extraTopFront;
        private Vector3 _extraBottomFront;

        private Vector3 _backOffset;

        private Mesh _mesh;

        [SerializeField]
        private int _targetSymbol;
        public void SetSymbol(int newTarget)
        {
            if(newTarget < 0 || newTarget > 5) // hardcoding from 0-5 because that's what I'm going for
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"Attempted to set an invalid value for symbol! Value: {newTarget}", MessageType.Error);
                return;
            }

            _targetSymbol = newTarget;
            UpdateMesh();
        }
        public int GetSymbol()
        {
            return _targetSymbol;
        }
        public int GetSymbolOptionCount()
        {
            return 6;
        }

        private void Awake()
        {
            // Init all possible vertice once for later use
            _middleHexFront = MeshGeneratorTools.GenerateHex(new Vector3(0f, 0f, 0f), _width);
            _rightHexFront = MeshGeneratorTools.GenerateHex(new Vector3(1f, 0f, 0f), _width);
            _leftHexFront = MeshGeneratorTools.GenerateHex(new Vector3(-1f, 0f, 0f), _width);
            _topHexFront = MeshGeneratorTools.GenerateHex(new Vector3(0f, Mathf.Sqrt(3f), 0f), _width);
            _bottomHexFront = MeshGeneratorTools.GenerateHex(new Vector3(0f, -Mathf.Sqrt(3f), 0f), _width);
            _extraTopRightFront = _rightHexFront[5] - new Vector3(_width, 0f, 0f);
            _extraBottomRightFront = _rightHexFront[3] - new Vector3(_width, 0f, 0f);
            _extraBottomLeftFront = _leftHexFront[2] + new Vector3(_width, 0f, 0f);
            _extraTopLeftFront = _leftHexFront[6] + new Vector3(_width, 0f, 0f);
            _extraTopFront = _topHexFront[0] + new Vector3(0f, -_width*Mathf.Sqrt(3f), 0f);
            _extraBottomFront = _bottomHexFront[0] + new Vector3(0f, _width * Mathf.Sqrt(3f), 0f);

            _backOffset = new Vector3(0f, 0f, _width);

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

            if (_targetSymbol == 0)
            {
                // Vertice. For 0, it's only the middle hex
                verticeList.AddRange(_middleHexFront); // 0-6
                int tot = verticeList.Count;
                verticeList.AddRange(MeshGeneratorTools.GenerateVerticeWithOffset(verticeList, _backOffset));

                // Triangles from hexes
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(0, tot));
                // Triangles from depth
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 1, 6, 5, 4, 3, 2 }, tot));
            }
            else if(_targetSymbol == 1)
            {
                // Vertice. For 1, it's left, right hexes
                verticeList.AddRange(_leftHexFront); // 0-6
                verticeList.AddRange(_rightHexFront); // 7-13
                int tot = verticeList.Count;
                verticeList.AddRange(MeshGeneratorTools.GenerateVerticeWithOffset(verticeList, _backOffset));

                // Triangles from hexes
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(0, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(7, tot));
                // Triangles from links
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(1, 6, 12, 11, tot)); // middle link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(1, 11, 10, 2, tot)); // middle link, lower
                // Triangles from depth
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 6, 5, 4, 3, 2, 10, 9, 8, 13, 12 }, tot));
            }
            else if(_targetSymbol == 2)
            {
                // Vertice. For 2, it's left, top, right hexes, but also the top extra
                verticeList.AddRange(_leftHexFront); // 0-6
                verticeList.AddRange(_topHexFront); // 7-13
                verticeList.AddRange(_rightHexFront); // 14-20
                verticeList.Add(_extraTopFront); // 21
                int tot = verticeList.Count;
                verticeList.AddRange(MeshGeneratorTools.GenerateVerticeWithOffset(verticeList, _backOffset));

                // Triangles from hexes
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(0, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(7, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(14, tot));
                // Triangles from extras
                triangleList.AddRange(new int[] { 9, 21, 10 });
                triangleList.AddRange(new int[] { 9 + tot, 10 + tot, 21 + tot });
                // Triangles from links
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(6, 5, 11, 10, tot)); // topleft link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(6, 10, 21, 1, tot)); // topleft link, lower
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(9, 8, 20, 19, tot)); // topright link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(9, 19, 18, 21, tot)); // topright link, lower
                // Triangles from depth
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 5, 4, 3, 2, 1, 21, 18, 17, 16, 15, 20, 8, 13, 12, 11 }, tot));
            }
            else if(_targetSymbol == 3)
            {
                // Vertice. For 3, it's left, top, right, but also the three top extras
                verticeList.AddRange(_leftHexFront); // 0-6
                verticeList.AddRange(_topHexFront); // 7-13
                verticeList.AddRange(_rightHexFront); // 14-20
                verticeList.Add(_extraTopLeftFront); // 21
                verticeList.Add(_extraTopFront); // 22
                verticeList.Add(_extraTopRightFront); // 23
                int tot = verticeList.Count;
                verticeList.AddRange(MeshGeneratorTools.GenerateVerticeWithOffset(verticeList, _backOffset));

                // Triangles from hexes
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(0, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(7, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(14, tot));
                // Triangles from extras
                triangleList.AddRange(new int[] { 1, 6, 21 });
                triangleList.AddRange(new int[] { 10, 9, 22 });
                triangleList.AddRange(new int[] { 18, 23, 19 });
                triangleList.AddRange(new int[] { 1 + tot, 21 + tot, 6 + tot });
                triangleList.AddRange(new int[] { 10 + tot, 22 + tot, 9 + tot });
                triangleList.AddRange(new int[] { 18 + tot, 19 + tot, 23 + tot });
                // Triangles from links
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(6, 5, 11, 10, tot)); // topleft link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(6, 10, 22, 21, tot)); // topleft link, lower
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(9, 8, 20, 19, tot)); // topright link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(9, 19, 23, 22, tot)); // topright link, lower
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(1, 21, 23, 18, tot)); // middle link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(1, 18, 17, 2, tot)); // middle link, lower
                // Triangles from depth
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 5, 4, 3, 2, 17, 16, 15, 20, 8, 13, 12, 11 }, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 21, 22, 23 }, tot));
            }
            else if(_targetSymbol == 4)
            {
                // Vertice. For 4, it's left, top, right, bottom, but also top and bottom extras
                verticeList.AddRange(_leftHexFront); // 0-6
                verticeList.AddRange(_topHexFront); // 7-13
                verticeList.AddRange(_rightHexFront); // 14-20
                verticeList.AddRange(_bottomHexFront); // 21-27
                verticeList.Add(_extraTopFront); // 28
                verticeList.Add(_extraBottomFront); // 29
                int tot = verticeList.Count;
                verticeList.AddRange(MeshGeneratorTools.GenerateVerticeWithOffset(verticeList, _backOffset));

                // Triangles from hexes
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(0, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(7, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(14, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(21, tot));
                // Triangles from extras
                triangleList.AddRange(new int[] { 9, 28, 10 });
                triangleList.AddRange(new int[] { 27, 26, 29 });
                triangleList.AddRange(new int[] { 9 + tot, 10 + tot, 28 + tot });
                triangleList.AddRange(new int[] { 27 + tot, 29 + tot, 26 + tot });
                // Triangles from links
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(6, 5, 11, 10, tot)); // topleft link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(6, 10, 28, 1, tot)); // topleft link, lower
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(9, 8, 20, 19, tot)); // topright link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(9, 19, 18, 28, tot)); // topright link, lower
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(2, 1, 29, 26, tot)); // bottomleft link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(2, 26, 25, 3, tot)); // bottomleft link, lower
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(27, 29, 18, 17, tot)); // bottomright link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(27, 17, 16, 22, tot)); // bottomright link, lower
                // Triangles from depth
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 5, 4, 3, 25, 24, 23, 22, 16, 15, 20, 8, 13, 12, 11}, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 1, 28, 18, 29 }, tot));
            }
            else
            {
                // Vertice. For 5, it's left, top, right, bottom, but also all six extras
                verticeList.AddRange(_leftHexFront); // 0-6
                verticeList.AddRange(_topHexFront); // 7-13
                verticeList.AddRange(_rightHexFront); // 14-20
                verticeList.AddRange(_bottomHexFront); // 21-27
                verticeList.Add(_extraTopLeftFront); // 28
                verticeList.Add(_extraTopFront); // 29
                verticeList.Add(_extraTopRightFront); // 30
                verticeList.Add(_extraBottomLeftFront); // 31
                verticeList.Add(_extraBottomFront); // 32
                verticeList.Add(_extraBottomRightFront); // 33
                int tot = verticeList.Count;
                verticeList.AddRange(MeshGeneratorTools.GenerateVerticeWithOffset(verticeList, _backOffset));

                // Triangles from hexes
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(0, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(7, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(14, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromHexCompact(21, tot));
                // Triangles from extras
                triangleList.AddRange(new int[] { 1, 6, 28 });
                triangleList.AddRange(new int[] { 9, 29, 10 });
                triangleList.AddRange(new int[] { 19, 18, 30 });
                triangleList.AddRange(new int[] { 1, 31, 2 });
                triangleList.AddRange(new int[] { 26, 32, 27 });
                triangleList.AddRange(new int[] { 17, 33, 18 });
                triangleList.AddRange(new int[] { 1 + tot, 28 + tot, 6 + tot });
                triangleList.AddRange(new int[] { 9 + tot, 10 + tot, 29 + tot });
                triangleList.AddRange(new int[] { 19 + tot, 30 + tot, 18 + tot });
                triangleList.AddRange(new int[] { 1 + tot, 2 + tot, 31 + tot });
                triangleList.AddRange(new int[] { 26 + tot, 27 + tot, 32 + tot });
                triangleList.AddRange(new int[] { 17 + tot, 18 + tot, 33 + tot });
                // Triangles from links
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(6, 5, 11, 10, tot)); // topleft link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(6, 10, 29, 28, tot)); // topleft link, lower
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(9, 8, 20, 19, tot)); // topright link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(9, 19, 30, 29, tot)); // topright link, lower
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(1, 28, 30, 18, tot)); // middle link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(1, 18, 33, 31, tot)); // middle link, lower
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(2, 31, 32, 26, tot)); // bottomleft link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(2, 26, 25, 3, tot)); // bottomleft link, lower
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(27, 32, 33, 17, tot)); // bottomright link, upper
                triangleList.AddRange(MeshGeneratorTools.GenerateTrianglesFromQuadCompact(27, 17, 16, 22, tot)); // bottomright link, lower
                // Triangles from depth
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 5, 4, 3, 25, 24, 23, 22, 16, 15, 20, 8, 13, 12, 11 }, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 28, 29, 30 }, tot));
                triangleList.AddRange(MeshGeneratorTools.GenerateDepthTriangles(new int[] { 31, 33, 32 }, tot));
            }

            _mesh.Clear();
            _mesh.vertices = verticeList.ToArray();
            _mesh.triangles = triangleList.ToArray();
        }
    }
}
