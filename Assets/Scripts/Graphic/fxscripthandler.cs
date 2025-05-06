using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static TextureFunction;

public class Efeitos
{
    public Sprite Stepng;
    private float stepDir;
    public float yForCamera;

    public static float Angle(Vector2 vector2)
    {
        return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * Mathf.Sign(vector2.x));
    }
    public void StepFX(Sprite sprite, float stepDis, Vector2 Dir, Transform pos, Rigidbody2D rb, float footSpawnY, float footSpawnX)
    {
        if (stepDir > 0)
        {
            stepDir = -stepDis;
        }
        else
        {
            stepDir = stepDis;
        }

        GameObject StepOBJ = new GameObject();
        StepOBJ.AddComponent<SpriteRenderer>();
        StepOBJ.AddComponent<TimerDestroy>();
        StepOBJ.GetComponent<SpriteRenderer>().sprite = sprite;
        StepOBJ.GetComponent<SpriteRenderer>().sortingLayerName = "TerrainEffect";
        StepOBJ.GetComponent<SpriteRenderer>().sortingOrder = 10;
        StepOBJ.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
        StepOBJ.transform.position = (Vector2)pos.position - new Vector2(footSpawnX, footSpawnY);
        //StepOBJ.transform.eulerAngles = new Vector3(0,0, Angle(Dir));
        //StepOBJ.transform.rotation.SetLookRotation(Dir);

        Vector2 v = rb.velocity;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        StepOBJ.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }
    public void ShipOnSeaEffect(MonoBehaviour a, GameObject obj, float amount, float speed)
    {
        float y;
        float trueY = obj.transform.position.y;
        WaitConditionRef<Vector3> effectShipOnSea = new (a, () => { }, obj.transform.position, () => { });
        effectShipOnSea = new (a, () => 
        {
            a.StartCoroutine(flutuation());
            IEnumerator flutuation()
            {
                y = Mathf.PingPong(Time.time * speed, 1) * amount;
                if (y > amount || y < 0)
                {
                    y = 0;
                }
                yForCamera = y;
                if (obj.GetComponent<Rigidbody2D>().velocity.y != 0)
                {
                    trueY = obj.transform.position.y - y;
                }
                obj.transform.position = new Vector3(obj.transform.position.x, trueY + y, obj.transform.position.z);
                yield return new WaitForSeconds(0.05f);
            }
        }, 
        obj.transform.position, () => 
        {

        }
        );
    }
    public void ReflectOnWater()
    {

    }
    public void DirectionWavesEffect()
    {

    }
}