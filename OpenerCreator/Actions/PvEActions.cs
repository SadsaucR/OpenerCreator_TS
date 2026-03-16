using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Textures;
using OpenerCreator.Helpers;
using Action = Lumina.Excel.Sheets.Action;

namespace OpenerCreator.Actions;

public class PvEActions : IActionManager
{
    private static PvEActions? SingletonInstance;
    private static readonly object LockObject = new();
    private readonly IEnumerable<Action> PveActions;
    private readonly Dictionary<uint, Action> PveActionDict;

    private PvEActions()
    {
        var pve = Sheets.ActionSheet.Where(IsPvEAction).ToList();
        PveActionDict = pve.ToDictionary(a => a.RowId);
        PveActions = pve;
    }

    public static uint TrueNorthId => 7546;

    public static PvEActions Instance
    {
        get
        {
            if (SingletonInstance == null)
            {
                lock (LockObject)
                {
                    SingletonInstance ??= new PvEActions();
                }
            }

            return SingletonInstance;
        }
    }

    public string GetActionName(int id)
    {
        return id >= 0
                   ? id == IActionManager.CatchAllActionId
                         ? IActionManager.CatchAllActionName
                         : PveActionDict.TryGetValue((uint)id, out var actionRow)
                             ? actionRow.Name.ExtractText()
                             : IActionManager.OldActionName
                   : GroupOfActions.TryGetDefault(id, out var group)
                       ? group.Name
                       : IActionManager.OldActionName;
    }

    public bool SameActionsByName(string name, int aId)
    {
        return GetActionName(aId).Contains(name, StringComparison.CurrentCultureIgnoreCase);
    }

    public bool IsActionOGCD(int id)
    {
        return id >= 0
                   ? id != IActionManager.CatchAllActionId && PveActionDict.TryGetValue(
                                                               (uint)id, out var action)
                                                           && ActionTypesExtension.GetType(action) == ActionTypes.OGCD
                   : GroupOfActions.TryGetDefault(id, out var group)
                     && !group.IsGCD;
    }

    public List<int> ActionsIdList(ActionTypes actionType)
    {
        return PveActions
               .Where(a => ActionTypesExtension.GetType(a) == actionType || actionType == ActionTypes.ANY)
               .Select(a => (int)a.RowId)
               .ToList();
    }

    public Action GetAction(uint id)
    {
        return PveActionDict[id];
    }

    public ushort? GetActionIcon(uint id)
    {
        return id == IActionManager.CatchAllActionId
                   ? IActionManager.GetCatchAllIcon
                   : PveActionDict.TryGetValue(id, out var action)
                       ? action.Icon
                       : null;
    }


    public List<int> GetNonRepeatedActionsByName(string name, Jobs job, ActionTypes actionType)
    {
        return PveActions
               .AsParallel()
               .Where(a =>
               {
                   bool nameMatch = a.Name.ToString().Contains(name, StringComparison.OrdinalIgnoreCase);
                   if (!nameMatch) return false;

                   bool typeMatch = (actionType == ActionTypes.ANY || ActionTypesExtension.GetType(a) == actionType);
                   if (!typeMatch) return false;

                   if (job == Jobs.ANY) return true;

                   var category = a.ClassJobCategory.Value;

                   return JobMatchesCategory(category, job);
               })
               .Select(a => (int)a.RowId)
               .OrderBy(id => id)
               .ToList();
    }

    private bool JobMatchesCategory(Lumina.Excel.Sheets.ClassJobCategory category, Jobs job)
    {
        return (int)job switch
        {
            1 or 19 => category.PLD,
            2 or 20 => category.MNK,
            3 or 21 => category.WAR,
            4 or 22 => category.DRG,
            5 or 23 => category.BRD,
            6 or 24 => category.WHM,
            7 or 25 => category.BLM,
            26 or 27 => category.SMN,
            28 => category.SCH,
            29 or 30 => category.NIN,
            31 => category.MCH,
            32 => category.DRK,
            33 => category.AST,
            34 => category.SAM,
            35 => category.RDM,
            36 => category.BLU,
            37 => category.GNB,
            38 => category.DNC,
            39 => category.RPR,
            40 => category.SGE,
            41 => category.VPR,
            42 => category.PCT,
            _ => false
        };
    }

    public static bool IsPvEAction(Action a)
    {
        return a.ActionCategory.RowId is 2 or 3 or 4        // GCD or Weaponskill or oGCD
               && a is { IsPvP: false, ClassJobLevel: > 0 } // not an old action
               && a.ClassJobCategory.RowId != 0;            // not an old action
    }

    public static ISharedImmediateTexture GetIconTexture(uint id)
    {
        var icon = Instance.GetActionIcon(id)?.ToString("D6");
        if (icon != null)
        {
            var path = $"ui/icon/{icon[0]}{icon[1]}{icon[2]}000/{icon}_hr1.tex";
            return Plugin.TextureProvider.GetFromGame(path);
        }

        return IActionManager.GetUnknownActionTexture;
    }
}
