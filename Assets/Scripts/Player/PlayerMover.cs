using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float minX = -2.5f, maxX = 2.5f; // 모바일 세로모드에 맞게 조정

    private Rigidbody2D _rb;
    private Camera _cam;
    private int _touchId = -1;
    private PlayerInput _playerInput;
    private Keyboard _keyboard;
    private Mouse _mouse;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
        _playerInput = GetComponent<PlayerInput>();
        _keyboard = Keyboard.current;
        _mouse = Mouse.current;
    }

    private void Update()
    {
        // if (GameManager.I == null || GameManager.I.State != GameState.Playing) return;

        float inputX = 0f;
        
        // 새로운 Input System - 키보드 입력
        if (_keyboard != null)
        {
            if (_keyboard.aKey.isPressed || _keyboard.leftArrowKey.isPressed)
                inputX = -1f;
            if (_keyboard.dKey.isPressed || _keyboard.rightArrowKey.isPressed)
                inputX = 1f;
        }
        
        // 새로운 Input System - 마우스 입력
        if (_mouse != null && _mouse.leftButton.isPressed)
        {
            Vector2 mousePos = _cam.ScreenToWorldPoint(_mouse.position.ReadValue());
            float targetX = Mathf.Clamp(mousePos.x, minX, maxX);
            float currentX = transform.position.x;
            float moveDirection = Mathf.Sign(targetX - currentX);
            
            float distance = Mathf.Abs(targetX - currentX);
            if (distance > 0.1f)
            {
                inputX = moveDirection * Mathf.Min(1f, distance * 3f);
            }
        }

        // 모바일 터치 컨트롤 - 화면 전체에서 좌우 드래그
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            Vector2 world = _cam.ScreenToWorldPoint(t.position);
            
            if (t.phase == UnityEngine.TouchPhase.Began || _touchId == -1) 
            {
                _touchId = t.fingerId;
            }
            
            if (t.fingerId == _touchId)
            {
                // 터치 위치로 직접 이동 (부드러운 이동)
                float targetX = Mathf.Clamp(world.x, minX, maxX);
                float currentX = transform.position.x;
                float moveDirection = Mathf.Sign(targetX - currentX);
                
                // 터치 위치와 현재 위치의 거리에 따라 이동 속도 조절
                float distance = Mathf.Abs(targetX - currentX);
                if (distance > 0.1f)
                {
                    inputX = moveDirection * Mathf.Min(1f, distance * 3f);
                }
                
                if (t.phase == UnityEngine.TouchPhase.Ended || t.phase == UnityEngine.TouchPhase.Canceled) 
                {
                    _touchId = -1;
                }
            }
        }

        Vector2 v = _rb.linearVelocity;
        v.x = inputX * moveSpeed;
        v.y = 0f;
        _rb.linearVelocity = v;

        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, minX, maxX);
        transform.position = p;
    }
}
