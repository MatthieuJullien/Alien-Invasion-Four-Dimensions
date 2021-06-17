using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour
{
    [SerializeField] private Text uitext;
    [SerializeField] private Image statutImage;
    [SerializeField] private Sprite lockSprite;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite completedSprite;

    public bool IsCompleted { get; private set; }

    private void Awake()
    {
        IsCompleted = false;
        uitext.color = Color.black;
        statutImage.sprite = lockSprite;
    }

    public void Unlock()
    {
        uitext.color = Color.white;
        uitext.fontStyle = FontStyle.Bold;
        statutImage.sprite = readySprite;
    }

    public void Complete()
    {
        IsCompleted = true;
        uitext.color = Color.green;
        uitext.fontStyle = FontStyle.Italic;
        statutImage.sprite = completedSprite;
    }
}
