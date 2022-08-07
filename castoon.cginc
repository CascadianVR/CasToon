
            #include "UnityStandardBRDF.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "UnityStandardUtils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
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

            float4 _RimColor;
            float _RimSize;
            float _RimIntensity;
            sampler2D _RimMaskMap;

            sampler2D _ShadowRamp;
            float4 _ShadowColor;
            float _ShadowOffset;

            sampler2D _MatCap;
            float _MatMultiply;
            float _MatAdd;
            sampler2D _MatMaskMap;
            
            float _Metallic;
            float _RefSmoothness;
            float _invertSmooth;
            sampler2D _SmoothnessMaskMap;
            sampler2D _MetalMaskMap;
            
            float4 _SpeccColor;
            float _SpecSmoothness;
            float _SpeccSize;
            sampler2D _SpecMaskMap;
            float _customcubemap;
            samplerCUBE _CustomReflection;            

            sampler2D _EmisTex;
            float3 _EmisColor;
            float _EmisPower;

            float _UnlitIntensity;
            float _NormFlatten;
            float _LightColorCont;

            float _rimtog;
            float _mattog;
            float _spectog;
            float _metaltog;
            float _emistog;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); 
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

            
            fixed4 frag (v2f i) : SV_Target
            {
                float avgDirIntensity = (_LightColor0.r + _LightColor0.g + _LightColor0.b)/3.0;
                float3 L = _WorldSpaceLightPos0.xyz;
                if (avgDirIntensity < 0.05f)
                {
                    L = float3(-1,1,1);
                }
                
                float3 V = normalize(_WorldSpaceCameraPos - i.wPos);
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.wPos));
                fixed3 norm = UnpackNormal(tex2D(_NormalMap, i.uv));
                float3 worldNormal = float3(i.tbn[0] * norm.r * _NormalStrength + i.tbn[1] * norm.g * _NormalStrength + i.tbn[2] * norm.b * float3(1,1-_NormFlatten,1));
                float3 R = reflect(-L, worldNormal);
                float3 VRef = normalize(reflect(-worldViewDir, worldNormal));

                
                //Shadow
                float shade;
                
                shade = 0.5*dot(i.normal,L)+0.5;
                shade = clamp(0,1,tex2D(_ShadowRamp, shade.xx) + (1-_ShadowColor.w));

                // sample the texture
                float3 col = saturate(tex2D(_MainTex, i.uv).xyz * _MainColor);
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
                    if (_invertSmooth == 1) { _RefSmoothness = 1 - (_RefSmoothness * tex2D(_SmoothnessMaskMap, i.uv));}
                    else { _RefSmoothness = _RefSmoothness * tex2D(_SmoothnessMaskMap, i.uv);}
                    
                    half4 skyData;
                    if (_customcubemap == 1){skyData = texCUBElod(_CustomReflection, float4(VRef,_RefSmoothness * UNITY_SPECCUBE_LOD_STEPS * 2));}
                    else {skyData = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, VRef, _RefSmoothness * UNITY_SPECCUBE_LOD_STEPS);}
        
                    half3 skyColor = saturate(DecodeHDR(skyData, unity_SpecCube0_HDR));
                    mainOut = lerp(mainOut, mainOut * skyColor, _Metallic * tex2D(_MetalMaskMap, i.uv).xyz);
                } 
        
                //Specular
                if (_spectog == 1)
                {
                    float3 spec = saturate(dot(V, R) - _SpeccSize);
                    spec = smoothstep(0,_SpecSmoothness,spec);
                    spec *= tex2D(_SpecMaskMap, i.uv).xyz;
                    mainOut = lerp(mainOut + (spec * _SpeccColor.xyz * mainOut * 5), mainOut, 1 - _SpeccColor.w);
                }
                
                
                //Lighting Options 
                float3 finalOut = saturate(lerp( mainOut * ShadeSHPerPixel(worldNormal, _LightColor0.rgb, i.wPos), mainOut, _UnlitIntensity));
                
                //Emission
                if (_emistog == 1)
                {
                    finalOut = lerp(finalOut, mainOut * _EmisColor * _EmisPower, tex2D(_EmisTex, i.uv).xyz);
                }
                
                //ShadeSH9(float4(worldNormal,1))
                return float4(finalOut,1); 
            }