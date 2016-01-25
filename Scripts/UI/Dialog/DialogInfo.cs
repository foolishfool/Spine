using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogInfo : BaseDialog
{
    private AsynState asynState = AsynState.Completed;
    public AsynState AsynState
    {
        get
        {
            return this.asynState;
        }
        set
        {
            this.asynState = value;
        }
    }

    public new void Close()
    {
        base.Close();
    }

    public void OpenPromptDialog(GameObject container, string data)
    {
        base.Open(container, string.Empty);
        Util.Get<UILabel>(base.Container, "Prompt").text = data;
        container.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    public void OpenDialog(GameObject container)
    {
        this.asynState = AsynState.Processing;
        base.Open(container, string.Empty);
    }

}
