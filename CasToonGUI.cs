using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System;

public class CasToonGUI : ShaderGUI
{
    MaterialEditor editor;
    MaterialProperty[] properties;
    bool Base = false;
    bool Shadow = false;
    bool Rim = false, RimDis = false;
    bool Matcap = false, MatDis = false;
    bool Metal = false, MetalDis = false;
    bool Spec = false, SpecDis = false;
    bool Emis = false, EmisDis = false;
    bool Lighting = false;

    override public void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.editor = materialEditor;
        this.properties = properties;
        Toggles();
    }

    void Toggles()
    {
        
        Base = EditorGUILayout.Foldout(Base, "Main");
        if (Base){MainGroup();}

        Shadow = EditorGUILayout.Foldout(Shadow, "Shadow");
        if (Shadow){ShadowGroup();}

        //Rimlight
        {
            GUILayout.BeginHorizontal();
            Rim = EditorGUILayout.Foldout(Rim, "Rim Light");

            MaterialProperty tog = FindProperty("_rimtog", properties);
            RimDis = Convert.ToBoolean(tog.floatValue);
            RimDis = GUILayout.Toggle(RimDis, GUIContent.none);
            if (RimDis == true) { tog.floatValue = 1f; } else { tog.floatValue = 0f; }

            GUILayout.EndHorizontal();
            MaterialProperty rimtog = FindProperty("_rimtog", properties);
            if (Rim) { RimGroup(); }
        }

        //Matcap
        {
            GUILayout.BeginHorizontal();
            Matcap = EditorGUILayout.Foldout(Matcap, "Matcap");

            MaterialProperty tog = FindProperty("_mattog", properties);
            MatDis = Convert.ToBoolean(tog.floatValue);
            MatDis = GUILayout.Toggle(MatDis, GUIContent.none);
            if (MatDis == true) { tog.floatValue = 1f; } else { tog.floatValue = 0f; }

            GUILayout.EndHorizontal();
            if (Matcap) { MatcapGroup(); }
        }

        //Metal
        {
            GUILayout.BeginHorizontal();
            Metal = EditorGUILayout.Foldout(Metal, "Metal");

            MaterialProperty tog = FindProperty("_metaltog", properties);
            MetalDis = Convert.ToBoolean(tog.floatValue);
            MetalDis = GUILayout.Toggle(MetalDis, GUIContent.none);
            if (MetalDis == true) { tog.floatValue = 1f; } else { tog.floatValue = 0f; }

            GUILayout.EndHorizontal();
            if (Metal) { MetalGroup(); }
        }

        //Specular
        {
            GUILayout.BeginHorizontal();
            Spec = EditorGUILayout.Foldout(Spec, "Specular");

            MaterialProperty tog = FindProperty("_spectog", properties);
            SpecDis = Convert.ToBoolean(tog.floatValue);
            SpecDis = GUILayout.Toggle(SpecDis, GUIContent.none);
            if (SpecDis == true) { tog.floatValue = 1f; } else { tog.floatValue = 0f; }

            GUILayout.EndHorizontal();
            if (Spec) { SpecGroup(); }
        }

        //Emission
        {
            GUILayout.BeginHorizontal();
            Emis = EditorGUILayout.Foldout(Emis, "Emission");

            MaterialProperty tog = FindProperty("_emistog", properties);
            EmisDis = Convert.ToBoolean(tog.floatValue);
            EmisDis = GUILayout.Toggle(EmisDis, GUIContent.none);
            if (EmisDis == true) { tog.floatValue = 1f; } else { tog.floatValue = 0f; }

            GUILayout.EndHorizontal();
            if (Emis) { EmisGroup(); }
        }

        Lighting = EditorGUILayout.Foldout(Lighting, "Lighting");
        if (Lighting){LightingGroup();}

    }

    void MainGroup()
    {
        MaterialProperty mainTex = FindProperty("_MainTex", properties);
        GUIContent albedoLabel = new GUIContent(mainTex.displayName, "Base (Albeto) Texture");
        MaterialProperty mainColor = FindProperty("_MainColor", properties);
		editor.TexturePropertySingleLine(albedoLabel, mainTex, mainColor);
        editor.TextureScaleOffsetProperty(mainTex);

        MaterialProperty normTex = FindProperty("_NormalMap", properties);
        GUIContent normalLabel = new GUIContent(normTex.displayName, "Normal Map Texture");
        editor.TexturePropertySingleLine(normalLabel, normTex);

        MaterialProperty normStrength = FindProperty("_NormalStrength", properties);
        editor.RangeProperty(normStrength, "Normal Strength");
    }

    void ShadowGroup()
    {
        MaterialProperty shadowRamp = FindProperty("_ShadowRamp", properties);
        GUIContent shadowLabel = new GUIContent(shadowRamp.displayName, "Shadow Gradient Map");
        MaterialProperty shadowColor = FindProperty("_ShadowColor", properties);
        editor.TexturePropertySingleLine(shadowLabel, shadowRamp, shadowColor);

        MaterialProperty shadowOffset = FindProperty("_ShadowOffset", properties);
        editor.RangeProperty(shadowOffset, "Shadow Offset");

        MaterialProperty shadmaskTex = FindProperty("_ShadMaskMap", properties);
        GUIContent shadtexLabel = new GUIContent(shadmaskTex.displayName, "Shadow Mask");
        editor.TexturePropertySingleLine(shadtexLabel, shadmaskTex);
    }

    void RimGroup()
    {
        EditorGUI.BeginDisabledGroup(RimDis == false);

        MaterialProperty rimColor = FindProperty("_RimColor", properties);
        editor.ColorProperty(rimColor, "Rim Color");

        MaterialProperty rimSize = FindProperty("_RimSize", properties);
        editor.FloatProperty(rimSize, "Rim Size");

        MaterialProperty rimIntensity = FindProperty("_RimIntensity", properties);
        editor.FloatProperty(rimIntensity, "Rim Intensity");

        MaterialProperty rimmaskTex = FindProperty("_RimMaskMap", properties);
        GUIContent rimtexLabel = new GUIContent(rimmaskTex.displayName, "Rimlight Mask");
        editor.TexturePropertySingleLine(rimtexLabel, rimmaskTex);

        EditorGUI.EndDisabledGroup();

    }

    void MatcapGroup()
    {
        EditorGUI.BeginDisabledGroup(MatDis == false);

        MaterialProperty matTex = FindProperty("_MatCap", properties);
        GUIContent matLabel = new GUIContent(matTex.displayName, "Matcap Tex");
        editor.TexturePropertySingleLine(matLabel, matTex);

        MaterialProperty matMul = FindProperty("_MatMultiply", properties);
        editor.RangeProperty(matMul, "Mat Multiply");

        MaterialProperty matAdd = FindProperty("_MatAdd", properties);
        editor.RangeProperty(matAdd, "Mat Add");

        MaterialProperty matmaskTex = FindProperty("_MatMaskMap", properties);
        GUIContent mattexLabel = new GUIContent(matmaskTex.displayName, "Matcap Mask");
        editor.TexturePropertySingleLine(mattexLabel, matmaskTex);

        EditorGUI.EndDisabledGroup();
    }

    void MetalGroup()
    {
        EditorGUI.BeginDisabledGroup(MetalDis == false);

        MaterialProperty metalref = FindProperty("_Metallic", properties);
        editor.RangeProperty(metalref, "Metallic");

        MaterialProperty refSmoothness = FindProperty("_RefSmoothness", properties);
        editor.RangeProperty(refSmoothness, "Roughness");

        MaterialProperty smoothtog = FindProperty("_invertSmooth", properties);
        bool smoothtogg = Convert.ToBoolean(smoothtog.floatValue);
        smoothtogg = GUILayout.Toggle(smoothtogg, GUIContent.none);
        if (smoothtogg == true) { smoothtog.floatValue = 1f; } else { smoothtog.floatValue = 0f; }

        MaterialProperty smoothmaskTex = FindProperty("_SmoothnessMaskMap", properties);
        GUIContent smoothtexLabel = new GUIContent(smoothmaskTex.displayName, "Smoothness Mask");
        editor.TexturePropertySingleLine(smoothtexLabel, smoothmaskTex);

        MaterialProperty metalmaskTex = FindProperty("_MetalMaskMap", properties);
        GUIContent metaltexLabel = new GUIContent(metalmaskTex.displayName, "Metal Mask");
        editor.TexturePropertySingleLine(metaltexLabel, metalmaskTex);

        MaterialProperty cubetog = FindProperty("_customcubemap", properties);
        bool cubetogg = Convert.ToBoolean(cubetog.floatValue);
        cubetogg = GUILayout.Toggle(cubetogg, GUIContent.none);
        if (cubetogg == true) { cubetog.floatValue = 1f; } else { cubetog.floatValue = 0f; }

        MaterialProperty customcubeTex = FindProperty("_CustomReflection", properties);
        GUIContent customcubeLabel = new GUIContent(customcubeTex.displayName, "Custom Reflection");
        editor.TexturePropertySingleLine(customcubeLabel, customcubeTex);

        EditorGUI.EndDisabledGroup();
    }

    void SpecGroup()
    {
        EditorGUI.BeginDisabledGroup(SpecDis == false);

        MaterialProperty specColor = FindProperty("_SpeccColor", properties);
        editor.ColorProperty(specColor, "Specular Color");

        MaterialProperty specSmoothness = FindProperty("_SpecSmoothness", properties);
        editor.RangeProperty(specSmoothness, "Smoothness");

        MaterialProperty speccSize = FindProperty("_SpeccSize", properties);
        editor.RangeProperty(speccSize, "Size");

        MaterialProperty specmaskTex = FindProperty("_SpecMaskMap", properties);
        GUIContent spectexLabel = new GUIContent(specmaskTex.displayName, "Specular Mask");
        editor.TexturePropertySingleLine(spectexLabel, specmaskTex);

        EditorGUI.EndDisabledGroup();
    }

    void EmisGroup()
    {
        EditorGUI.BeginDisabledGroup(EmisDis == false);

        MaterialProperty maskTex = FindProperty("_EmisTex", properties);
        GUIContent texLabel = new GUIContent(maskTex.displayName, "Emission Map");
        editor.TexturePropertySingleLine(texLabel, maskTex);

        MaterialProperty Color = FindProperty("_EmisColor", properties);
        editor.ColorProperty(Color, "Emission Color");

        MaterialProperty Power = FindProperty("_EmisPower", properties);
        editor.FloatProperty(Power, "Emisison Power");

        EditorGUI.EndDisabledGroup();
    }

    void LightingGroup()
    {
        MaterialProperty unlitIntensity = FindProperty("_UnlitIntensity", properties);
        editor.RangeProperty(unlitIntensity, "Unlit Intensity");

        MaterialProperty normFlatten = FindProperty("_NormFlatten", properties);
        editor.RangeProperty(normFlatten, "Flatten Light Direction");
    }
}
#endif