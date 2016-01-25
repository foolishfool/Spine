using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIContainer : MonoBehaviour {

    public GameObject loginPanel;
    public GameObject mainPanel;
    public GameObject fightPanel;
    public GameObject duplicatePanel;
    public GameObject worldPanel;
    public GameObject taskPanel;
    public GameObject awardPanel;
    public GameObject skillPanel;
    public GameObject characterPanel;
    public GameObject healthPanel;

    public List<GameObject> panels = new List<GameObject>();

    public List<GameObject> AllPanel
    {
        get
        {
            this.panels.Clear();
            this.AddPanel(this.loginPanel);
            this.AddPanel(this.mainPanel);
            this.AddPanel(this.fightPanel);
            this.AddPanel(this.duplicatePanel);
            this.AddPanel(this.worldPanel);
            this.AddPanel(this.taskPanel);
            this.AddPanel(this.awardPanel);
            this.AddPanel(this.skillPanel);
            this.AddPanel(this.characterPanel);
            this.AddPanel(this.healthPanel);

            return this.panels;
            
        }
    }

    public static UIContainer instance;

    void Awake()
    {
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        instance = this;
    }

    private void AddPanel(GameObject go)
    {
        if (go != null)
        {
            this.panels.Add(go);
        }
    }

    //public void ClearAll()
    //{
    //    List<GameObject> all = AllPanel;
    //    foreach (GameObject obj in all)
    //    {
    //        if (obj != null)
    //            DestroyImmediate(obj, true);
    //    }
    //    panels.Clear();
    //}
}
