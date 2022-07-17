using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject enemy;

    int ticks = 1;

    // Update is called once per frame
    void Update()
    {
        float t = Time.timeSinceLevelLoad;
        if(t - (ticks-1)*30 > 0)
        {
            StartCoroutine(create((int)(ticks * ticks / (0.5f * ticks))));
            ticks += 1;
        }
    }

    public IEnumerator create(int amount)
    {
        for(int i = 0; i < amount; ++i)
        {
            Vector3 pos = transform.position;
            pos.x += Random.Range(-0.3f, 0.3f);
            pos.z += Random.Range(-0.3f, 0.3f);
            Instantiate(enemy, transform.position, transform.rotation);
            AI_manager.total_enemies++;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
    }
}
