Shader "Filin/FGB_Standart" 
{
 
    Properties
    {
        _MainColor("Main Color", Color) = (1.0,1.0,1.0,1.0)
        _ShadowColor("Shadow Color", Color) = (0.5,0.5,0.5,1.0)

        [Space]
        [Toggle(SF_MAIN_TEXTURE)] _SF_MAIN_TEXTURE("Main texture", Int) = 0
        _MainTex("Main Texture", 2D) = "white" {}

        [Space]
        [Toggle(SF_DIFFUSE)] _SF_DIFFUSE("Diffuse", Int) = 0
        [Toggle(SF_DIFFUSE_RAMP)] _SF_DIFFUSE_RAMP("Diffuse ramp", Int) = 0
        _MainColorRampTex("Diffuse Ramp Map", 2D) = "white" {}

        [Space]
        [Toggle(SF_RIM_LIGHT)] _SF_RIM_LIGHT("Rim Light", Int) = 0
        _RimColor("Rim Color", Color) = (1.0,1.0,1.0,1.0)
        _RimLightPower("Rim Light Power", Float) = 1.0

        [Space]
        [Toggle(SF_SPECULAR)] _SF_SPECULAR("Specular", Int) = 0
        _SpecTex("Specular Texture", 2D) =  "white" {} 
        _SpecShininess("Specular Shininess", Range(0, 1)) = 0.5
        _SpecIntensity("Specular Intensity", Color) = (1,1,1,1)

        [Space]
        [Toggle(SF_REFLECTION)] _SF_REFLECTION("Reflection", Int) = 0
        _ReflectionTex("Reflection Texture", 2D) =  "white" {} 
        _ReflectionCube("Reflection Cube", Cube) = "" {}

        [Space]
        [Toggle(SF_EMISSION)] _SF_EMISSION("Emission", Int) = 0
        _EmissionMap("Emission Map", 2D) = "black" {}
        _EmissionIntensity("Emission Intensity", Range(0, 1)) = 1

        [Space]
        [Toggle(SF_COLOR_LERP)] _SF_COLOR_LERP("Color Lerp", Int) = 0
        _ColorToLerp("Color To Lerp", Color) = (1,1,1,1)
        _LerpValue("Lerp Value", Range(0.0, 1.0)) = 0.0
    }
    SubShader {
 
        Tags { "RenderType" = "Opaque" }
 
        Pass {
 
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM


            #pragma multi_compile_fwdbase
            #pragma vertex DefaultVertexShader
            #pragma fragment DefaultFragmentShader
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            #pragma multi_compile __ SF_COLOR_LERP

            #pragma shader_feature SF_MAIN_TEXTURE
            #pragma shader_feature SF_DIFFUSE
            #pragma shader_feature SF_DIFFUSE_RAMP
            #pragma shader_feature SF_RIM_LIGHT
            #pragma shader_feature SF_SPECULAR
            #pragma shader_feature SF_REFLECTION
            #pragma shader_feature SF_EMISSION
 
            #include "Assets/Materials/Shader/FilinShader.cginc"
 
            ENDCG
 
        }
    }
 
 
 
    FallBack "Diffuse"
 
}