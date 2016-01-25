using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerData{
    static PlayerData instance;
    public static PlayerData GetInstance()
    {
        if (instance == null)
            instance = new PlayerData();
        return instance;
    }
    //当前选中出战的英雄id
    public List<int> CurrentFightHero = new List<int>() { 2,2,1,1,1};
    //当前备战的英雄id
    public List<int> CurrentFightBakHero = new List<int>() { 1,2};

    /*
     * 包裹里的所有卡片，符文，金币等等
     * 
     * 
     * */
}
