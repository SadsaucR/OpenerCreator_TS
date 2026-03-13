using System;

namespace OpenerCreator.Helpers;

public enum Jobs
{
    ANY = 0,

    // Tanks (坦克)
    PLD = 19, // 騎士
    WAR = 21, // 戰士
    DRK = 32, // 暗黑騎士
    GNB = 37, // 絕槍戰士

    // Healers (治療)
    WHM = 24, // 白魔道士
    SCH = 28, // 學者
    AST = 33, // 占星術師
    SGE = 40, // 賢者

    // Melee (近戰輸出)
    MNK = 20, // 武僧
    DRG = 22, // 龍騎士
    NIN = 30, // 忍者
    SAM = 34, // 武士
    RPR = 39, // 奪魂者
    VPR = 41, // 毒蛇劍士 (7.0 新職業)

    // Physical Ranged (遠程物理)
    BRD = 23, // 吟遊詩人
    MCH = 31, // 機工士
    DNC = 38, // 舞者

    // Magical Ranged (遠程魔法)
    BLM = 25, // 黑魔道士
    SMN = 27, // 召喚士
    RDM = 35, // 赤魔道士
    PCT = 42, // 繪靈法師 (7.0 新職業)
    BLU = 36  // 青魔道士
}

[Flags]
public enum JobCategory
{
    None = 0,
    Tank = 1 << 0,
    Healer = 1 << 1,
    Melee = 1 << 2,
    PhysicalRanged = 1 << 3,
    MagicalRanged = 1 << 4
}

public static class JobsExtensions
{
    public static string PrettyPrint(this Jobs job)
    {
        return job switch
        {
            Jobs.ANY => "全部",
            Jobs.PLD => "騎士",
            Jobs.WAR => "戰士",
            Jobs.DRK => "暗黑騎士",
            Jobs.GNB => "絕槍戰士",
            Jobs.WHM => "白魔道士",
            Jobs.SCH => "學者",
            Jobs.AST => "占星術師",
            Jobs.SGE => "賢者",
            Jobs.MNK => "武僧",
            Jobs.DRG => "龍騎士",
            Jobs.NIN => "忍者",
            Jobs.SAM => "武士",
            Jobs.RPR => "奪魂者",
            Jobs.VPR => "毒蛇劍士",
            Jobs.BRD => "吟遊詩人",
            Jobs.MCH => "機工士",
            Jobs.DNC => "舞者",
            Jobs.BLM => "黑魔道士",
            Jobs.SMN => "召喚士",
            Jobs.RDM => "赤魔道士",
            Jobs.PCT => "繪靈法師",
            Jobs.BLU => "青魔道士",
            _ => job.ToString()
        };
    }

    public static JobCategory GetCategory(this Jobs job)
    {
        return job switch
        {
            Jobs.PLD or Jobs.WAR or Jobs.DRK or Jobs.GNB => JobCategory.Tank,
            Jobs.WHM or Jobs.SCH or Jobs.AST or Jobs.SGE => JobCategory.Healer,
            Jobs.MNK or Jobs.DRG or Jobs.NIN or Jobs.SAM or Jobs.RPR or Jobs.VPR => JobCategory.Melee,
            Jobs.BRD or Jobs.MCH or Jobs.DNC => JobCategory.PhysicalRanged,
            Jobs.BLM or Jobs.SMN or Jobs.RDM or Jobs.PCT or Jobs.BLU => JobCategory.MagicalRanged,
            _ => JobCategory.None
        };
    }

    public static bool FilterBy(JobCategory filter, Jobs job)
    {
        return (filter & GetCategory(job)) != 0;
    }

    public static JobCategory Toggle(JobCategory filter, JobCategory category)
    {
        if ((filter & category) != 0)
            return filter & ~category; // remove
        return filter | category;      // add
    }
}
