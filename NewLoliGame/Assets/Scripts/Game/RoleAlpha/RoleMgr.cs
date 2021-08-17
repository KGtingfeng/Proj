using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleMgr : MonoBehaviour
{

    public static RoleMgr roleMgr;
    Dictionary<string, GameObject> roleModels = new Dictionary<string, GameObject>();

    public GameObject mapCom;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        roleMgr = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject currentModel;
    public string currentkey;
    public void CreateRole(UnityEngine.Object roleObj)
    {
        currentkey = roleObj.name;
        //hide other
        foreach (var model in roleModels)
        {
            if (model.Key != roleObj.name)
                model.Value.SetActive(false);
        }

        if (!roleModels.ContainsKey(roleObj.name))
        {
            GameObject child = (GameObject)Instantiate(roleObj);
            child.transform.parent = transform;
            child.transform.position = new Vector3(0, -4.74f, 0);
            child.layer = LayerMask.NameToLayer("Role");
            for (int i = 0; i < child.transform.childCount; i++)
            {
                child.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Role");
            }
            child.SetActive(true);
            roleModels.Add(roleObj.name, child);
        }
        else
        {

            roleModels[roleObj.name].SetActive(true);
        }
        currentModel = roleModels[roleObj.name];
        RefreshSkeletonAnimation();
    }

    public void RefreshShow(string key)
    {
        if (key != currentkey && roleModels.ContainsKey(key))
        {
            foreach (var model in roleModels)
            {
                if (model.Key != key)
                    model.Value.SetActive(false);
            }
            currentkey = key;
            currentModel = roleModels[key];
            roleModels[key].SetActive(true);
            RefreshSkeletonAnimation();
        }
    }

    SkeletonAnimation skeletonAnimation;
    public void RefreshSkeletonAnimation()
    {
        skeletonAnimation = null;
        Component[] components = currentModel.GetComponentsInChildren(typeof(SkeletonAnimation));
        if (components != null && components.Length > 0)
        {
            skeletonAnimation = components[0] as SkeletonAnimation;  
        }
    }

    public SkeletonAnimation GetSkeletonAnimation
    {
        get { return skeletonAnimation; }
    }


    public GameObject GetMapingCom()
    {
        GameObject go = Instantiate(mapCom);
        go.SetActive(true);
        return go;
    }



}
