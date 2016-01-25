using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;
using ProtoTblConfig;
using UnityEngine;
using System.Collections;

namespace clientTBL
{
    public class DataManager<StructArry,Struct> 
        where StructArry :class, ProtoBuf.IExtensible,new()
        where Struct : class, ProtoBuf.IExtensible, new()
    {
        public Dictionary<int, Struct> DataStore = new Dictionary<int, Struct>();
        public  List<Struct> DataPool = new List<Struct>();       
        static DataManager()
        {
        }

        public void refresh(string fielpath)
        {
            if (fielpath.Length != 0)
            {
                //WWW www = new WWW(fielpath);
                //yield return www;

                //MemoryStream ms = new MemoryStream(www.bytes);
                TextAsset txt = Resources.Load(fielpath) as TextAsset;
                MemoryStream ms = new MemoryStream(txt.bytes);

                StructArry pbi = Serializer.DeserializeWithLengthPrefix<StructArry>(ms, PrefixStyle.None);
                Type type = pbi.GetType();
                string strname = type.Name;
                switch (strname)
                {
                    case "MsgAttackData":
                        MsgAttackData struct1 = pbi as MsgAttackData;
                        DataPool = struct1.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            AttackData dt = DataPool[i] as AttackData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgAwakeningSkill":
                        MsgAwakeningSkill struct2 = pbi as MsgAwakeningSkill;
                        DataPool = struct2.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            AwakeningSkill dt = DataPool[i] as AwakeningSkill;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgAwakeningSkillLevel":
                        MsgAwakeningSkillLevel struct3 = pbi as MsgAwakeningSkillLevel;
                        DataPool = struct3.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            AwakeningSkillLevel dt = DataPool[i] as AwakeningSkillLevel;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgElementFactor":
                        MsgElementFactor struct4 = pbi as MsgElementFactor;
                        DataPool = struct4.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            ElementFactor dt = DataPool[i] as ElementFactor;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgBuffData":
                        MsgBuffData struct5 = pbi as MsgBuffData;
                        DataPool = struct5.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            BuffData dt = DataPool[i] as BuffData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgCombinationData":
                        MsgCombinationData struct6 = pbi as MsgCombinationData;
                        DataPool = struct6.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            CombinationData dt = DataPool[i] as CombinationData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgConstData":
                        MsgConstData struct7 = pbi as MsgConstData;
                        DataPool = struct7.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            ConstData dt = DataPool[i] as ConstData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgCurrency":
                        MsgCurrency struct8 = pbi as MsgCurrency;
                        DataPool = struct8.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            Currency dt = DataPool[i] as Currency;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgDungeon":
                        MsgDungeon struct9 = pbi as MsgDungeon;
                        DataPool = struct9.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            Dungeon dt = DataPool[i] as Dungeon;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgEnemyData":
                        MsgEnemyData struct10 = pbi as MsgEnemyData;
                        DataPool = struct10.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            EnemyData dt = DataPool[i] as EnemyData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgEvaluateBonus":
                        MsgEvaluateBonus struct11 = pbi as MsgEvaluateBonus;
                        DataPool = struct11.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            EvaluateBonus dt = DataPool[i] as EvaluateBonus;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgFieldEffect":
                        MsgFieldEffect struct12 = pbi as MsgFieldEffect;
                        DataPool = struct12.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            FieldEffect dt = DataPool[i] as FieldEffect;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgHeroData":
                        MsgHeroData struct13 = pbi as MsgHeroData;
                        DataPool = struct13.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            HeroData dt = DataPool[i] as HeroData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgHeroLvData":
                        MsgHeroLvData struct14 = pbi as MsgHeroLvData;
                        DataPool = struct14.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            HeroLvData dt = DataPool[i] as HeroLvData;
                            DataStore.Add((int)dt.level, DataPool[i]);
                        }
                        break;

                    case "MsgHeroEvolution":
                        MsgHeroEvolution struct15 = pbi as MsgHeroEvolution;
                        DataPool = struct15.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            HeroEvolution dt = DataPool[i] as HeroEvolution;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgPotentialData":
                        MsgPotentialData struct16 = pbi as MsgPotentialData;
                        DataPool = struct16.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            PotentialData dt = DataPool[i] as PotentialData;
                            DataStore.Add((int)dt.element, DataPool[i]);
                        }
                        break;

                    case "MsgItemData":
                        MsgItemData struct17 = pbi as MsgItemData;
                        DataPool = struct17.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            ItemData dt = DataPool[i] as ItemData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgLeaderSkill":
                        MsgLeaderSkill struct18 = pbi as MsgLeaderSkill;
                        DataPool = struct18.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            LeaderSkill dt = DataPool[i] as LeaderSkill;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgMonsterData":
                        MsgMonsterData struct19 = pbi as MsgMonsterData;
                        DataPool = struct19.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            MonsterData dt = DataPool[i] as MonsterData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgMonsterSkill":
                        MsgMonsterSkill struct20 = pbi as MsgMonsterSkill;
                        DataPool = struct20.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            MonsterSkill dt = DataPool[i] as MonsterSkill;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgNormalSkill":
                        MsgNormalSkill struct21 = pbi as MsgNormalSkill;
                        DataPool = struct21.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            NormalSkill dt = DataPool[i] as NormalSkill;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgNormalSkillLvUp":
                        MsgNormalSkillLvUp struct22 = pbi as MsgNormalSkillLvUp;
                        DataPool = struct22.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            NormalSkillLvUp dt = DataPool[i] as NormalSkillLvUp;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgRoleData":
                        MsgRoleData struct23 = pbi as MsgRoleData;
                        DataPool = struct23.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            RoleData dt = DataPool[i] as RoleData;
                            DataStore.Add((int)dt.level, DataPool[i]);
                        }
                        break;

                    case "MsgRuneData":
                        MsgRuneData struct24 = pbi as MsgRuneData;
                        DataPool = struct24.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            RuneData dt = DataPool[i] as RuneData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgRuneSet":
                        MsgRuneSet struct25 = pbi as MsgRuneSet;
                        DataPool = struct25.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            RuneSet dt = DataPool[i] as RuneSet;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgRuneEnhance":
                        MsgRuneEnhance struct26 = pbi as MsgRuneEnhance;
                        DataPool = struct26.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            RuneEnhance dt = DataPool[i] as RuneEnhance;
                            DataStore.Add((int)dt.level, DataPool[i]);
                        }
                        break;

                    case "MsgRuneSynchro":
                        MsgRuneSynchro struct27 = pbi as MsgRuneSynchro;
                        DataPool = struct27.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            RuneSynchro dt = DataPool[i] as RuneSynchro;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgSpecialSkill":
                        MsgSpecialSkill struct28 = pbi as MsgSpecialSkill;
                        DataPool = struct28.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            SpecialSkill dt = DataPool[i] as SpecialSkill;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgSpecialSkillLvUp":
                        MsgSpecialSkillLvUp struct29 = pbi as MsgSpecialSkillLvUp;
                        DataPool = struct29.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            SpecialSkillLvUp dt = DataPool[i] as SpecialSkillLvUp;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgDecisionFactor":
                        MsgDecisionFactor struct30 = pbi as MsgDecisionFactor;
                        DataPool = struct30.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            DecisionFactor dt = DataPool[i] as DecisionFactor;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgTaskData":
                        MsgTaskData struct31 = pbi as MsgTaskData;
                        DataPool = struct31.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            TaskData dt = DataPool[i] as TaskData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgHeroText":
                        MsgHeroText struct32 = pbi as MsgHeroText;
                        DataPool = struct32.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            HeroText dt = DataPool[i] as HeroText;
                            Util.TextDic.Add(Util.GetConfigString(dt.name), Util.GetConfigString(dt.textDes));
                        }
                        break;

                    case "MsgDungeonText":
                        MsgDungeonText struct33 = pbi as MsgDungeonText;
                        DataPool = struct33.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            DungeonText dt = DataPool[i] as DungeonText;
                            Util.TextDic.Add(Util.GetConfigString(dt.name), Util.GetConfigString(dt.textDes));
                        }
                        break;

                    case "MsgMonsterText":
                        MsgMonsterText struct34 = pbi as MsgMonsterText;
                        DataPool = struct34.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            MonsterText dt = DataPool[i] as MonsterText;
                            Util.TextDic.Add(Util.GetConfigString(dt.name), Util.GetConfigString(dt.textDes));
                        }
                        break;

                    case "MsgFieldText":
                        MsgFieldText struct35 = pbi as MsgFieldText;
                        DataPool = struct35.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            FieldText dt = DataPool[i] as FieldText;
                            Util.TextDic.Add(Util.GetConfigString(dt.name), Util.GetConfigString(dt.textDes));
                        }
                        break;

                    case "MsgCombinationText":
                        MsgCombinationText struct36 = pbi as MsgCombinationText;
                        DataPool = struct36.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            CombinationText dt = DataPool[i] as CombinationText;
                            Util.TextDic.Add(Util.GetConfigString(dt.name), Util.GetConfigString(dt.textDes));
                        }
                        break;

                    case "MsgSkillText":
                        MsgSkillText struct37 = pbi as MsgSkillText;
                        DataPool = struct37.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            SkillText dt = DataPool[i] as SkillText;
                            Util.TextDic.Add(Util.GetConfigString(dt.name), Util.GetConfigString(dt.textDes));
                        }
                        break;

                    case "MsgCurrencyText":
                        MsgCurrencyText struct38 = pbi as MsgCurrencyText;
                        DataPool = struct38.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            CurrencyText dt = DataPool[i] as CurrencyText;
                            Util.TextDic.Add(Util.GetConfigString(dt.name), Util.GetConfigString(dt.textDes));
                        }
                        break;

                    case "MsgBuffText":
                        MsgBuffText struct39 = pbi as MsgBuffText;
                        DataPool = struct39.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            BuffText dt = DataPool[i] as BuffText;
                            Util.TextDic.Add(Util.GetConfigString(dt.name), Util.GetConfigString(dt.textDes));
                        }
                        break;

                    case "MsgItemText":
                        MsgItemText struct40 = pbi as MsgItemText;
                        DataPool = struct40.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            ItemText dt = DataPool[i] as ItemText;
                            Util.TextDic.Add(Util.GetConfigString(dt.name), Util.GetConfigString(dt.textDes));
                        }
                        break;

                    case "MsgRuneText":
                        MsgRuneText struct41 = pbi as MsgRuneText;
                        DataPool = struct41.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            RuneText dt = DataPool[i] as RuneText;
                            Util.TextDic.Add(Util.GetConfigString(dt.name), Util.GetConfigString(dt.textDes));
                        }
                        break;

                    case "MsgTaskText":
                        MsgTaskText struct42 = pbi as MsgTaskText;
                        DataPool = struct42.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            TaskText dt = DataPool[i] as TaskText;
                            Util.TextDic.Add(Util.GetConfigString(dt.name), Util.GetConfigString(dt.textDes));
                        }
                        break;

                    case "MsgWorldData":
                        MsgWorldData struct43 = pbi as MsgWorldData;
                        DataPool = struct43.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            WorldData dt = DataPool[i] as WorldData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgAirRaidData":
                        MsgAirRaidData struct44 = pbi as MsgAirRaidData;
                        DataPool = struct44.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            AirRaidData dt = DataPool[i] as AirRaidData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgTalentData":
                        MsgTalentData struct45 = pbi as MsgTalentData;
                        DataPool = struct45.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            TalentData dt = DataPool[i] as TalentData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;
                    case "MsgSummonData":
                        MsgSummonData struct46 = pbi as MsgSummonData;
                        DataPool = struct46.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            SummonData dt = DataPool[i] as SummonData;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgSummonPrize":
                        MsgSummonPrize struct47 = pbi as MsgSummonPrize;
                        DataPool = struct47.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            SummonPrize dt = DataPool[i] as SummonPrize;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgStarWeight":
                        MsgStarWeight struct48 = pbi as MsgStarWeight;
                        DataPool = struct48.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            StarWeight dt = DataPool[i] as StarWeight;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgSpecialEffect":
                        MsgSpecialEffect struct49 = pbi as MsgSpecialEffect;
                        DataPool = struct49.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            SpecialEffect dt = DataPool[i] as SpecialEffect;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;

                    case "MsgFlyingEffect":
                        MsgFlyingEffect struct50 = pbi as MsgFlyingEffect;
                        DataPool = struct50.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            FlyingEffect dt = DataPool[i] as FlyingEffect;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;
                    case "MsgDistanceFactor":
                        MsgDistanceFactor struct51 = pbi as MsgDistanceFactor;
                        DataPool = struct51.struct_data as List<Struct>;
                        for (int i = 0; i < DataPool.Count; i++)
                        {
                            DistanceFactor dt = DataPool[i] as DistanceFactor;
                            DataStore.Add((int)dt.id, DataPool[i]);
                        }
                        break;
                }
            }          
        }
        public List<Struct> Data()
        {

            return DataPool;
        }

    }
}
