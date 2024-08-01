using System.Collections;
using UnityEngine;

namespace BA.Flight
{
    public class MissleExplodeRadius : MonoBehaviour
    {
        public float force = 10;
        public float radius = 10;
        public float destroyRadius = 10;

        public void AddForce()
        {

                Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
                foreach (Collider collider in colliders)
                {
                    try
                    {
                        collider.transform.GetComponentInParent<Rigidbody>().AddExplosionForce(force, transform.position, radius);
                    }
                   catch
                    {
    
                    }
                }
                Collider[] collidersDestroy = Physics.OverlapSphere(transform.position, destroyRadius);
                foreach (Collider collider in colliders)
                {
                    Destroy(collider.gameObject);
                }
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
            Gizmos.DrawWireSphere(transform.position, destroyRadius);
        }
    }
}