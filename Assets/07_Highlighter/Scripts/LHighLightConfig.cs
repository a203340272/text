using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

[RequireComponent(typeof(Button3D))]
[RequireComponent(typeof(FlashingHighlighter))]
[RequireComponent(typeof(MouseOver3D))]
[RequireComponent(typeof(Highlighter))]
[RequireComponent(typeof(ClickModelL))]
public class LHighLightConfig : MonoBehaviour
{
    Button3D button3D;
    ClickModelL clickModel;
    MouseOver3D mouseOver3D;
    FlashingHighlighter flashingHighlighter;
    Highlighter highlighter;

    void Start()
    {
        button3D = gameObject.GetComponent<Button3D>();
        clickModel = gameObject.GetComponent<ClickModelL>();
        mouseOver3D = gameObject.GetComponent<MouseOver3D>();
        flashingHighlighter = gameObject.GetComponent<FlashingHighlighter>();
        highlighter = gameObject.GetComponent<Highlighter>();
        Debug.Log("========拿到高亮物体：" + gameObject.name + ", button3D:" + button3D + ", clickModel:" + clickModel);
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

    // 闪烁
    void HighlightFlashingOpen()
    {
        highlighter.ConstantOn(Color.red, 2f);
    }
    void HighlightFlashingClose()
    {
        // 关闭闪烁
        highlighter.ConstantOff();
    }


}

