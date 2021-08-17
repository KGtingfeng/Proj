using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleMappingCom : MonoBehaviour
{
    public static RoleMappingCom roleMappingCom;

    MeshRenderer meshRenderer;
    Material material;
    MaterialPropertyBlock materialProperty;


    private void Awake()
    {
        alpha = 0;
        roleMappingCom = this;
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.sharedMaterial;
        materialProperty = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(materialProperty);



        Color color = material.color;
        color.a = alpha;
        material.color = material.color;
        materialProperty.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(materialProperty);




    }

    // Start is called before the first frame update
    void Start()
    {

        //FadeIn();


    }

    float alpha = 0;
    // Update is called once per frame
    void Update()
    {

        if (isFadeIn)
        {
            if (alpha < 1)
            {
                alpha += Time.deltaTime * 0.9f;
                if (alpha > 1)
                {
                    alpha = 1;
                    isFadeIn = false; 
                }

                if (alpha > 0.55f)
                {
                    fadeInComplete?.Invoke();
                    fadeInComplete = null;
                }
                RefreshAlpha();
            }
        }

        if (isFadeOut)
        {
            if (alpha >= 0)
            {
                alpha -= Time.deltaTime * 0.9f;
                if (alpha <= 0)
                {
                    alpha = 0;
                    isFadeOut = false;
                    fadeOutComplete?.Invoke();
                    fadeOutComplete = null;
                }
                RefreshAlpha();
            }
        }
    }


    void RefreshAlpha()
    {
        Color color = material.color;
        color.a = alpha;
        materialProperty.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(materialProperty);
    }

    bool isFadeIn;
    bool isFadeOut;

    Action fadeInComplete;
    Action fadeOutComplete;

    public void FadeIn(Action calllBack = null)
    {
        Debug.Log("coming....." + alpha);
        //RoleMgr.roleMgr.GetSkeletonAnimation.timeScale = 0;
        fadeInComplete = calllBack;
        alpha = 0;
        RefreshAlpha();
        isFadeIn = true;
        isFadeOut = false;
    }

    public void FadeOut(Action calllBack = null)
    {
        if (alpha == 0)
        {
            calllBack?.Invoke();
            return;
        }
        fadeOutComplete = calllBack;
        alpha = 1;
        RefreshAlpha();
        isFadeIn = false;
        isFadeOut = true;
    }

    public float IsAlpha
    {
        get
        {
            return alpha;
        }
    }





}
