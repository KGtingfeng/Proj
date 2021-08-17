using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPinyin;

public class SpecialChar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //xjp
        //jin
        //jing
        //pin
        //ping


        string test = "xi近凭";
        string result = Pinyin.GetPinyin(test).Trim();
        Debug.Log(result);
        Debug.Log(Pinyin.GetInitials(test));
        //string[] arry = result.Split(' ');
        //Debug.Log(arry.Length);
        //foreach (var item in arry)
        //{
        //    Debug.Log(item);
        //}
    }

}

 
