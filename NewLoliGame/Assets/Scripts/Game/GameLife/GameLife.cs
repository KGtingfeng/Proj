using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLife : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnApplicationPause(bool pause)
    {

        Debug.Log("<color=red>OnApplicationPause</color>");

    }

    private void OnApplicationQuit()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<Announcement>(NetHeaderConfig.QUERY_ANNOUNCEMENT, wWWForm, null);
        Debug.Log("<color=red>OnApplicationQuit</color>");
    }


}
