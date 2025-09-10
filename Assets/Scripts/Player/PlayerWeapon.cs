using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRateLevel = 1f; // 1=매우 느림, 10=매우 빠름
    
    [Header("Bullet Properties - 돌팔매 수준 (1~10 레벨)")]
    [SerializeField] private float bulletSpeed = 1f; // 1 = 기본 느린 속도
    [SerializeField] private int bulletDamage = 1; // 1 = 기본 약한 데미지
    [SerializeField] private float bulletSize = 1f; // 1 = 기본 작은 크기
    [SerializeField] private float bulletRangeLevel = 1f; // 1=화면10%, 10=화면100%

    private float _fireCooldown;
    private Keyboard _keyboard;

    private void Start()
    {
        _keyboard = Keyboard.current;
        
        // Muzzle이 없으면 자동 생성
        if (muzzle == null)
        {
            GameObject muzzleObj = new GameObject("Muzzle");
            muzzleObj.transform.SetParent(transform);
            muzzleObj.transform.localPosition = Vector3.up * 0.5f; // 플레이어 위쪽
            muzzle = muzzleObj.transform;
            Debug.Log("Muzzle created automatically at: " + muzzle.position);
        }
        else
        {
            Debug.Log("Muzzle already assigned: " + muzzle.name);
        }
    }

    private void Update()
    {
        _fireCooldown -= Time.deltaTime;

        // 자동 발사 (항상 발사)
        if (_fireCooldown <= 0f)
        {
            Fire();
            // fireRateLevel 1=0.5초/발, 10=5발/초
            float actualFireRate = fireRateLevel * 0.5f; // 1 -> 0.5발/초, 10 -> 5발/초
            _fireCooldown = 1f / actualFireRate;
        }
    }

    private void Fire()
    {
        if (bulletPrefab == null || muzzle == null) return;

        // Pool 시스템 사용 (기존 코드와 동일)
        var bulletObj = Pool.Get(bulletPrefab, muzzle.position, muzzle.rotation);
        
        // 총알 스탯 설정
        if (bulletObj.TryGetComponent<Bullet>(out var bullet))
        {
            // rangeLevel을 비율로 변환 (1=10%, 10=100%)
            float rangeRatio = bulletRangeLevel * 0.1f; // 1 -> 0.1, 10 -> 1.0
            bullet.SetBulletStats(bulletSpeed, bulletDamage, bulletSize, rangeRatio);
        }
        
        bulletObj.SetActive(true);
    }
    
    // 무기 업그레이드 메서드들
    public void UpgradeFireRate(float newFireRateLevel)
    {
        fireRateLevel = Mathf.Clamp(newFireRateLevel, 1f, 10f);
        Debug.Log($"Fire rate level upgraded to: {fireRateLevel} (shots per 2sec: {fireRateLevel})");
    }
    
    public void UpgradeBulletSpeed(float newSpeed)
    {
        bulletSpeed = newSpeed;
        Debug.Log($"Bullet speed upgraded to: {bulletSpeed}");
    }
    
    public void UpgradeBulletDamage(int newDamage)
    {
        bulletDamage = newDamage;
        Debug.Log($"Bullet damage upgraded to: {bulletDamage}");
    }
    
    public void UpgradeBulletSize(float newSize)
    {
        bulletSize = newSize;
        Debug.Log($"Bullet size upgraded to: {bulletSize}");
    }
    
    public void UpgradeBulletRange(float newRangeLevel)
    {
        bulletRangeLevel = Mathf.Clamp(newRangeLevel, 1f, 10f); // 1~10 레벨
        Debug.Log($"Bullet range level upgraded to: {bulletRangeLevel} ({bulletRangeLevel * 10}% of screen height)");
    }
    
    // 전체 무기 업그레이드
    public void UpgradeWeapon(float newFireRateLevel, float newSpeed, int newDamage, float newSize, float newRangeLevel)
    {
        fireRateLevel = Mathf.Clamp(newFireRateLevel, 1f, 10f);
        bulletSpeed = newSpeed;
        bulletDamage = newDamage;
        bulletSize = newSize;
        bulletRangeLevel = Mathf.Clamp(newRangeLevel, 1f, 10f);
        
        Debug.Log("Weapon fully upgraded!");
    }
}
