using UnityEngine;

public class ChinaFestivalFallingItems : MonoBehaviour
{
    public float fallSpeed = 5f;
    private ChinaFestivalsManager.FestivalItem itemData;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetItem(ChinaFestivalsManager.FestivalItem item)
    {
        itemData = item;
        spriteRenderer.sprite = item.itemSprite;
    }
    public ChinaFestivalsManager.FestivalItem GetItem()
    {
        return itemData;
    }
    private void Update()
    {
        if (ChinaFestivalsManager.Instance.isGamePaused) return;

        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}
