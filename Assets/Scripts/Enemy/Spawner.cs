using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector2 spawnXRange = new Vector2(-3.2f, 3.2f);
    [SerializeField] private float baseInterval = 1.2f;
    [SerializeField] private float difficultyGrowth = 0.02f; // 시간에 따라 간격 감소

    private float _cd;
    private float _elapsed;

    private void Update()
    {
        if (GameManager.I == null || GameManager.I.State != GameState.Playing) return;

        _elapsed += Time.deltaTime;
        _cd -= Time.deltaTime;
        float interval = Mathf.Max(0.25f, baseInterval - _elapsed * difficultyGrowth);
        if (_cd <= 0f)
        {
            _cd = interval;
            float x = Random.Range(spawnXRange.x, spawnXRange.y);
            Vector3 pos = new Vector3(x, transform.position.y, 0f);
            var e = Pool.Get(enemyPrefab, pos, Quaternion.identity);
            e.SetActive(true);
        }
    }
}
