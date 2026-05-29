using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Переключает сцену GameScene -> GameScene2 через сцену Loading
/// при контакте игрока с этим игровым объектом (Trigger или Collision).
/// </summary>
public class SceneChanger : MonoBehaviour
{
    [Tooltip("Тег игрового объекта, который считается игроком.")]
    public string playerTag = "Player";

    [Tooltip("Если true — срабатывает только когда активная сцена == SceneNames.GameScene.")]
    public bool onlyWhenInGameScene = true;

    [Tooltip("Если true — объект деактивируется после первого срабатывания.")]
    public bool singleUse = true;

    private bool _triggered;

    private void OnTriggerEnter(Collider other)
    {
        TryTrigger(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryTrigger(collision.gameObject);
    }

    private void TryTrigger(GameObject other)
    {
        if (_triggered) return;
        if (other == null) return;
        if (!other.CompareTag(playerTag)) return;

        if (onlyWhenInGameScene && SceneManager.GetActiveScene().name != SceneNames.GameScene)
            return;

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.SwitchGameSceneToGameScene2();
        }
        else
        {
            Debug.LogWarning("SceneTriggerToGameScene2: SceneLoader.Instance == null. Убедитесь, что SceneLoader присутствует в сцене и инициализирован.");
        }

        _triggered = true;
        if (singleUse)
            gameObject.SetActive(false);
    }
}