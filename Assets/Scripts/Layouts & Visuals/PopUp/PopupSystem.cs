using UnityEngine;

public class PopupSystem : MonoBehaviour
{
    public static PopupSystem Instance { get; private set; }

    [SerializeField] private PopupPanel _popupPanel;
    [SerializeField] private PopupLibrary _popupLibrary;

    public PopupLibrary Library => _popupLibrary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Show(PageCollection pageCollection)
    {
        if (_popupPanel != null && pageCollection != null)
        {
            _popupPanel.StartDialogue(pageCollection);
        }
        else
        {
            Debug.LogWarning("PopupSystem: PopupPanel or PageCollection missing.");
        }
    }

    public bool IsPopupActive => _popupPanel != null && _popupPanel.IsPopupActive;
}
