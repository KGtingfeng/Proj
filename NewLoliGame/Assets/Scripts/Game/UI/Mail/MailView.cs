using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

[ViewAttr("Game/UI/Y_Mail", "Y_Mail", "Mail")]
public class MailView : BaseView
{
    //分页大小
    const int PAGE_SIZE = 20;
    GComponent mailCom;
    GList mailList;
    GTextField numText;
    GButton getBtn;

    Mail mail;
    List<MailInfo> mailInfos;
    bool haveMails;
    int page;
    int totalPage;
    public override void InitUI()
    {
        base.InitUI();
        mailCom = SearchChild("n14").asCom;
        mailList = mailCom.GetChild("n9").asList;
        numText = mailCom.GetChild("n12").asTextField;
        getBtn = mailCom.GetChild("n13").asButton;
        mailList.scrollPane.onScroll.Add(OnScroll);
        controller = mailCom.GetController("c1");
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        getBtn.onClick.Set(() => { GetAllMailAward(); });
        SearchChild("n16").onClick.Set(() =>
        {
            UIMgr.Ins.showNextView<MainView>();
        });

        EventMgr.Ins.RegisterEvent<int>(EventConfig.READ_MAIL, ReadMail);
        EventMgr.Ins.RegisterEvent<int>(EventConfig.GET_MAIL, GetMailAward);
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        mail = data as Mail;
        mailList.SetVirtual();
        mailList.itemRenderer = RenderItem;
        SetMails(mail);
          
    }

    void SetMails(Mail mail)
    {
        this.mail = mail;
        mailInfos = mail.results;
        page = 1;
        totalPage = mail.total / PAGE_SIZE;
        totalPage += mail.total % PAGE_SIZE == 0 ? 0 : 1;
        if (page < totalPage)
            haveMails = true;
        mailList.scrollPane.ScrollTop();
        RefreshMailList();
    }

    void RefreshMailList()
    {
        if (mailInfos.Count == 0)
            controller.selectedIndex = 1;
        else
            controller.selectedIndex = 0;

        mailList.numItems = mailInfos.Count;
        mailList.onClickItem.Set(OnClickItem);
        getBtn.visible = false;
        foreach (var mailInfo in mailInfos)
        {
            if (mailInfo.status == 256)
            {
                getBtn.visible = true;
                break;
            }
        }
        numText.text = "数量" + mail.total + "/100";
        SetItemEffect();
    }

    void RenderItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject.asCom;
        Controller controller = gComponent.GetController("c1");
        GTextField titleText = gComponent.GetChild("n5").asTextField;
        GTextField timeText = gComponent.GetChild("n6").asTextField;

        string content = mailInfos[index].title;
        content = GameTool.GetCutText(content, 15);
        titleText.text = content;
        DateTime time = TimeUtil.getTime(mailInfos[index].createTime).AddDays(30);
        timeText.text = "到期时间：" + time.Year + "年" + time.Month + "月" + time.Day + "日";
        if (mailInfos[index].status == 1)
            controller.selectedIndex = 2;
        else
        {
            if (string.IsNullOrEmpty(mailInfos[index].extraData))
                controller.selectedIndex = 1;
            else
                controller.selectedIndex = 0;
        }
    }

    void OnClickItem(EventContext context)
    {
        int index = mailList.GetChildIndex((GObject)context.data);
        //真实index
        int realIndex = mailList.ChildIndexToItemIndex(index);
        UIMgr.Ins.showNextPopupView<MailInfoView, MailInfo>(mailInfos[realIndex]);
    }

    void OnScroll()
    {
        if (haveMails)
        {
            int itemIndex = mailList.ChildIndexToItemIndex(1);
            if (itemIndex > (page - 1) * 20 + 10)
                GetMails();
        }
    }

    void GetMails()
    {
        page++;
        if (page == totalPage)
            haveMails = false;
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("page", page);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<Mail>(NetHeaderConfig.MAILS_LIST, wWWForm, RequestMailList, false);
    }

    void RequestMailList(Mail mail)
    {
        if (mail.results.Count > 0)
        {
            mailInfos.AddRange(mail.results);
            RefreshMailList();
        }
    }

    void GetAllMailAward()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<TaskAward>(NetHeaderConfig.GET_MAILS_AWARD, wWWForm, RequestGetAward);
    }

    void RequestGetAward(TaskAward playerProps)
    {
        GameTool.ShowAwards(playerProps);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("page", 1);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<Mail>(NetHeaderConfig.MAILS_LIST, wWWForm, SetMails, false);
    }

    void ReadMail(int mailId)
    {
        mailInfos.Find(a => a.mailId == mailId).status = 1;
        RefreshMailList();
    }

    void GetMailAward(int mailId)
    {
        MailInfo info = mailInfos.Find(a => a.mailId == mailId);
        mailInfos.Remove(info);
        mail.total--;
        RefreshMailList();
    }

    void SetItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < mailList.numChildren; i++)
        {
            GObject item = mailList.GetChildAt(i);

            item.alpha = 0;

            pos = GetItemPos(i, item);
            item.SetPosition(pos.x, pos.y + 200, pos.z);
            float time = i * 0.1f;

            item.TweenMoveY(pos.y, (time + 0.1f)).SetEase(EaseType.CubicOut).OnStart(() =>
            {
                item.TweenFade(1, (time + 0.15f)).SetEase(EaseType.QuadOut);
            });
        }
    }

    Vector3 GetItemPos(int index, GObject gObject)
    {
        Vector3 pos = gObject.position;
        if (pos == Vector3.zero)
        {
            pos.y = index * 255;
        }
        return pos;
    }
}
