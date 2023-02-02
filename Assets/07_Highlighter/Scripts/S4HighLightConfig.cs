
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

[RequireComponent(typeof(Button3D))]
[RequireComponent(typeof(FlashingHighlighter))]
[RequireComponent(typeof(MouseOver3D))]
[RequireComponent(typeof(Highlighter))]
[RequireComponent(typeof(ClickModelS4))]
public class S4HighLightConfig : MonoBehaviour
{
    Button3D button3D;
    ClickModelS4 clickModel;
    MouseOver3D mouseOver3D;
    FlashingHighlighter flashingHighlighter;
    Highlighter highlighter;
    void Start()
    {
        button3D = gameObject.GetComponent<Button3D>();
        clickModel = gameObject.GetComponent<ClickModelS4>();
        mouseOver3D = gameObject.GetComponent<MouseOver3D>();
        flashingHighlighter = gameObject.GetComponent<FlashingHighlighter>();
        highlighter = gameObject.GetComponent<Highlighter>();
        Debug.Log("========�õ��������壺" + gameObject.name + ", button3D:" + button3D + ", clickModel:" + clickModel);
        if (button3D && clickModel)
        {
            button3D.interactable = true;
            button3D.onClick.AddListener(clickModel.OnClick3D);
        }
        if (mouseOver3D && flashingHighlighter)
        {
            flashingHighlighter.highlighter = highlighter;
            mouseOver3D.interactable = true;
            mouseOver3D.onMouseOver.AddListener(flashingHighlighter.OnHover);
        }

    }

    // ��˸
    void HighlightFlashingOpen()
    {
        // ������˸
        //highlighter.FlashingOn();
        // ������˸��Ƶ��Ϊ 2f
        //highlighter.FlashingOn(2f);
        // ������˸����ɫ
        //highlighter.FlashingOn(Color.blue, Color.red);
        // ������˸����ɫ��Ƶ��Ϊ 2f
        highlighter.ConstantOn(Color.red, 2f);
    }
    void HighlightFlashingClose()
    {
        // �ر���˸
        highlighter.ConstantOff();
    }
}
