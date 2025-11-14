using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerTimeRewind : MonoBehaviour
{
    [Header("Rewind Settings")]
    [SerializeField] private float maxRewindDuration = 3f;
    [SerializeField] private float recordFrequency = 0.02f;

    private struct PositionRecord
    {
        public Vector3 position;
        public Quaternion rotation;
        public float timestamp;
    }

    private Queue<PositionRecord> positionHistory = new Queue<PositionRecord>();
    private bool isRewinding = false;
    private float recordTimer = 0f;
    private float rewindTimer = 0f;
    private float rewindStartTime;
    private Rigidbody2D rb;
    private List<PositionRecord> rewindBuffer = new List<PositionRecord>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        PlayerEvents.OnColorAbility += HandleAbilityInput;
        PlayerEvents.OnColorChanged += HandleColorChange;
    }

    private void OnDisable()
    {
        PlayerEvents.OnColorAbility -= HandleAbilityInput;
        PlayerEvents.OnColorChanged -= HandleColorChange;
    }

    private void Update()
    {
        if (LevelColorManager.Instance.CurrentColor != LevelColor.Brown)
            return;

        if (isRewinding)
        {
            HandleRewind();
        }
        else
        {
            RecordPosition();
        }
    }

    private void RecordPosition()
    {
        recordTimer += Time.deltaTime;

        if (recordTimer >= recordFrequency)
        {
            recordTimer = 0f;

            PositionRecord record = new PositionRecord
            {
                position = transform.position,
                rotation = transform.rotation,
                timestamp = Time.time
            };

            positionHistory.Enqueue(record);

            while (positionHistory.Count > 0 &&
                   Time.time - positionHistory.Peek().timestamp > maxRewindDuration)
            {
                positionHistory.Dequeue();
            }
        }
    }

    private void HandleAbilityInput()
    {
        if (LevelColorManager.Instance.CurrentColor != LevelColor.Brown)
            return;

        if (!isRewinding && positionHistory.Count > 0)
        {
            StartRewind();
        }
        else if (isRewinding)
        {
            StopRewind();
        }
    }

    private void StartRewind()
    {
        isRewinding = true;
        rewindStartTime = Time.time;
        rewindTimer = 0f;

        rewindBuffer.Clear();
        rewindBuffer.AddRange(positionHistory);

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
    }

    private void HandleRewind()
    {
        rewindTimer += Time.deltaTime;

        if (rewindTimer >= recordFrequency && rewindBuffer.Count > 0)
        {
            rewindTimer = 0f;

            PositionRecord record = rewindBuffer[rewindBuffer.Count - 1];
            transform.position = record.position;
            transform.rotation = record.rotation;
            rewindBuffer.RemoveAt(rewindBuffer.Count - 1);
        }

        if (Time.time - rewindStartTime >= maxRewindDuration || rewindBuffer.Count == 0)
        {
            StopRewind();
        }
    }

    private void StopRewind()
    {
        isRewinding = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rewindBuffer.Clear();
    }

    private void HandleColorChange(LevelColor newColor)
    {
        if (newColor != LevelColor.Brown)
        {
            if (isRewinding)
                StopRewind();

            positionHistory.Clear();
        }
    }
}
