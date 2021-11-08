using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSwayComponent : BaseComponent
{
    private float swayRate;
    public override void Startup()
    {
        int min = 3;
        int max = 6;
        swayRate = min + (RandomNumber.Get() / 100f) * (max - min);
        enabled = false;
        
    }

    public override string ToDetailedString()
    {
        return "";
    }
    private void FixedUpdate()
    {
        transform.localScale += new Vector3(Mathf.Sin(Time.time * swayRate) / 1500, Mathf.Cos(Time.time * swayRate) / 1500, 1);
    }
    private IEnumerator SwayCoroutine()
    {
        yield return new WaitForSeconds(RandomNumber.Get() / 100);
        while (true)
        {
            //transform.Rotate(new Vector3(0, 0, Mathf.Sin(Time.time * 5)/10));
            transform.localScale += new Vector3(Mathf.Sin(Time.time * swayRate) / 1500, Mathf.Cos(Time.time * swayRate) / 1500, 1);
            yield return new WaitForFixedUpdate();
        }
    }
    private void OnBecameInvisible()
    {
        this.enabled = false;
        //StopAllCoroutines();
    }
    private void OnBecameVisible()
    {
        this.enabled = true;
        //StartCoroutine("SwayCoroutine");
    }
}
