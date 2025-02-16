using HarmonyLib;

using System.Collections.Generic;

using Vintagestory.API.Client;
using Vintagestory.GameContent;
namespace WaypointTogetherContinued.Patches;

[HarmonyPatch(typeof(GuiDialogEditWayPoint), "ComposeDialog")]
public static class Patch_EditWayPointShareSwitch_ComposeDialog
{
    public static GuiComposer AddShareComponent(GuiComposer composer, ref ElementBounds textBounds, ref ElementBounds toggleBounds)
    {
        return WayPointShareSwitch.AddShareComponent(composer, ref textBounds, ref toggleBounds);
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return WayPointShareSwitch.Transpiler(instructions);
    }
}
