using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour
{

    /// <summary>
    /// 中心箭头
    /// </summary>
    public GameObject centerObj;
    /// <summary>
    /// 消息图片对象
    /// </summary>
    public GameObject roateObj;
    /// <summary>
    /// 四元数
    /// </summary>
    Quaternion qua;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (centerObj != null)
        {
            //roateObj围绕centerObj旋转，x,y不旋转
            roateObj.transform.RotateAround(centerObj.transform.position, new Vector3(0, 0, 1), 80f * Time.deltaTime);
            //这里处理不然roateObj图片的显示位置发生变化
            qua = roateObj.transform.rotation;
            //qua.z = 0;
            roateObj.transform.rotation = qua;
        }

    }




}

