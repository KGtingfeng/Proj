using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;



public class TestCompine : MonoBehaviour
{
    GComponent ui;
    GList gList;
    GGraph spineGraph;
    List<int> combineIds = new List<int>()
    {
        1,2,3,4,5,6,7
    };
    List<int> pools = new List<int>();

    List<GGraph> gGraphs = new List<GGraph>();
    /// <summary>
    /// 表示已经放置好的id
    /// </summary>
    List<int> placedIds = new List<int>();


    // Start is called before the first frame update
    void Start()
    {

        ui = GetComponent<UIPanel>().ui;
        gList = ui.GetChild("n6").asList;

 

        GGroup gGroup = ui.GetChild("n37").asGroup;
        int cnt = ui.numChildren;
        for (int i = 0; i < cnt; i++)
        {
            GObject gObject = ui.GetChildAt(i);
            if (gObject.group == gGroup)
            {
                gGraphs.Add(gObject.asGraph);
            }
        }
        //放置底图
        PlacePartToWhole(0, 1);
        combineIds.RemoveAt(0);
        gGraphs.RemoveAt(0);


        pools.AddRange(combineIds);
        MathUtil.Shuffle(pools);


        initList();
        ui.GetChild("n36").asCom.onDrop.Add(OnDrop);



    }

    int rightIndex = 0;
    void PlacePartToWhole(int index, int id)
    {
        Texture2D texture2D = Resources.Load<Texture>("Game/Combine/Watch/game_watch1_" + id) as Texture2D;
        NTexture texture = new NTexture(texture2D);
        Image image = new Image();
        image.texture = texture;
        gGraphs[rightIndex].SetNativeObject(image);
        placedIds.Add(id);
    }

    /// <summary>
    /// 用于外部区域
    /// </summary>
    /// <param name="context">Context.</param>
    void OnDrop(EventContext context)
    {
        Debug.Log("Outer OnDrop over");
        string extral = (string)context.data;
        string[] arry = extral.Split(',');
        int index = int.Parse(arry[0]);
        int id = int.Parse(arry[1]);

        //ID要相同
        if (combineIds[rightIndex] != pools[index])
        {
            GComponent gComponent = gList.GetChildAt(index).asCom;
            Debug.Log(gComponent.GetChild("n0"));
            GLoader gLoader = gComponent.GetChild("n0").asLoader;
            gLoader.color = Color.white;
        }
        else
        {
            PlacePartToWhole(index, id);
            rightIndex++;
        }

    }

    void initList()
    {
        Refresh();

        LongPressGesture gesture = new LongPressGesture(gList);
        gesture.once = true;
        gesture.trigger = 0.2f;
        gesture.onAction.Add(OnLongPress);
    }


    void Refresh()
    {
        gList.itemRenderer = RenderListItem;
        gList.numItems = pools.Count;
    }

    void RenderListItem(int index, GObject obj)
    {

        //Debug.Log("obj: " + obj);
        GComponent item = obj.asCom;
        GLoader itemLoader = item.GetChild("n0").asLoader;
        itemLoader.url = "Game/Combine/Watch/game_watch1_" + pools[index];

        string extral = index + "," + pools[index];
        itemLoader.data = extral;
        item.onDrop.Add(onDrop);
        item.data = extral;

    }
    /// <summary>
    /// 用于scroll item
    /// </summary>
    /// <param name="context">Context.</param>
    void onDrop(EventContext context)
    {
        string extral = (string)context.data;
        string[] arry = extral.Split(',');
        int index = int.Parse(arry[0]);
        int id = int.Parse(arry[1]);

        Debug.Log("index = " + index + " id=" + id);
        Debug.Log(gList.GetChildAt(index));
        GComponent gComponent = gList.GetChildAt(index).asCom;
        Debug.Log(gComponent.GetChild("n0"));
        GLoader gLoader = gComponent.GetChild("n0").asLoader;
        gLoader.color = Color.white;
 
    }



    void OnLongPress(EventContext context)
    {
        //find out which item is under finger
        //逐层往上知道查到点击了那个item
        GObject obj = GRoot.inst.touchTarget;
        GObject p = obj.parent;
        //Debug.Log("obj: " + obj + "  parent: " + p);
        while (p != null)
        {
            if (p == gList)
                break;

            p = p.parent;
        }

        if (p == null)
            return;

        string extral = (string)obj.data;
        string[] arry = extral.Split(',');
        int index = int.Parse(arry[0]);
        int id = int.Parse(arry[1]);

        //该对象已经放置好了就不需要 处理了
        if (placedIds.Contains(id))
            return;

        DragDropManager.inst.StartDrag(obj, obj.icon, obj.data);
        GLoader itemLoader = obj.asLoader;
        itemLoader.color = Color.gray;
 
    }
 
}
