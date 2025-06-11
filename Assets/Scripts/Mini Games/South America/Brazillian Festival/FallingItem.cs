using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public float fallSpeed = 5f;
    private BrazillianFestivalManager.FestivalItem itemData;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetItem(BrazillianFestivalManager.FestivalItem item)
    {
        itemData = item;
        spriteRenderer.sprite = item.itemSprite;
    }
    public BrazillianFestivalManager.FestivalItem GetItem()
    {
        return itemData;
    }
    private void Update()
    {
        if (BrazillianFestivalManager.Instance.isGamePaused) return;

        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}
