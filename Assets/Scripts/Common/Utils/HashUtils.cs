using Unity.Mathematics;

namespace Common.Utils
{
    public static class HashUtils
    {
        private const int P1 = 73856093;
        private const int P2 = 19349663;
        private const int P3 = 83492791;
        
        public static int HashCell(int3 cell)
        {
            return cell.x * P1 ^ cell.y * P2 ^ cell.z * P3;
        }
        
        public static int HashCellAbs(int3 cell)
        {
            return math.abs(HashCell(cell));
        }
    }
}