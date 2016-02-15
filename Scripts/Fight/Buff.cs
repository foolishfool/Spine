using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTblConfig;

public class Buff : MonoBehaviour {
    /// <summary>
    /// 配置表数据
    /// </summary>
    public BuffData buffData;

    /// <summary>
    /// 在配置表中的id
    /// </summary>
    public int id;

    /// <summary>
    /// buff来源
    /// </summary>
    public FightUnit source;

    /// <summary>
    /// buff作用对象
    /// </summary>
    public FightUnit target;

    /// <summary>
    /// 持续时间
    /// </summary>
    public float lastTime;

    /// <summary>
    /// buff数值
    /// </summary>
    public float buffValue;

    /// <summary>
    /// dot伤害攻击类型
    /// </summary>
    public HurtType hurtType;

    /// <summary>
    /// 起始特效
    /// </summary>
    public GameObject startEffect;
    string startEffectName;
    SpecialEffect startEffectConfig;
    /// <summary>
    /// 进行特效
    /// </summary>
    public GameObject processEffect;
    string processEffectName;
    SpecialEffect processEffectConfig;
    /// <summary>
    /// 结束特效
    /// </summary>
    public GameObject endEffect;
    string endEffectName;
    SpecialEffect endEffectConfig;

    /// <summary>
    /// 绑定的绝技，buff的持续时间由绝技决定
    /// </summary>
    public UniqueSkill bindSkill;

    float startTime;//buff起效时间

    public virtual void Start()
    {
        target.RegisterBuff(this);

        Dictionary<int, SpecialEffect> effectDic = Util.GetDic<MsgSpecialEffect, SpecialEffect>();
        int startEffectId = (int)buffData.startEffect;
        if(startEffectId != 0)
        {
            startEffectConfig = effectDic[startEffectId];
            startEffectName = Util.GetConfigString(startEffectConfig.name);
        }
        int processEffectId = (int)buffData.processEffect;
        if(processEffectId != 0)
        {
            processEffectConfig = effectDic[processEffectId];
            processEffectName = Util.GetConfigString(processEffectConfig.name);
        }
        int endEffectId = (int)buffData.endEffect;
        if (endEffectId != 0)
        {
            endEffectConfig = effectDic[endEffectId];
            endEffectName = Util.GetConfigString(endEffectConfig.name);
        }

        startTime = Time.time; 
        StartCoroutine(DisplayEffect(startEffectName, startEffectConfig, EffectId.StartEffect)); 
        StartCoroutine(DisplayEffect(processEffectName, processEffectConfig, EffectId.ProcessEffect));

        BuffStart();

        float interval = buffData.interval;
        if (interval > 0)
            StartCoroutine(StartBuffProcess(interval));
    }

    enum EffectId
    {
        StartEffect,
        ProcessEffect,
        EndEffect,
    }

    IEnumerator DisplayEffect(string effectName, SpecialEffect effectConfig, EffectId id)
    {
        if (!string.IsNullOrEmpty(effectName))
        {
            yield return StartCoroutine(AssetManager.LoadAsset(effectName, AssetManager.AssetType.Effect, false));
            GameObject obj = AssetManager.GetGameObject(effectName, target.GetEffectPoint((EffectPoint)effectConfig.bone));
            if (id == EffectId.StartEffect)
                startEffect = obj;
            else if (id == EffectId.ProcessEffect)
                processEffect = obj;
            else if (id == EffectId.EndEffect)
                endEffect = obj;
        }
    }

    public virtual void OnDestroy()
    {
        StopAllCoroutines();
        if (startEffect) Destroy(startEffect);
        if (processEffect) Destroy(processEffect);
        if (endEffect) Destroy(endEffect);
    }

    public virtual void Update()
    {
        //绝技维持的增益buff持续时间跟随绝技
        if (bindSkill != null && buffData.buffType == 0)
        { 
            if(!bindSkill.isUsing)
                Clear();
        }
        else
        {
			if ( startTime + lastTime < Time.time)
                Clear();
        }			
    }

    IEnumerator StartBuffProcess(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            BuffProcess();
        }
    }

    /// <summary>
    /// buff起效
    /// </summary>
    public virtual void BuffStart(){ }

    /// <summary>
    /// buff每隔间隔起效
    /// </summary>
    public virtual void BuffProcess(){ }

    /// <summary>
    /// buff结束
    /// </summary>
    public virtual void BuffEnd(){ }

    /// <summary>
    /// 清除buff
    /// </summary>
    public void Clear()
    {
        if (IsInvoking("ToDestroySelf"))
            return;
        BuffEnd();
        StartCoroutine(DisplayEffect(endEffectName, endEffectConfig, EffectId.EndEffect));
        enabled = false;
        target.LogoffBuff(this);
        if (processEffect) Destroy(processEffect);
        //0.5秒内销毁buff
		Invoke("ToDestroySelf",0.5f);
    }

    void ToDestroySelf()
    {
        Destroy(gameObject);
    }

    public static Buff AddNewBuff(FightUnit target, FightUnit source,BuffData buffData, float buffValue, float lastTime)
    {
        return AddNewBuff(target, source, buffData, buffValue, lastTime, HurtType.None);
    }

    public static Buff AddNewBuff(FightUnit target, FightUnit source, BuffData buffData, float buffValue, float lastTime, HurtType hurtType)
    {
        BuffLogicType type = (BuffLogicType)buffData.effectType;
        Buff buff = null;
        switch (type)
        {
            case BuffLogicType.Yunxuan:
                buff = NGUITools.AddChild<YunxuanBuff>(target.gameObject);
                break;
            case BuffLogicType.Jiansu:
                buff = NGUITools.AddChild<JiansuBuff>(target.gameObject);
                break;
            case BuffLogicType.Jinzu:
                buff = NGUITools.AddChild<JinzuBuff>(target.gameObject);
                break;
            case BuffLogicType.Chenmo:
                buff = NGUITools.AddChild<ChenmoBuff>(target.gameObject);
                break;
            case BuffLogicType.Hunluan:
                buff = NGUITools.AddChild<HunluanBuff>(target.gameObject);
                break;
            case BuffLogicType.Meihuo:
                buff = NGUITools.AddChild<MeihuoBuff>(target.gameObject);
                break;
            case BuffLogicType.Bingdong:
                buff = NGUITools.AddChild<BingdongBuff>(target.gameObject);
                break;
            case BuffLogicType.Mabi:
                buff = NGUITools.AddChild<MabiBuff>(target.gameObject);
                break;
            case BuffLogicType.Shihua:
                buff = NGUITools.AddChild<ShihuaBuff>(target.gameObject);
                break;
            case BuffLogicType.Zhongdu:
                buff = NGUITools.AddChild<ZhongduBuff>(target.gameObject);
                break;
            case BuffLogicType.Xchixu:
                buff = NGUITools.AddChild<XchixuBuff>(target.gameObject);
                break;
            case BuffLogicType.Huifu:
                buff = NGUITools.AddChild<HuifuBuff>(target.gameObject);
                break;
            case BuffLogicType.Falihuifu:
                buff = NGUITools.AddChild<FalihuifuBuff>(target.gameObject);
                break;
            case BuffLogicType.Faliranshao:
                buff = NGUITools.AddChild<FaliranshaoBuff>(target.gameObject);
                break;
            case BuffLogicType.Cuozhi:
                buff = NGUITools.AddChild<Cuozhibuff>(target.gameObject);
                break;
            case BuffLogicType.Kuangbao:
                buff = NGUITools.AddChild<KuangbaoBuff>(target.gameObject);
                break;
            case BuffLogicType.Xjianmian:
                buff = NGUITools.AddChild<XjianmianBuff>(target.gameObject);
                break;
            case BuffLogicType.Xfantan:
                buff = NGUITools.AddChild<XfantanBuff>(target.gameObject);
                break;
            case BuffLogicType.Xixue:
                buff = NGUITools.AddChild<XixueBuff>(target.gameObject);
                break;
            case BuffLogicType.Zuzhou:
                buff = NGUITools.AddChild<ZuzhouBuff>(target.gameObject);
                break;
            case BuffLogicType.Zhufu:
                buff = NGUITools.AddChild<ZhufuBuff>(target.gameObject);
                break;
            case BuffLogicType.Xhudun:
                buff = NGUITools.AddChild<XhudunBuff>(target.gameObject);
                break;
            case BuffLogicType.Kuangye:
                buff = NGUITools.AddChild<KuangyeBuff>(target.gameObject);
                break;
            case BuffLogicType.Shanbi:
                buff = NGUITools.AddChild<ShanbiBuff>(target.gameObject);
                break;
            case BuffLogicType.Canbao:
                buff = NGUITools.AddChild<CanbaoBuff>(target.gameObject);
                break;
            case BuffLogicType.Yingyong:
                buff = NGUITools.AddChild<YingyongBuff>(target.gameObject);
                break;
            case BuffLogicType.Chihuan:
                buff = NGUITools.AddChild<ChihuanBuff>(target.gameObject);
                break;
            case BuffLogicType.Zhimang:
                buff = NGUITools.AddChild<ZhimangBuff>(target.gameObject);
                break;
            case BuffLogicType.Wudi:
                buff = NGUITools.AddChild<WudiBuff>(target.gameObject);
                break;
            case BuffLogicType.Mianyikongzhi:
                buff = NGUITools.AddChild<MianyikongzhiBuff>(target.gameObject);
                break;
            case BuffLogicType.Xyishang:
                buff = NGUITools.AddChild<XyishangBuff>(target.gameObject);
                break;
            case BuffLogicType.Pojia:
                buff = NGUITools.AddChild<PojiaBuff>(target.gameObject);
                break;
            default:
                buff = NGUITools.AddChild<Buff>(target.gameObject);
                break;
        }
        buff.target = target;
        buff.source = source;
        buff.buffData = buffData;
        buff.buffValue = buffValue;
        buff.lastTime = lastTime;
        buff.id = (int)buffData.id;
        buff.hurtType = hurtType;
        return buff;
    }
}
