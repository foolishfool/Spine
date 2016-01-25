using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class KongxiManager : MonoBehaviour {
    public static KongxiManager instance;

    [System.Serializable]
    public class AirWave
    {
        public float firstTime;//首波出现的时间
        public float[] waveInterval;//波次之间的间隔时间随机范围
        public float interval;//每波两只之间的间隔时间
        public int[] crowID;//可以刷哪些id，对应airRaid表
        public int[] crowNum;//一波刷几只的随机范围
    }
    /// <summary>
    /// 乌鸦（临时，应动态加载过来）
    /// </summary>
    public GameObject wuya;
    /// <summary>
    /// 每波战斗开始，开始生成乌鸦
    /// </summary>
    public List<AirWave> waves = new List<AirWave>();


    public Transform left;
    public Transform right;

    int index = -1;

    void Awake()
    {
        instance = this;
    }

    public void Init(string config)
    {
        index = -1;
        JsonData dt = JsonMapper.ToObject(config);
        for (int i = 0; i < dt.Count; i++)
        {
            JsonData waveDt = dt[i];

            if (waveDt != null && waveDt.Count > 0)
            {
                AirWave w = new AirWave();
                w.firstTime = float.Parse(waveDt[0].ToString());
                JsonData waveInterval = waveDt[1];
                w.waveInterval = new float[2] { float.Parse(waveInterval[0].ToString()), float.Parse(waveInterval[1].ToString()) };
                w.interval = float.Parse(waveDt[2].ToString());
                JsonData crowID = waveDt[3];
                w.crowID = new int[crowID.Count];
                for (int j = 0; j < crowID.Count; j++ )
                    w.crowID[j] = (int)crowID[j];
                JsonData crowNum = waveDt[4];
                w.crowNum = new int[2] { int.Parse(crowNum[0].ToString()), int.Parse(crowNum[0].ToString()) };

                waves.Add(w);
            }
            else
                waves.Add(null);
        }
        StartKongxi(null);
    }

    void Start()
    {
        CEventDispatcher.GetInstance().AddEventListener(CEventType.NEXT_BATTALE_START, StartKongxi);
        CEventDispatcher.GetInstance().AddEventListener(CEventType.GAME_WIN, StopKongxi);
        CEventDispatcher.GetInstance().AddEventListener(CEventType.GAME_OVER, StopKongxi);
    }

    void OnDestroy()
    {
        CEventDispatcher.GetInstance().RemoveEventListener(CEventType.NEXT_BATTALE_START, StartKongxi);
        CEventDispatcher.GetInstance().RemoveEventListener(CEventType.GAME_WIN, StopKongxi);
        CEventDispatcher.GetInstance().RemoveEventListener(CEventType.GAME_OVER, StopKongxi);
    }

    void StartKongxi(CBaseEvent evt)
    {
        index++;
        if (index >= waves.Count || waves[index] == null)
            return;
        StopAllCoroutines();
        StartCoroutine(Kongxi(waves[index]));     
    }

    void StopKongxi(CBaseEvent evt)
    {
        StopAllCoroutines();
    }

    IEnumerator Kongxi(AirWave wave)
    {
        yield return new WaitForSeconds(wave.firstTime);
        float waveInterval = Random.Range(wave.waveInterval[0], wave.waveInterval[1]);
        while (true)
        {
            int count = Random.Range(wave.crowNum[0],wave.crowNum[1] + 1);
            bool isToRight = Random.value <= 0.5f;
            Vector3 from, to;
            if (isToRight)
            {
                from = left.localPosition;
                to = right.localPosition;
            }
            else
            {
                to = left.localPosition;
                from = right.localPosition;
            }
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(wuya) as GameObject;
                obj.transform.parent = transform;
                obj.transform.localScale = Vector3.one;
                if (isToRight)
                    obj.transform.localRotation = Quaternion.Euler(0,180,0);
                obj.SetActive(true);
                KongxiWuya kongxiWuya = obj.GetComponent<KongxiWuya>();
                Vector3 target = GetOneTarget(isToRight ? (FightGroup)FightManager.GetInstance().enemyGroup : (FightGroup)FightManager.GetInstance().mineGroup);
                kongxiWuya.from = from;
                kongxiWuya.to = to;
                kongxiWuya.target = target;
                kongxiWuya.speed = 100f;
                yield return new WaitForSeconds(wave.interval);
            }
            yield return new WaitForSeconds(waveInterval);
        }
    }

    Vector3 GetOneTarget(FightGroup group)
    {
        int i = Random.Range(0,group.fightUnits.Count);
        Vector3 target = left.localPosition;
        target.x = group.fightUnits[i].transform.localPosition.x;
        return target;
    }
}
