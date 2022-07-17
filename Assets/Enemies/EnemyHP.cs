using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHP : MonoBehaviour
{
    public int hp;
    [SerializeField] GameObject hitnum_prefab;
    private NavMeshAgent agent;
    Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = PMove.instance.gameObject.transform;
        AI_manager.instance.queue.Add(this);
    }
    public void damage(int dmg, Vector3 hit_loc)
    {
        hp -= dmg;
        if (hp < 0)
        {
            Destroy(gameObject);
            AI_manager.instance.queue.Remove(this);
        }

        GameObject hitnum = Instantiate(hitnum_prefab, hit_loc, Quaternion.identity);
        hitnum.GetComponent<HitNumber>().init(dmg);
    }

    public void updateTarget()
    {
        agent.SetDestination(target.position);
    }

    
}
