
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickModelP1 : MonoBehaviour
{
    public GameObject MainTarget;
    // Start is called before the first frame update
    GameObject toolBoxPanel;
    void Awake()
    {

    }

    void Start()
    {

    }

    public void OnClick3D()
    {
        Debug.Log("===================== ÃÅ´°1 ±»µã»÷ ==============");
        MainTarget.SendMessage("Panel1");
    }
}
