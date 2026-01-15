#ifndef TRIPLANAR_NOISE_INCLUDED
#define TRIPLANAR_NOISE_INCLUDED

#include "SimpleNoise.hlsl"

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

    // Warp
    float w_xy, w_xz, w_yz;

    unity_simple_noise_float(dir.xy * 0.5 + time * 0.2, scale, w_xy);
    unity_simple_noise_float(dir.xz * 0.5 + time * 0.2, scale, w_xz);
    unity_simple_noise_float(dir.yz * 0.5 + time * 0.2, scale, w_yz);

    float2 warp_xy = (w_xy - 0.5) * 0.2;
    float2 warp_xz = (w_xz - 0.5) * 0.2;
    float2 warp_yz = (w_yz - 0.5) * 0.2;

    // Main noise
    float n_xy;
    float n_xz;
    float n_yz;

    unity_simple_noise_float(dir.xy + warp_xy + time, scale, n_xy);
    unity_simple_noise_float(dir.xz + warp_xz + time, scale, n_xz);
    unity_simple_noise_float(dir.yz + warp_yz + time, scale, n_yz);

    Out =
        n_xy * abs_dir.z +
        n_xz * abs_dir.y +
        n_yz * abs_dir.x;
}

#endif