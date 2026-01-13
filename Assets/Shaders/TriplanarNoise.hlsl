#ifndef TRIPLANAR_NOISE_INCLUDED
#define TRIPLANAR_NOISE_INCLUDED

#include "SimpleNoise.hlsl"

float hash21(float2 p)
{
    p = frac(p * float2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return frac(p.x * p.y);
}

float noise_2d(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);

    float a = hash21(i);
    float b = hash21(i + float2(1, 0));
    float c = hash21(i + float2(0, 1));
    float d = hash21(i + float2(1, 1));

    float2 u = f * f * (3.0 - 2.0 * f);

    return lerp(
        lerp(a, b, u.x),
        lerp(c, d, u.x),
        u.y
    );
}

void triplanar_noise_float(
    float3 world_pos,
    float scale,
    float time,
    out float Out
)
{
    float3 dir = normalize(world_pos);

    float3 abs_dir = abs(dir);
    abs_dir /= (abs_dir.x + abs_dir.y + abs_dir.z);

    float n_xy;
    float n_xz;
    float n_yz;

    unity_simple_noise_float(dir.xy + time, scale, n_xy);
    unity_simple_noise_float(dir.xz + time, scale, n_xz);
    unity_simple_noise_float(dir.yz + time, scale, n_yz);

    Out =
        n_xy * abs_dir.z +
        n_xz * abs_dir.y +
        n_yz * abs_dir.x;
}

#endif
