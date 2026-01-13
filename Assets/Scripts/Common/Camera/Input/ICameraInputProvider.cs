using UnityEngine;

namespace Common.Camera.Input
{
    public interface ICameraInputProvider
    {
        Vector2 RotationDelta { get; }
        float ZoomDelta { get; }
        void UpdateInput();
    }
}