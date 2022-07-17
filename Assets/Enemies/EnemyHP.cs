using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public int hp;
    [SerializeField] GameObject hitnum_prefab;

    public void damage(int dmg, Vector3 hit_loc)
    {
        hp -= dmg;
        if (hp < 0)
            Destroy(gameObject);

        GameObject hitnum = Instantiate(hitnum_prefab, hit_loc, Quaternion.identity);
        hitnum.GetComponent<HitNumber>().init(dmg);
    }
}
