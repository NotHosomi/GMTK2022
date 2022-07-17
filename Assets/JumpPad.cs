using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] Vector3 vel;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(rb)
        {
            rb.velocity *= 0;
            rb.AddForce(vel);
        }
        PMove p = other.GetComponent<PMove>();
        if(p)
        {
            p.isGrounded = false;
        }
    }

    [SerializeField] float prediction_timestep;
    [SerializeField] int prediction_iterations;
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        Vector3 v = vel;
        for (int i = 0; i < prediction_iterations; ++i)
        {
            Gizmos.DrawLine(pos, pos + v * prediction_timestep);
            pos = pos + v * prediction_timestep;
            v.y -= 20 * prediction_timestep;
        }
    }
}
