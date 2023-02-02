using UnityEngine;
using System.Collections;
using HighlightingSystem;

public class FlashingHighlighter : MonoBehaviour
{
    public Highlighter highlighter;
    public Color constantColor = new Color(52.0f / 255.0f, 255.0f / 255.0f, 2.0f / 255.0f, 255.0f / 255.0f);
    public Color hoverColor = new Color(209.0f / 255.0f, 255.0f / 255.0f, 0.0f / 255.0f, 255.0f / 255.0f);

    public GameObject AlarmCanvas;   //∑÷“≥√Ê
    public GameObject InquireCanvas;
    public GameObject AnalyseCanvas;

    public void OnHover()
    {
        
        if (highlighter != null)
        {
            if (AlarmCanvas.transform.GetComponent<CanvasGroup>().alpha == 0 && InquireCanvas.transform.GetComponent<CanvasGroup>().alpha == 0 && AnalyseCanvas.transform.GetComponent<CanvasGroup>().alpha == 0)
            {
                highlighter.Hover(hoverColor);
            }
        }
    }
    public void OnHover1()
    {

        if (highlighter != null)
        {
            if (AlarmCanvas.transform.GetComponent<CanvasGroup>().alpha == 0 && InquireCanvas.transform.GetComponent<CanvasGroup>().alpha == 0 && AnalyseCanvas.transform.GetComponent<CanvasGroup>().alpha == 0)
            {
                highlighter.Hover(Color.red);
            }
        }
    }
    public void Show()
    {
        if (highlighter != null)
        {
            highlighter.ConstantOnImmediate(constantColor);
        }
    }
    public void Hide()
    {
        if (highlighter != null)
        {
            highlighter.ConstantOffImmediate();
        }
    }
}
