using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


[ViewAttr("Game/UI/F_Room", "F_Room", "Frame_choosesound", true)]
public class ChooseSoundView : BaseView
{
    public static ChooseSoundView ins;
    GList soundList;

    List<Role> roles;
    NormalInfo info;

    List<GameInitCardsConfig> configs=new List<GameInitCardsConfig>();
    public override void InitUI()
    {
        base.InitUI();
        soundList = SearchChild("n13").asList;
        InitEvent();
        ins = this;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n2").onClick.Set(() =>
        {
            GRoot.inst.StopDialogSound();
            onDeleteAnimation<ChooseSoundView>();
        });
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        info = data as NormalInfo;
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<Role>>(NetHeaderConfig.ACTOR_LIST, wWWForm, Refresh);

    }

    void Refresh(List<Role> roles)
    {
        this.roles = roles;
        SortRole();
        configs.Clear();
        foreach(var item in roles)
        {
            configs.Add(JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == item.id));
        }
        Role role = roles.Find(a => a.id == info.index);
        int index = roles.IndexOf(role);
        soundList.SetVirtual();
        soundList.itemRenderer = ItemRender;
        soundList.numItems = roles.Count;
        soundList.onClickItem.Set(OnClickItem);

        soundList.selectedIndex = index;
        soundList.ScrollToView(index);
    }

    void SortRole()
    {
        roles.Sort(delegate (Role roleA, Role roleB)
        {
            if (roleA.id == roleB.id)
                return 0;
            return roleA.id.CompareTo(roleB.id);
        });
    }

    void ItemRender(int index, GObject gObject)
    {
        GButton gCom = gObject.asButton;
        gCom.title = configs[index].name_cn;
    }

    void OnClickItem(EventContext context)
    {
        int selectedIndex = soundList.GetChildIndex((GObject)context.data);
        int itemIndex = soundList.ChildIndexToItemIndex(selectedIndex);
        GRoot.inst.StopDialogSound();
        EventMgr.Ins.DispachEvent(EventConfig.CHANGE_RING, roles[itemIndex].id);
        AudioClip audioClip = Resources.Load(SoundConfig.WAKEUP_BELL_AUDIO_EFFECT_URL + roles[itemIndex].id) as AudioClip;
        GRoot.inst.PlayDialogSound(audioClip);
    }

    public void NewbieHide()
    {
        GRoot.inst.StopDialogSound();
        onDeleteAnimation<ChooseSoundView>();
    }
}
