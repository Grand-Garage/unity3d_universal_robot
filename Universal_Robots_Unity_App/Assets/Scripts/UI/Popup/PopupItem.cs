using System;
using UnityEngine;


    [RequireComponent(typeof(CanvasGroup))]
    public class PopupItem : MonoBehaviour
    {
        public static event Action OnPopupChange;

        public bool escapable;

        protected Transform oldParent;
        public InputReg activeInputs = new InputReg();


        public virtual void Enable()
        {
            if (oldParent == Popup.popupParent) return;
            oldParent = transform.parent;
            transform.SetParent(Popup.popupParent);

            Cursor.lockState = activeInputs.curserMode;
            InputTerminal.DisableInput(activeInputs);

            OnPopupChange?.Invoke();
            AnimationIN();
        }

        public virtual void EscapePressed() 
        {
            if (escapable) Disable();
        }

        public virtual void Disable()
        {
            if (oldParent == null) return;
            transform.SetParent(oldParent);
            oldParent = null;

            InputTerminal.ReleaseInput(activeInputs);

            OnPopupChange?.Invoke();
            AnimationOUT();
        }

        public void Toggle()
        {
            if (oldParent == null) Enable();
            else Disable();
        }

        // /////////////////////

        protected virtual void AnimationIN()
        {
            transform.localScale = Vector3.one * 10;
            LeanTween.scale(gameObject, Vector3.one, .1f);
            CanvasGroup cg = GetComponent<CanvasGroup>();
            cg.LeanAlpha(1, .1f);
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        protected virtual void AnimationOUT()
        {
            LeanTween.scale(gameObject, Vector3.one * 10, .1f);
            CanvasGroup cg = GetComponent<CanvasGroup>();
            cg.LeanAlpha(0, .1f);
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }
