using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ViewAttr("Game/UI/P_Friend", "P_Friend", "Frame_delete_friend")]
public class DelFriendConfirmView : BaseView
{
    OtherPlayer otherPlayer;

    public override void InitUI()
    {
        base.InitUI();
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();

        SearchChild("n6").onClick.Set(() =>//取消
        {
            OnHideAnimation();
        });

        SearchChild("n5").onClick.Set(() =>//确认删除好友
        {
            OnHideAnimation();
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("id", otherPlayer.playerId.ToString());
            GameMonoBehaviour.Ins.RequestInfoPost<HolderData>(NetHeaderConfig.FRIEND_DELETE, wWWForm, () =>
            {
                EventMgr.Ins.DispachEvent(EventConfig.HIDE_PLAYER_INFO);//隐藏好友详情面板

                EventMgr.Ins.DispachEvent<int>(EventConfig.REFRESH_AFTER_DELETE, otherPlayer.playerId);//刷新前一个页面
            });

        });
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);

        otherPlayer = data as OtherPlayer;
    }
}
