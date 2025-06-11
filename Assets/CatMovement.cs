using UnityEngine;
using System.Collections;

public class CatMovement : MonoBehaviour
{
    public float delayBetweenPoints = 0.5f;
    public GameObject levelCompletePanel;

    public void MoveThroughPoints(Vector3[] positions)
    {
        StartCoroutine(MoveSequentially(positions));
    }
    private void Update()
    {
        //this.gameObject.GetComponent<Animator>().Play("CatIdle");
    }

    private IEnumerator MoveSequentially(Vector3[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {

            transform.position = positions[i];
            Debug.Log($"Cat moved to point {i + 1}: {positions[i]}");


            yield return new WaitForSeconds(delayBetweenPoints);
        }

      
        {
            //this.gameObject.GetComponent<Animator>().Play("CatIdle");
            Invoke("LevelCompletePanelOn", 2f);
            Debug.Log("Cat finished moving through all points.");
        }
    }

    private void LevelCompletePanelOn()
    {
        levelCompletePanel.SetActive(true);
    }
}
