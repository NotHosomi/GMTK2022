using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PAttack : MonoBehaviour
{
    [SerializeField] GameObject[] UI_dice;
    [SerializeField] GameObject projectile;
    int LM;
    GameObject highlight_face;

    int[] v;

    // Start is called before the first frame update
    void Start()
    {
        LM = LayerMask.GetMask("3D UI");

        v = new int[UI_dice.Length];

        for (int i = 0; i < UI_dice.Length; ++i)
        {
            v[i] = findDiceValue(i);
            highlight_face = UI_dice[i].transform.GetChild(v[i] - 1).gameObject;
            setAlpha(highlight_face, 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {
            throwDice(v);
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < UI_dice.Length; ++i)
        {
            v[i] = findDiceValue(i);
            setAlpha(highlight_face, 0);
            highlight_face = UI_dice[i].transform.GetChild(v[i] - 1).gameObject;
            setAlpha(highlight_face, 0.5f);
        }
    }

    void setAlpha(GameObject go, float a)
    {
        Color c = go.GetComponent<MeshRenderer>().material.color;
        c.a = a;
        go.GetComponent<MeshRenderer>().material.color = c;
    }

    int findDiceValue(int dice_id)
    {
        Transform d = UI_dice[dice_id].transform;
        Vector3 dir = -Camera.main.transform.forward;
        float[] axes = { 0, 0, 0 };
        axes[0] = Vector3.Dot(dir, d.right);
        axes[1] = Vector3.Dot(dir, d.up);
        axes[2] = Vector3.Dot(dir, d.forward);
        float[] abs_axes = { 0, 0, 0 };
        for (int i = 0; i < axes.Length; i++)
            abs_axes[i] = Mathf.Abs(axes[i]);
        float peak = abs_axes.Max();
        int axis = System.Array.IndexOf(abs_axes, peak);
        axis = (axis + 1) * (int)Mathf.Sign(axes[axis]);

        switch (axis) // lookup dice value
        {
            case  1: return 1;
            case  2: return 2;
            case  3: return 4;
            case -1: return 6;
            case -2: return 5;
            case -3: return 3;
            default: return 0;
        }
    }

    void throwDice(int[] dice)
    {
        for(int i = 0; i < dice.Length; ++i)
        {
            Vector3 pos = transform.position;
            GameObject d = Instantiate(projectile, pos, Random.rotation);
            Vector3 vel = (Camera.main.transform.forward + Vector3.up * 0.25f) * 10f + GetComponent<Rigidbody>().velocity;
            vel.x += Random.Range(-0.25f, 0.25f);
            vel.y += Random.Range(-0.25f, 0.25f);
            d.GetComponent<Rigidbody>().velocity = vel;
            d.GetComponent<PProjectile>().damage = dice[i];
        }
    }

}
