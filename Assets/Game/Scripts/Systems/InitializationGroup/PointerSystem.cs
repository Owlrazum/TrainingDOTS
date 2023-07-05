using Unity.Mathematics;
using Unity.Entities;

using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(InitializationSystemGroup))]
partial struct PointerSystem : ISystem, InputActions.IGameplayActions
{
    public void OnCreate(ref SystemState state)
    {
        state.EntityManager.CreateSingleton<PointerRay>();
        state.EntityManager.CreateSingleton<Click>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = CameraDelegatesContainer.GetScreenRay(mousePos);
        SystemAPI.SetSingleton<PointerRay>(new () {origin = ray.origin, direction = ray.direction});

        SystemAPI.SetSingleton<Click>(new() { state = Mouse.current.leftButton.wasReleasedThisFrame });
    }
}