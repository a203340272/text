using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class Selectable3D : MonoBehaviour
{
    public static bool block;

    [SerializeField, SetProperty("interactable")]
    private bool _interactable;
    public bool interactable
    {
        get
        {
            return _interactable;
        }
        set
        {
            if (cachedCollider != null)
            {
                cachedCollider.enabled = value;
            }
            _interactable = value;
        }
    }
    public bool async;
    private Collider cachedCollider;

    protected virtual void Awake()
    {
        cachedCollider = GetComponent<Collider>();

        // Sync
        interactable = _interactable;
    }
    protected virtual bool IsPointerOverGameObject()
    {
        return false;
        //if (EventSystem.current.currentSelectedGameObject != null)
        //{
        //    Debug.Log(EventSystem.current.currentSelectedGameObject.tag);
        //}
        //return EventSystem.current.IsPointerOverGameObject();
    }
}
