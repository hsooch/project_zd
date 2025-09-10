using UnityEngine;
using System;

[RequireComponent(typeof(Health))]
public class Barricade : MonoBehaviour
{
    public event Action OnBarricadeDestroyed;
    private Health _hp;

    private void Awake()
    {
        _hp = GetComponent<Health>();
        _hp.OnDied += HandleDestroyed;
    }
    
    private void HandleDestroyed()
    {
        Debug.Log("Barricade destroyed!");
        OnBarricadeDestroyed?.Invoke();
        gameObject.SetActive(false); // 바리케이드 파괴시 비활성화
    }

    public void ApplyDamage(int dmg) => _hp.ApplyDamage(dmg);
    public int CurrentHP => _hp.CurrentHP;
}
