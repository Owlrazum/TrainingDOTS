using Unity.Mathematics;
using Unity.Assertions;

using UnityEngine;

public class CameraController : MonoBehaviour
{
    new Camera camera;

    void Awake()
    {
        bool found = TryGetComponent(out camera);
        Assert.IsTrue(found);
        CameraDelegatesContainer.GetScreenRay += GetScreenRay;
    }

    void OnDestroy()
    {
        CameraDelegatesContainer.GetScreenRay -= GetScreenRay;
    }

    Ray GetScreenRay(float2 screenPos)
    {
        return camera.ScreenPointToRay(new float3(screenPos, 0));
    }
}