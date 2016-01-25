using UnityEngine;
using System.Collections;
[RequireComponent(typeof(UILabel))]
public class LabelWidget : BaseWidget {
    public const string nameExtension = "_Label";
    UILabel label;

    public string Value
    {
        get
        {
            if(!label) label = GetComponent<UILabel>();
            return label.text;
        }
        set
        {
            if (!label) label = GetComponent<UILabel>();
            label.text = value;
        }
    }
}
