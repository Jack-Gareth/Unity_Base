using UnityEngine;
using System;

public class DeathCounter : Singleton<DeathCounter>
{
    private int deathCount = 0;
    
    public int DeathCount => deathCount;
    
    public event Action<int> OnDeathCountChanged;
    
    public void IncrementDeathCount()
    {
        deathCount++;
        OnDeathCountChanged?.Invoke(deathCount);
    }
    
    public void ResetDeathCount()
    {
        deathCount = 0;
        OnDeathCountChanged?.Invoke(deathCount);
    }
}
