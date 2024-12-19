namespace WaypointTogetherContinued.src.patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Reflection;
    using System;
    using Vintagestory.API.Client;
    using Vintagestory.API.Config;
    using Vintagestory.GameContent;
    using GuiComposerHelpers = Vintagestory.API.Client.GuiComposerHelpers;

    public static class WaypointShareSwitchPatch
    {
        static void OnShareSwitch(bool on) { }
        public static GuiComposer AddShareComponent(GuiComposer composer, ref ElementBounds textBounds, ref ElementBounds toggleBounds)
        {
            if (GuiComposerHelpers.GetSwitch(composer, Settings.ShouldShareSwitchName) == null)
            {
                string shareString = Lang.Get("waypointtogethercontinued:share");
                composer = composer.AddStaticText(shareString, CairoFont.WhiteSmallText(), textBounds = textBounds.BelowCopy(0, 9, 0, 0));

                return GuiComposerHelpers.AddSwitch(composer, OnShareSwitch, toggleBounds = toggleBounds.BelowCopy(0, 5, 0, 0).WithFixedWidth(200), Settings.ShouldShareSwitchName);
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
                    yield return new CodeInstruction(OpCodes.Call, typeof(AddWaypointShareSwitchPatch).GetMethod("AddShareComponent", BindingFlags.Static | BindingFlags.Public));

                    found = true;
                }

                yield return instruction;
            }

            if (found is false)
            {
                throw new ArgumentException("Cannot find `waypoint-color` in GuiDialogAddWayPoint.ComposeDialog");
            }
        }
    }

    [HarmonyPatch(typeof(GuiDialogAddWayPoint), "ComposeDialog")]
    public static class AddWaypointShareSwitchPatch
    {
        public static GuiComposer AddShareComponent(GuiComposer composer, ref ElementBounds textBounds, ref ElementBounds toggleBounds)
        {
            return WaypointShareSwitchPatch.AddShareComponent(composer, ref textBounds, ref toggleBounds);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return WaypointShareSwitchPatch.Transpiler(instructions);
        }
    }

    [HarmonyPatch(typeof(GuiDialogEditWayPoint), "ComposeDialog")]
    public static class EditWaypointShareSwitchPatch
    {
        public static GuiComposer AddShareComponent(GuiComposer composer, ref ElementBounds textBounds, ref ElementBounds toggleBounds)
        {
            return WaypointShareSwitchPatch.AddShareComponent(composer, ref textBounds, ref toggleBounds);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return WaypointShareSwitchPatch.Transpiler(instructions);
        }
    }
}
