using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//More like "explode and kill enemies caught in explosion".
public class AreaExplosion : MonoBehaviour
{
    [SerializeField]
    private float radius;
    [SerializeField]
    private float applyExplosionForce;
    [SerializeField]
    private float force, upwardsModifier;

    [SerializeField]
    private LayerMask layerMask;

    [Header("Gizmo-settings:")]
    [SerializeField]
    private bool drawImpactAreaGizmo;


    private void OnDrawGizmos()
    {
        if (drawImpactAreaGizmo)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, radius);
        }
    }

    public void KillEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);

        foreach (Collider other in colliders)
        {
            other.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius, upwardsModifier, ForceMode.Impulse);
        }

    }
}
