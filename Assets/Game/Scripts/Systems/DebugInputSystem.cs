using Unity.Entities;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

partial class DebugInputSystem : SystemBase, InputActions.IGameplayActions
{
    InputActions inputActions;

    protected override void OnStartRunning() => inputActions.Enable();
    protected override void OnStopRunning() => inputActions.Disable();

    protected override void OnCreate()
    {
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnUpdate()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Debug.Log("Press " + Mouse.current.position);
        }
    }
}