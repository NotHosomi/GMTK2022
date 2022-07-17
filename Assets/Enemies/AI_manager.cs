using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_manager : MonoBehaviour
{
    public static int total_enemies;
    public static AI_manager instance;
    public List<EnemyHP> queue;
    int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (queue.Count == 0)
            return;
        int m = queue.Count > 5 ? 5 : queue.Count;
        for (int i = 0; i < m; ++i)
        {
            Debug.Log("QC: " + queue.Count);
            Debug.Log("index: " + index);
            queue[index].updateTarget();
            ++index;
            if (index >= queue.Count)
                index = 0;
        }
    }
}
