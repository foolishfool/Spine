using System;
using UnityEngine;

public class BaseDialog 
{
    private string name;
    private UILabel title;
    private UILabel prompt;
    private GameObject container;
    protected DialogType _type;

    public string Name
    {
        get
        {
            return this.name;
        }
        set
        {
            this.name = value;
        }
    }

    public DialogType Type
    {
        get
        {
            return this._type;
        }
        set
        {
            this._type = value;
        }
    }

    public GameObject Container
    {
        get
        {
            return this.container;
        }
        set
        {
            this.container = value;
        }
    }

    public void InitDialog(GameObject _container)
    {
        GameObject topobj = Util.Child(_container, "TopName");
        if (topobj != null)
        {
            this.prompt = topobj.GetComponent<UILabel>();
        }

        GameObject objtitle = Util.Child(_container, "Title");
        if (objtitle != null)
        {
            this.title = objtitle.GetComponent<UILabel>();
        }
    }

    protected void SetPromptText(string text)
    {
        this.prompt.text = text;
    }

    protected void Open(GameObject container, string data)
    {
        this.container = container;
        this.InitDialog(container);

        if (!data.Equals(string.Empty) && this.title != null)
        {
            this.title.text = data;
        }
    }

    protected void Close()
    {

    }
}
