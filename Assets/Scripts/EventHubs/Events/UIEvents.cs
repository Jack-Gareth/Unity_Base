using System;

public static class UIEvents
{
    public static event Action<int, int> OnDiamondCountUpdated;
    public static event Action<int> OnDeathUpdated;

    public static void RaiseDiamondCountUpdated(int current, int total) => OnDiamondCountUpdated?.Invoke(current, total);
    public static void RaiseDeathUpdated(int newCount) => OnDeathUpdated?.Invoke(newCount);
    
}
