using UnityEngine;

namespace MyFramework
{
    public static class MeshUtil
    {
        public static Mesh CreateGridMesh(float size)
        {
            Mesh result = new Mesh();
            result.name = "Grid";

            Vector3[] vertices = new Vector3[4];
            /*
             * 2 3
             * 0 1
             */
            vertices[0] = Vector3.zero;
            vertices[1] = new Vector3(size, 0f, 0f);
            vertices[2] = new Vector3(0f, 0f, size);
            vertices[3] = new Vector3(size, 0f, size);

            int[] indices = new int[6];
            indices[0] = 0;
            indices[1] = 3;
            indices[2] = 1;

            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;

            Vector2[] uvs = new Vector2[4];
            uvs[0] = new Vector2(0f, 0f);
            uvs[1] = new Vector2(1f, 0f);
            uvs[2] = new Vector2(0f, 1f);
            uvs[3] = new Vector2(1f, 1f);

            result.vertices = vertices;
            result.triangles = indices;
            result.uv = uvs;

            return result;
        }
    }
}