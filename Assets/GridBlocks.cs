using UnityEngine;

public class GridBlocks : MonoBehaviour
{
    public int row = 0;
    public int col = 0;
    public string boxID = "";
    public bool isProhibitted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxID = row.ToString() + col.ToString();
        AustraliaKoalaGameManager.Instance.GridBlocks.Add(this);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}



}
