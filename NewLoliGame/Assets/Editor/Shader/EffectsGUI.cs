using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EffectsGUI : ShaderGUI {

    public enum BlendMode
    {
        alpha_blend,
        premultiplied_alpha_blend,
        additive_one_one,
        additive_srcAlpha_one,
        soft_additive
    }

    private static class ParticlesShaderStyles
    {
        //public static GUIContent rimText = new GUIContent("边缘泛光", "边缘泛光的颜色和强度");

        public static string blendMode = "混合模式";
        public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));

        public static GUIContent albedoText = new GUIContent("主纹理", "可以带可以不带alpha通道的纹理!");
        public static GUIContent SecondText = new GUIContent("副纹理", "开启副纹理，需要副纹理的alpha通道来混合!");
        public static GUIContent maskText = new GUIContent("遮罩纹理", "遮罩纹理的R通道用于遮罩, G通道和B通道用于噪声纹理做消融等效果!");
        public static GUIContent maskRampText = new GUIContent("渐变纹理贴图", "用mask贴图的r通道的亮度作为渐变贴图的uv来采样!");
        public static GUIContent dissolveText = new GUIContent("溶解纹理", "溶解纹理用RG通道分别计算，如果不用两个溶解贴图，则G通道值为255");
        public static GUIContent airDistortionText = new GUIContent("空气扭曲", "空气扭曲效果");
    }

    MaterialProperty blendMode = null;
    MaterialProperty mainColor = null;
    MaterialProperty mainIntensity = null;
    MaterialProperty albedoMap = null;
    MaterialProperty secondColor = null;
    MaterialProperty secondMap = null;
    MaterialProperty secondIntensity = null;
    MaterialProperty secTexUVAnimParas = null;
    MaterialProperty maskMap = null;
    MaterialProperty maskUVAnimation = null;
    MaterialProperty maskPower = null;
    MaterialProperty maskRampMap = null;
    MaterialProperty maskRampColor = null;
    MaterialProperty dissolveMap = null;
    MaterialProperty dissolveColor = null;
    MaterialProperty dissolveThreshold = null;
    MaterialProperty airDistortionBumpMap = null;
    MaterialProperty airDistortionBumpScale = null;
    MaterialProperty airDistortionTint = null;
    MaterialProperty rimColor = null;
    MaterialProperty rimIntensity = null;
    MaterialProperty uvAnimation = null;

    MaterialProperty cull = null;
    MaterialProperty zwrite = null;


    MaterialProperty testColor = null;



    MaterialEditor m_MaterialEditor;

    bool m_FirstTimeApply = true;
    struct ShaderFeaturesSwitch
    {
        public bool m_rimEnable;
        public bool m_UVAnimationEnable;
        public bool m_maskEnable;
        public bool m_maskRampEnable;
        public bool m_dissolveEnable;
        public bool m_airDistortionEnable;
        public ShaderFeaturesSwitch(bool value)
        {
            m_rimEnable = value;
            m_UVAnimationEnable = value;
            m_maskEnable = value;
            m_maskRampEnable = value;
            m_dissolveEnable = value;
            m_airDistortionEnable = value;
        }
    }

    private static bool m_alphaAddActive = false;
    private static bool m_alphaAddIsActive = false;
    private ShaderFeaturesSwitch m_shaderFeature = new ShaderFeaturesSwitch(false);

    public void FindProperties(MaterialProperty[] props)
    {
        blendMode = FindProperty("_Mode", props); 
        mainColor = FindProperty("_MainColor", props);
        mainIntensity = FindProperty("_MainIntentiy", props);
        albedoMap = FindProperty("_MainTex", props);
        secondColor = FindProperty("_SecondColor", props);
        secondMap = FindProperty("_SecondTex", props);
        secondIntensity = FindProperty("_SecondIntentiy", props);
        secTexUVAnimParas = FindProperty("_SecTexUVAnimParas", props);
        maskMap = FindProperty("_MaskDissolveTexture", props);
        maskUVAnimation = FindProperty("_MaskUVAnimation", props);
        maskPower = FindProperty("_MaskPower", props);
        maskRampMap = FindProperty("_MaskRampTex", props);
        maskRampColor = FindProperty("_MaskRampColor", props);
        dissolveMap = FindProperty("_DissolveMap", props);
        dissolveColor = FindProperty("_DissolveColor", props);
        dissolveThreshold = FindProperty("_DissolveThreshold", props);
        //空气扭曲
        airDistortionBumpMap = FindProperty("_AirDistortionBump", props);
        airDistortionBumpScale = FindProperty("_AirDistortionBumpScale", props);
        airDistortionTint = FindProperty("_AirDistortionTint", props);
        //边缘泛光
        rimColor = FindProperty("_RimColor", props);
        rimIntensity = FindProperty("_RimIntensity", props);
        //uv animation
        uvAnimation = FindProperty("_UVAnimationParas", props);

        //cull
        cull = FindProperty("_Cull", props);
        //zwrite
        zwrite = FindProperty("_ZWrite", props);


        //testColor = FindProperty("_TestColor", props);
    }

    public void GetSwitchFromMaterial(Material material)
    {
        m_shaderFeature.m_rimEnable = material.IsKeywordEnabled("_RIM_ENABLE");
        m_shaderFeature.m_UVAnimationEnable = material.IsKeywordEnabled("_UV_ANIMATION");
        m_shaderFeature.m_maskEnable = material.IsKeywordEnabled("_MASK_ENABLE");
        m_shaderFeature.m_dissolveEnable = material.IsKeywordEnabled("_DISSOLVE_ENABLE");
        m_shaderFeature.m_maskRampEnable = material.IsKeywordEnabled("_MASK_RAMP_ENABLE");
        m_shaderFeature.m_airDistortionEnable = material.IsKeywordEnabled("_AIR_DISTORTION_ENABLE");
    }

    public void ShaderPropertiesGUI(Material material)
    {
        EditorGUIUtility.labelWidth = 1;
        // Use default labelWidth
        EditorGUIUtility.labelWidth = 0f;

        // Detect any changes to the material
        EditorGUI.BeginChangeCheck();
        {
            BlendModePopup();
            //m_MaterialEditor.ShaderProperty(testColor, "颜色", 0);
            m_MaterialEditor.TexturePropertySingleLine(ParticlesShaderStyles.albedoText, albedoMap, mainColor, mainIntensity);
            m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);

            EditorGUILayout.Separator();
            GUILayout.Label("----------副纹理(需要用副纹理的alpha来混合)----------", EditorStyles.boldLabel);
            bool isKeyworld = material.IsKeywordEnabled("_SECONDMAP_ENALBE");
            bool bValue = EditorGUILayout.Toggle("副纹理开关", isKeyworld);
            if (bValue != isKeyworld)
            {
                if (bValue)
                    material.EnableKeyword("_SECONDMAP_ENALBE");
                else
                    material.DisableKeyword("_SECONDMAP_ENALBE");
            }
            if(material.IsKeywordEnabled("_SECONDMAP_ENALBE"))
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("混合", GUILayout.Width(80)))
                {
                    material.SetFloat("_SecBlendMode", 0);
                }
                if (GUILayout.Button("叠加", GUILayout.Width(80)))
                {
                    material.SetFloat("_SecBlendMode", 1);
                }
                GUILayout.EndHorizontal();
                m_MaterialEditor.TexturePropertySingleLine(ParticlesShaderStyles.SecondText, secondMap, secondColor, secondIntensity);
                m_MaterialEditor.TextureScaleOffsetProperty(secondMap);
                m_MaterialEditor.ShaderProperty(secTexUVAnimParas, "       UV滚动(xy): ");
            }

            GUILayout.Label("----------各效果开关----------", EditorStyles.boldLabel);
            //rim
            bValue = EditorGUILayout.Toggle("边缘泛光开关", m_shaderFeature.m_rimEnable);
            if (bValue != m_shaderFeature.m_rimEnable)
                m_shaderFeature.m_rimEnable = bValue;
            if (m_shaderFeature.m_rimEnable)
            {
                m_MaterialEditor.ShaderProperty(rimColor, "颜色", 2);
                m_MaterialEditor.ShaderProperty(rimIntensity, "强度", 2);
            }
            //uv animation

            bValue = EditorGUILayout.Toggle("UV滚动开关", m_shaderFeature.m_UVAnimationEnable);
            if (bValue != m_shaderFeature.m_UVAnimationEnable)
                m_shaderFeature.m_UVAnimationEnable = bValue;
            if (m_shaderFeature.m_UVAnimationEnable)
            {
                GUILayout.Label("       xy为主纹理使用，zw为溶解纹理使用:", EditorStyles.boldLabel);
                m_MaterialEditor.ShaderProperty(uvAnimation, "参数", 2);
            }
            //mask
            bValue = EditorGUILayout.Toggle("遮罩开关", m_shaderFeature.m_maskEnable);
            if (bValue != m_shaderFeature.m_maskEnable)
                m_shaderFeature.m_maskEnable = bValue;
            if(m_shaderFeature.m_maskEnable)
            {
                GUILayout.Label("       xy为遮罩使用，zw保留:", EditorStyles.boldLabel);
                m_MaterialEditor.ShaderProperty(maskUVAnimation, "参数", 2);
                m_MaterialEditor.TexturePropertySingleLine(ParticlesShaderStyles.maskText, maskMap, maskPower);
                //mask ramp
                bValue = EditorGUILayout.Toggle("     渐变贴图开关", m_shaderFeature.m_maskRampEnable);
                if (bValue != m_shaderFeature.m_maskRampEnable)
                    m_shaderFeature.m_maskRampEnable = bValue;
                if (m_shaderFeature.m_maskRampEnable)
                {
                    m_MaterialEditor.TexturePropertySingleLine(ParticlesShaderStyles.maskRampText, maskRampMap, maskRampColor);
                }
            }
            //dissolve
            bValue = EditorGUILayout.Toggle("溶解开关", m_shaderFeature.m_dissolveEnable);
            if (bValue != m_shaderFeature.m_dissolveEnable)
                m_shaderFeature.m_dissolveEnable = bValue;
            if (m_shaderFeature.m_dissolveEnable)
            {
                m_MaterialEditor.TexturePropertySingleLine(ParticlesShaderStyles.dissolveText, dissolveMap, dissolveColor, dissolveThreshold);
                m_MaterialEditor.TextureScaleOffsetProperty(dissolveMap);
                //m_MaterialEditor.ShaderProperty(dissolveColor, "溶解颜色", 3);
                //m_MaterialEditor.ShaderProperty(dissolveThreshold, "溶解强度", 3);
            }
            //air distortion
            bValue = EditorGUILayout.Toggle("空气扭曲开关", m_shaderFeature.m_airDistortionEnable);
            if (bValue != m_shaderFeature.m_airDistortionEnable)
                m_shaderFeature.m_airDistortionEnable = bValue;
            if (m_shaderFeature.m_airDistortionEnable)
            {
                m_MaterialEditor.TexturePropertySingleLine(ParticlesShaderStyles.airDistortionText, airDistortionBumpMap, airDistortionBumpScale);
                m_MaterialEditor.TextureScaleOffsetProperty(airDistortionBumpMap);
                m_MaterialEditor.RangeProperty(airDistortionTint, "透明度:");
            }

            EditorGUILayout.Separator();

           EditorGUILayout.Separator();

            //double side
            bool curCullMode = (int)cull.floatValue == (int)UnityEngine.Rendering.CullMode.Off;
            bValue = EditorGUILayout.Toggle("双面开关", curCullMode);
            if(bValue != curCullMode)
            {
                if (bValue)
                    material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                else
                    material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Back);
            }

            //zwrite
            EditorGUILayout.Separator();
            bool curZWrite = (int)zwrite.floatValue == 1;
            bValue = EditorGUILayout.Toggle("深度写入(是否被遮挡)", curZWrite);
            if(bValue != curZWrite)
            {
                if (bValue)
                    material.SetInt("_ZWrite", 1);
                else
                    material.SetInt("_ZWrite", 0);
            }

            //render queue
            GUILayout.Label("---------渲染顺序设置----------", EditorStyles.boldLabel);
            m_MaterialEditor.RenderQueueField();

        }

        if (EditorGUI.EndChangeCheck())
        {
            foreach (var obj in blendMode.targets)
            {
                MaterialChanged((Material)obj, m_shaderFeature);
            }
        }
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    { 

        FindProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
        m_MaterialEditor = materialEditor;
        Material material = materialEditor.target as Material;

        // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
        // material to a standard shader.
        // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
        if (m_FirstTimeApply)
        {
            GetSwitchFromMaterial(material);
            MaterialChanged(material, m_shaderFeature);
            m_FirstTimeApply = false;
        }
        ShaderPropertiesGUI(material);
    }

    void BlendModePopup()
    {
        EditorGUI.showMixedValue = blendMode.hasMixedValue;
        var mode = (BlendMode)blendMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (BlendMode)EditorGUILayout.Popup(ParticlesShaderStyles.blendMode, (int)mode, ParticlesShaderStyles.blendNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("Blend Mode");
            blendMode.floatValue = (float)mode;
        }

        EditorGUI.showMixedValue = false;
    }

    public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
    {
        if (blendMode == BlendMode.additive_srcAlpha_one && !m_alphaAddActive)
            m_alphaAddActive = true;
        if(blendMode != BlendMode.additive_srcAlpha_one)
        {
            m_alphaAddIsActive = false;
            m_alphaAddActive = false;
        }
           
        switch (blendMode)
        {
            case BlendMode.alpha_blend:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                break;
            case BlendMode.premultiplied_alpha_blend:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                break;
            case BlendMode.additive_one_one:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                break;
            case BlendMode.additive_srcAlpha_one:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                //if (m_alphaAddActive && !m_alphaAddIsActive)
                //{
                //    material.SetFloat("_MainIntentiy", 2);
                //    m_alphaAddIsActive = true;
                //}
                break;
            case BlendMode.soft_additive:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                break;         
        }
    }

    void SetMaterialKeywords(Material material, ShaderFeaturesSwitch shaderFeature)
    {
        //rim
        if (shaderFeature.m_rimEnable)
            material.EnableKeyword("_RIM_ENABLE");
        else
            material.DisableKeyword("_RIM_ENABLE");
        //uv animation
        if (shaderFeature.m_UVAnimationEnable)
            material.EnableKeyword("_UV_ANIMATION");
        else
            material.DisableKeyword("_UV_ANIMATION");
        //mask
        if (shaderFeature.m_maskEnable)
            material.EnableKeyword("_MASK_ENABLE");
        else
            material.DisableKeyword("_MASK_ENABLE");
        //dissolve
        if (shaderFeature.m_maskRampEnable)
            material.EnableKeyword("_MASK_RAMP_ENABLE");
        else
            material.DisableKeyword("_MASK_RAMP_ENABLE");
        //dissolve
        if (shaderFeature.m_dissolveEnable)
            material.EnableKeyword("_DISSOLVE_ENABLE");
        else
            material.DisableKeyword("_DISSOLVE_ENABLE");
        //air distortion
        if (shaderFeature.m_airDistortionEnable)
            material.EnableKeyword("_AIR_DISTORTION_ENABLE");
        else
            material.DisableKeyword("_AIR_DISTORTION_ENABLE");
    }

    void MaterialChanged(Material material, ShaderFeaturesSwitch shaderFeature)
    {
        SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
        SetMaterialKeywords(material, shaderFeature);
    }

}
