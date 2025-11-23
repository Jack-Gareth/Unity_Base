using UnityEngine;
using System.Collections;

public class PlayerScaler : MonoBehaviour
{
    [Header("Scaling")]
    [SerializeField] private Transform visualModel;
    [SerializeField][Range(0.05f, 2f)] private float resizeDuration = 0.25f;

    private Vector3 defaultScale;
    private Coroutine scaleRoutine;

    private void Awake()
    {
        if (visualModel == null)
            visualModel = transform;

        defaultScale = visualModel.localScale;
    }

    public void ResizeToScale(float scaleFactor)
    {
        Vector3 targetScale = defaultScale * scaleFactor;
        
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(SmoothScale(targetScale));
    }

    public void ResetToDefaultSize()
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(SmoothScale(defaultScale));
    }

    private IEnumerator SmoothScale(Vector3 target)
    {
        Vector3 start = visualModel.localScale;
        float t = 0f;

        while (t < resizeDuration)
        {
            t += Time.deltaTime;
            visualModel.localScale = Vector3.Lerp(start, target, t / resizeDuration);
            yield return null;
        }

        visualModel.localScale = target;
        scaleRoutine = null;
    }

    public void ForceResetSize()
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        visualModel.localScale = defaultScale;
    }
}
