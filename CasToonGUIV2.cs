using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System;

public class CasToonGUIV2 : ShaderGUI
{
    MaterialEditor editor;
    
    // Enum Selection _________________________________________
    
    public enum _CullingMode{
        CullingOff, FrontCulling, BackCulling
    }
    public enum _RenderingMode{
        Opaque, Transparent
    }
    
    // Material Styles _______________________________________
    
    private static class Styles
    {
        public static GUIContent baseMapLabel = new GUIContent("Base Map");
        public static GUIContent normalMapLabel = new GUIContent("Normal Map");
        public static GUIContent shadowLabel = new GUIContent("Shadow Ramp");
        public static GUIContent shadowMaskLabel = new GUIContent("Shadow Mask");
        public static GUIContent rimlightMaskLabel = new GUIContent("Rimlight Mask");
        public static GUIContent matcapTexLabel = new GUIContent("Matcap Map");
        public static GUIContent matcapMaskLabel = new GUIContent("Matcap Mask");
        public static GUIContent smoothnessMap = new GUIContent("Smoothness Map");
        public static GUIContent metalMap = new GUIContent("Metallic Map");
        public static GUIContent cubemap = new GUIContent("Forced Cubemap");
        public static GUIContent specMask = new GUIContent("Specular Mask");
        public static GUIContent emisTex = new GUIContent("Emisison Mask");
    }

    // Material Foldouts ______________________________________
    
    static bool main = false;
    static bool shadow = false;
    static bool rimtog = false;
    static bool mattog = false;
    static bool spectog = false;
    static bool metaltog = false;
    static bool emistog = false;
    static bool lightingtog = false;
    bool maindis = false;
    bool shadowdis = false;
    bool rimtogdis = false;
    bool mattogdis = false;
    bool spectogdis = false;
    bool metaltogdis = false;
    bool emistogdis = false;
    bool lightingtogdis = false;
    
    // Material Properties ____________________________________

    MaterialProperty MainTex = null;
    MaterialProperty MainTex_ST = null;
    MaterialProperty MainColor = null;
    MaterialProperty NormalMap = null;
    MaterialProperty NormalStrength = null;
    MaterialProperty ShadMaskMap = null;
    MaterialProperty Transparency = null;

    MaterialProperty RimColor = null;
    MaterialProperty RimSize = null;
    MaterialProperty RimIntensity = null;
    MaterialProperty RimMaskMap = null;

    MaterialProperty ShadowRamp = null;
    MaterialProperty ShadowColor = null;
    MaterialProperty ShadowOffset = null;

    MaterialProperty MatCap = null;
    MaterialProperty MatMultiply = null;
    MaterialProperty MatAdd = null;
    MaterialProperty MatMaskMap = null;
            
    MaterialProperty Metallic = null;
    MaterialProperty RefSmoothness = null;
    MaterialProperty invertSmooth = null;
    MaterialProperty SmoothnessMaskMap = null;
    MaterialProperty MetalMaskMap = null;
    MaterialProperty fallbackColor = null;
    MaterialProperty customcubemap = null;
    MaterialProperty CustomReflection = null;    
            
    MaterialProperty SpeccColor = null;
    MaterialProperty SpecSmoothness = null;
    MaterialProperty SpeccSize = null;
    MaterialProperty SpecMaskMap = null;

    MaterialProperty EmisTex = null;
    MaterialProperty EmisColor = null;
    MaterialProperty EmisPower = null;

    MaterialProperty UnlitIntensity = null;
    MaterialProperty NormFlatten = null;
    MaterialProperty LightColorCont = null;
    
    MaterialProperty rimtogprop = null;
    MaterialProperty mattogprop = null;
    MaterialProperty spectogprop = null;
    MaterialProperty metaltogprop = null;
    MaterialProperty emistogprop = null;
    
    // Material Editor

    public void FindProperties(MaterialProperty[] props)
    {

        MainTex = FindProperty("_MainTex", props, true);
        MainTex_ST = FindProperty("_MainTex_ST", props, false);
        MainColor = FindProperty("_MainColor", props, true);
        NormalMap = FindProperty("_NormalMap", props, false);
        NormalStrength = FindProperty("_NormalStrength", props, false);
        ShadMaskMap = FindProperty("_ShadMaskMap", props, false);
        Transparency = FindProperty("_Transparency", props, false);

        RimColor = FindProperty("_RimColor", props, false);
        RimSize = FindProperty("_RimSize", props, false);
        RimIntensity = FindProperty("_RimIntensity", props, false);
        RimMaskMap = FindProperty("_RimMaskMap", props, false);

        ShadowRamp = FindProperty("_ShadowRamp", props, false);
        ShadowColor = FindProperty("_ShadowColor", props, false);
        ShadowOffset = FindProperty("_ShadowOffset", props, false);

        MatCap = FindProperty("_MatCap", props, false);
        MatMultiply = FindProperty("_MatMultiply", props, false);
        MatAdd = FindProperty("_MatAdd", props, false);
        MatMaskMap = FindProperty("_MatMaskMap", props, false);
        
        Metallic = FindProperty("_Metallic", props, false);
        RefSmoothness = FindProperty("_RefSmoothness", props, false);
        invertSmooth = FindProperty("_invertSmooth", props, false);
        SmoothnessMaskMap = FindProperty("_SmoothnessMaskMap", props, false);
        MetalMaskMap = FindProperty("_MetalMaskMap", props, false);
        fallbackColor = FindProperty("_fallbackColor", props, false);
        customcubemap = FindProperty("_customcubemap", props, false);
        CustomReflection = FindProperty("_CustomReflection", props, false);;    
        
        SpeccColor = FindProperty("_SpeccColor", props, false);
        SpecSmoothness = FindProperty("_SpecSmoothness", props, false);
        SpeccSize = FindProperty("_SpeccSize", props, false);
        SpecMaskMap = FindProperty("_SpecMaskMap", props, false);

        EmisTex = FindProperty("_EmisTex", props, false);
        EmisColor = FindProperty("_EmisColor", props, false);
        EmisPower = FindProperty("_EmisPower", props, false);;

        UnlitIntensity = FindProperty("_UnlitIntensity", props, false);
        NormFlatten = FindProperty("_NormFlatten", props, false);
        LightColorCont = FindProperty("_LightColorCont", props, false);
        
        rimtogprop = FindProperty("_rimtog", props, false);
        mattogprop = FindProperty("_mattog", props, false);
        spectogprop = FindProperty("_spectog", props, false);
        metaltogprop = FindProperty("_metaltog", props, false);
        emistogprop = FindProperty("_emistog", props, false);
       
    }

    static bool Foldout(bool display, string title)
    {
        var style = new GUIStyle("IN Title");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 22;
        style.contentOffset = new Vector2(1f, 4f);

        var rect = GUILayoutUtility.GetRect(16f, 22f, style);
        GUI.Box(rect, title, style);

        var e = Event.current;

        var toggleRect = new Rect(rect.x + 4f, rect.y + 6f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }

        return display;
    }

    override public void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material mat = materialEditor.target as Material;
        editor = materialEditor;
        EditorGUIUtility.fieldWidth = 0;
        FindProperties(properties);
        
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        
        GUILayout.BeginHorizontal();
        //maindis = GUILayout.Toggle(maindis, "", GUILayout.Width(20));
        main = Foldout(main, "Base Color, Normal Map and Transparency");
        GUILayout.EndHorizontal();
        if(main)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            MainGroup(mat);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        //shadowdis = GUILayout.Toggle(shadowdis, "", GUILayout.Width(20));
        shadow = Foldout(shadow, "Shadow Settings");
        GUILayout.EndHorizontal();
        if(shadow)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            ShadowGroup(mat);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        rimtogdis = Convert.ToBoolean(rimtogprop.floatValue);
        rimtogdis = GUILayout.Toggle(rimtogdis, "", GUILayout.Width(20));
        if (rimtogdis == true) { rimtogprop.floatValue = 1f; } else { rimtogprop.floatValue = 0f; }
        rimtog = Foldout(rimtog, "Rimlight Settings");
        GUILayout.EndHorizontal();
        EditorGUI.BeginDisabledGroup(!rimtogdis); 
        if(rimtog)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            RimGroup(mat);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        mattogdis = Convert.ToBoolean(mattogprop.floatValue);
        mattogdis = GUILayout.Toggle(mattogdis, "", GUILayout.Width(20));
        if (mattogdis == true) { mattogprop.floatValue = 1f; } else { mattogprop.floatValue = 0f; }
        mattog = Foldout(mattog, "Matcap Settings");
        GUILayout.EndHorizontal();
        EditorGUI.BeginDisabledGroup(!mattogdis);
        if(mattog)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            MatcapGroup(mat);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        metaltogdis = Convert.ToBoolean(metaltogprop.floatValue);
        metaltogdis = GUILayout.Toggle(metaltogdis, "", GUILayout.Width(20));
        if (metaltogdis == true) { metaltogprop.floatValue = 1f; } else { metaltogprop.floatValue = 0f; }
        metaltog = Foldout(metaltog, "Metallic/Smoothness Settings");
        GUILayout.EndHorizontal();
        EditorGUI.BeginDisabledGroup(!metaltogdis);
        if(metaltog)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            MetalGroup(mat);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        spectogdis = Convert.ToBoolean(spectogprop.floatValue);
        spectogdis = GUILayout.Toggle(spectogdis, "", GUILayout.Width(20));
        if (spectogdis == true) { spectogprop.floatValue = 1f; } else { spectogprop.floatValue = 0f; }
        spectog = Foldout(spectog, "Specular Settings");
        GUILayout.EndHorizontal();
        EditorGUI.BeginDisabledGroup(!spectogdis);
        if(spectog)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            SpecGroup(mat);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        emistogdis = Convert.ToBoolean(emistogprop.floatValue);
        emistogdis = GUILayout.Toggle(emistogdis, "", GUILayout.Width(20));
        if (emistogdis == true) { emistogprop.floatValue = 1f; } else { emistogprop.floatValue = 0f; }
        emistog = Foldout(emistog, "Emission Settings");
        GUILayout.EndHorizontal();
        EditorGUI.BeginDisabledGroup(!emistogdis);
        if(emistog)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            EmisGroup(mat);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        //lightingtogdis = GUILayout.Toggle(lightingtogdis, "", GUILayout.Width(20));
        lightingtog = Foldout(lightingtog, "Lighting Settings");
        GUILayout.EndHorizontal();
        if(lightingtog)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            LightingGroup(mat);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space(10);
        
        if (EditorGUI.EndChangeCheck())
        {
            editor.PropertiesChanged();
        }
    }

    void MainGroup(Material mat)
    {
        GUILayout.Label("Base Map", EditorStyles.boldLabel);
        editor.TexturePropertySingleLine(Styles.baseMapLabel, MainTex, MainColor);
        editor.TextureScaleOffsetProperty(MainTex);
        
        EditorGUILayout.Space(10); 

        GUILayout.Label("Normal Map", EditorStyles.boldLabel);
        editor.TexturePropertySingleLine(Styles.normalMapLabel, NormalMap);
        editor.RangeProperty(NormalStrength, "Normal Strength");

        EditorGUILayout.Space(10);

        if (mat.HasProperty("_Transparency"))
        {
            GUILayout.Label("Transparency", EditorStyles.boldLabel);
            editor.RangeProperty(Transparency, "Transparency");
        }
    }

    void ShadowGroup(Material mat)
    {
        GUILayout.Label("Shadow Settings", EditorStyles.boldLabel);
        editor.TexturePropertySingleLine(Styles.shadowLabel, ShadowRamp, ShadowColor);
        editor.RangeProperty(ShadowOffset, "Shadow Offset");
        editor.TexturePropertySingleLine(Styles.shadowMaskLabel, ShadMaskMap);
    }
    
    
    void RimGroup(Material mat)
    {
        GUILayout.Label("Rimlight Settings", EditorStyles.boldLabel);
        editor.ColorProperty(RimColor, "Rim Color");
        editor.FloatProperty(RimSize, "Rim Size");
        editor.FloatProperty(RimIntensity, "Rim Intensity");
        editor.TexturePropertySingleLine(Styles.rimlightMaskLabel, RimMaskMap);
    }
    
    void MatcapGroup(Material mat)
    {
        GUILayout.Label("Rimlight Settings", EditorStyles.boldLabel);
        editor.TexturePropertySingleLine(Styles.matcapTexLabel, MatCap);
        editor.RangeProperty(MatMultiply, "Mat Multiply");
        editor.RangeProperty(MatAdd, "Mat Add");
        editor.TexturePropertySingleLine(Styles.matcapMaskLabel, MatMaskMap);
    }
    
    void MetalGroup(Material mat)
    {
        editor.RangeProperty(Metallic, "Metallic");
        editor.RangeProperty(RefSmoothness, "Smoothness");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Invert Smoothness (Roughness)");
        if(mat.GetFloat("_invertSmooth") == 0){
            if (GUILayout.Button("Off",GUILayout.Width(130)))
            {
                mat.SetFloat("_invertSmooth",1);
            }
        }else{
            if (GUILayout.Button("Active",GUILayout.Width(130)))
            {
                mat.SetFloat("_invertSmooth",0);
            }
        }
        EditorGUILayout.EndHorizontal();
        editor.TexturePropertySingleLine(Styles.smoothnessMap, SmoothnessMaskMap);
        editor.TexturePropertySingleLine(Styles.metalMap, MetalMaskMap);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Use Fallback Cubemap");
        if(mat.GetFloat("_customcubemap") == 0){
           if (GUILayout.Button("Off",GUILayout.Width(130)))
           {
               mat.SetFloat("_customcubemap",1);
           }
        }else{
           if (GUILayout.Button("Active",GUILayout.Width(130)))
           {
               mat.SetFloat("_customcubemap",0);
           }
        }
        EditorGUILayout.EndHorizontal();
        editor.ColorProperty(fallbackColor, "Fallback Color");
        editor.TexturePropertySingleLine(Styles.cubemap, CustomReflection);
        EditorGUI.EndDisabledGroup();
    }
    
    void SpecGroup(Material mat)
    {
        editor.ColorProperty(SpeccColor, "Specular Color");
        editor.RangeProperty(SpecSmoothness, "Smoothness");
        editor.RangeProperty(SpeccSize, "Size");
        editor.TexturePropertySingleLine(Styles.specMask, SpecMaskMap);
    }

    void EmisGroup(Material mat)
    {
        editor.TexturePropertySingleLine(Styles.emisTex, EmisTex);
        editor.ColorProperty(EmisColor, "Emission Color");
        editor.FloatProperty(EmisPower, "Emisison Power");
    }
  
    void LightingGroup(Material mat)
    {
        editor.RangeProperty(UnlitIntensity, "Unlit Intensity");
        editor.RangeProperty(NormFlatten, "Flatten Light Direction");
    }
    
}
#endif