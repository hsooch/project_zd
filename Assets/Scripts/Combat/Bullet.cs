using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 1f; // 기본값 1
    [SerializeField] private int damage = 1; // 기본값 1  
    [SerializeField] private float size = 1f; // 기본값 1
    [SerializeField] private float rangeRatio = 1f; // 기본값 1

    private Vector3 _startPosition;
    private float _maxDistance;

    private void OnEnable() 
    { 
        _startPosition = transform.position;
        CalculateMaxDistance();
        ApplyBulletSize();
    }
    
    private void CalculateMaxDistance()
    {
        // 카메라의 세로 크기 계산 (Orthographic Size * 2)
        Camera cam = Camera.main;
        if (cam != null)
        {
            float screenHeight = cam.orthographicSize * 2f;
            // rangeRatio는 PlayerWeapon에서 이미 0.1~1.0으로 변환됨
            // 0.1 = 화면10%, 1.0 = 화면100%
            _maxDistance = screenHeight * rangeRatio;
        }
        else
        {
            _maxDistance = 10f * rangeRatio; // 기본값
        }
    }
    
    private void ApplyBulletSize()
    {
        // size 1일 때 기본 작은 크기 0.2, 값이 클수록 커짐
        float baseSize = 0.2f; // 기본 작은 크기
        float actualSize = baseSize * size; // 1 -> 0.2, 2 -> 0.4, 0.5 -> 0.1 등
        transform.localScale = Vector3.one * actualSize;
    }

    private void Update()
    {
        // speed 값이 1일 때 진짜 느린 돌멩이 속도
        float baseSpeed = 1.5f; // 매우 느린 기본 속도
        float actualSpeed = baseSpeed * speed; // 1 -> 1.5, 2 -> 3, 3 -> 4.5 등
        transform.Translate(Vector3.up * actualSpeed * Time.deltaTime);
        
        // 시작 위치에서 최대 거리만큼 이동했으면 사라짐
        float distanceTraveled = Vector3.Distance(_startPosition, transform.position);
        if (distanceTraveled >= _maxDistance)
        {
            gameObject.SetActive(false);
        }
        
        // 화면 밖으로 나가면 사라짐 (추가 안전장치)
        Camera cam = Camera.main;
        if (cam != null)
        {
            float screenHeight = cam.orthographicSize;
            float screenWidth = screenHeight * cam.aspect;
            
            if (transform.position.y > screenHeight || transform.position.y < -screenHeight || 
                transform.position.x > screenWidth || transform.position.x < -screenWidth)
            {
                gameObject.SetActive(false);
            }
        }
    }
    
    // 무기 업그레이드용 설정 메서드들
    public void SetBulletStats(float newSpeed, int newDamage, float newSize, float newRangeRatio)
    {
        speed = newSpeed;
        damage = newDamage;
        size = newSize;
        rangeRatio = newRangeRatio;
        
        CalculateMaxDistance();
        ApplyBulletSize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 적(Enemy)에만 데미지 적용, 바리케이드는 통과
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            var hp = enemy.GetComponent<Health>();
            if (hp != null)
            {
                hp.ApplyDamage(damage);
                gameObject.SetActive(false);
            }
        }
    }
}
