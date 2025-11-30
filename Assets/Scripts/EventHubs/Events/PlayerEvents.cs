using System;
using UnityEngine;

public static class PlayerEvents
{
    public static event Action OnColorAbility;
    public static event Action<LevelColor> OnColorChanged;
    public static event Action OnPlayerRespawn;

    public static void TriggerColorAbility() => OnColorAbility?.Invoke();
    public static void TriggerColorChange(LevelColor color) => OnColorChanged?.Invoke(color);
    public static void TriggerPlayerRespawn() => OnPlayerRespawn?.Invoke();
}
