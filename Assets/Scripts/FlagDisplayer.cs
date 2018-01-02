using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagDisplayer : MonoBehaviour
{

    public void showFlag(bool show)
    {
        GetComponent<SpriteRenderer>().enabled = show;
    }
}
