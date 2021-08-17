using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
[ViewAttr("Game/UI/D_Login", "D_Login", "TFont")]
public class TestFontView : BaseView
{
    public override void InitUI()
    {
        base.InitUI();

 
        InitEvent();

        GTextField textFielNormal = SearchChild("n0").asTextField;
        GTextField textFieldBodl = SearchChild("n1").asTextField;


        textFielNormal.text = "[color=#F884FF]舒言[/color]拉着“我”的[size=150]手[/size]，在时间隧道里。";
        //Debug.Log("normal: " + textFielNormal._textField.textFormat.font);
        //Debug.Log("bold : " + textFieldBodl._textField.textFormat.font);

        TypingEffect typingEffect = new TypingEffect(textFielNormal);
        typingEffect.Start();
        typingEffect.PrintAll(1f);

        SearchChild("n3").onClick.Set(() =>
        {
            typingEffect.ShowAllRightNow();
        });

    }
}



