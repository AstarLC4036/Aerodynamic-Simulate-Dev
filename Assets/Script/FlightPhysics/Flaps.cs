using System.Collections;
using UnityEngine;

namespace Assets.Script.Flight
{
    public class Flaps : MonoBehaviour
    {
        /*
        public Vector2 size = Vector2.one;
        private Mesh surfaceMesh;

        // Use this for initialization
        void Start()
        {
            surfaceMesh = new Mesh();
            Vector3[] vertices = new Vector3[] {
                new Vector3(-size.x / 2, 0, size.y / 2),
                new Vector3(size.x / 2, 0,  size.y / 2),
                new Vector3(-size.x / 2, 0, -size.y / 2),
                new Vector3(size.x / 2, 0,  -size.y / 2)
            };
            int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };
            surfaceMesh.vertices = vertices;
            surfaceMesh.triangles = triangles;
            surfaceMesh.RecalculateNormals();
        }

        private void OnValidate()
        {
            if (surfaceMesh == null) surfaceMesh = new Mesh();
            Vector3[] vertices = new Vector3[] {
                new Vector3(-size.x / 2, 0, size.y / 2),
                new Vector3(size.x / 2, 0,  size.y / 2),
                new Vector3(-size.x / 2, 0, -size.y / 2),
                new Vector3(size.x / 2, 0,  -size.y / 2)
            };
            int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };
            surfaceMesh.Clear();
            surfaceMesh.vertices = vertices;
            surfaceMesh.triangles = triangles;
            surfaceMesh.RecalculateNormals();
        }
        */

        public void SetAngle(float angle)
        {

        }
    }
}