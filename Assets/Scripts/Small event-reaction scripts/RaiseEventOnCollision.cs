using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Events;

public class RaiseEventOnCollision : MonoBehaviour
{
    [SerializeField]
    private string tagToCheckFor;
    [SerializeField]
    private VoidEvent eventToRaise;

    [SerializeField]
    private bool destroyObjectThatCollidesWithThis;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(tagToCheckFor))
        {
            eventToRaise.Raise();
            if (destroyObjectThatCollidesWithThis)
            {
                Destroy(other.transform.gameObject);
            }
        }

    }
}
