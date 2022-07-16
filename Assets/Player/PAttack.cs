using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PAttack : MonoBehaviour
{
    [SerializeField] GameObject[] UI_dice;
    int LM;
    GameObject highlight_face;

    // Start is called before the first frame update
    void Start()
    {
        LM = LayerMask.GetMask("3D UI");

        RaycastHit hit;
        if (Physics.Linecast(Camera.main.transform.position, UI_dice[0].transform.position, out hit, LM))
        {
            highlight_face = hit.collider.gameObject;
            setAlpha(highlight_face, 0.5f);
            Debug.Log("Current face: " + highlight_face.name);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        int[] v = new int[UI_dice.Length];
        for (int i = 0; i < UI_dice.Length; ++i)
        {
            v[i] = findDiceValue(i);
            setAlpha(highlight_face, 0);
            highlight_face = UI_dice[i].transform.GetChild(v[i]-1).gameObject;
            setAlpha(highlight_face, 0.5f);
        }

        //RaycastHit hit;
        //if (Physics.Linecast(Camera.main.transform.position, UI_dice[0].transform.position, out hit, LM))
        //{
        //    if(hit.collider.gameObject != highlight_face)
        //    {
        //        if (highlight_face != null)
        //            setAlpha(highlight_face, 0);
        //        highlight_face = hit.collider.gameObject;
        //        setAlpha(highlight_face, 0.5f);
        //        Debug.Log("Current face: " + highlight_face.name);
        //    }
        //}
        //else
        //{
        //    Debug.LogError("No dice collider located for UI dice 0");
        //}
        //
        //Debug.DrawLine(Camera.main.transform.position, UI_dice[0].transform.position, Color.red, 5);
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
}
