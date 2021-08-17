using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate2DTexture : MonoBehaviour
{
 
    public float glPlFltDeltaLimit;
    public float glPlFltDeltaReduce;
    public int glPlIntLapsBeforeStopping;
    public bool glPlBoolCanRotate { get; set; }
    public AudioClip glPlSpinSound;
    private float glPrFltDeltaRotation;
    private float glPrFltPreviousRotation;
    private float glPrFltCurrentRotation;
    private int glPrIntCurrentLaps;
    private float glPrFloatRotation;
    private float glPrFltQuarterRotation;
    private bool boolCountRotations;


    void Start()
    { 
        glPrIntCurrentLaps = glPlIntLapsBeforeStopping;
        glPrFloatRotation = 0f;
        glPlBoolCanRotate = true;
        boolCountRotations = true;
    }

    // Update is called once per frame
    void Update()
    {
        RotateThis();
        CountRotations();
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

    bool isDoubleClick = false;
    private void RotateThis()
    {
        if (Input.touchCount > 1)
        {
            isDoubleClick = true;
            return;
        }

        if (Input.GetMouseButtonDown(0) && glPlBoolCanRotate)
        {
            isDoubleClick = false;
            // Get initial rotation of this game object
            glPrFltDeltaRotation = 0f;
            //先计算基本角度 之前的角度
            glPrFltPreviousRotation = angleBetweenPoints(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        }
        if (Input.GetMouseButton(0) && glPlBoolCanRotate)
        {
            // Rotate along the mouse under Delta Rotation Limit 计算出当下的角度
            glPrFltCurrentRotation = angleBetweenPoints(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //这里的意思 如果当前的角度大于点击时候的角度，那么就使用之前的角度即可
            glPrFltDeltaRotation = Mathf.DeltaAngle(glPrFltCurrentRotation, glPrFltPreviousRotation);
            //60度的限制
            if (Mathf.Abs(glPrFltDeltaRotation) > glPlFltDeltaLimit)
            {
                glPrFltDeltaRotation = glPlFltDeltaLimit * Mathf.Sign(glPrFltDeltaRotation);
            }
            glPrFltPreviousRotation = glPrFltCurrentRotation;
            transform.Rotate(Vector3.back * Time.deltaTime, glPrFltDeltaRotation); 
        }
        if (!isDoubleClick)
        {

            transform.Rotate(Vector3.back * Time.deltaTime, glPrFltDeltaRotation);
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
        //计算出方向
        Vector2 v2FromLine = v2Position2 - v2Position1;

        Vector2 v2ToLine = new Vector2(1, 0);
        //计算出和右边的角度
        float fltAngle = Vector2.Angle(v2FromLine, v2ToLine);
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
