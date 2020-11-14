using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoateScript : MonoBehaviour
{
    public Vector3 rotation = new Vector3(1,0,1);


    void Update()
    {
        transform.Rotate(rotation, Space.Self);
    }
}
