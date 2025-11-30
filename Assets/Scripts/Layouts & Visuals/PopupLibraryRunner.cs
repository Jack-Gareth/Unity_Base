using UnityEngine;

public class PopupLibraryRunner : MonoBehaviour
{
    public PopupLibrary popupLibrary;

    private void Start()
    {
        PopupSystem.Instance.Show(popupLibrary.testingPage);
    }
}
