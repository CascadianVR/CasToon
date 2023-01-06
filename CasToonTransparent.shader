Shader".Cascadian/CasToonTransparent"
{
    
    Properties
    {
        _MainTex ("Color Map", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (1,1,1,1) 
        [Normal] [NoScale] _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalStrength("Normal Strength", Range(0,2)) = 1
        _Transparency("Transparency", Range(0,1)) = 1

        _ShadowRamp("Shadow Ramp", 2D) = "white" {}        
        _ShadowColor("Shadow Color", Color) = (0.5,0.5,0.5,1)
        _ShadowOffset("Shadow Offset", Range(-1,1)) = 0
        _ShadMaskMap("Shadow Mask", 2D) = "white" {}
	    _ShadowMaskStrength("Shadow Mask Strength", Range(0,1)) = 0

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
        _invertSmooth("invert smoothness", Float) = 0
        _metallicSpecIntensity("Metallic Specular Size", Range(0,1)) = 1
        _metallicSpecSize("Metallic Specular Intensity", Range(0,1)) = 1
        _SmoothnessMaskMap("Smoothness Mask", 2D) = "white" {}
        _MetalMaskMap("Metal Mask", 2D) = "white" {}
        _customcubemap("Use Custom Cubemap", Float) = 0
    	_MultiplyReflection("Multiply Reflection", Range(0,1)) = 0
        _AddReflection("Add Reflection", Range(0,1)) = 0
        _CustomReflection("Custom Cubemap", CUBE) = "white" {}
        _fallbackColor("Fallback Color", Color) = (1,1,1)

        _SpeccColor("Specular Color", Color) = (1,1,1,1)
        _SpecSmoothness("Smoothness", Range(0,5)) = 0.5
        _SpeccSize("Size", Range(0,1)) = 0.5
        _SpecMaskMap("Specular Mask", 2D) = "white" {}
    	
    	_OutlineColor("Outline Color", Color) = (0,0,0,1)
    	_outlineSize("Outline Size", float) = 1
	    _OutlineMask("Outline Mask", 2D) = "white" {}

        _EmisTex("Emission Map", 2D) = "white" {}
        _EmisColor("Emission Color", Color) = (1,1,1,1)
        _EmisPower("Emission Power", Float) = 1

        _UnlitIntensity("Unlit Intensity", Range(0,1)) = 0.1
        _NormFlatten("Normal Flatten", Range(0,1)) = 0.5
    	_BakedColorContribution("Baked Color Contribution", Range(0,1)) = 1.0
    	
    	_AudioLink ("AudioLink Texture", 2D) = "black" {}
    	_Bass ("Bass", Range(0,1)) = 0
    	_LowMid ("LowMid", Range(0,1)) = 0
    	_HighMid ("HighMid", Range(0,1)) = 0
    	_Treble ("Treble", Range(0,1)) = 0
    	_minAudioBrightness ("Minimum Brightness", Range(0,1)) = 0.5
    	_audioStrength ("Audio Strength", Range(0,5)) = 1
        
    	_HideMeshMap("Hide Mesh Map", 2D) = "white" {}
    	[Enum(OFF,0,FRONT,1,BACK,2)] _CullingMode("Culling Mode", int) = 2
    	
    	_rimtog("toggle rimlight", Float) = 0
        _mattog("toggle matcap", Float) = 0
        _spectog("toggle specular", Float) = 0
        _metaltog("toggle metal", Float) = 0
        _outlinetog("toggle outline", Float) = 0
        _emistog("toggle emissison", Float) = 0
        _emistogscroll("toggle emissison", Float) = 0
        _audioLinktog("toggle AudioLink", Float) = 0
    }
    SubShader
    {
	    Tags { "Queue"="Transparent" "RenderType"="Transparent"
        "LightMode" = "ForwardBase" "VRCFallback" = "ToonFade"}
    	
    	ZWrite On
    	ZTest LEqual
    	
	    BlendOp Add, Max
        Blend SrcAlpha OneMinusSrcAlpha, One One
    	
    	
        LOD 100 
        Cull [_CullingMode]
        
	    Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog
            #pragma target 3.0

            #define _IS_TRANSPARENT 1

			#include  "CasToon.cginc"
            
            ENDCG
        }
        
        // shadow caster rendering pass, implemented manually
		// using macros from UnityCG.cginc
		Pass
		{
			Tags {"LightMode"="ShadowCaster"}
			Cull Back
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			struct v2f { 
				V2F_SHADOW_CASTER;
			};

			sampler2D _HideMeshMap;

			v2f vert(appdata_base v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				o.pos = v.vertex;
				if (tex2Dlod(_HideMeshMap, v.texcoord).x < 0.5)
				{
					o.pos = 0.0 / 0.0;
					return o;
				}

				o.pos = UnityClipSpaceShadowCasterPos(v.vertex, v.normal);
				o.pos = UnityApplyLinearShadowBias(o.pos);
				
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				return 0;
			}
			ENDCG
		}
    	
    	Pass
		{
			Tags {"LightMode" = "ForwardAdd"}
            // And it's additive to the base pass.
            Blend OneMinusDstColor One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
           
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
			#include "Lighting.cginc"
			#include "UnityStandardUtils.cginc"

            float4 _MainTex_ST;

            struct v2f {
                float4  pos         : SV_POSITION;
                float2  uv          : TEXCOORD0;
                float3  normal      : TEXCOORD1;
                float3  lightDir    : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            	float3 tbn[3] : TEXCOORD4; //5&6
            };

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = TRANSFORM_TEX (v.texcoord, _MainTex).xy;
                o.normal = v.normal.xyz;
            	float4 tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
				float3 bitangent = cross(o.normal,tangent) * tangent.w ;
				o.tbn[0] = tangent; 
				o.tbn[1] = bitangent;
				o.tbn[2] = o.normal;
                o.lightDir = ObjSpaceLightDir (v.vertex).xyz;
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            sampler2D _MainTex;
            sampler2D _NormalMap;
            sampler2D _ShadowRamp;
            float _NormalStrength;
            float4 _ShadowColor;

            fixed4 frag(v2f i) : COLOR
            {
			    float4 L = normalize(_WorldSpaceLightPos0);
                fixed atten = LIGHT_ATTENUATION(i);
                float3 norm = UnpackScaleNormal(tex2D(_NormalMap, i.uv), _NormalStrength);
				float3 worldNormal = (i.tbn[0] * norm.r + i.tbn[1] * norm.g + i.tbn[2]* norm.b);
            	float shade = clamp(0, 1, 0.5 * dot(worldNormal, L) + 0.5);
				shade = clamp(0,1,tex2D(_ShadowRamp, shade.xx) + (1-_ShadowColor.w));
                float3 color = shade * atten * 2 * _LightColor0.rgb * tex2D(_MainTex, i.uv).rgb;
                return float4(color,1);
            }
            ENDCG
		}
    }

    CustomEditor "CasToonGUIV2" 
}
