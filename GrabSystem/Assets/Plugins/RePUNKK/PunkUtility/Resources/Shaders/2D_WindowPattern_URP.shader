Shader "Custom/2D_WindowPattern_URP"
{
    Properties
    {
        [MainTexture] _MainTex ("Main Texture", 2D) = "white" {}
        [MainColor] _Color ("Color", Color) = (1,1,1,1)
        
        [Header(Background)]
        _BackgroundColor ("Background Color", Color) = (0.2, 0.3, 0.5, 1)
        
        [Header(Pattern Settings)]
        _PatternTexture ("Pattern Texture", 2D) = "white" {}
        _PatternColor ("Pattern Color", Color) = (1,1,1,1)
        _PatternScale ("Pattern Scale", Float) = 1.0
        _PatternRotation ("Pattern Rotation (Degrees)", Float) = 0.0
        _RandomRotation ("Random Rotation Amount", Range(0, 360)) = 0.0
        
        [Header(Animation)]
        _MovementDirection ("Movement Direction", Vector) = (1, 0, 0, 0)
        _MovementSpeed ("Movement Speed", Float) = 1.0
        
        [Header(Blending)]
        _PatternOpacity ("Pattern Opacity", Range(0, 1)) = 1.0
        
        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
        [Enum(Off,0,On,1)] _ZWrite ("Z Write", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("Z Test", Float) = 4
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 screenPos : TEXCOORD1;
                float4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            TEXTURE2D(_PatternTexture);
            SAMPLER(sampler_PatternTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _BackgroundColor;
                float4 _PatternTexture_ST;
                float4 _PatternColor;
                float _PatternScale;
                float _PatternRotation;
                float _RandomRotation;
                float4 _MovementDirection;
                float _MovementSpeed;
                float _PatternOpacity;
            CBUFFER_END

            // Random function for per-tile random rotation
            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            // 2D rotation matrix
            float2x2 rotate2D(float angle)
            {
                float c = cos(angle);
                float s = sin(angle);
                return float2x2(c, -s, s, c);
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                // Use world position instead of screen space for pattern that rotates with sprite
                float3 worldPos = vertexInput.positionWS;
                output.screenPos = worldPos.xy;
                
                output.color = input.color * _Color;
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Sample main texture for alpha masking
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // Use screen position for window-like effect
                float2 patternUV = input.screenPos;
                
                // Apply movement animation
                float2 movement = _MovementDirection.xy * _MovementSpeed * _Time.y;
                patternUV += movement;
                
                // Scale the pattern
                patternUV *= _PatternScale;
                
                // Calculate tile ID for random rotation
                float2 tileID = floor(patternUV);
                float2 localUV = frac(patternUV);
                
                // Center the UV coordinates for rotation
                localUV = localUV - 0.5;
                
                // Apply base rotation
                float baseRotation = radians(_PatternRotation);
                localUV = mul(rotate2D(baseRotation), localUV);
                
                // Apply random rotation per tile
                if (_RandomRotation > 0)
                {
                    float randomAngle = random(tileID) * radians(_RandomRotation);
                    localUV = mul(rotate2D(randomAngle), localUV);
                }
                
                // Return to 0-1 range
                localUV = localUV + 0.5;
                
                // Sample pattern texture
                half4 pattern = SAMPLE_TEXTURE2D(_PatternTexture, sampler_PatternTexture, localUV);
                pattern *= _PatternColor;
                
                // Blend background and pattern
                half4 finalColor = lerp(_BackgroundColor, pattern, pattern.a * _PatternOpacity);
                
                // Apply main texture alpha and vertex color
                finalColor.a *= mainTex.a * input.color.a;
                finalColor.rgb *= input.color.rgb;
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    FallBack "Sprites/Default"
}