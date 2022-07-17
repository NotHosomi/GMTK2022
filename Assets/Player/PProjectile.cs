using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PProjectile : MonoBehaviour
{
    public int damage;
    [SerializeField] ParticleSystem particle_sys;

    private void Start()
    {
        GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)).normalized * 12;
    }

    private void OnCollisionEnter(Collision collision)
    {
        EnemyHP e = collision.gameObject.GetComponent<EnemyHP>();
        if (e)
        {
            e.damage(damage, collision.contacts[0].point);

        }
        Instantiate(particle_sys, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
