using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


public class AutoDestroy : MonoBehaviour
{
    public float duration = 2;
    public GoWrapper goWrapper;
    // Start is called before the first frame update
    void Start()
    {

        Timers.inst.StartCoroutine(DestorySelf());
    }

    IEnumerator DestorySelf()
    {
        yield return new WaitForSeconds(duration);
        goWrapper.Dispose();
       
    }


}
