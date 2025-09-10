using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class GameSetup : EditorWindow
{
    [MenuItem("Window/Game Setup")]
    [MenuItem("Assets/Game Setup")]
    [MenuItem("GameObject/Game Setup")]
    public static void ShowWindow()
    {
        GetWindow<GameSetup>("Game Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Setup Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Setup Player with Input System"))
        {
            SetupPlayer();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Input Actions Asset"))
        {
            CreateInputActionsAsset();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Setup Complete Game"))
        {
            SetupPlayer();
            CreateInputActionsAsset();
            Debug.Log("Game setup complete!");
        }
    }

    private void SetupPlayer()
    {
        // Player GameObject 찾기 또는 생성
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            player = new GameObject("Player");
            Debug.Log("Created Player GameObject");
        }

        // PlayerMover 컴포넌트 추가
        if (player.GetComponent<PlayerMover>() == null)
        {
            player.AddComponent<PlayerMover>();
            Debug.Log("Added PlayerMover component");
        }

        // Rigidbody2D 컴포넌트 추가
        if (player.GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // 중력 비활성화
            rb.freezeRotation = true; // 회전 고정
            Debug.Log("Added Rigidbody2D component");
        }

        // PlayerInput 컴포넌트 추가
        if (player.GetComponent<PlayerInput>() == null)
        {
            PlayerInput playerInput = player.AddComponent<PlayerInput>();
            
            // Input Actions Asset 찾기
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Scripts/Input/PlayerInputActions.inputactions");
            if (inputActions != null)
            {
                playerInput.actions = inputActions;
                playerInput.defaultActionMap = "Player";
                playerInput.notificationBehavior = PlayerNotifications.SendMessages;
                Debug.Log("Added PlayerInput component with Input Actions");
            }
            else
            {
                Debug.LogWarning("Input Actions Asset not found! Please create it first.");
            }
        }

        // 플레이어 위치 설정
        player.transform.position = new Vector3(0, -4, 0);

        // 선택된 오브젝트로 설정
        Selection.activeGameObject = player;
    }

    private void CreateInputActionsAsset()
    {
        // Input Actions Asset 생성
        var inputActions = ScriptableObject.CreateInstance<InputActionAsset>();
        inputActions.name = "PlayerInputActions";

        // Action Map 생성
        var actionMap = inputActions.AddActionMap("Player");

        // Move 액션 생성
        var moveAction = actionMap.AddAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        // Asset 저장
        AssetDatabase.CreateAsset(inputActions, "Assets/Scripts/Input/PlayerInputActions.inputactions");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Created Input Actions Asset!");
    }
}
