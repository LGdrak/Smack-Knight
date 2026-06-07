using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    public string name;

    // a class for writing dialogue
    
    [TextArea(2, 10)]
    public string[] sentences;
}
