using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    public static readonly List<Enemy> All = new List<Enemy>();

    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float attackInterval = 0.5f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1f;

    private float _atkCd;
    private Health _hp;
    private Transform _player;
    private Barricade _barricade;
    private bool _isAttacking = false;
    private float _attackTimer = 0f;
    private Vector3 _attackStartPos;
    private Vector3 _attackTargetPos;

    private void OnEnable()
    {
        All.Add(this);
        _hp = GetComponent<Health>();
        _hp.OnDied += HandleDie;
        _player = FindFirstObjectByType<PlayerMover>()?.transform;
        // 바리케이드는 충돌 시 동적으로 찾기
    }

    private void OnDisable()
    {
        All.Remove(this);
        if (_hp != null) _hp.OnDied -= HandleDie;
    }

    private void Update()
    {
        // 테스트용: GameManager 체크 제거
        // if (GameManager.I == null || GameManager.I.State != GameState.Playing) return;
        
        if (_player == null) return;
        
        // 공격 중이면 공격 애니메이션 처리
        if (_isAttacking)
        {
            HandleAttackAnimation();
            return;
        }
        
        // 바리케이드 공격 중이면 제자리에서 공격
        if (_barricade != null)
        {
            // 바리케이드가 파괴되었는지 확인
            if (!_barricade.gameObject.activeInHierarchy)
            {
                _barricade = null;
                return;
            }
            
            // 바리케이드 공격
            if (_atkCd <= 0f)
            {
                _atkCd = attackInterval;
                _barricade.ApplyDamage(contactDamage);
            }
            
            _atkCd -= Time.deltaTime;
            return; // 바리케이드 공격 중에는 이동하지 않음
        }
        
        // 플레이어와의 거리 확인
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        
        // 공격 범위 내에 있고 공격 쿨다운이 끝났으면 공격 시작
        if (distanceToPlayer <= attackRange && _atkCd <= 0f)
        {
            StartAttack();
            return;
        }
        
        // 플레이어를 향해 이동
        Vector3 direction = (_player.position - transform.position).normalized;
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + movement;
        
        // 화면 밖으로 나가지 않도록 제한
        newPosition.x = Mathf.Clamp(newPosition.x, -3f, 3f);
        newPosition.y = Mathf.Clamp(newPosition.y, -5f, 5f);
        
        transform.position = newPosition;

        _atkCd -= Time.deltaTime;
    }

    private void StartAttack()
    {
        _isAttacking = true;
        _attackTimer = 0f;
        _attackStartPos = transform.position;
        
        // 플레이어 방향으로 살짝 뒤로 이동할 위치 계산
        Vector3 directionToPlayer = (_player.position - transform.position).normalized;
        _attackTargetPos = _attackStartPos - directionToPlayer * 0.8f; // 뒤로 0.8만큼 이동
        
        _atkCd = attackCooldown;
    }
    
    private void HandleAttackAnimation()
    {
        _attackTimer += Time.deltaTime;
        
        if (_attackTimer < 0.2f) // 뒤로 이동 (몸을 젖힘)
        {
            float progress = _attackTimer / 0.2f;
            transform.position = Vector3.Lerp(_attackStartPos, _attackTargetPos, progress);
        }
        else if (_attackTimer < 0.4f) // 앞으로 돌진 (몸통박치기)
        {
            float progress = (_attackTimer - 0.2f) / 0.2f;
            Vector3 chargeTarget = _player.position;
            transform.position = Vector3.Lerp(_attackTargetPos, chargeTarget, progress);
        }
        else // 공격 완료
        {
            _isAttacking = false;
            
            // 플레이어 즉사 처리
            TriggerGameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 플레이어와 닿으면 즉시 게임오버
        if (col.TryGetComponent<PlayerMover>(out var player))
        {
            TriggerGameOver();
        }
        // 바리케이드와 닿으면 공격 시작
        else if (col.TryGetComponent<Barricade>(out var barricade) && _barricade == null)
        {
            _barricade = barricade;
        }
    }
    
    private void TriggerGameOver()
    {
        Debug.Log("Game Over! Zombie touched the player!");
        
        // GameManager가 있으면 게임오버 호출
        if (GameManager.I != null)
        {
            // GameManager의 HandleGameOver 메서드 호출 (private이므로 다른 방식 사용)
            Time.timeScale = 0f; // 게임 일시정지
            Debug.Log("Game has been paused - Game Over!");
        }
        else
        {
            // GameManager가 없으면 게임 일시정지만
            Time.timeScale = 0f;
            Debug.Log("Game Over - Time stopped!");
        }
    }

    private void HandleDie()
    {
        Debug.Log("Zombie killed by bullet!");
        // 코인 드랍 등 훅 가능
        gameObject.SetActive(false);
    }
}
