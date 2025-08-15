Shader "Skybox/TripleBlendCubemapWithIndividualProperties"
{
    Properties
    {
        // ======== Cubemap 1 (Day) ========
        _Tint1 ("Tint Color 1 (Day)", Color) = (1,1,1,1)
        [Gamma] _Exposure1 ("Exposure 1 (Day)", Range(0,8)) = 1.0
        _Rotation1 ("Rotation 1 (Day)", Range(0,360)) = 0.0
        [NoScaleOffset] _Tex1 ("Cubemap 1 (Day)", CUBE) = "grey" {}

        // ======== Cubemap 2 (Evening) ========
        _Tint2 ("Tint Color 2 (Evening)", Color) = (1,1,1,1)
        [Gamma] _Exposure2 ("Exposure 2 (Evening)", Range(0,8)) = 1.0
        _Rotation2 ("Rotation 2 (Evening)", Range(0,360)) = 0.0
        [NoScaleOffset] _Tex2 ("Cubemap 2 (Evening)", CUBE) = "grey" {}

        // ======== Cubemap 3 (Night) ========
        _Tint3 ("Tint Color 3 (Night)", Color) = (1,1,1,1)
        [Gamma] _Exposure3 ("Exposure 3 (Night)", Range(0,8)) = 1.0
        _Rotation3 ("Rotation 3 (Night)", Range(0,360)) = 0.0
        [NoScaleOffset] _Tex3 ("Cubemap 3 (Night)", CUBE) = "grey" {}

        // ======== Global Blend Control ========
        _Blend ("Blend (0=Day, 0.5=Evening, 1=Night)", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Cubemap samplers
            samplerCUBE _Tex1;
            samplerCUBE _Tex2;
            samplerCUBE _Tex3;

            // Tint colors
            fixed4 _Tint1;
            fixed4 _Tint2;
            fixed4 _Tint3;

            // Exposures
            float _Exposure1;
            float _Exposure2;
            float _Exposure3;

            // Rotations
            float _Rotation1;
            float _Rotation2;
            float _Rotation3;

            // Blend control
            float _Blend;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            v2f vert (float3 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.texcoord = vertex;
                return o;
            }

            float3 RotateDir(float3 dir, float rotationDegrees)
            {
                float theta = radians(rotationDegrees);
                float cosTheta = cos(theta);
                float sinTheta = sin(theta);
                float3 rotated;
                rotated.x = dir.x * cosTheta - dir.z * sinTheta;
                rotated.y = dir.y;
                rotated.z = dir.x * sinTheta + dir.z * cosTheta;
                return rotated;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 dir = normalize(i.texcoord);

                // Apply individual rotations
                float3 dir1 = RotateDir(dir, _Rotation1);
                float3 dir2 = RotateDir(dir, _Rotation2);
                float3 dir3 = RotateDir(dir, _Rotation3);

                // Sample and apply tint/exposure
                fixed4 col1 = texCUBE(_Tex1, dir1) * _Tint1 * _Exposure1;
                fixed4 col2 = texCUBE(_Tex2, dir2) * _Tint2 * _Exposure2;
                fixed4 col3 = texCUBE(_Tex3, dir3) * _Tint3 * _Exposure3;

                // Blend logic
                float b = saturate(_Blend);
                float blend12 = saturate(b * 2.0);              // 0 → 1 between Day and Evening
                float blend23 = saturate((b - 0.5) * 2.0);      // 0 → 1 between Evening and Night

                fixed4 blend1 = lerp(col1, col2, blend12);      // Day → Evening
                fixed4 finalCol = lerp(blend1, col3, blend23);  // → Night

                finalCol.a = 1.0;
                return finalCol;
            }
            ENDCG
        }
    }
    Fallback Off
}
