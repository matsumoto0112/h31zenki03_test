using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldRotater : MonoBehaviour
{

    [SerializeField]
    private float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.R))
        {
            this.transform.Rotate(new Vector3(0, 0, speed));
        }
        else if (Input.GetKey(KeyCode.L))
        {
            this.transform.Rotate(new Vector3(0, 0, -speed));
        }
    }
}
