using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class TextMeshInputHelper : MonoBehaviour
{

    public TMP_InputField tmp;
    public InputReg inputRegistery;

    private void OnEnable()
    {
        tmp.onSelect.AddListener(Select);
        tmp.onEndEdit.AddListener(DeSelect);
    }

    private void OnDisable()
    {
        tmp.onSelect.RemoveListener(Select);
        tmp.onEndEdit.RemoveListener(DeSelect);
    }

    private void Select(string arg0)
    {
        InputTerminal.DisableInput(inputRegistery);
    }

    private void DeSelect(string arg0)
    {
        InputTerminal.ReleaseInput(inputRegistery);
        EventSystem.current.SetSelectedGameObject(null);
    }

}
