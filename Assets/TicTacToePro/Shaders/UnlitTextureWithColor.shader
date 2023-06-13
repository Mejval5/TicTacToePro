Shader "Unlit/TextureWithColor" {
    Properties{
        _MainTex("Base (RGB)", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    }

        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100
            Cull Off Lighting Off

            Pass {
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma target 2.0
                    #pragma multi_compile_fog

                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float2 texcoord : TEXCOORD0;
                        fixed4 color : COLOR;
                        UNITY_VERTEX_INPUT_INSTANCE_ID
                    };

                    struct v2f {
                        float4 vertex : SV_POSITION;
                        float2 texcoord : TEXCOORD0;
                        fixed4 color : COLOR;
                        UNITY_FOG_COORDS(1)
                        UNITY_VERTEX_OUTPUT_STEREO
                    };

                    sampler2D _MainTex;
                    float4 _MainTex_ST;
                    fixed4 _TintColor;

                    v2f vert(appdata_t v)
                    {
                        v2f o;
                        UNITY_SETUP_INSTANCE_ID(v);
                        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        o.color = v.color;
                        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                        return o;
                    }

                    fixed4 frag(v2f i) : SV_Target
                    {
                        fixed4 col = tex2D(_MainTex, i.texcoord) * _TintColor * i.color;
                        UNITY_OPAQUE_ALPHA(col.a);
                        return col;
                    }
                ENDCG
            }
    }

}