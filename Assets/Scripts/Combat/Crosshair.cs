using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;

    private bool isViewFps = false;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (gameManager.IsCurrentViewFPS != isViewFps)
        {
            isViewFps = gameManager.IsCurrentViewFPS;
            crosshairImage.enabled = (isViewFps);
        }
    }
}
