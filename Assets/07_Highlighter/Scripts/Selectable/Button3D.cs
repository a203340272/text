using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Button3D : Selectable3D
{
    private const string hoverClipName = "hover";
    private const string pressDownClipName = "pressDown";
    private const string pressUpClipName = "pressUp";

    [Serializable]
    public class Button3DClickedEvent : UnityEvent
    {
        // 
    }

    [SerializeField]
    public Animation cachedAnimation;

    [SerializeField]
    private bool enableHoverAnimation;
    [SerializeField]
    private bool enablePressDownAnimation;
    [SerializeField]
    private bool enablePressUpAnimation;

    [SerializeField]
    public Button3DClickedEvent onClick = new Button3DClickedEvent();

    private bool entered;

    protected override void Awake()
    {
        base.Awake();

        entered = false;
    }
    private void OnMouseEnter()
    {
        if (IsPointerOverGameObject())
        {
            return;
        }

        if (interactable)
        {
            if (enableHoverAnimation && cachedAnimation != null)
            {
                cachedAnimation.CrossFade(hoverClipName);
            }

            entered = true;
        }
    }
    private void OnMouseDown()
    {
        if (block)
        {
            return;
        }

        if (IsPointerOverGameObject())
        {
            return;
        }

        if (interactable)
        {
            if (enablePressDownAnimation && cachedAnimation != null)
            {
                cachedAnimation.CrossFade(pressDownClipName);
            }
        }
    }
    private void OnMouseUp()
    {
        if (block)
        {
            return;
        }

        if (IsPointerOverGameObject())
        {
            return;
        }

        if (entered && interactable)
        {
            onClick.Invoke();

            if (!async)
            {
                OnButtonClick();
            }
        }
    }
    private void OnMouseExit()
    {
        if (IsPointerOverGameObject())
        {
            return;
        }

        if (interactable)
        {
            entered = false;
        }
    }
    private void OnButtonClick()
    {
        if (enablePressUpAnimation && cachedAnimation != null)
        {
            cachedAnimation.CrossFade(pressUpClipName);
        }
    }
}
