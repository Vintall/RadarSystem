Shader "Custom/RadarImageShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("RenderTexture", 2D) = "" {}
        scutter_ray_angle("Scutter ray angle", Range(30, 150)) = 90
        scutter_ray_broadness_angle("Scutter ray broadness angle", Range(0, 30)) = 3
    }
        SubShader
        {
            //Tags { "RenderType"="Opaque" "RenderType" = "Transparent" }
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha


            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;
            float scutter_ray_angle;
            float scutter_ray_broadness_angle;

            int points_count;
            float2 points_array[500];

            struct Input
            {
                float2 uv_MainTex;
            };

            fixed4 _Color;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            float f1(float x) { float y = 1 - (x - 0.5) / sqrt(3); return y; }
            float f2(float x) { float y = 1 - (x - 0.5) / (-sqrt(3)); return y; }
            float2 f_angle(float angle) { return float2(0.5 + cos(angle / 57.2958) / 2, 1 - sin(angle / 57.2958) / 2); }
            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                float2 radar_center = float2(0.5, 1.);
                float circle_radius = length(IN.uv_MainTex - radar_center);
                float2 test_point = float2(0.3, 0.55);
                o.Albedo = float3(0, 0, 0);
                if (IN.uv_MainTex.y < f1(IN.uv_MainTex.x) && IN.uv_MainTex.y < f2(IN.uv_MainTex.x))
                {
                    if (circle_radius < 0.5)
                    {
                        o.Albedo = float3(0.1, 0.1, 0.1);
                        for (int i = 0; i < 500; i++)
                        {
                            if (i == points_count)
                                break;

                            float2 value = points_array[i];

                            if (length(IN.uv_MainTex - value) < 0.01)
                                o.Albedo = float3(0, length(IN.uv_MainTex - value) * 100, 0);
                        }
                        float2 ray_point1 = f_angle(scutter_ray_angle - scutter_ray_broadness_angle);
                        float2 ray_point2 = f_angle(scutter_ray_angle + scutter_ray_broadness_angle);
                        if (length(ray_point1 - IN.uv_MainTex) < 0.01)
                            o.Albedo = float3(length(ray_point1 - IN.uv_MainTex) * 100, 0, 0);
                        if (length(ray_point2 - IN.uv_MainTex) < 0.01)
                            o.Albedo = float3(length(ray_point2 - IN.uv_MainTex) * 100, 0, 0);
                        if (ray_point1.x + (IN.uv_MainTex.y - ray_point1.y) * (radar_center.x - ray_point1.x) / (radar_center.y - ray_point1.y) > IN.uv_MainTex.x &&
                            ray_point2.x + (IN.uv_MainTex.y - ray_point2.y) * (radar_center.x - ray_point2.x) / (radar_center.y - ray_point2.y) < IN.uv_MainTex.x)
                            o.Albedo = float3(0.1 * circle_radius, 0.3 * circle_radius, 0.1 * circle_radius);
                        if (circle_radius > 0.49)
                            o.Albedo = float3(0.7 * circle_radius, 1 * circle_radius, 1 * circle_radius);
                    }
                }
                else
                if (circle_radius < 0.5)
                {
                    if (abs(IN.uv_MainTex.y - f1(IN.uv_MainTex.x)) < 0.01)
                        o.Albedo = float3(0.7 * circle_radius, 1 * circle_radius, 1 * circle_radius);
                    if (abs(IN.uv_MainTex.y - f2(IN.uv_MainTex.x)) < 0.01)
                        o.Albedo = float3(0.7 * circle_radius, 1 * circle_radius, 1 * circle_radius);
                }
            }
            ENDCG
        }
            FallBack "Diffuse"
}
