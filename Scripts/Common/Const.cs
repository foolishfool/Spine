using UnityEngine;
using System.Collections;

public class Const{

    public static string GamePrefabDir = "Game/";
    public static string PanelPrefabDir = "Assets/Prefabs/UI/";
    public static string MiscPrefabDir = "Assets/Prefabs/Misc/";
    public static string MapDataDir = "data/map/";

    public static string GuiSoundAssetDir = "Sounds/GUI/";
    public static string ParticleSoundAssetDir = "Sounds/Particle/";
    public static string WeaponSoundAssetDir = "Sounds/Weapon/";


    public static string LoaderLevel = "loader";
    public static string LoginLevel = "login";
    public static string HomeLevel = "home";
    public static string FightLevel = "fight";
    public static float MoveSpeed = 130f;
	public static float PursueSpeed = 280f;
    public static float MoveToNextSpeed = 0.7f;
    public static float DieTime = 1.5f;
    public static float HurtNumEmitterTime = 0f;
    public static float HealthBarDisplayTime = 3f;
    //public static float ModifyYStep = 10f;
    //public static Vector2 OverlayRule = new Vector2(9f,9f);//判断是否重叠的标准
    public static string IdleAction = "idle01";
    public static string RunAction = "run01";
    public static string WinAction = "win01";
    public static string FightPanel = "FightPanel";//战斗界面主UI
    public static string EvaluatePanel = "EvaluatePanel";//通关结算界面UI

    public static float greyScale = 0.5f;//释放绝技时变灰

    public static int heroid = 0;
    public static int monsterid = 0;

    #region 配置表中的常数
    public static int CONST_MAX_LEVEL;//最高等级
    public static int CONST_STAMINA_RECOVER;//体力值回复间隔(单位秒)，即多少秒回复1点
    public static string CONST_STAMINA_GIFT;//领取体力值
    public static int CONST_MAX_FRIEND;//最多可购买多少个好友数量
    public static int CONST_HERO_LVUP_CONSUME;//英雄强化消耗的基础游戏币
    public static int CONST_HERO_POTENTIAL_UP;//每次提升的潜力值
    public static int CONST_HERO_POTENTIAL_COIN;//提升潜力值消耗的游戏币
    public static int CONST_MAX_ENERGY;//能量值上限
    public static int CONST_ENERGY_RECOVER_TIME;//能量回复间隔
    public static int CONST_ENERGY_RECOVER_VAL;//能量回复速度
    public static int CONST_MIN_ENERGY_LIMIT;//施放绝技最低需求的能量值
    public static int CONST_RUNE_MAX_LEVEL;//符文最大强化等级
    public static int CONST_RUNE_LEVEL_INTERVAL;//符文每强化多少级提升一次扩展属性
    public static int CONST_ATTACK_INTERVAL;//普通攻击间隔常数
    public static float CONST_BASIC_MISS_RATE;//基础Miss率
    public static float CONST_DODGE_FACTOR;//闪避系数
    public static float CONST_MAX_MISS_RATE;//修正Miss率上限
    public static float CONST_MIN_MISS_RATE;//修正Miss率下限
    public static float CONST_BASIC_CRIT_RATE;//基础暴击率
    public static float CONST_CRIT_FACTOR;//暴击系数
    public static float CONST_MAX_CRIT_RATE;//修正暴击率上限
    public static float CONST_MIN_CRIT_RATE;//修正暴击率下限
    public static int CONST_BASIC_CRIT_DMG;//基础暴击伤害系数
    public static float CONST_RESIST_FACTOR;//抵抗系数
    public static int CONST_MAX_DEF_EFFECT;//防御效果上限
    public static float CONST_MIN_DEF_EFFECT;//防御效果下限
    public static float CONST_DEF_FACTOR;//防御常数
    public static float CONST_COMBO_TIME;//combo最大间隔时间
    public static int CONST_SPECIALSKILL_RNDMAX;//绝技决策随机上限
    public static int CONST_SPECIALSKILL_RNDMIN;//绝技决策随机下限
    public static int CONST_MERCENARY_COOLDOWN;//队长招募基础CD，单位秒
    public static int CONST_MERCENARY_TIME;//队长招募登录时长，单位秒
    public static float CONST_MERCENARY_FEE;//队长招募系统，系统从佣金扣除的部分
    public static int CONST_RUNE_EXPAND_INTERVAL;//符文强化多少次，扩展属性提升一个步长
    public static int CONST_MAX_SUMMON_VAL_INIT;//召唤累计值上限初始化
    public static int CONST_FIRST_HERO;//初始英雄
    public static int CONST_DUNGEON_MAX_TIME_POINT;//评价时间分上限
    public static float CONST_DUNGEON_TIME_FACTOR;//时间分系数
    public static float CONST_DUNGEON_LIFE_PCT;//生命基准百分比
    public static int CONST_DUNGEON_MAX_LIFE_POINT;//生存积分上限
    public static int CONST_DUNGEON_MAX_COMBO_POINT;//连击最高分
    public static float CONST_DUNGEON_COMBO_FACTOR;//连击加分系数
    public static int CONST_DUNGEON_COMBO_BASIC;//连击加分修正值
    public static int CONST_DUNGEON_TECH_BASIC;//技巧基准分
    public static int CONST_DUNGEON_TECH_FACTOR;//技巧扣分系数
    public static int CONST_DUNGEON_DEATH_BASIC;//死亡基准分
    public static float CONST_DUNGEON_DEATH_FACTOR;//死亡扣分系数
    public static int CONST_DUNGEON_SPECSKILL_FACTOR;//绝技分系数
    public static int CONST_DUNGEON_MAX_SPECSKILL_POINT;//绝技最高分
    public static int CONST_DUNGEON_SPECSKILL_BASIC;//绝技分修正值
    #endregion
}
