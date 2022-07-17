using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float xr = 0f;
    public float yr = 0f;   // Update is called once per frame
    public float zr = 0.1f;
    float counter = 0f;

    void Update()
    {
        xr = Mathf.Abs(Mathf.Sin(counter/1000))/2;
        yr = Mathf.Min(Mathf.Abs(Mathf.Tan(counter * 0.75f / 1000)) / 2, 0.5f) + 0.1f;
        
        transform.Rotate(new Vector3(xr, yr, zr));
        counter++;
    }
}
