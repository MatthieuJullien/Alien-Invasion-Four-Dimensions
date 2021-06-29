using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour
{
    [SerializeField] private ObjectiveEnum label;
    [SerializeField] private Vector3 position;
    [SerializeField] private Text uitext;
    [SerializeField] private Image statutImage;
    [SerializeField] private Sprite lockSprite;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite completedSprite;
    [SerializeField] private Color lockedColor;
    [SerializeField] private Color readyColor;
    [SerializeField] private Color completedColor;

    public bool IsCompleted { get; private set; }
    public ObjectiveEnum Label { get => label; }
    public Vector3 Position { get => position; }

    //Audio
    private static AudioManager audioMan;
    private FMOD.Studio.EventInstance objectiveEvent;

    private void Awake()
    {
        IsCompleted = false;
        uitext.color = lockedColor;
        statutImage.sprite = lockSprite;
    }

    private void Start()
    {
        audioMan = AudioManager.Instance;
        objectiveEvent = audioMan.guiObjectives;
    }

    public void Unlock()
    {
        uitext.color = readyColor;
        uitext.fontStyle = FontStyle.Bold;
        statutImage.sprite = readySprite;
    }

    public void Complete()
    {
        audioMan.Play(objectiveEvent);
        IsCompleted = true;
        uitext.color = completedColor;
        uitext.fontStyle = FontStyle.Italic;
        statutImage.sprite = completedSprite;
    }
}
