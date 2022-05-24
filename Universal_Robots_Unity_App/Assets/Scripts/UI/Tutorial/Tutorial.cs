using System;
using System.Collections;
using Treeka;
using UnityEngine;

public class Tutorial : PopupItem
{
    public static Tutorial Instance;
    public static int tutId = -1;
    public static event Action OnClick;

    public TMPro.TMP_Text text;
    public float speed = .1f;

    public static void Show(string text) => Instance.Display(text);
    public static void Close() => Instance.Disable();


    public void Click()
    {
        Disable();
        OnClick?.Invoke();
    }

    public override void Enable()
    {
        if(tutId == -1) TutorialLogic.Show(++tutId);
        else base.Enable();
    }

    public void Next()
    {
        if (++tutId >= 8) tutId = 0;
        TutorialLogic.Show(tutId);
    }

    public void Back()
    {
        if (--tutId <= -1) tutId = 7;
        TutorialLogic.Show(tutId);
    }

    public void Display(string text)
    {
        if (oldParent != null) Disable();
        this.text.text = text;
        StopAllCoroutines();
        StartCoroutine(AnimCo());
        Enable();
    }

    // ////////////////////////////////////////////////////// Private

    private IEnumerator AnimCo()
    {
        for (int i = 0; i < text.text.Length; i++)
        {
            text.maxVisibleCharacters = i;
            yield return new WaitForSeconds(speed);
        }
    }

    private void Start()
    {
        Instance = this;

        //if (ReadWrite.Exists(Application.persistentDataPath + "/robot.config")) return;
        //else StartTutorial();
    }

    protected override void AnimationIN()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, .1f);
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.LeanAlpha(1, .1f);
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    protected override void AnimationOUT()
    {
        LeanTween.scale(gameObject, Vector3.zero, .1f);
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.LeanAlpha(0, .1f);
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}