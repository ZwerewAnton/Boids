#ifndef BACK_FACE_FRESNEL_INCLUDED
#define BACK_FACE_FRESNEL_INCLUDED

void back_face_fresnel_float(float3 camera_pos, float3 world_pos, float3 normal, float power, out float Out)
{
    float3 inverse_normal = normal * -1;
    float t = pow(1 - dot(normalize(camera_pos - world_pos), inverse_normal), power);
    Out = t;
}

#endif