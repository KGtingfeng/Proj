using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class WordAplhaEffect : MonoBehaviour
{

    public static WordAplhaEffect instance;

    private void Awake()
    {
        instance = this;
    }


    TextField textField;
    Dictionary<int, float> keyValuePairs = new Dictionary<int, float>();

    /// <summary>
    /// 播放渐变效果
    /// </summary>
    /// <param name="textField"></param>
    /// <param name="index"></param>
    public void PlayAlphaEffect(ref TextField textField, int index)
    {
        if (keyValuePairs.ContainsKey(index))
            keyValuePairs[index] = 0.2f;
        else
            keyValuePairs.Add(index, 0.2f);

        this.textField = textField;
        StartCoroutine(PlayAlphaEffect(index));
    }

    /// <summary>
    /// 显示渐变效果
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IEnumerator PlayAlphaEffect(int index)
    {
       
        int endIndex = index;
        if (!keyValuePairs.ContainsKey(index))
            yield break;
        float alpha = keyValuePairs[index] + 0.1f;
        keyValuePairs[index] = alpha;

        Color[] colors = textField.graphics.mesh.colors;
        if (endIndex < colors.Length)
        {
            colors[endIndex - 1].a = alpha;
            colors[endIndex - 2].a = alpha;
            colors[endIndex - 3].a = alpha;
            colors[endIndex - 4].a = alpha;
            textField.graphics.mesh.colors = colors;
        }

        if (alpha > 1.0f)
        {
            keyValuePairs.Remove(index);
            yield break;
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(PlayAlphaEffect(index));

    }



    public void ShowRightNow()
    {
        StopAllCoroutines();
        for (int i = 0; i < textField.graphics.mesh.colors.Length; i++)
        {
            Color color = textField.graphics.mesh.colors[i];
            color.a = 1;
            textField.graphics.mesh.colors[i] = color;
        }
        textField.graphics.SetMeshDirty();
        keyValuePairs.Clear();
    }

}


