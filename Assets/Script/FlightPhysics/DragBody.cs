using BA.Flight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BA.Flight
{
    public class DragBody : MonoBehaviour
    {
        public Vector3 size = Vector3.one;
        public GameObject aircraft;
        private Rigidbody aircraftRB;
        private Aircraft aircraftComponent;

        private Mesh surfaceMesh;

        private void Start()
        {
            aircraftRB = aircraft.GetComponent<Rigidbody>();
            aircraftComponent = aircraft.GetComponent<Aircraft>();

            surfaceMesh = new Mesh();
            Vector3[] vertices = new Vector3[] {
            new Vector3(-size.x / 2, size.y / 2, size.z / 2),
            new Vector3(size.x / 2,  size.y / 2, size.z / 2),
            new Vector3(-size.x / 2, size.y / 2, -size.z / 2),
            new Vector3(size.x / 2,  size.y / 2, -size.z / 2),
            new Vector3(-size.x / 2, -size.y / 2, size.z / 2),
            new Vector3(size.x / 2,  -size.y / 2, size.z / 2),
            new Vector3(-size.x / 2, -size.y / 2, -size.z / 2),
            new Vector3(size.x / 2,  -size.y / 2, -size.z / 2)
        };
            int[] triangles = new int[] { 0, 1, 2, 1, 3, 2, 0, 2, 6, 0, 6, 4, 0, 5, 1, 0, 4, 5, 1, 7, 3, 1, 5, 7, 2, 3, 7, 2, 7, 6, 4, 6, 5, 5, 6, 7 };
            surfaceMesh.vertices = vertices;
            surfaceMesh.triangles = triangles;
            surfaceMesh.RecalculateNormals();
        }

        private void OnValidate()
        {
            aircraftRB = aircraft.GetComponent<Rigidbody>();
            aircraftComponent = aircraft.GetComponent<Aircraft>();

            surfaceMesh = new Mesh();
            Vector3[] vertices = new Vector3[] {
            new Vector3(-size.x / 2, size.y / 2, size.z / 2),
            new Vector3(size.x / 2,  size.y / 2, size.z / 2),
            new Vector3(-size.x / 2, size.y / 2, -size.z / 2),
            new Vector3(size.x / 2,  size.y / 2, -size.z / 2),
            new Vector3(-size.x / 2, -size.y / 2, size.z / 2),
            new Vector3(size.x / 2,  -size.y / 2, size.z / 2),
            new Vector3(-size.x / 2, -size.y / 2, -size.z / 2),
            new Vector3(size.x / 2,  -size.y / 2, -size.z / 2)
        };
            int[] triangles = new int[] { 0, 1, 2, 1, 3, 2, 0, 2, 6, 0, 6, 4, 0, 5, 1, 0, 4, 5, 1, 7, 3, 1, 5, 7, 2, 3, 7, 2, 7, 6, 4, 6, 5, 5, 6, 7 };
            surfaceMesh.vertices = vertices;
            surfaceMesh.triangles = triangles;
            surfaceMesh.RecalculateNormals();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawMesh(surfaceMesh, 0, transform.position, transform.rotation);
        }

        private void FixedUpdate()
        {
            UpdateDrag();
        }

        private void UpdateDrag()
        {
            Vector3 normalizedVelo = aircraftRB.velocity.normalized;
            float dymaticDragArea = size.x * Mathf.Abs(normalizedVelo.x) + size.y * Mathf.Abs(normalizedVelo.y) + size.z + Mathf.Abs(normalizedVelo.z);
            float dragForce = 0.5f * dymaticDragArea * aircraftRB.velocity.sqrMagnitude * aircraftComponent.dragCoiffient;
            Vector3 drag = dragForce * -normalizedVelo;
            Debug.DrawLine(transform.position, transform.position + drag, Color.red);
            aircraftRB.AddRelativeForce(drag);
        }
    }
}
