// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader".Cascadian/CasToonOpaque"
{

    Properties
    {
        _MainTex ("Color Map", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (1,1,1,1) 
        [Normal] [NoScale] _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalStrength("Normal Strength", Range(0,2)) = 1

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
    	
    	// Utilities
    	_HideMeshMap("Hide Mesh Map", 2D) = "white" {}
    	[Enum(OFF,0,FRONT,1,BACK,2)] _CullingMode("Culling Mode", int) = 2
    	
        _OrificeData("OrificeData", 2D) = "white" {}
		_EntryOpenDuration("Entry Trigger Duration", Range( 0 , 1)) = 0.1
		_Shape1Depth("Shape 1 Trigger Depth", Range( 0 , 5)) = 0.1
		_Shape1Duration("Shape 1 Trigger Duration", Range( 0 , 1)) = 0.1
		_Shape2Depth("Shape 2 Trigger Depth", Range( 0 , 5)) = 0.2
		_Shape2Duration("Shape 2 Trigger Duration", Range( 0 , 1)) = 0.1
		_Shape3Depth("Shape 3 Trigger Depth", Range( 0 , 5)) = 0.3
		_Shape3Duration("Shape 3 Trigger Duration", Range( 0 , 1)) = 0.1
		_BlendshapePower("Blend Shape Power", Range(0,5)) = 1
		_BlendshapeBadScaleFix("Blend Shape Bad Scale Fix", Range(1,100)) = 1
        
    	_rimtog("toggle rimlight", Float) = 0
        _mattog("toggle matcap", Float) = 0
        _spectog("toggle specular", Float) = 0
        _metaltog("toggle metal", Float) = 0
        _outlinetog("toggle outline", Float) = 0
        _emistog("toggle emissison", Float) = 0
        _emistogscroll("toggle emissison", Float) = 0
        _audioLinktog("toggle AudioLink", Float) = 0
        _orificetog("toggle Orifice", Float) = 0
    }
    SubShader
    {
    	
	    Pass // Main
        {
        	Tags { "Queue"="Geometry" "RenderType"="Opaque"
	        "LightMode" = "ForwardBase" "VRCFallback"="Toon"}
	        LOD 100
	        Cull [_CullingMode]
        	
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog
            #pragma target 3.0
			#pragma multi_compile _ LIGHTMAP_ON
            
            #define _IS_TRANSPARENT 0
 
			#include "castoon.cginc"
            
            ENDCG
        }

    	Pass //Outline
        {
        	Tags { "Queue"="Geometry" "RenderType"="Opaque"}
	        LOD 100
	        Cull FRONT
        	
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog
            #pragma target 3.0
			#pragma multi_compile _ LIGHTMAP_ON

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

			struct appdata {
			    float4 vertex : POSITION;
			    float3 normal : NORMAL;
			    float4 uv : TEXCOORD0;
			    UNITY_VERTEX_INPUT_INSTANCE_ID
			};
            
			struct v2f { 
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
			};

            sampler2D _OutlineMask;
			float4 _OutlineColor;
            float _outlineSize;
            float _outlinetog;
            
			v2f vert(appdata v)
			{
				v2f o;
				if (_outlinetog == 0)
				{
					o.pos = 0.0/0.0;
				}
				else
				{
					float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
					o.pos = UnityObjectToClipPos(float4(v.vertex.xyz + v.normal * 0.01 * _outlineSize * tex2Dlod(_OutlineMask, v.uv),1));
				}
				
				return o;
			}

            float4 frag(v2f i) : SV_Target
			{
				float3 bakedLight = saturate(ShadeSHPerPixel(lerp(float3(0.33,0.33,0.33),i.normal,0.5), _LightColor0, i.pos ));
			    bakedLight = ((bakedLight.x + bakedLight.y + bakedLight.z) / 3).xxx;

				float3 color = min(_OutlineColor.xyz, _OutlineColor.xyz * bakedLight);
				
				return float4(color,1);
			}
            
			ENDCG
        }
    	
	    Pass // Cast Shadows
		{
			Tags {"LightMode"="ShadowCaster"}
			Cull [_CullingMode]
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			struct v2f { 
				float4 pos : SV_POSITION;
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
    	
    	Pass // Realtime Lights Contribution
		{
			Tags {"LightMode" = "ForwardAdd"}
            // And it's additive to the base pass.
            Blend One One
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fwdadd
               
                #include "UnityCG.cginc"
                #include "AutoLight.cginc"
 
                float4 _MainTex_ST;
 
                struct v2f {
                    float4  pos         : SV_POSITION;
                    float2  uv          : TEXCOORD0;
                    float3  normal      : TEXCOORD1;
                    float3  lightDir    : TEXCOORD2;
                    LIGHTING_COORDS(3,4)
                };
 
                v2f vert (appdata_base v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos (v.vertex);
                    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex).xy;
                    o.normal = v.normal.xyz;
                    o.lightDir = ObjSpaceLightDir (v.vertex).xyz;
                    TRANSFER_VERTEX_TO_FRAGMENT(o);
                    return o;
                }
 
                sampler2D _MainTex;
                fixed4 _LightColor0;
 
                fixed4 frag(v2f i) : COLOR
                {
                    fixed atten = LIGHT_ATTENUATION(i);
                    fixed4 c;
                    c.rgb = saturate(dot(i.normal, i.lightDir)) * atten * 2 * _LightColor0.rgb * tex2D(_MainTex, i.uv).rgb;
                    c.a = 1.0;
                    return c;
                }
            ENDCG
		}

    }

    CustomEditor "CasToonGUIV2"
}
