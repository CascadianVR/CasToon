Shader".Cascadian/CasToon"
{

    Properties
    {
        _MainTex ("Color Map", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (1,1,1,1) 
        [Normal] [NoScale] _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalStrength("Normal Strength", Range(0,2)) = 1

        _ShadowRamp("Shadow Ramp", 2D) = "white" {}        
        _ShadowColor("Shadow Color", Color) = (1,1,1,1)
        _ShadowOffset("Shadow Offset", Range(-5,5)) = 0
        _ShadMaskMap("Shadow Mask", 2D) = "white" {}

        _RimColor ("Rim Color", Color) = (1,1,1,1) 
        _RimSize ("Rim Size", Float) = 1.5
        _RimIntensity("Rim Intensity", Float) = 1
        _RimMaskMap("Rimlight Mask", 2D) = "white" {}

        _MatCap("Matcap Map", 2D) = "white" {}
        _MatMultiply("Mat Multiply", Range(0,1)) = 1
        _MatAdd("Mat Add", Range(0,1)) = 0
        _MatMaskMap("Matcap Mask", 2D) = "white" {}
    
        _Metallic("Metallic", Range(0,1)) = 1
        _RefSmoothness("Reflection Smoothness", Range(0,1)) = 0.5 
        _invertSmooth("invery smoothness", Float) = 0
        _SmoothnessMaskMap("Smoothness Mask", 2D) = "white" {}
        _MetalMaskMap("Metal Mask", 2D) = "white" {}
        _customcubemap("Use Custom Cubemap", Float) = 0
        _CustomReflection("Custom Cubemap", CUBE) = "white" {}
        _fallbackColor("Fallback Color", Color) = (1,1,1)

        _SpeccColor("Specular Color", Color) = (1,1,1,1)
        _SpecSmoothness("Smoothness", Range(0,2)) = 0.5
        _SpeccSize("Size", Range(0,1)) = 0.5
        _SpecMaskMap("Specular Mask", 2D) = "white" {}

        _EmisTex("Emission Map", 2D) = "white" {}
        _EmisColor("Emission Color", Color) = (1,1,1,1)
        _EmisPower("Emission Power", Float) = 1

        _UnlitIntensity("Unlit Intensity", Range(0,1)) = 0.1
        _NormFlatten("Normal Flatten", Range(0,1)) = 0.2

        _rimtog("toggle rimlight", Float) = 0
        _mattog("toggle matcap", Float) = 0
        _spectog("toggle specular", Float) = 0
        _metaltog("toggle metal", Float) = 0
        _emistog("toggle emissison", Float) = 0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque"
        "LightMode" = "ForwardBase"}
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog
            #pragma target 3.0

			#include  "castoon.cginc"
            
            ENDCG
        }

    }

    CustomEditor "CasToonGUI"
}
