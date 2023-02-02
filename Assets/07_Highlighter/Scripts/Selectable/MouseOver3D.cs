
using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MouseOver3D : Selectable3D
{
    [Serializable]
    public class MouseOver3DEvent : UnityEvent
    {
        // 
    }

    [SerializeField]
    public MouseOver3DEvent onMouseEnter = new MouseOver3DEvent();
    [SerializeField]
    public MouseOver3DEvent onMouseOver = new MouseOver3DEvent();
    [SerializeField]
    public MouseOver3DEvent onMouseExit = new MouseOver3DEvent();

    public void OnMouseEnter()
    {
        if (IsPointerOverGameObject())
        {
            return;
        }

        if (onMouseEnter != null)
        {
            onMouseEnter.Invoke();
        }
    }
    private void OnMouseOver()
    {
        if (IsPointerOverGameObject())
        {
            return;
        }

        if (onMouseExit != null)
        {
            onMouseOver.Invoke();
        }
    }
    private void OnMouseExit()
    {
        if (IsPointerOverGameObject())
        {
            return;
        }

        if (onMouseExit != null)
        {
            onMouseExit.Invoke();
        }
    }
}
