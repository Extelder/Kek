using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorStartLocker : MonoBehaviour
{
    private void Start()
    {
        GameCursor.Instance.Hide();
    }
}