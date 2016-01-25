//伤害类型
public enum HurtType
{
    None,//无0
    Fire,//火1
    Water,//水2
    Earth,//土3
    Light,//光4
    Dark,//暗5
    True,//真实伤害6
}
//技能结果
public enum SkillType
{
    None,
    Hurt,//伤害1
    Cure,//治疗2
}
//附加效果
public enum SkillAttach
{
    None,
    Beatback,//击退1
    EnegyRecover,//能量回复2
    ManaRecover,//法力回复3
    Taunt,//嘲讽4
    Dispel,//驱散5
    CallTotem,//召唤图腾6
	GetBlood,//吸血
	LoseMana,//消耗法力
}
//弹道
public enum SkillCurve
{
    None,
    Melee,//近战1
    Directional,//远程指向性2
    Parabola,//抛物线3
    Magica,//魔法类，在魔法特效出现后延迟一段时间产生伤害 4
    LineAoe,//非指向性直线aoe 5
}
//技能选取方式
public enum SkillSelect
{
    None,
    /*--单体攻击--*/
    NowTarget,//当前目标1
    LongAway,//最远目标2
    Weakest,//hp最少目标3
    Random,//随机目标4
    /*--群体攻击--*/
    NowTargetNearby,//当前目标附近5
    LongAwayNearby,//最远目标附近6
    WeakestSeveral,//HP最少的几个7
    SelfNearby,//自身周围8
    All,//全屏范围9
    Foreground,//前方一定距离10
    RandomSeveral,//随机几个11
    /*--辅助型技能--*/
    NearbyFriend,//自身周围友方12
    AllFriend,//全屏友方13
    WeakestSeveralFriend,//hp最少的N个友方14
    Self,//自己15
    Front,//最前方友方16
}
//技能释放条件(AI)
public enum SkillCondition
{
    None,//无（总是释放）0
    HPHigher,//自身血量高于XX1
    HPLower,//自身血量低于XX2
    FirendHPLower,//有友方血量低于XX3
}

//被动技能(觉醒技能)触发条件
public enum PassiveCondition
{
    None,//无（总是释放）0
    HPHigher,//生命值高于XX1
    HPLower,//生命值低于XX2
    SpecailScene,//特定场景效果下
    NormalCritical,//普通攻击产生暴击一定几率触发
    Dodge,//闪避一定几率触发
    GetHit,//受到攻击（持续伤害不触发)时一定几率触发
    KillEnemy,//击杀目标一定几率触发
    StartNextBattle,//进入下一场战斗
}
//特效挂点
public enum EffectPoint
{
    None,//无 （默认原点） 0
    Foot,//脚底 1
    LeftHand,//左手 2
    LeftWeapon,//左手武器 3
    RightHand,//右手 4
    RightWeapon,//右手武器 5
    Head,//头顶 6
    Body,//身体 7
}
//特效挂载对象
public enum EffectTarget
{
    None,//无 （默认自身位置向前一定距离（配置表中的范围大小）） 0
    Self,//自身 1
    Enemy,//敌人 2 
    Screen,//全屏 3
    EnemyCenter,//地方阵营中心
    MineCenter,//我方阵营中心
}

//buff效果类型
public enum BuffLogicType
{
    None,//空buff 0
    Yunxuan,	//	晕眩	1
    Jiansu,	//	减速	2
    Jinzu,	//	禁足	3
    Chenmo,	//	沉默	4
    Hunluan,	//	混乱	5
    Meihuo,	//	魅惑	6
    Bingdong,	//	冰冻	7
    Mabi,	//	麻痹	8
    Shihua,	//	石化	9
    Zhongdu,	//	中毒	10
	Xchixu,//X属性持续伤害 11
    Huifu,	//	恢复	12
    Falihuifu,	//	法力回复	13
    Faliranshao,	//	法力燃烧	14
    Cuozhi,	//	挫志	15
    Kuangbao,	//	狂暴	16
	Xjianmian, // X属性伤害减免 17
	Xfantan, // X属性伤害反弹 18
    Xixue,	//	吸血	19
    Zuzhou,	//	诅咒	20
    Zhufu,	//	祝福	21
	Xhudun,    //  X属性护盾  22
    Kuangye,	//	狂野	23
    Shanbi,	//	闪避	24
    Canbao,	//	残暴	25
    Yingyong,	//	英勇	26
    Chihuan,	//	迟缓	27
    Zhimang,	//	致盲	28
    Wudi,	//	无敌	29
    Mianyikongzhi,	//	免疫控制	30
	Xyishang, // X属性易伤 31
    Pojia,	//	破甲	32
}