#include "UnityStandardBRDF.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
#include "UnityStandardUtils.cginc"
#include "AudioLink.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float4 uv : TEXCOORD0;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    uint vertexId : SV_VertexID;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float3 normal : TEXCOORD1;
    float3 wPos : TEXCOORD2;
    float3 tbn[3] : TEXCOORD3; //4&5
    float3 oPos : TEXCOORD6;
};

// Main Options
sampler2D _MainTex;
float4 _MainTex_ST;
float3 _MainColor;
float _Transparency;
sampler2D _NormalMap;
float _NormalStrength;
sampler2D _DetailNormalMap;
float4 _DetailNormalMap_ST;
float _DetailNormalStrength;

// Shadows
sampler2D _ShadowRamp;
float4 _ShadowColor;
float _ShadowOffset;
sampler2D _ShadMaskMap;
float _ShadowMaskStrength;

// Rimlighting
float4 _RimColor;
float _RimSize;
float _RimIntensity;
sampler2D _RimMaskMap;

// Matcaps
sampler2D _MatCap;
float _MatMultiply;
float _MatAdd;
sampler2D _MatMaskMap;

// Metallics and Reflection
float _Metallic;
float _RefSmoothness;
float _invertSmooth;
sampler2D _SmoothnessMaskMap;
sampler2D _MetalMaskMap;
float _metallicSpecIntensity;
float _metallicSpecSize;
float3 _fallbackColor;
float _customcubemap;
samplerCUBE _CustomReflection;
float _MultiplyReflection;
float _AddReflection;

// Fake Specular
float4 _SpeccColor;
float _SpecSmoothness;
float _SpeccSize;
sampler2D _SpecMaskMap;

// Emission
sampler2D _EmisTex;
float3 _EmisColor;
float _EmisPower;
// Emission Scroll
float _EmisScrollSpeed;
float _EmisScrollFrequency;
float _EmisScrollMinBrightness;
float _EmisScrollMaxBrightness;
float _EmisScrollSpace;
// Audio Link
float _Bass;
float _LowMid;
float _HighMid;
float _Treble;
float _minAudioBrightness;
float _audioStrength;

// Lighting
float _UnlitIntensity;
float _NormFlatten;
float _BakedColorContribution;

// Utilities
sampler2D _HideMeshMap;

// UI folder disables
float _rimtog;
float _mattog;
float _spectog;
float _metaltog;
float _emistog;
float _emistogscroll;
float _audioLinktog;

v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.oPos = v.vertex;
    if (tex2Dlod(_HideMeshMap, v.uv).x < 0.5)
        o.vertex.w = 0.0 / 0.0;  
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.normal = UnityObjectToWorldNormal( v.normal );
    float4 tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
    float3 bitangent = cross(o.normal,tangent) * tangent.w ;
    o.tbn[0] = tangent; 
    o.tbn[1] = bitangent;
    o.tbn[2] = o.normal;
    TRANSFER_VERTEX_TO_FRAGMENT(o);
    o.wPos = mul(unity_ObjectToWorld, v.vertex);

    return o;
}

fixed3 DecodeLightProbe( fixed3 N ){
    return ShadeSH9(float4(N,1));
}

float remap( float value, float low1, float high1, float low2, float high2)
{
    return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
}

float remapf3( float3 value, float3 low1, float3 high1, float3 low2, float3 high2)
{
    return float3(
        low2.x + (value.x - low1.x) * (high2.x - low2.x) / (high1.x - low1.x),
        low2.y + (value.y - low1.y) * (high2.y - low2.y) / (high1.y - low1.y),
        low2.z + (value.z - low1.z) * (high2.z - low2.z) / (high1.z - low1.z)
    );
}

float generic_desaturate(float3 color, float factor)
{
    float3 lum = float3(0.299, 0.587, 0.114);
    float gray = dot(lum, color);
    return lerp(color, gray, factor);
}


fixed4 frag (v2f i) : SV_Target
{
    float avgDirIntensity = (_LightColor0.r + _LightColor0.g + _LightColor0.b)/3.0;
    float4 L = normalize(_WorldSpaceLightPos0);
    
    if (avgDirIntensity < 0.05f)
    {
        L.xyz = normalize(float3(-1,1,1));
    }

    
    L.xyz = lerp(L, normalize(float3(L.x,0,L.z)), _NormFlatten);
    
    float3 V = normalize(_WorldSpaceCameraPos - i.wPos);
    float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.wPos));
    
    float3 norm = UnpackScaleNormal(tex2D(_NormalMap, i.uv), _NormalStrength);
    float3 worldNormal = (i.tbn[0] * norm.r + i.tbn[1] * norm.g + i.tbn[2]* norm.b);

    float3 detailNorm = UnpackScaleNormal(tex2D(_DetailNormalMap, i.uv * _DetailNormalMap_ST.xy + _DetailNormalMap_ST.zw), _DetailNormalStrength);
    float3 detailWorldNormal = (i.tbn[0] * detailNorm.r + i.tbn[1] * detailNorm.g + i.tbn[2]* detailNorm.b);

    worldNormal = BlendNormals(worldNormal, detailWorldNormal);
    
    float3 R = reflect(-L, worldNormal);
    float3 VRef = normalize(reflect(-worldViewDir, worldNormal));

    float shade = clamp(0, 1, 0.5 * dot(worldNormal, L) + 0.5);
    shade = clamp(0,1,tex2D(_ShadowRamp, shade.xx) + (1-_ShadowColor.w));

    // sample the texture
    float4 maincol = tex2D(_MainTex, i.uv);
    float3 col = saturate(maincol.xyz * _MainColor);
    float3 mainOut = lerp(col * _ShadowColor.xyz, col, shade);
    mainOut = lerp(col, mainOut, tex2D(_ShadMaskMap, i.uv).xxx);
    
    //Rimlight
    if (_rimtog == 1)
    {
        float dotprod = dot(worldNormal, V);
        dotprod = saturate(abs(dotprod) * _RimSize);
        float3 rimLight = saturate(smoothstep(0.6, 1.0, 1- dotprod) * _RimIntensity);
        rimLight *= tex2D(_RimMaskMap, i.uv).xyz;
        rimLight = lerp(mainOut, _RimColor.xyz, rimLight);
        mainOut = lerp(mainOut, rimLight, _RimColor.w);
    }

    //Matcap
    if (_mattog == 1)
    {
        half3 upView = normalize(half3(0, 1, 0) - worldViewDir * dot(worldViewDir, half3(0, 1, 0)));
        half3 rightView = normalize(cross(worldViewDir, upView));
        float2 matuv = half2(dot(rightView, worldNormal), dot(upView, worldNormal)) * 0.45 + 0.5;
        float3 matcap = tex2D(_MatCap, matuv.xy);
        matcap *= tex2D(_MatMaskMap, i.uv).xyz; 
        mainOut = lerp(mainOut * matcap.xyz, mainOut, 1 - _MatMultiply);
        mainOut = lerp(mainOut + matcap.xyz, mainOut, 1 - _MatAdd);
     }

    
    //Metal
    if (_metaltog == 1)
    {
        float roughness = _RefSmoothness;
        float baseRoughness = roughness;
        
        if (_invertSmooth == 0)
        {
            roughness = 1 - (roughness * (1 - tex2D(_SmoothnessMaskMap, i.uv)));
            baseRoughness = 1 - baseRoughness;
        }
        else { roughness = roughness * tex2D(_SmoothnessMaskMap, i.uv);}
        
        float3 color0 = BoxProjectedCubemapDirection(R, i.vertex, unity_SpecCube0_ProbePosition, unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax);
        float3 color1 = BoxProjectedCubemapDirection(R, i.vertex, unity_SpecCube1_ProbePosition, unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax);

        float4 col0 = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, VRef, roughness * 10);
        float4 col1 = UNITY_SAMPLE_TEXCUBE_SAMPLER_LOD(unity_SpecCube1, unity_SpecCube0, VRef, roughness * 10);

        color0.rgb = DecodeHDR(col0, unity_SpecCube0_HDR);
        color1.rgb = DecodeHDR(col1, unity_SpecCube1_HDR);

        float3 reflection = lerp(color1, color0, unity_SpecCube0_BoxMin.w);
        
        float3 reflectionFallback;
        if (_customcubemap == 1)
        {
            half4 skyData = texCUBElod(_CustomReflection, float4(VRef,baseRoughness * 10));
            reflectionFallback = saturate(DecodeHDR(skyData, unity_SpecCube0_HDR)) * _fallbackColor;

            reflection = lerp(reflection, reflection * reflectionFallback, _MultiplyReflection);
            reflection = lerp(reflection, reflection + reflectionFallback, _AddReflection);
        }

        roughness = saturate(1-lerp (roughness,(1 - _RefSmoothness) * (1 - _RefSmoothness), 1));
        if (_invertSmooth == 0) roughness = 1 - roughness;
        
        float3 metallicSpec = max(0.01, GGXTerm(max(0, dot( worldNormal, normalize(worldViewDir + L ))), roughness * _metallicSpecSize))*col;

        mainOut = lerp(mainOut, mainOut + metallicSpec, _metallicSpecIntensity);
        mainOut = lerp(mainOut, mainOut * reflection, _Metallic * tex2D(_MetalMaskMap, i.uv).xyz);
    } 
    
    //Specular | http://www.conitec.net/shaders/shader_work3.htm (general approach)
    if (_spectog == 1)
    {
        float3 spec = saturate(dot(V, R) - _SpeccSize);
        spec = smoothstep(0,_SpecSmoothness,spec);
        spec *= tex2D(_SpecMaskMap, i.uv).xyz;
        mainOut = lerp(mainOut + (spec * _SpeccColor.xyz), mainOut, 1 - _SpeccColor.w);
    }
    
    //Lighting Options
    float3 bakedLight = saturate(ShadeSHPerPixel(L.xyz,_LightColor0, i.vertex.xyz ));
    bakedLight = lerp(generic_desaturate(bakedLight, 1), bakedLight, _BakedColorContribution);
    float3 finalOut = saturate(lerp( mainOut * bakedLight, mainOut, _UnlitIntensity));
    
    //Emission
    if (_emistog == 1)
    {
        float emisStrength = 0;
        if (_emistogscroll == 1)
        {
            if (_EmisScrollSpace == 1)
                emisStrength = (sin(_Time.y * _EmisScrollSpeed + i.wPos.y * _EmisScrollFrequency) + 1)/2;
            else
                emisStrength = (sin(_Time.y * _EmisScrollSpeed + i.oPos.y * _EmisScrollFrequency) + 1)/2;
            
            emisStrength = remap(emisStrength,0,1,_EmisScrollMinBrightness,_EmisScrollMaxBrightness);
        }

        if (_audioLinktog == 1)
        {
            float4 audioLink;
            audioLink.x = AudioLinkLerp( ALPASS_AUDIOLINK + int2( 0 , i.uv.y) ).yyyy;
            audioLink.y = AudioLinkLerp( ALPASS_AUDIOLINK + int2( 0 , i.uv.y) + 1).yyyy;
            audioLink.z = AudioLinkLerp( ALPASS_AUDIOLINK + int2( 0 , i.uv.y) + 2).yyyy;
            audioLink.w = AudioLinkLerp( ALPASS_AUDIOLINK + int2( 0 , i.uv.y) + 3).yyyy;
            
            emisStrength += lerp(0,audioLink.x, _Bass);
            emisStrength += lerp(0,audioLink.y, _LowMid);
            emisStrength += lerp(0,audioLink.z, _HighMid);
            emisStrength += lerp(0,audioLink.w, _Treble);
            
            emisStrength = remap(emisStrength, 0, 1, _minAudioBrightness, 1) * _audioStrength;
        }
        
        finalOut = lerp(finalOut, mainOut * _EmisColor * _EmisPower * emisStrength, tex2D(_EmisTex, i.uv).xyz);
    }
    
    #if _IS_TRANSPARENT
        return float4(finalOut, maincol.w * _Transparency);
    #endif

    
    return float4(finalOut, 1);

}

