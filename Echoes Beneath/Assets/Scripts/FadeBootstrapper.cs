using UnityEngine;

public class FadeBootstrapper : MonoBehaviour
{
    [SerializeField] private GameObject fadeCanvasPrefab;

    private void Awake()
    {
        if (FadeController.Instance == null)
        {
            GameObject instance = Instantiate(fadeCanvasPrefab);
            DontDestroyOnLoad(instance);
        }
        Destroy(gameObject); // самоуничтожаемся, чтобы не оставаться в сцене
    }
}