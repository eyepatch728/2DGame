using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleController : MonoBehaviour, IDropZone
{
    public Collider2D[] PuzzlePieces;
    public GameObject Target;
    public string SoundName = "";
    public bool StartsWithAVowel = false;
    float SnapDistance = 2.0f;
    Vector3[] originalPositions;
    int piecesInPlace = 0;

    // Start is called before the first frame update
    void Start()
    {
        originalPositions = new Vector3[PuzzlePieces.Length];

        for (int i = 0; i < PuzzlePieces.Length; i++)
        {
            originalPositions[i] = PuzzlePieces[i].transform.position;
            GameManager2.Instance.ScrollPanel.AddItem(PuzzlePieces[i].GetComponent<SpriteRenderer>());
		}
		GameManager2.Instance.ScrollPanel.UpdateItemsPosition();

        if (SoundName != "")
		{
            //SoundManager.Instance.PlaySFX("Build" + SoundName + "Puzzle");
		}
    }
  
    private void NotifyOnTarget(Transform transform)
    {
        transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
        //SoundManager.Instance.PlaySFX("PuzzlePlace");

        if (piecesInPlace == PuzzlePieces.Length)
        {
            //SoundManager.Instance.PlaySFX("PuzzleDone");
            Target.SetActive(false);
            Sequence s = DOTween.Sequence().AppendInterval(0.6f);
            for (int i = 0; i < PuzzlePieces.Length; i++)
			{
                OutlineController outlineController = PuzzlePieces[i].GetComponent<OutlineController>();
                if (i == 0)
                    s.Append(DOTween.ToAlpha(() => outlineController.OffsetColor, (x) => outlineController.SetOffsetColor(x), 0.3f, 1.0f).SetEase(Ease.Flash, 4.0f, 0.0f));
                else
                    s.Join(DOTween.ToAlpha(() => outlineController.OffsetColor, (x) => outlineController.SetOffsetColor(x), 0.3f, 1.0f).SetEase(Ease.Flash, 4.0f, 0.0f));
            }
            //s.AppendInterval(0.5f);
            s.AppendInterval(1.5f); // For Sound
            //s.AppendCallback(() => GameManager2.Instance.NextLevel());

			// Sound
			string sfxName = StartsWithAVowel ? "YouBuiltAn" : "YouBuildA";
			//float sfxDuration = SoundManager.Instance.GetClip(sfxName).length;
            DOTween.Sequence()
                //.AppendCallback(() => SoundManager.Instance.PlaySFX(sfxName))
                .AppendInterval(1);
				//.AppendCallback(() => SoundManager.Instance.PlaySFX(SoundName));
		}
    }

	public bool CanDrop(Collider2D collider)
	{
		Collider2D puzzleCollider = PuzzlePieces.FirstOrDefault(x => x == collider);
		if (puzzleCollider != null)
		{
			int index = System.Array.IndexOf(PuzzlePieces, puzzleCollider);
			if (puzzleCollider.transform.position.Distance(originalPositions[index]) < SnapDistance)
			{
				OutlineController outlineController = puzzleCollider.GetComponent<OutlineController>();
				DOTween.Sequence()
					.Append(puzzleCollider.transform.DOMove(originalPositions[index], 0.3f))
					.AppendCallback(() => NotifyOnTarget(puzzleCollider.transform))
					.Append(DOTween.ToAlpha(() => outlineController.OffsetColor, (x) => outlineController.SetOffsetColor(x), 0.3f, 0.5f).SetEase(Ease.Flash, 2.0f, 0.0f));

				puzzleCollider.enabled = false;
				piecesInPlace++;
                return true;
			}
		}
        return false;
	}

	public Color? GetPanelColor()
	{
        return null;
	}
}
