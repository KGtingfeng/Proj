using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameRoleMainTipsConfig
{


    public int id;

    public int body_voice1;
    public int body_face1;
    public string body_context1;


    public int body_voice2;
    public int body_face2;
    public string body_context2;



    public int body_voice3;
    public int body_face3;
    public string body_context3;



    public int head_voice1;
    public int head_face1;
    public string head_context1;

    public int head_voice2;
    public int head_face2;
    public string head_context2;

    public int head_voice3;
    public int head_face3;
    public string head_context3;




    List<BodyInfo> bodyInfos;
    List<BodyInfo> headInfos;




    List<BodyInfo> BodyInfo
    {
        get
        {
            if (bodyInfos == null)
            {
                bodyInfos = new List<BodyInfo>();
                if (body_voice1 != 0)
                {
                    BodyInfo bA = new BodyInfo(id,body_face1, body_voice1, body_context1);
                    bodyInfos.Add(bA);
                }

                if (body_voice2 != 0)
                {
                    BodyInfo bB = new BodyInfo(id, body_face2, body_voice2, body_context2);
                    bodyInfos.Add(bB);
                }


                if (body_voice3 != 0)
                {
                    BodyInfo bC = new BodyInfo(id, body_face3, body_voice3, body_context3);
                    bodyInfos.Add(bC);
                }

            }

            return bodyInfos;
        }
    }



    List<BodyInfo> HeadInfo
    {
        get
        {

            if (headInfos == null)
            {

                headInfos = new List<BodyInfo>();
                if (head_voice1 != 0)
                {
                    BodyInfo hA = new BodyInfo(id, head_face1, head_voice1, head_context1);
                    headInfos.Add(hA);
                }

                if (head_voice2 != 0)
                {
                    BodyInfo hB = new BodyInfo(id, head_face2, head_voice2, head_context2);
                    headInfos.Add(hB);
                }


                if (head_voice3 != 0)
                {
                    BodyInfo hC = new BodyInfo(id, head_face3, head_voice3, head_context3);
                    headInfos.Add(hC);
                }


            }

            return headInfos;

        }
    }



    public BodyInfo GetHeadInfo()
    {
        if (HeadInfo.Count == 0)
            return null;

        if (HeadInfo.Count == 1)
            return HeadInfo[0];

        int index = UnityEngine.Random.Range(0, HeadInfo.Count);
        return HeadInfo[index];

    }


    public BodyInfo GetBodyInfo()
    {
        if (BodyInfo.Count == 0)
            return null;

        if (BodyInfo.Count == 1)
            return BodyInfo[0];

        int index = UnityEngine.Random.Range(0, BodyInfo.Count); 
        return BodyInfo[index];

    }

}



public class BodyInfo
{
    public int id;
    public int faceId;
    public int voiceId;
    public string context;

    public BodyInfo(int id,int faceId, int voiceId, string context)
    {
        this.id = id;
        this.faceId = faceId;
        this.voiceId = voiceId;
        this.context = context;
    }
}
