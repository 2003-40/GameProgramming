using UnityEngine;

/// <summary>
/// Pulls nearby pickups toward the player after a short delay so ore drops feel responsive.
/// </summary>
public class ItemMagnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    [SerializeField] private float magnetRadius = 2.0f;
    [SerializeField] private float moveSpeed = 6.0f;
    [SerializeField] private float activateDelay = 0.4f;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private ItemPickup itemPickup;
    private bool isAttracted;
    private float timer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        itemPickup = GetComponent<ItemPickup>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        // The delay lets physics bounce finish before the magnet starts pulling the pickup.
        if (playerTransform == null)
        {
            return;
        }

        if (timer < activateDelay)
        {
            timer += Time.deltaTime;
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= magnetRadius)
        {
            isAttracted = true;
        }

        if (!isAttracted)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            playerTransform.position,
            moveSpeed * Time.deltaTime);

        if (distance < 0.2f)
        {
            TryCollectItem();
        }
    }

    private void TryCollectItem()
    {
        if (itemPickup == null)
        {
            Destroy(gameObject);
            return;
        }

        itemPickup.TryCollect();
    }
}
