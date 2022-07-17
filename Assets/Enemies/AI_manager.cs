using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_manager : MonoBehaviour
{

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
        for (int i = 0; i < 5; ++i)
        {
            queue[index].updateTarget();
            ++index;
            if (index > queue.Count)
                index = 0;
        }
    }
}
