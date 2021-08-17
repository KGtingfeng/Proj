using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game_common", "Y_Game_common", "game instructions")]
public class GameDetailView : BaseView
{
    GTextField title;
    GTextField content;
    GLoader bgLoader;
    GLoader gameLoader;
    GGroup gGroup;
    GComponent button;
    GameDetailConfig config;

    float groupX;
    float contentX;

    bool canClick;
    public override void InitUI()
    {
        base.InitUI();
        title = SearchChild("n5").asTextField;
        content = SearchChild("n2").asTextField;
        bgLoader = SearchChild("n10").asLoader;
        gameLoader = SearchChild("n8").asLoader;
        gGroup = SearchChild("n9").asGroup;
        bgLoader.url = UrlUtil.GetGameDetailUrl(0);
        groupX = gGroup.x;
        contentX = content.x;
        button = SearchChild("n6").asCom;
        button.onClick.Set(OnClick);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        canClick = true;
        config = data as GameDetailConfig;
        title.text = config.title;
        content.text = config.content;
        gameLoader.url = UrlUtil.GetGameDetailUrl(config.id);
        gGroup.x = -700;
        content.x = 1500;
        gameLoader.scale = Vector2.zero * 0.5f;
        gameLoader.alpha = 0;
        button.scale = Vector2.one*0.5f;
        button.alpha = 0;

        gGroup.TweenMoveX(groupX, 0.5f);
        content.TweenMoveX(contentX, 0.5f).OnComplete(() =>
        {
            gameLoader.TweenFade(1, 0.5f);
            gameLoader.TweenScale(Vector2.one, 0.5f).OnComplete(() =>
            {
                button.TweenFade(1, 0.5f);
                button.TweenScale(Vector2.one, 0.5f);
            });
        });

    }

    void OnClick()
    {
        if (!canClick)
            return;
        canClick = false;
        TouchScreenView.Ins.PlayChangeEffect(() =>
        {
            config.callback?.Invoke();
            onHide();
        });
    }

}
