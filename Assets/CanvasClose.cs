using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasClose : MonoBehaviour
{
    public Button Button_Close;
    public GameObject Canvas1;
    // Start is called before the first frame update
    void Start()
    {
        Button_Close.onClick.AddListener(btnClose);
    }

    private void btnClose()
    {
        Canvas1.SendMessage("Hide");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
