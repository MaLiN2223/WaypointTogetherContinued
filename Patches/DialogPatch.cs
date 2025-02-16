using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
namespace WaypointTogetherContinued.Patches;
public static class WayPointShareSwitch
{
    public static readonly MethodInfo AddShareComponentMethod = AccessTools.Method(typeof(WayPointShareSwitch), nameof(AddShareComponent));
    public static GuiComposer AddShareComponent(GuiComposer composer, ref ElementBounds textBounds, ref ElementBounds toggleBounds)
    {
        if (composer.GetSwitch(Settings.ShouldShareSwitchName) is null)
        {
            string shareString = Lang.Get("waypointtogethercontinued:share");
            composer = composer.AddStaticText(shareString, CairoFont.WhiteSmallText(), textBounds = textBounds.BelowCopy(0, 9, 0, 0));

            GuiComposer c = composer.AddSwitch((bool _) => { }, toggleBounds = toggleBounds.BelowCopy(0, 5, 0, 0).WithFixedWidth(200), Settings.ShouldShareSwitchName);
            var sw = composer.GetSwitch(Settings.ShouldShareSwitchName);
            sw.On = ModConfig.ClientConfig?.DefaultSharing ?? false;
            return c;
        }

        return composer;
    }
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = false;

        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "waypoint-color")
            {
                yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                yield return new CodeInstruction(OpCodes.Ldloca_S, 1);
                yield return new CodeInstruction(OpCodes.Call, AddShareComponentMethod);

                found = true;
            }

            yield return instruction;
        }

        if (!found)
        {
            throw new ArgumentException("Cannot find `waypoint-color` in GuiDialogAddWayPoint.ComposeDialog");
        }
    }
}
