using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FillColorOnTouch : MonoBehaviour
{
    public Collider2D[] ShapeMasks;
    public SpriteRenderer Base;
    bool[] filled;
    int fillCount = 0;
    SpriteRenderer[] circles;

    Color[] initialCircleColors; // We need to save those as they'll change later with other colors from the pallete

    // Start is called before the first frame update
    void Start()
    {
        filled = new bool[ShapeMasks.Length];
        circles = new SpriteRenderer[ShapeMasks.Length];
        for (int i = 0; i < ShapeMasks.Length; i++)
		{
            circles[i] = ShapeMasks[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
            filled[i] = false; // not needed but...
            circles[i].gameObject.SetActive(false);
		}

        initialCircleColors = circles.Select(x => x.color).ToArray();

        // For Debugging purpose
#if UNITY_EDITOR
        GameData.Instance.GameType = GameData.eGameType.Coloring;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        List<Touch> inputs = DragManager.Instance.GetNewInputs();
        foreach (var input in inputs)
		{
            Vector3 worldPos = DragManager.GetWorldSpacePos(input.position);
            worldPos.z = 0f;
            int foundIndex = -1;
            int foundSorting = -1000;
            for (int i = 0; i < ShapeMasks.Length; i++)
			{
                if (ShapeMasks[i].OverlapPoint(worldPos) && circles[i].sortingOrder > foundSorting)
				{
                    foundIndex = i;
                    foundSorting = circles[i].sortingOrder;
				}
			}
            if (foundIndex >= 0)
            {
                //SoundManager.Instance.PlaySFX("PaintFill");
                if (filled[foundIndex])
                {
                    SpriteRenderer sr = Instantiate(circles[foundIndex], circles[foundIndex].transform.parent);
                    if (GameManager2.Instance.ColorWheel.IsActive)
                        sr.color = GameManager2.Instance.ColorWheel.SelectedColor;
                    else
                        sr.color = initialCircleColors[foundIndex];
                    sr.transform.localScale = Vector3.zero;
                    sr.transform.position = worldPos;
                    sr.transform.DOScale(Vector3.one * 40.0f, 1.0f).SetEase(Ease.InSine).OnComplete(() => {
                        Destroy(circles[foundIndex].gameObject);
                        circles[foundIndex] = sr;
                    });

                    continue;
                }

                filled[foundIndex] = true;
                circles[foundIndex].gameObject.SetActive(true);
                if (GameManager2.Instance.ColorWheel.IsActive)
                    circles[foundIndex].color = GameManager2.Instance.ColorWheel.SelectedColor;
                circles[foundIndex].transform.localScale = Vector3.zero;
                circles[foundIndex].transform.position = worldPos;
                circles[foundIndex].transform.DOScale(Vector3.one * 40.0f, 1.0f).SetEase(Ease.InSine);//OnComplete(NotifyComplete);
            }
		}
    }

 //   void NotifyComplete()
	//{
 //       fillCount++;
 //       if (fillCount == ShapeMasks.Length)
	//	{
 //           Debug.Log("Done");
 //           GameManager.Instance.ShowNextLevel();
	//	}
 //   }

    public bool IsDone()
    {
        return fillCount >= ShapeMasks.Length;
    }
}
