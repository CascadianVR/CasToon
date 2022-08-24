// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D


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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 wPos : TEXCOORD2;
                float3 tbn[3] : TEXCOORD3; //4&5
            };


            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _MainColor;
            sampler2D _NormalMap;
            float _NormalStrength;
            sampler2D _ShadMaskMap;
            float _Transparency;

            sampler2D _ShadowRamp;
            float4 _ShadowColor;
            float _ShadowOffset;

            float4 _RimColor;
            float _RimSize;
            float _RimIntensity;
            sampler2D _RimMaskMap;

            sampler2D _MatCap;
            float _MatMultiply;
            float _MatAdd;
            sampler2D _MatMaskMap;
            
            float _Metallic;
            float _RefSmoothness;
            float _invertSmooth;
            sampler2D _SmoothnessMaskMap;
            sampler2D _MetalMaskMap;
            float3 _fallbackColor;
            float _customcubemap;
            samplerCUBE _CustomReflection;    

            float4 _SpeccColor;
            float _SpecSmoothness;
            float _SpeccSize;
            sampler2D _SpecMaskMap;

            sampler2D _EmisTex;
            float3 _EmisColor;
            float _EmisPower;

            float _UnlitIntensity;
            float _NormFlatten;
            float _LightColorCont;

            sampler2D _HideMeshMap;

            float _Bass;
            float _LowMid;
            float _HighMid;
            float _Treble;
            float _minAudioBrightness;
            float _audioStrength;

            float _rimtog;
            float _mattog;
            float _spectog;
            float _metaltog;
            float _emistog;
            float _audioLinktog;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                if (tex2Dlod(_HideMeshMap, v.uv).x < 0.5)
                    o.vertex.w = 0.0 / 0.0;  
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); 
                o.normal = UnityObjectToWorldNormal( v.normal );
                float3 tangent = UnityObjectToWorldNormal(v.tangent);
                float3 bitangent = cross(tangent, o.normal);
                o.tbn[0] = tangent;
                o.tbn[1] = bitangent;
                o.tbn[2] = o.normal;
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float Remap( float value, float low1, float high1, float low2, float high2)
            {
                return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float avgDirIntensity = (_LightColor0.r + _LightColor0.g + _LightColor0.b)/3.0;
                float3 L = _WorldSpaceLightPos0.xyz;
                
                if (avgDirIntensity < 0.05f)
                {
                    L = normalize(float3(-1,1,1));
                }
                
                float3 V = normalize(_WorldSpaceCameraPos - i.wPos);
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.wPos));
                fixed3 norm = UnpackNormal(tex2D(_NormalMap, i.uv));
                float3 worldNormal = normalize(float3(i.tbn[0] * norm.r * _NormalStrength + i.tbn[1] * norm.g * _NormalStrength + i.tbn[2] * norm.b * float3(1,1-_NormFlatten,1)));
                float3 R = reflect(-L, worldNormal);
                float3 VRef = normalize(reflect(-worldViewDir, worldNormal));
                
                //Shadow
                float shade;
                shade = 0.5*dot(worldNormal,L)+0.5;
                shade = clamp(0,1,tex2D(_ShadowRamp, shade.xx) + (1-_ShadowColor.w));

                // sample the texture
                float4 maincol = tex2D(_MainTex, i.uv);
                float3 col = saturate(maincol.xyz * _MainColor);
                float3 mainOut = lerp(col * _ShadowColor.xyz, col, shade);
                mainOut = lerp(col, mainOut, tex2D(_ShadMaskMap, i.uv).xyz);
                
                
                //Rimlight
                if (_rimtog == 1)
                {
                    float3 rimLight = smoothstep(0.6, 1.0, saturate(1 - dot(worldNormal, V) * _RimSize) * _RimIntensity);
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
                    if (_invertSmooth == 0) { _RefSmoothness = 1 - (_RefSmoothness * tex2D(_SmoothnessMaskMap, i.uv));}
                    else { _RefSmoothness = _RefSmoothness * tex2D(_SmoothnessMaskMap, i.uv);}
                    
                    half4 skyData;
                    if (_customcubemap == 1){skyData = texCUBElod(_CustomReflection, float4(VRef,_RefSmoothness * UNITY_SPECCUBE_LOD_STEPS * 2));}
                    else {skyData = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, VRef, _RefSmoothness * UNITY_SPECCUBE_LOD_STEPS);}
        
                    half3 skyColor = saturate(DecodeHDR(skyData, unity_SpecCube0_HDR)) * _fallbackColor;
                    mainOut = lerp(mainOut, mainOut * skyColor, _Metallic * tex2D(_MetalMaskMap, i.uv).xyz);
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
                float3 temp = ShadeSHPerPixel(i.normal, _LightColor0, i.wPos );
                float temp2 = saturate((temp.x + temp.y + temp.z) / 3);
                float3 finalOut = saturate(lerp( mainOut * temp2, mainOut, _UnlitIntensity));
                
                //Emission
                if (_emistog == 1)
                {
                    if (_audioLinktog == 1)
                    {
                        float4 audioLink;
                        audioLink.x = AudioLinkLerp( ALPASS_AUDIOLINK + int2( 0 , i.uv.y) ).gggg;
                        audioLink.y = AudioLinkLerp( ALPASS_AUDIOLINK + int2( 0 , i.uv.y) + 1).gggg;
                        audioLink.z = AudioLinkLerp( ALPASS_AUDIOLINK + int2( 0 , i.uv.y) + 2).gggg;
                        audioLink.w = AudioLinkLerp( ALPASS_AUDIOLINK + int2( 0 , i.uv.y) + 3).gggg;

                        float emisStrength = 0;
                        emisStrength += lerp(0,audioLink.x, _Bass);
                        emisStrength += lerp(0,audioLink.y, _LowMid);
                        emisStrength += lerp(0,audioLink.z, _HighMid);
                        emisStrength += lerp(0,audioLink.w, _Treble);
                        
                        emisStrength = Remap(emisStrength, 0, 1, _minAudioBrightness, 1) * _audioStrength;
                        
                        finalOut = lerp(finalOut, mainOut * _EmisColor * _EmisPower * emisStrength, tex2D(_EmisTex, i.uv).xyz);
                    }
                    else
                    {
                        finalOut = lerp(finalOut, mainOut * _EmisColor * _EmisPower, tex2D(_EmisTex, i.uv).xyz);
                    }
                }
                
                #if _IS_TRANSPARENT
                    return float4(finalOut, maincol.w * _Transparency);
                #endif

                return float4(finalOut, 1);

            }
