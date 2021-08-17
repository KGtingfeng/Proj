using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;



public class TestSpine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string path = "Game/Spine Skeletons/spineboy-unity/Test";
        Object tmpObj =  Resources.Load(path);
        GameObject spineObj = Instantiate( tmpObj) as GameObject;
        spineObj.layer = 5;
        spineObj.SetActive(true);



        // Object skeleton= Resources.Load(path);

        //string path = "Game/Spine Skeletons/spineboy-unity/spineboy-unity_SkeletonData";
        ////Debug.Log("skeleton " + skeleton);
        //SkeletonDataAsset skeletonDataAsset = Resources.Load<SkeletonDataAsset>(path);
        //Debug.Log("load by Type: " + skeletonDataAsset);

        //Material material = Resources.Load<Material>("Game/Spine Skeletons/spineboy-unity/spineboy_Material");
        //Debug.Log("material " + material);
        //if (skeletonDataAsset != null)
        //    skeletonDataAsset.GetSkeletonData(false);
        //yield return new WaitForSeconds(1f);


        //var sg = SkeletonGraphic.NewSkeletonGraphicGameObject(skeletonDataAsset, transform, material); // Spawn a new SkeletonGraphic GameObject.
        //sg.gameObject.name = "SkeletonGraphic Instance";

        //// Extra Stuff
        //sg.Initialize(false);
        //sg.Skeleton.SetSlotsToSetupPose();
        //sg.AnimationState.SetAnimation(0, "idle", true);
        //sg.Skeleton.SetSkin("base");
        //sg.gameObject.layer = 5;



    }

    // Update is called once per frame
    void Update()
    {

    }
}
