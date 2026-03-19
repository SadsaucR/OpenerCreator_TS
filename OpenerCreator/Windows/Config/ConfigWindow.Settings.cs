using System;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace OpenerCreator.Windows.Config;

public partial class ConfigWindow
{
    private void Settings()
    {
        using var tabItem = ImRaii.TabItem("設定");
        if (!tabItem.Success)
            return;

        var changed = false;

        if (ImGui.CollapsingHeader("倒數設定", ImGuiTreeNodeFlags.DefaultOpen))
        {
            changed |= ImGui.Checkbox("啟用倒數", ref Plugin.Config.IsCountdownEnabled);
            if (ImGui.InputInt("設定秒數", ref Plugin.Config.CountdownTime))
            {
                Plugin.Config.CountdownTime = Math.Clamp(Plugin.Config.CountdownTime, 0, 30);
                changed = true;
            }
        }

        ImGui.Spacing();

        if (ImGui.CollapsingHeader("循環錄製設定", ImGuiTreeNodeFlags.DefaultOpen))
        {
            changed |= ImGui.Checkbox("發生錯誤時停止錄製", ref Plugin.Config.StopAtFirstMistake);
            changed |= ImGui.Checkbox("若循環中不包含「真北」，則自動忽略", ref Plugin.Config.IgnoreTrueNorth);
            changed |= ImGui.Checkbox("對下一個技能啟用虛線提示", ref Plugin.Config.AbilityAnts);
        }

        if (changed)
            Plugin.Config.Save();
    }
}
