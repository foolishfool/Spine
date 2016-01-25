using UnityEngine;
using System.Collections;
using clientTBL;
using ProtoTblConfig;
using System.Collections.Generic;

public class tmpPause : MonoBehaviour {
    public void PauseOrContinue()
    {
        Global.Paused = !Global.Paused;
        if (Global.Paused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
