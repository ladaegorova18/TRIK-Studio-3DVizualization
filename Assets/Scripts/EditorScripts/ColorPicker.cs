using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public SliderArea SatValSlider;
    public Slider HueSlider;
    public Image SVDisplayColor;
    public Color Color;
    IEnumerator coroutine;

    public UnityEvent onValueChanged;

    public delegate void ColorChanged(Color color);
    public event ColorChanged NotifyColorChanged;

    // Start is called before the first frame update
    void Start()
    {
        if (onValueChanged == null)
            onValueChanged = new UnityEvent();

        UpdateColor();
    }

    void UpdateColor()
    {
        Vector2 satValValue;
        float hue;
        float saturation;
        float value;


        satValValue = SatValSlider.Value();
        hue = HueSlider.value;
        saturation = satValValue.x;
        value = satValValue.y;

        Color prevColor = Color;

        Color = Color.HSVToRGB(hue, saturation, value);

        Color currentColor = Color;

        bool valueChange = currentColor.r != prevColor.r || currentColor.g != prevColor.g || currentColor.b != prevColor.b;
        if (valueChange)
        {
            onValueChanged.Invoke();
            SVDisplayColor.color = Color.HSVToRGB(hue, 1, 1);
            NotifyColorChanged(Color);
        }
    }

    void OnEnable()
    {
        SatValSlider.onValueChanged.AddListener(delegate { UpdateColor(); });
        HueSlider.onValueChanged.AddListener(delegate { UpdateColor(); });
    }


    void OnDisable()
    {
        SatValSlider.onValueChanged.RemoveAllListeners();
        HueSlider.onValueChanged.RemoveAllListeners();
    }
}