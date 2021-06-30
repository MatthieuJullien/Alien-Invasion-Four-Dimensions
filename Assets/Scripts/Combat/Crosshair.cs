using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;

    private bool isViewFps = false;
    private GameManager gameManager;

    public bool HasWeapons { private get; set; }

    private void Start()
    {
        gameManager = GameManager.Instance;
        crosshairImage.enabled = false;
        HasWeapons = false;
    }

    private void Update()
    {
        if (gameManager.IsCurrentViewFPS != isViewFps)
        {
            isViewFps = gameManager.IsCurrentViewFPS;
            UpdateCrosshairVisibility();
        }
    }

    public void UpdateCrosshairVisibility()
    {
        crosshairImage.enabled = (isViewFps && HasWeapons);
    }
}
