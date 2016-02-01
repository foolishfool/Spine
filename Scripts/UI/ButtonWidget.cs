using UnityEngine;
using System.Collections;

public class ButtonWidget : BaseWidget {
    public const string nameExtension = "_Button";
    public UILabel label;

    /// <summary>
    /// 按钮的文字
    /// </summary>
    public string Value
    {
        get
        {
            if (!label) label = GetComponentInChildren<UILabel>();
            return label.text;
        }
        set
        {
            if (!label) label = GetComponentInChildren<UILabel>();
            label.text = value;
        }
    }

    public WidgetEvent onClick;

    void OnClick()
    {
		if (onClick != null)
            onClick();
    }
		

		
	void Update() 
	{

	}

}
