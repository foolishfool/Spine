using UnityEngine;
using System.Collections;

public class DragRecord : DragClickButton {
    bool isClicked = false;

    public override void OnClick()
    {
        isClicked = true;
        Destroy(gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (!isClicked)
            DungeonRecord.unKillDragObj++;
    }
}
