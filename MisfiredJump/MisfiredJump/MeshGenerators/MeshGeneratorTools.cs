using System.Collections.Generic;
using UnityEngine;

namespace MisfiredJump
{
    public static class MeshGeneratorTools
    {
        public static Vector3[] GenerateHex(Vector3 centralPoint, float width)
        {
            return new Vector3[]{
                centralPoint,
                centralPoint + new Vector3(width, 0f, 0f), // Right
                centralPoint + new Vector3(width/2f, -width*Mathf.Sqrt(3f)/2f, 0f), // Bottom Right
                centralPoint + new Vector3(-width/2f, -width*Mathf.Sqrt(3f)/2f, 0f), // Bottom Left
                centralPoint + new Vector3(-width, 0f, 0f), // Left
                centralPoint + new Vector3(-width/2f, width*Mathf.Sqrt(3f)/2f, 0f), // Top Left
                centralPoint + new Vector3(width/2f, width*Mathf.Sqrt(3f)/2f, 0f), // Top Right
            };
        }
        public static Vector3[] GenerateQuadlikeHex(Vector3 centralPoint, float width, bool quadLikeTop, bool quadLikeBottom)
        {
            var hex = GenerateHex(centralPoint, width);
            if (quadLikeTop)
            {
                hex[5] += new Vector3(-width / 2f, 0f, 0f); // shift TL a bit to the left
                hex[6] += new Vector3(width / 2f, 0f, 0f); // shift TR a bit to the right
            }
            if (quadLikeBottom)
            {
                hex[3] += new Vector3(-width / 2f, 0f, 0f); // shift BL a bit to the left
                hex[2] += new Vector3(width / 2f, 0f, 0f); // shift BR a bit to the right
            }

            return hex;
        }

        public static Vector3[] GeneratePodHex(Vector3 centralPoint, float width, Direction facing)
        {
            float pointiness = 3f;

            if (facing == Direction.East || facing == Direction.West) // East/West: flat bottom aka normal hex
            {
                // setup pointing
                float xRight = width;
                float xLeft = -width;
                if(facing == Direction.East)
                {
                    xRight *= pointiness;
                }
                else
                {
                    xLeft *= pointiness;
                }
                
                return new Vector3[]{
                    centralPoint,
                    centralPoint + new Vector3(xRight, 0f, 0f), // Right
                    centralPoint + new Vector3(width/2f, -width*Mathf.Sqrt(3f)/2f, 0f), // Bottom Right
                    centralPoint + new Vector3(-width/2f, -width*Mathf.Sqrt(3f)/2f, 0f), // Bottom Left
                    centralPoint + new Vector3(xLeft, 0f, 0f), // Left
                    centralPoint + new Vector3(-width/2f, width*Mathf.Sqrt(3f)/2f, 0f), // Top Left
                    centralPoint + new Vector3(width/2f, width*Mathf.Sqrt(3f)/2f, 0f), // Top Right
                };
            }
            else // North/South: flat sides so not the usual hex
            {
                // setup pointing
                float yTop = width;
                float yBottom = -width;
                if (facing == Direction.North)
                {
                    yTop *= pointiness;
                }
                else
                {
                    yBottom *= pointiness;
                }

                return new Vector3[]{
                    centralPoint,
                    centralPoint + new Vector3(0f, yTop, 0f), // Top
                    centralPoint + new Vector3(width*Mathf.Sqrt(3f)/2f, width/2f, 0f), // Top Right
                    centralPoint + new Vector3(width*Mathf.Sqrt(3f)/2f, -width/2f, 0f), // Bottom Right
                    centralPoint + new Vector3(0f, yBottom, 0f), // Bottom
                    centralPoint + new Vector3(-width*Mathf.Sqrt(3f)/2f, -width/2f, 0f), // Bottom Left
                    centralPoint + new Vector3(-width*Mathf.Sqrt(3f)/2f, width/2f, 0f), // Top Left
                };
            }


        }

        public static List<Vector3> GenerateVerticeWithOffset(List<Vector3> list, Vector3 offset)
        {
            var newList = new List<Vector3>();
            for (int i = 0; i < list.Count; i++)
            {
                newList.Add(list[i] + offset);
            }
            return newList;
        }
        private static int[] GenerateTrianglesFromQuad(int a, int b, int c, int d, bool clockwise)
        {
            if (clockwise)
            {
                return new int[]
                {
                    a, b, d,
                    d, b, c
                };
            }
            else
            {
                return new int[]
                {
                    a, d, b,
                    b, d, c
                };
            }
        }
        public static List<int> GenerateTrianglesFromQuadCompact(int a, int b, int c, int d, int tot)
        {
            var list = new List<int>();
            list.AddRange(GenerateTrianglesFromQuad(a, b, c, d, clockwise: true)); // front side
            list.AddRange(GenerateTrianglesFromQuad(a + tot, b + tot, c + tot, d + tot, clockwise: false)); // back side
            return list;
        }
        private static int[] GenerateDepthTrianglesFromEdge(int frontA, int frontB, int tot)
        {
            int backA = frontA + tot;
            int backB = frontB + tot;
            return GenerateTrianglesFromQuad(frontA, frontB, backB, backA, clockwise: true);
        }
        public static List<int> GenerateDepthTriangles(int[] frontVertice, int tot)
        {
            List<int> triangleList = new List<int>();
            for (int i = 0; i < frontVertice.Length - 1; i++)
            {
                triangleList.AddRange(GenerateDepthTrianglesFromEdge(frontVertice[i], frontVertice[i + 1], tot));
            }
            triangleList.AddRange(GenerateDepthTrianglesFromEdge(frontVertice[frontVertice.Length - 1], frontVertice[0], tot));

            return triangleList;
        }

        private static int[] GenerateTrianglesFromHex(int o, int a, int b, int c, int d, int e, int f, bool clockwise)
        {
            if (clockwise)
            {
                return new int[]
                {
                    o, a, b,
                    o, b, c,
                    o, c, d,
                    o, d, e,
                    o, e, f,
                    o, f, a
                };
            }
            else
            {
                return new int[]
                {
                    o, b, a,
                    o, c, b,
                    o, d, c,
                    o, e, d,
                    o, f, e,
                    o, a, f
                };
            }
        }
        public static List<int> GenerateTrianglesFromHexCompact(int o, int tot)
        {
            var list = new List<int>();
            list.AddRange(GenerateTrianglesFromHex(o, o + 1, o + 2, o + 3, o + 4, o + 5, o + 6, clockwise: true)); // front side
            list.AddRange(GenerateTrianglesFromHex(o + tot, o + 1 + tot, o + 2 + tot, o + 3 + tot, o + 4 + tot, o + 5 + tot, o + 6 + tot, clockwise: false)); // back side
            return list;
        }
    }
}
