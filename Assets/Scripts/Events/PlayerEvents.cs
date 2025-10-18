using System;
using UnityEngine;

public static class PlayerEvents
{
    public static event Action OnGravityFlip;
    public static event Action<LevelColor> OnColorChanged;
    public static event Action OnPlayerRespawn;

    public static void TriggerGravityFlip() => OnGravityFlip?.Invoke();
    public static void TriggerColorChange(LevelColor color) => OnColorChanged?.Invoke(color);
    public static void TriggerPlayerRespawn() => OnPlayerRespawn?.Invoke();
}
