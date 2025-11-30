using UnityEngine;

public class ItemCollectionManager : MonoBehaviour
{
    public static ItemCollectionManager Instance { get; private set; }

    [SerializeField] private int totalItemsInLevel = 3;

    private int itemsCollected = 0;

    public int TotalItems => totalItemsInLevel;
    public int ItemsCollected => itemsCollected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        PlayerEvents.OnPlayerRespawn += ResetCollection;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerRespawn -= ResetCollection;
    }

    public void CollectItem()
    {
        itemsCollected = Mathf.Clamp(itemsCollected + 1, 0, totalItemsInLevel);
        // Notify global UI + systems through UIEvents
        UIEvents.RaiseDiamondCountUpdated(itemsCollected, totalItemsInLevel);
    }

    private void ResetCollection()
    {
        itemsCollected = 0;
        UIEvents.RaiseDiamondCountUpdated(itemsCollected, totalItemsInLevel);
    }
}
