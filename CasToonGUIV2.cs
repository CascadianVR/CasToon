using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System;

public class CasToonGUIV2 : ShaderGUI
{
    private MaterialEditor editor;
    
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
        public static readonly GUIContent baseMapLabel = new GUIContent("Base Map");
        public static readonly GUIContent normalMapLabel = new GUIContent("Normal Map");
        public static readonly GUIContent shadowLabel = new GUIContent("Shadow Ramp");
        public static readonly GUIContent shadowMaskLabel = new GUIContent("Shadow Mask");
        public static readonly GUIContent rimlightMaskLabel = new GUIContent("Rimlight Mask");
        public static readonly GUIContent matcapTexLabel = new GUIContent("Matcap Map");
        public static readonly GUIContent matcapMaskLabel = new GUIContent("Matcap Mask");
        public static readonly GUIContent smoothnessMap = new GUIContent("Smoothness Map");
        public static readonly GUIContent metalMap = new GUIContent("Metallic Map");
        public static readonly GUIContent cubemap = new GUIContent("Forced Cubemap");
        public static readonly GUIContent specMask = new GUIContent("Specular Mask");
        public static readonly GUIContent emisTex = new GUIContent("Emisison Mask");
        public static readonly GUIContent meshHideMask = new GUIContent("Mesh Hide Mask");
        public static readonly GUIContent orificeData = new GUIContent("Orifice Data");
    }

    // Material Foldouts ______________________________________

    private bool main;
    private bool shadow;
    private bool rimtog;
    private bool mattog;
    private bool spectog;
    private bool metaltog;
    private bool emistog;
    private bool emistogscroll;
    private bool lightingtog;
    private bool audioLinktog;
    private bool utilitiestog;
    private bool orificetog;
    private bool rimtogdis;
    private bool mattogdis;
    private bool spectogdis;
    private bool metaltogdis;
    private bool emistogdis;
    private bool emistogscrolldis;
    private bool audioLinktogdis;
    private bool orificetogdis;
    
    // Material Properties ____________________________________

    private MaterialProperty MainTex;
    private MaterialProperty MainTex_ST;
    private MaterialProperty MainColor;
    private MaterialProperty NormalMap;
    private MaterialProperty NormalStrength;
    private MaterialProperty ShadMaskMap;
    private MaterialProperty Transparency;

    private MaterialProperty RimColor;
    private MaterialProperty RimSize;
    private MaterialProperty RimIntensity;
    private MaterialProperty RimMaskMap;

    private MaterialProperty ShadowRamp;
    private MaterialProperty ShadowColor;
    private MaterialProperty ShadowOffset;

    private MaterialProperty MatCap;
    private MaterialProperty MatMultiply;
    private MaterialProperty MatAdd;
    private MaterialProperty MatMaskMap;

    private MaterialProperty Metallic;
    private MaterialProperty RefSmoothness;
    private MaterialProperty invertSmooth;
    private MaterialProperty SmoothnessMaskMap;
    private MaterialProperty MetalMaskMap;
    private MaterialProperty fallbackColor;
    private MaterialProperty customcubemap;
    private MaterialProperty CustomReflection;
    private MaterialProperty MultiplyReflection;
    private MaterialProperty AddReflection;

    private MaterialProperty SpeccColor;
    private MaterialProperty SpecSmoothness;
    private MaterialProperty SpeccSize;
    private MaterialProperty SpecMaskMap;

    private MaterialProperty EmisTex;
    private MaterialProperty EmisColor;
    private MaterialProperty EmisPower;

    private MaterialProperty UnlitIntensity;
    private MaterialProperty NormFlatten;
    private MaterialProperty BakedColorContribution;

    private MaterialProperty Bass;
    private MaterialProperty LowMid;
    private MaterialProperty HighMid;
    private MaterialProperty Treble;
    private MaterialProperty AudioBrightness;
    private MaterialProperty AudioStrength;

    private MaterialProperty HideMeshMap;

    private MaterialProperty _OrificeData;
    private MaterialProperty _EntryOpenDuration;
    private MaterialProperty _Shape1Depth;
    private MaterialProperty _Shape1Duration;
    private MaterialProperty _Shape2Depth;
    private MaterialProperty _Shape2Duration;
    private MaterialProperty _Shape3Depth;
    private MaterialProperty _Shape3Duration;
    private MaterialProperty _BlendshapePower;
    private MaterialProperty _BlendshapeBadScaleFix;

    private MaterialProperty rimtogprop;
    private MaterialProperty mattogprop;
    private MaterialProperty spectogprop;
    private MaterialProperty metaltogprop;
    private MaterialProperty emistogprop;
    private MaterialProperty emistogscrollprop;
    private MaterialProperty audiolinkprop;
    private MaterialProperty orificeprop;
    
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
        CustomReflection = FindProperty("_CustomReflection", props, false);
        MultiplyReflection = FindProperty("_MultiplyReflection", props, false);
        AddReflection = FindProperty("_AddReflection", props, false);
        
        SpeccColor = FindProperty("_SpeccColor", props, false);
        SpecSmoothness = FindProperty("_SpecSmoothness", props, false);
        SpeccSize = FindProperty("_SpeccSize", props, false);
        SpecMaskMap = FindProperty("_SpecMaskMap", props, false);

        EmisTex = FindProperty("_EmisTex", props, false);
        EmisColor = FindProperty("_EmisColor", props, false);
        EmisPower = FindProperty("_EmisPower", props, false);;

        UnlitIntensity = FindProperty("_UnlitIntensity", props, false);
        NormFlatten = FindProperty("_NormFlatten", props, false);
        BakedColorContribution = FindProperty("_BakedColorContribution", props, false);
        
        Bass = FindProperty("_Bass", props, false);
        LowMid = FindProperty("_LowMid", props, false);
        HighMid = FindProperty("_HighMid", props, false);
        Treble = FindProperty("_Treble", props, false);
        AudioBrightness = FindProperty("_minAudioBrightness", props, false);
        AudioStrength = FindProperty("_audioStrength", props, false);
        
        HideMeshMap = FindProperty("_HideMeshMap", props, false);
        
        _OrificeData = FindProperty("_OrificeData", props, false);
        _EntryOpenDuration = FindProperty("_EntryOpenDuration", props, false);
        _Shape1Depth = FindProperty("_Shape1Depth", props, false);
        _Shape1Duration = FindProperty("_Shape1Duration", props, false);
        _Shape2Depth = FindProperty("_Shape2Depth", props, false);
        _Shape2Duration = FindProperty("_Shape2Duration", props, false);
        _Shape3Depth = FindProperty("_Shape3Depth", props, false);
        _Shape3Duration = FindProperty("_Shape3Duration", props, false);
        _BlendshapePower = FindProperty("_BlendshapePower", props, false);
        _BlendshapeBadScaleFix = FindProperty("_BlendshapeBadScaleFix", props, false);

        rimtogprop = FindProperty("_rimtog", props, false);
        mattogprop = FindProperty("_mattog", props, false);
        spectogprop = FindProperty("_spectog", props, false);
        metaltogprop = FindProperty("_metaltog", props, false);
        emistogprop = FindProperty("_emistog", props, false);
        emistogscrollprop = FindProperty("_emistogscroll", props, false);
        audiolinkprop = FindProperty("_audioLinktog", props, false);
        orificeprop = FindProperty("_orificetog", props, false);
       
    }

    // Referenced https://github.com/unity3d-jp/UnityChanToonShaderVer2_Project for foldout function (modified)
    private static bool Foldout(bool visible, string label)
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

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
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
        
        GUILayout.BeginHorizontal();
        utilitiestog = Foldout(utilitiestog, "Utilities");
        GUILayout.EndHorizontal();
        if(utilitiestog)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            UtilitiesGroup(mat);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space(10);
        
        if (EditorGUI.EndChangeCheck())
        {
            editor.PropertiesChanged();
        }
    }

    private void MainGroup(Material mat)
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

    private void ShadowGroup(Material mat)
    {
        GUILayout.Label("Shadow Settings", EditorStyles.boldLabel);
        editor.TexturePropertySingleLine(Styles.shadowLabel, ShadowRamp, ShadowColor);
        editor.RangeProperty(ShadowOffset, "Shadow Offset");
        editor.TexturePropertySingleLine(Styles.shadowMaskLabel, ShadMaskMap);
    }


    private void RimGroup(Material mat)
    {
        editor.ColorProperty(RimColor, "Rim Color");
        editor.FloatProperty(RimSize, "Rim Size");
        editor.FloatProperty(RimIntensity, "Rim Intensity");
        editor.TexturePropertySingleLine(Styles.rimlightMaskLabel, RimMaskMap);
    }

    private void MatcapGroup(Material mat)
    {
        editor.TexturePropertySingleLine(Styles.matcapTexLabel, MatCap);
        editor.RangeProperty(MatMultiply, "Mat Multiply");
        editor.RangeProperty(MatAdd, "Mat Add");
        editor.TexturePropertySingleLine(Styles.matcapMaskLabel, MatMaskMap);
    }

    private void MetalGroup(Material mat)
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
        EditorGUI.BeginDisabledGroup(!Convert.ToBoolean(mat.GetFloat("_customcubemap")));
        editor.ColorProperty(fallbackColor, "Fallback Color");
        editor.TexturePropertySingleLine(Styles.cubemap, CustomReflection);
        editor.ShaderProperty(MultiplyReflection, "Multiply Reflection");
        editor.ShaderProperty(AddReflection, "Add Reflection");
        EditorGUI.EndDisabledGroup();
        EditorGUI.EndDisabledGroup();
    }

    private void SpecGroup(Material mat)
    {
        editor.ColorProperty(SpeccColor, "Specular Color");
        editor.RangeProperty(SpecSmoothness, "Smoothness");
        editor.RangeProperty(SpeccSize, "Size");
        editor.TexturePropertySingleLine(Styles.specMask, SpecMaskMap);
    }

    private void EmisGroup(Material mat)
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
            editor.RangeProperty(AudioBrightness, "Minimum Brightness");
            editor.RangeProperty(AudioStrength, "Maximum Brightness");
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);
        
    }

    private void LightingGroup(Material mat)
    {
        editor.RangeProperty(UnlitIntensity, "Unlit Intensity");
        editor.RangeProperty(NormFlatten, "Flatten Light Direction");
        editor.ShaderProperty(BakedColorContribution, "Baked Light Color Contribution");
        editor.TexturePropertySingleLine(Styles.meshHideMask, HideMeshMap);
    }

    private void UtilitiesGroup(Material mat)
    {
        editor.TexturePropertySingleLine(Styles.meshHideMask, HideMeshMap);
        
        FoldoutToggle(ref orificetog, ref orificetogdis, orificeprop, "Orifice", 30);
        EditorGUI.BeginDisabledGroup(!orificetogdis);
        if(orificetog)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            editor.TexturePropertySingleLine(Styles.orificeData, _OrificeData);
            editor.RangeProperty(_EntryOpenDuration, "Open Duration");
            editor.RangeProperty(_Shape1Depth, "Shape 1 Depth");
            editor.RangeProperty(_Shape1Duration, "Shape 1 Duration");
            editor.RangeProperty(_Shape2Depth, "Shape 2 Depth");
            editor.RangeProperty(_Shape2Duration, "Shape 2 Duration");
            editor.RangeProperty(_Shape3Depth, "Shape 3 Depth");
            editor.RangeProperty(_Shape3Duration, "Shape 3 Duration");
            editor.RangeProperty(_BlendshapePower, "Blend Shape Power");
            editor.RangeProperty(_BlendshapeBadScaleFix, "Blendshape Bad Scale Fix");
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);
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