using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickModel : MonoBehaviour
{
   // public GameObject MainTarget;
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
        Debug.Log("===================== 3D Cube 被点击 ==============");
       // MainTarget.SendMessage("Server4");
    }
}
