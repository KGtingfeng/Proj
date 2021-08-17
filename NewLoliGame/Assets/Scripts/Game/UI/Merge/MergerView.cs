using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/M_Merge", "M_Merge", "Main")]
//[ViewAttr("Game/UI/J_Story", "J_Story", "Watch_timeshoose")]
public class MergerView : BaseView
{
    int touchId;
    float _InitX;
    float _InitY;
    float _startStageX;
    float _startStageY;
    float _lastStageX;
    float _lastStageY;


    GObject rotateObj;
    GObject moveObj;
    float baseAngel;

    //GObject _button;
    GObject _touchArea;
    //GObject _thumb;
    //GObject _center;

    //GTweener _tweener;
    public int radius { get; set; }



    public float glPlFltDeltaLimit = 60;
    public float glPlFltDeltaReduce = 2;
    public int glPlIntLapsBeforeStopping = 25;
    public bool glPlBoolCanRotate { get; set; }
    public AudioClip glPlSpinSound;
    private float glPrFltDeltaRotation;
    private float glPrFltPreviousRotation;
    private float glPrFltCurrentRotation;
    private int glPrIntCurrentLaps;
    private float glPrFloatRotation;
    private float glPrFltQuarterRotation;
    private bool boolCountRotations;


    GGraph graph;
    GComponent itemParent;


    public override void InitUI()
    {
        base.InitUI();
        InitEvent();

        //glPrIntCurrentLaps = glPlIntLapsBeforeStopping;
        //glPrFloatRotation = 0f;
        //glPlBoolCanRotate = true;
        //boolCountRotations = true;
    }


    Vector3 basePos;
    GObject n33;
    Image image;

    string itemUrl = "ui://swdt5zfnuftts";

    public override void InitEvent()
    {
        base.InitEvent();

        //rotateObj = SearchChild("n33");
        //baseAngel = rotateObj.rotation + 90;
        //rotateObj.displayObject.pivot = new Vector2(0.5f, 0.5f);
        moveObj = SearchChild("n6");
        itemParent = SearchChild("n7").asCom;
        //GList list;

        Debug.Log("itemParent:  " + itemParent);

        //UIPackage.GetItemByURL(itemUrl);
        //GComponent gObject = UIPackage.CreateObjectFromURL(itemUrl).asCom;
        //Debug.Log("gObject: " + gObject);
        //gObject.SetHome(itemParent);
        //gObject.displayObject. = itemParent;
        //Debug.Log("creategObj: " + gObject);
        //itemParent.AddChild(gObject);
        //gObject.displayObject.home = itemParent.displayObject.cachedTransform;
       

        for(int i = 0; i < 5; i++)
        {
            GComponent gObject = UIPackage.CreateObjectFromURL(itemUrl).asCom;
            gObject.visible = true;
            itemParent.AddChild(gObject);
            gObject.x = (float)(i * (gObject.width / 2) + gObject.width*.2);
        }
        //gObject.x = 0;
        //Debug.Log("n6: " + moveObj);

        //graph = SearchChild("n37").asGraph;

        //RenderTexture renderTexture = Resources.Load<RenderTexture>("Game/Choose/RotateTexture");
        //image = new Image();
        //image.width = 300;
        //image.height = 346;
        //image.texture = new NTexture(renderTexture);

        //Material material = Resources.Load<Material>("Game/Choose/RotateTexture");
        //image.material = material;
        //graph.SetNativeObject(image);


        //_touchArea = SearchChild("joystick_touch");


        //_InitX = _touchArea.x;// + _touchArea.width / 2;
        //_InitY = _touchArea.y; //+ _touchArea.height / 2;
        //Debug.LogError("basePos: " + _InitX + "  " + _InitY);
        //touchId = -1;
        //radius = 150;

        //_touchArea.onTouchBegin.Add(this.OnTouchBegin);
        //_touchArea.onTouchMove.Add(this.OnTouchMove);
        //_touchArea.onTouchEnd.Add(this.OnTouchEnd);
        //n33 = SearchChild("joystick_center");
        //basePos = GRoot.inst.GlobalToLocal(new Vector3(n33.x, n33.y));
        //ui.onTouchMove.Add(this.OnTouchMove);





    }




    private void OnTouchBegin(EventContext context)
    {
        if (touchId == -1)//First touch
        {
            InputEvent evt = (InputEvent)context.data;
            touchId = evt.touchId;


            Vector2 pt = GRoot.inst.GlobalToLocal(new Vector2(evt.x, evt.y));
            //Vector3 vecA = pt;

            ////Vector3 direction = vecB - vecA;                                    ///< 终点减去起点（AB方向与X轴的夹角）
            //Vector3 direction = vecA - basePos;                                   //BA方向与X轴的夹角）
            //float angle = Vector3.Angle(direction, Vector3.right);              ///< 计算旋转角度
            //direction = Vector3.Normalize(direction);                           ///< 向量规范化
            //float dot = Vector3.Dot(direction, Vector3.up);                  ///< 判断是否Vector3.right在同一方向
            //if (dot < 0)
            //{

            //    angle = 360 - angle;
            //    Debug.LogError("....");
            //}

            //targetAngle = angle;
            //targetVec = new Vector3(0, 0, angle);
            /////< 补充点 通过Atan2与方向向量的两条边可以计算出转向的角 通过计算结果可以看到targetAngle与-AtanTarget相加正好是360
            ////即二者都指向同一方向。具体使用场景需要根据具体需求分析。
            //offsetDegress = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //Debug.Log("star: " + offsetDegress);
            basePos = GRoot.inst.GlobalToLocal(new Vector3(rotateObj.x, rotateObj.y));



            glPrFltDeltaRotation = 0f;

            //先计算基本角度 之前的角度
            glPrFltPreviousRotation = angleBetweenPoints(pt, pt);
            context.CaptureTouch();
        }
    }
    float offsetDegress;
    private void OnTouchEnd(EventContext context)
    {
        InputEvent inputEvt = (InputEvent)context.data;
        if (touchId != -1 && inputEvt.touchId == touchId)
        {
            touchId = -1;


        }
    }
    private float targetAngle, AtanTarget;
    private Vector3 targetVec;
    private Vector3 beforePos;


    private void OnTouchMove(EventContext context)
    {

        InputEvent evt = (InputEvent)context.data;

        if (touchId != -1 && evt.touchId == touchId)
        {
            Vector2 pt = GRoot.inst.GlobalToLocal(new Vector2(evt.x, evt.y));



            //basePos = GRoot.inst.GlobalToLocal(new Vector3(rotateObj.x, rotateObj.y));
            //// Rotate along the mouse under Delta Rotation Limit 计算出当下的角度
            //glPrFltCurrentRotation = angleBetweenPoints(basePos, pt);
            ////这里的意思 如果当前的角度大于点击时候的角度，那么就使用之前的角度即可
            //glPrFltDeltaRotation = Mathf.DeltaAngle(glPrFltCurrentRotation, glPrFltPreviousRotation);
            ////60度的限制
            //if (Mathf.Abs(glPrFltDeltaRotation) > glPlFltDeltaLimit)
            //{
            //    glPrFltDeltaRotation = glPlFltDeltaLimit * Mathf.Sign(glPrFltDeltaRotation);
            //}
            //glPrFltPreviousRotation = glPrFltCurrentRotation;
            //rotateObj.displayObject.gameObject.transform.Rotate(Vector3.back * Time.deltaTime, glPrFltDeltaRotation);
            //rotateObj.

            //Vector3 vecA = pt; 
            ////Vector3 direction = vecB - vecA;                                    ///< 终点减去起点（AB方向与X轴的夹角）
            //Vector3 direction = basePos - vecA;                                   //BA方向与X轴的夹角）
            //float angle = Vector3.Angle(direction, Vector3.right);              ///< 计算旋转角度
            //direction = Vector3.Normalize(direction);                           ///< 向量规范化
            //float dot = Vector3.Dot(direction, Vector3.up);                  ///< 判断是否Vector3.right在同一方向
            //if (dot < 0)
            //{
            //    angle = 360 - angle;
            //    //Debug.LogError("....");
            //}

            //targetAngle = angle;
            //targetVec = new Vector3(0, 0, angle);
            /////< 补充点 通过Atan2与方向向量的两条边可以计算出转向的角 通过计算结果可以看到targetAngle与-AtanTarget相加正好是360
            ////即二者都指向同一方向。具体使用场景需要根据具体需求分析。
            //AtanTarget = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            ////if(Mathf.Abs(rotateObj.rotation - AtanTarget) < 11)
            //{
            //    rotateObj.rotation = AtanTarget;//.LerpAngle(rotateObj.rotation, AtanTarget, Time.deltaTime*5); //Quaternion.Slerp(, Quaternion.Euler(0, 0, AtanTarget), 0.1f);
            //    //rotateObj.rotation = Mathf.Lerp(rotateObj.rotation, AtanTarget, 0.5f); //Quaternion.Slerp(, Quaternion.Euler(0, 0, AtanTarget), 0.1f);
            //    offsetDegress = 0;
            //    Debug.LogError("....: " + AtanTarget + "   " + rotateObj.rotation);
            //}
            ////else
            //{
            //    //Debug.Log("大于该角度了 " + AtanTarget);
            //}


        }

    }

    bool isDown;
    Vector3 downPosition;
    float beforeX;
    void Update()
    {
        //RotateThis();
        //CountRotations();
        //if (image != null)

        //image.texture = new NTexture(NGUIEditorExtensions.RenderToTexture(ArrowTest.arrowTest.tCamera, 200, 200));


        if (Input.GetMouseButtonDown(0))
        {

            isDown = true;

            //downPosition = Input.mousePosition;
            beforeX = Input.mousePosition.x;

        }



        if (Input.GetMouseButton(0) && isDown)
        {

            float nowX = Input.mousePosition.x;
            float dis = nowX - beforeX;
            Debug.Log(beforeX + "  _  " + beforeX + dis);
            beforeX = nowX;

            moveObj.x += dis;

        }


        if (Input.GetMouseButtonUp(0))
        {
            isDown = false;
        }


    }

    private void CountRotations()
    {
        if (boolCountRotations)
        {
            if (Mathf.Sign(glPrFltDeltaRotation) == 1)
            {
                glPrFloatRotation += glPrFltDeltaRotation;
            }

            if (glPrFloatRotation >= 360)
            {
                glPrFloatRotation -= 360;
                glPrIntCurrentLaps -= 1;
                if (glPrIntCurrentLaps <= 0)
                {
                    glPlBoolCanRotate = false;
                    StartCoroutine(EnableSpinForever(22));
                }
            }
        }
    }

    Vector2 center = new Vector2(0.5f, 0.5f);
    private void RotateThis()
    {
        //if(rotateObj.displayObject.pivot != center)
        //{
        //rotateObj.displayObject.pivot = new Vector2(0.5f, 0.5f);
        //}
        //rotateObj.SetPivot(0.5f, 0.5f, true);
        //Debug.Log(".." + rotateObj.displayObject + "   "   + rotateObj.displayObject.pivot);
        if (Input.GetMouseButtonDown(0) && glPlBoolCanRotate)
        {
            //rotateObj.pivot = center;
            //rotateObj.SetPivot(0.5f, 0.5f);

            Vector2 screenPos = GRoot.inst.LocalToGlobal(rotateObj.position);
            screenPos.y = Screen.height - screenPos.y;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            // Get initial rotation of this game object
            glPrFltDeltaRotation = 0f;
            //先计算基本角度 之前的角度
            glPrFltPreviousRotation = angleBetweenPoints(worldPos, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //Debug.Log("1111111");
        }
        else if (Input.GetMouseButton(0) && glPlBoolCanRotate)
        {
            rotateObj.displayObject.pivot = new Vector2(0.5f, 0.5f);
            //rotateObj.displayObject.pivot = new Vector2(0.5f, 0.5f);
            //Debug.Log("Input.mousePosition: " + Input.mousePosition);
            //rotateObj.pivot = center;
            //rotateObj.SetPivot(0.5f, 0.5f);
            Vector2 screenPos = GRoot.inst.LocalToGlobal(rotateObj.position);
            screenPos.y = Screen.height - screenPos.y;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            // Rotate along the mouse under Delta Rotation Limit 计算出当下的角度
            glPrFltCurrentRotation = angleBetweenPoints(worldPos, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //这里的意思 如果当前的角度大于点击时候的角度，那么就使用之前的角度即可
            glPrFltDeltaRotation = Mathf.DeltaAngle(glPrFltCurrentRotation, glPrFltPreviousRotation);
            //60度的限制
            if (Mathf.Abs(glPrFltDeltaRotation) > glPlFltDeltaLimit)
            {
                glPrFltDeltaRotation = glPlFltDeltaLimit * Mathf.Sign(glPrFltDeltaRotation);
            }
            glPrFltPreviousRotation = glPrFltCurrentRotation;
            //rotateObj.rotation = glPrFltCurrentRotation;
            //rotateObj.TweenRotate(glPrFltCurrentRotation,  Time.deltaTime);
         
            rotateObj.displayObject.gameObject.transform.Rotate(Vector3.back * Time.deltaTime, glPrFltDeltaRotation);
            //Debug.Log("aaaaaaa " + glPrFltDeltaRotation);
        }
        else
        {
            //rotateObj.displayObject.pivot = new Vector2(0.5f, 0.5f);
            // Inertia
            rotateObj.displayObject.gameObject.transform.Rotate(Vector3.back * Time.deltaTime, glPrFltDeltaRotation);
            //transform.Rotate(Vector3.back * Time.deltaTime, glPrFltDeltaRotation);
            glPrFltDeltaRotation = Mathf.Lerp(glPrFltDeltaRotation, 0, glPlFltDeltaReduce * Time.deltaTime);
        }

        //PlaySound();
    }

    /// <summary>
    /// 计算两个点的角度
    /// </summary>
    /// <returns>The between points.</returns>
    /// <param name="v2Position1">V2 position1. 物体位置</param>
    /// <param name="v2Position2">V2 position2. 鼠标位置</param>
    private float angleBetweenPoints(Vector2 v2Position1, Vector2 v2Position2)
    {
        //Debug.Log("v2Position1: " + v2Position1 + "  " + v2Position2);
        //计算出方向
        Vector2 v2FromLine = v2Position2 - v2Position1;

        Vector2 v2ToLine = new Vector2(1, 0);
        //计算出和右边的角度
        float fltAngle = Vector2.Angle(v2FromLine, v2ToLine);
        //Debug.Log(fltAngle + " _ " + v2FromLine.ToString() + "  " + v2ToLine.ToString());
        // If rotation is more than 180
        Vector3 v3Cross = Vector3.Cross(v2FromLine, v2ToLine);
        if (v3Cross.z > 0)
        {
            fltAngle = 360f - fltAngle;
        }

        return fltAngle;
    }

    private IEnumerator EnableSpinForever(int intWaitSeconds)
    {
        yield return new WaitForSeconds(intWaitSeconds);
        glPlBoolCanRotate = true;
        boolCountRotations = false;
    }

    private void PlaySound()
    {
        glPrFltQuarterRotation += Mathf.Abs(glPrFltDeltaRotation);

        if (glPrFltQuarterRotation >= 90)
        {
            glPrFltQuarterRotation -= 90;
            //SoundManager.instance.RandomizeSfxShot(glPlSpinSound);
        }
    }

}
