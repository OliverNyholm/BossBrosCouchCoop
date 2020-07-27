using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyboardKey : MonoBehaviour
{
    private char myLetter = '@';
    private int myRowIndex = 0;
    private int myColumnIndex = 0;

    private void Awake()
    {
        myLetter = GetComponentInChildren<TextMeshProUGUI>().text[0];
    }

    public void SetColumnAndRow(int aColumn, int aRow)
    {
        myColumnIndex = aColumn;
        myRowIndex = aRow;
    }

    public char GetLetter()
    {
        return myLetter;
    }

    public int GetRowIndex()
    {
        return myRowIndex;
    }

    public int GetColumnIndex()
    {
        return myColumnIndex;
    }
}
