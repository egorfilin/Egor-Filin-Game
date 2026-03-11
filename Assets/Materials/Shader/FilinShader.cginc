#ifndef UNITY_COMMON_SHADERS_INCLUDED
    #define UNITY_COMMON_SHADERS_INCLUDED

    #include "UnityCG.cginc"
    #include "AutoLight.cginc"
    #include "UnityLightingCommon.cginc"

    fixed4 _MainColor;
    fixed4 _ShadowColor;

    #if SF_MAIN_TEXTURE
        sampler2D _MainTex;
        float4 _MainTex_ST;
    #endif

    #if SF_DIFFUSE && SF_DIFFUSE_RAMP
        sampler1D _MainColorRampTex;
    #endif

    #if SF_RIM_LIGHT
        fixed4 _RimColor;
        fixed _RimLightPower;
    #endif
    
    #if SF_SPECULAR
        sampler2D _SpecTex;
        float _SpecShininess;
        float4 _SpecIntensity;
    #endif
    
    #if SF_REFLECTION
        uniform sampler2D _ReflectionTex;
        uniform samplerCUBE _ReflectionCube;
        uniform fixed4 _ReflectionColor;             
    #endif
    
    #if SF_EMISSION
        uniform sampler2D _EmissionMap;
        float _EmissionIntensity;
    #endif
    
    #ifdef SF_COLOR_LERP
        fixed3 _ColorToLerp;
        float _LerpValue;
    #endif
    
    #ifdef UNITY_UI_CLIP_RECT
        float4 _ClipRect;
    #endif
    
    #if SF_COLOR_FILL
        float _FillLerpStart;
        float _FillLerpEnd;
        float _PlaneRotationX;
        float4 _InactiveFillColor;
        float _FillProgress;
    #endif
    
    struct Input
    {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 lightDir : TEXCOORD1;
        float3 worldNormal : TEXCOORD2;
        float3 viewDir : TEXCOORD3;
        float3 worldPos : TEXCOORD4;
        
        #if SF_COLOR_FILL
            float3 localPos : TEXCOORD5;
        #endif
        
        #ifndef UNITY_UI_CLIP_RECT
            LIGHTING_COORDS(5,6)
            UNITY_FOG_COORDS(7)
        #endif
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    
    #if SF_UI_RENDERING
        float3 _CustomLightDir;
        float _CustomScale;
        float _Perspective;
        float _PerspectiveNearPlane;
        float3 _AnchorOffset;
        float4 _CustomLightColor;
        
        
        float4 ObjectToWorldPos(float3 pos)
        {
            float4x4 worldMatrix = unity_ObjectToWorld;
            worldMatrix._14_24_34 = 0.0f;

            float4 result = mul(worldMatrix, float4(pos + _AnchorOffset, 1.0));
            result.xy = lerp(result, result.xy / (result.z + _PerspectiveNearPlane), _Perspective);
            result.xyz *= _CustomScale * (1.0 + _Perspective);

            result.xyz += unity_ObjectToWorld._14_24_34;

            return result;
        }
    #else
        float4 ObjectToWorldPos(float3 pos)
        {
            return mul(unity_ObjectToWorld, float4(pos, 1.0));
        }
    #endif

    float4 WorldToClipPos(float4 pos)
    {
        return mul(UNITY_MATRIX_VP, pos);
    }
    
    Input DefaultVertexShader(appdata_full v)
    {
        UNITY_SETUP_INSTANCE_ID(v);

        Input o;
        UNITY_INITIALIZE_OUTPUT(Input, o);
        UNITY_TRANSFER_INSTANCE_ID(v, o);

        float4 worldPos = ObjectToWorldPos(v.vertex);

        #if SF_UI_RENDERING
            o.lightDir = -_CustomLightDir;
        #else
            o.lightDir = UnityWorldSpaceLightDir(worldPos);
        #endif

        
        #if SF_MAIN_TEXTURE
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
        #else
            o.uv = v.texcoord;
        #endif
    
        o.pos = WorldToClipPos(worldPos);
        o.worldPos = worldPos.xyz;
        o.worldNormal = UnityObjectToWorldNormal(v.normal);
        o.viewDir = UnityWorldSpaceViewDir(worldPos);
        
        #if SF_COLOR_FILL
            o.localPos = v.vertex;
        #endif
                                                                                                                                  
        #ifndef SF_UI_RENDERING
            TRANSFER_VERTEX_TO_FRAGMENT(o);
            UNITY_TRANSFER_FOG(o,o.pos);
        #endif
        
        return o;
    }
    
    half4 DefaultFragmentShader(Input IN) : COLOR
    {
        IN.lightDir = normalize(IN.lightDir);
        IN.worldNormal = normalize(IN.worldNormal);
        IN.viewDir = normalize(IN.viewDir);
    
        float NdotLUnclamped = dot(IN.worldNormal, IN.lightDir);
        #if SF_UI_RENDERING
            float atten = 1.0;
            float3 lightColor = _CustomLightColor.rgb * _CustomLightColor.a;
        #else
            float atten = LIGHT_ATTENUATION(IN);
            float3 lightColor =  _LightColor0.rgb;
        #endif
        float NdotL = saturate(NdotLUnclamped);
    
        float4 albedo = _MainColor;
        float3 color = 0.0;
    
        #if SF_MAIN_TEXTURE
            albedo *= tex2D(_MainTex, IN.uv);
        #endif
    
        #if SF_DIFFUSE
            float3 diffuse = albedo.rgb * lightColor;
            #if SF_DIFFUSE_RAMP
                float ramp = max(0.5 - NdotLUnclamped * 0.5, 0.5 - atten * 0.5);
                diffuse.rgb *=  tex1D(_MainColorRampTex, ramp).rgb;
            #else
                diffuse = lerp(_ShadowColor.rgb * diffuse, diffuse, NdotL * atten);
            #endif
            color.rgb += diffuse;
        #else
            color.rgb += albedo;
        #endif
    
        #if SF_RIM_LIGHT
            float3 rim = albedo.rgb * _RimColor.rgb * pow(1.0 - dot(IN.worldNormal, IN.viewDir), _RimLightPower);
            color.rgb += rim;
        #endif
    
        float4 resultColor = float4(color, albedo.a);
    
        #if SF_REFLECTION
            float3 reflectionColor = tex2D(_ReflectionTex, IN.uv);
            float3 worldReflection = reflect(-IN.viewDir, IN.worldNormal);
            resultColor.rgb += texCUBE(_ReflectionCube, worldReflection).rgb * reflectionColor;
        #endif
        
        #if SF_SPECULAR
            float3 specularReflection;
            if (NdotL < 0.0) 
            {
                specularReflection = 0.0;
            } 
            else 
            {
                specularReflection = tex2D(_SpecTex, IN.uv) * atten * _SpecIntensity *
                    pow(max(0.0, dot(reflect(-IN.lightDir, IN.worldNormal), IN.viewDir)),
                        (_SpecShininess * 199.0 + 1.0));
            }
            resultColor.rgb += specularReflection; 
        #endif
    
        #if SF_EMISSION
            resultColor.rgb += tex2D(_EmissionMap, IN.uv).rgb * _EmissionIntensity;
        #endif
    
        #ifdef SF_COLOR_LERP
            resultColor.rgb = lerp(resultColor.rgb, _ColorToLerp, _LerpValue);
        #endif
    
        #ifndef SF_UI_RENDERING
            UNITY_APPLY_FOG(IN.fogCoord, resultColor);
        #endif

        // resultColor.a *= _MainColor.a;
        /*
        #ifdef UNITY_UI_CLIP_RECT
            resultColor.a *= UnityGet2DClipping(IN.worldPos.xy, _ClipRect);
        #endif
        */
        #if SF_COLOR_FILL
            if (IN.localPos.z + IN.localPos.x * _PlaneRotationX < 
                lerp(_FillLerpStart, _FillLerpEnd, _FillProgress))
            {
                resultColor.rgb *= _InactiveFillColor.rgb;
            }
        #endif
        /*
        #ifdef UNITY_UI_ALPHACLIP
            clip (resultColor.a - 0.001);
        #endif
        */
        return resultColor;
    }
    
    uniform float4x4 _World2Receiver;
    uniform float4 _MatrixShadowColor;
    
    float4 ShadowMatrixVertexShader(float4 vertex : POSITION) : SV_POSITION
    {
       float4x4 modelMatrix = unity_ObjectToWorld;
    
        float4 lightDirection;
        if (_WorldSpaceLightPos0.w != 0.0) 
        {
           lightDirection = normalize(mul(modelMatrix, vertex - _WorldSpaceLightPos0));
        } 
        else 
        {
           lightDirection = -normalize(_WorldSpaceLightPos0); 
        }
    
       float4 vertexInWorldSpace = mul(modelMatrix, vertex);
       float4 world2ReceiverRow1 = _World2Receiver[1];
       float distanceOfVertex = dot(world2ReceiverRow1, vertexInWorldSpace); 
       float lengthOfLightDirectionInY = dot(world2ReceiverRow1, lightDirection); 
    
       if (distanceOfVertex > 0.0 && lengthOfLightDirectionInY < 0.0)
       {
          lightDirection = lightDirection * (distanceOfVertex / (-lengthOfLightDirectionInY));
       }
       else
       {
          lightDirection = float4(0.0, 0.0, 0.0, 0.0); 
       }
    
       return mul(UNITY_MATRIX_VP, vertexInWorldSpace + lightDirection);
    }
    
    float4 ShadowMatrixFragmentShader() : COLOR 
    {
       return _MatrixShadowColor;
    }
#endif