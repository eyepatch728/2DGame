using Unity.VisualScripting;
using UnityEngine;

public class GOdestory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("player")) 
        
        {
            Invoke(nameof(destroyThis), 0.1f);
        
        }
    }

    public void destroyThis() 
    {
        Destroy(this.gameObject);


    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
