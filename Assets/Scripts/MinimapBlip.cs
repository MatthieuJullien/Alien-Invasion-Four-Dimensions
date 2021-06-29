using UnityEngine;

public class MinimapBlip : MonoBehaviour
{
    [SerializeField] private float shrinkSpeed = 1f;

    private float scaleFactor;
    private Vector3 startScale;
    private int grow;
    private bool on;

    private void Start()
    {
        startScale = transform.localScale;
        Reset();
    }

    private void Update()
    {
        if (!on) return;

        scaleFactor += Time.deltaTime * shrinkSpeed * grow;
        transform.localScale = startScale * scaleFactor;

        if (scaleFactor >= 1f)
        {
            grow = -1;
        }
        else if (scaleFactor <= 0.03f)
        {
            on = false;
            Reset();
        }
    }

    private void Reset()
    {
        on = true;
        scaleFactor = 0.5f;
        grow = 1;
    }
}
