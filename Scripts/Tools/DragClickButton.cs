using UnityEngine;
using System.Collections;
[RequireComponent(typeof(UIWidget))]
public class DragClickButton : BaseWidget {
    public UIWidget ui;
    public WidgetEvent onClick;
    public const string nameExtension = "_DragButton";

    public virtual void Start()
    {
        ui = GetComponent<UIWidget>();
        DragClick.instance.AddListenUI(ui);
    }

    public virtual void OnDestroy()
    {
        if (DragClick.hasInstance)
            DragClick.instance.RemoveListenUI(ui);
    }

    public virtual void OnClick()
    {
        if (onClick != null)
            onClick();
    }
}
