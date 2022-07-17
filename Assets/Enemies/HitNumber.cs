using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitNumber : MonoBehaviour
{

    const float decay_start_time = 1f;
    const float fade_speed = 2;
    const float rise_speed = 1f;

    const float rise_end_time = 0.7f;
    const float rise_offset = 0.5f;
    //Vector2 rise_dest;

    const float grow_end_time = 0.3f;
    const float grow_size = 2;
    float age = 0;

    private void Start()
    {
        //init(132, AttackEvents.BLOCK);
        //rise_dest = gameObject.transform.position + new Vector3(0, rise_offset, 0);
    }

    public void init(int amount)
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);

        string str = "-<b>" + amount.ToString() + "<b>";
        switch (amount)
        {
            case 0:
                str = "<color=#D6D6D6><i>BLOCK";
                break;
            case 1:
                str = "<color=#DFEAEB>" + str;
                break;
            case 2:
                str = "<color=#FFF194>" + str;
                break;
            case 3:
                str = "<color=#FFEE00>" + str;
                break;
            case 4:
                str = "<color=#FCB603>" + str;
                break;
            case 5:
                str = "<color=#D93D00>" + str;
                break;
            case 6:
                str = "<color=#BF0000>" + str;
                break;
        }

        TextMeshPro tm = gameObject.GetComponent<TextMeshPro>();
        tm.text = str;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.position - Camera.main.transform.position);

        //if(age < rise_end_time)
        //{
        //    gameObject.transform.position = Vector3.Lerp(
        //        gameObject.transform.position,
        //        rise_dest,
        //        Time.deltaTime);
        //    //gameObject.transform.position += new Vector3(0, rise_speed, 0) * Time.deltaTime;
        //}
        if (age < grow_end_time)
        {
            gameObject.transform.localScale = Vector3.Lerp(
                gameObject.transform.localScale,
                new Vector3(1, 1, 1) * grow_size,
                Time.deltaTime);
            ///gameObject.transform.localScale += new Vector3(1, 1, 1) * grow_speed * Time.deltaTime;
        }

        if (age > decay_start_time)
        {

            Color32 col_main = fade(gameObject.GetComponent<TextMeshPro>().faceColor);
            Color32 col_outline = fade(gameObject.GetComponent<TextMeshPro>().outlineColor);
            if (col_main.a == 0)
            {
                // Fade has finished
                Destroy(gameObject);
                return;
            }
            gameObject.GetComponent<TextMeshPro>().faceColor = col_main;
            gameObject.GetComponent<TextMeshPro>().outlineColor = col_outline;

            // gameObject.GetComponent<TextMeshPro>().alpha += 255 * fade_out_speed * Time.deltaTime;
            gameObject.transform.position += new Vector3(0, rise_speed, 0) * Time.deltaTime;
        }
        age += Time.deltaTime;
    }

    private Color32 fade(Color32 col)
    {
        float alpha = col.a;
        alpha -= 255 * fade_speed * Time.deltaTime;
        if (alpha > 255)
            alpha = 255;
        else if (alpha < 0)
            alpha = 0;
        col.a = (byte)alpha;
        return col;
    }
}
