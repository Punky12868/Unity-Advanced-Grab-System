Shader "Custom/WorldOffsetShaderURP"
{
    Properties
    {
        _BaseColor("Main Color", Color) = (1, 1, 1, 1)
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Scale("Texture Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            // Include necessary URP shader libraries
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };

            sampler2D _MainTex;
            float4 _BaseColor;
            float _Scale;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 UV;
                fixed4 c;

                // Determine which face we are on
                if (abs(i.worldNormal.x) > 0.5)
                {
                    UV = i.worldPos.yz * _Scale; // Side
                }
                else if (abs(i.worldNormal.z) > 0.5)
                {
                    UV = i.worldPos.xy * _Scale; // Front
                }
                else
                {
                    UV = i.worldPos.xz * _Scale; // Top
                }

                // Sample the texture
                c = tex2D(_MainTex, UV);
                
                // Apply color
                c *= _BaseColor;

                // Add fog
                UNITY_APPLY_FOG(i.fogCoord, c);

                return c;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
