using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHP = 10;
    public int CurrentHP { get; private set; }

    public event Action<int, int> OnDamaged; // curr, max
    public event Action OnDied;

    private void OnEnable() => CurrentHP = maxHP;

    public void ApplyDamage(int dmg)
    {
        if (CurrentHP <= 0) return;
        CurrentHP = Mathf.Max(0, CurrentHP - dmg);
        OnDamaged?.Invoke(CurrentHP, maxHP);
        if (CurrentHP == 0) OnDied?.Invoke();
    }
}
