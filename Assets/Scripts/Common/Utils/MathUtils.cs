using Unity.Mathematics;

namespace Common.Utils
{
    public static class MathUtils
    {
        public static float3 ClampMagnitude(float3 vector, float maxLength)
        {
            var lenSq = math.lengthsq(vector);
            if (lenSq > maxLength * maxLength)
            {
                vector *= maxLength * math.rsqrt(lenSq);
            }
            return vector;
        }
    }
}