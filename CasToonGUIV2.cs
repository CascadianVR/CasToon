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
    
    bool main = false;
    bool shadow = false;
    bool rimtog = false;
    bool mattog = false;
    bool spectog = false;
    bool metaltog = false;
    bool emistog = false;
    bool emistogscroll = false;
    bool lightingtog = false;
    bool audioLinktog = false;
    bool maindis = false;
    bool shadowdis = false;
    bool rimtogdis = false;
    bool mattogdis = false;
    bool spectogdis = false;
    bool metaltogdis = false;
    bool emistogdis = false;
    bool emistogscrolldis = false;
    bool lightingtogdis = false;
    bool audioLinktogdis = false;
    
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
    
    MaterialProperty Bass;
    MaterialProperty LowMid;
    MaterialProperty HighMid;
    MaterialProperty Treble;
    
    MaterialProperty rimtogprop = null;
    MaterialProperty mattogprop = null;
    MaterialProperty spectogprop = null;
    MaterialProperty metaltogprop = null;
    MaterialProperty emistogprop = null;
    MaterialProperty emistogscrollprop = null;
    MaterialProperty audiolinkprop = null;
    
    // Material Editor

    public void GetProperties(MaterialProperty[] props)
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
        
        Bass = FindProperty("_Bass", props, false);
        LowMid = FindProperty("_LowMid", props, false);
        HighMid = FindProperty("_HighMid", props, false);
        Treble = FindProperty("_Treble", props, false);
        
        rimtogprop = FindProperty("_rimtog", props, false);
        mattogprop = FindProperty("_mattog", props, false);
        spectogprop = FindProperty("_spectog", props, false);
        metaltogprop = FindProperty("_metaltog", props, false);
        emistogprop = FindProperty("_emistog", props, false);
        emistogscrollprop = FindProperty("_emistogscroll", props, false);
        audiolinkprop = FindProperty("_audioLinktog", props, false);
       
    }

    // Referenced https://github.com/unity3d-jp/UnityChanToonShaderVer2_Project for foldout function (modified)
    static bool Foldout(bool visible, string label)
    {
        var style = new GUIStyle("IN Title");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 22;
        style.contentOffset = new Vector2(1f, 4f);
        style.hover.textColor = new Color(0, 255, 229);

        var rect = GUILayoutUtility.GetRect(16f, 22f, style);
        GUI.Box(rect, label, style);

        var e = Event.current;

        var toggleRect = new Rect(rect.x + 4f, rect.y + 6f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, visible, false);
        }

        if (rect.Contains(e.mousePosition) && e.type == EventType.MouseDown)
        {
            visible = !visible;
            e.Use();
        }

        return visible;
    }

    override public void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material mat = materialEditor.target as Material;
        editor = materialEditor;
        EditorGUIUtility.fieldWidth = 0;
        GetProperties(properties);
        
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        
        GUILayout.BeginHorizontal();
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

        FoldoutToggle(ref rimtog,ref rimtogdis, rimtogprop, "Rimlight Settings");
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
        
        FoldoutToggle(ref mattog,ref mattogdis, mattogprop, "Matcap Settings");
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
        
        FoldoutToggle(ref metaltog,ref metaltogdis, metaltogprop, "Metallic/Smoothness Settings");
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
        
        FoldoutToggle(ref spectog,ref spectogdis, spectogprop, "Specular Settings");
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
        
        FoldoutToggle(ref emistog,ref emistogdis, emistogprop, "Emission Settings");
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
        editor.ColorProperty(RimColor, "Rim Color");
        editor.FloatProperty(RimSize, "Rim Size");
        editor.FloatProperty(RimIntensity, "Rim Intensity");
        editor.TexturePropertySingleLine(Styles.rimlightMaskLabel, RimMaskMap);
    }
    
    void MatcapGroup(Material mat)
    {
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
        
        FoldoutToggle(ref emistogscroll, ref emistogscrolldis, emistogscrollprop, "Emission Scroll", 30);
        EditorGUI.BeginDisabledGroup(!emistogscrolldis);
        if(emistogscroll)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            EditorGUILayout.PrefixLabel("Use agawg Cubemap");
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);
        
        FoldoutToggle(ref audioLinktog, ref audioLinktogdis, audiolinkprop, "AudioLink", 30);
        EditorGUI.BeginDisabledGroup(!audioLinktogdis);
        if(audioLinktog)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            editor.RangeProperty(Bass, "Bass");
            editor.RangeProperty(LowMid, "LowMid");
            editor.RangeProperty(HighMid, "HighMid");
            editor.RangeProperty(Treble, "Treble");
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);
        
    }

    void LightingGroup(Material mat)
    {
        editor.RangeProperty(UnlitIntensity, "Unlit Intensity");
        editor.RangeProperty(NormFlatten, "Flatten Light Direction");
    }
    
    private void FoldoutToggle(ref bool foldtog, ref bool dis, MaterialProperty prop, string title, int offset = 0)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        dis = Convert.ToBoolean(prop.floatValue);
        dis = GUILayout.Toggle(dis, "", GUILayout.Width(20));
        if (dis == true) { prop.floatValue = 1f; } else { prop.floatValue = 0f; }
        foldtog = Foldout(foldtog, title);
        GUILayout.EndHorizontal();
    }
    
}
#endif