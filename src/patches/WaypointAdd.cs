using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;
using GuiComposerHelpers = Vintagestory.API.Client.GuiComposerHelpers;



public static class ClientWaypointManager
{
    static BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    private static string shouldShareSwitchName = "shouldShareButton";

    public static void PatchAll(Harmony harmony, ICoreAPI api)
    {
        api.Logger.Notification("Applying Harmony patches...");
        WaypointLogic.PatchHarmony(harmony);
        api.Logger.Notification("Applying Harmony patches... OK");
    }

    public static class WaypointLogic
    {
        public static void PatchHarmony(Harmony harmony)
        {
            harmony.Patch(typeof(GuiDialogAddWayPoint).GetMethod("onSave", flags),
                postfix: typeof(WaypointLogic).GetMethod(nameof(PostOnAddSave)));
            harmony.Patch(typeof(GuiDialogEditWayPoint).GetMethod("onSave", flags),
                postfix: typeof(WaypointLogic).GetMethod(nameof(PostOnAddSave)));
        }

        public static void PostOnAddSave(GuiDialogAddWayPoint __instance, ref ICoreClientAPI ___capi)
        {
            if (__instance.SingleComposer.GetSwitch(shouldShareSwitchName).Enabled)
            {
                string curName = __instance.SingleComposer.GetTextInput("nameInput").GetText();
                string message = Lang.Get("waypointtogethercontinued:waypoint-shared");
                ___capi.ShowChatMessage(message);
                WaypointTogetherContinued.Core mod = ___capi.ModLoader.GetModSystem<WaypointTogetherContinued.Core>();
                mod.client.network.ShareWaypoint(curName);
            }
        }
    }


    public static class WaypointShareSwitchPatch
    {
        static void OnShareSwitch(bool on) { }
        public static GuiComposer AddShareComponent(GuiComposer composer, ref ElementBounds textBounds, ref ElementBounds toggleBounds)
        {
            if (GuiComposerHelpers.GetSwitch(composer, shouldShareSwitchName) == null)
            {
                string shareString = Lang.Get("waypointtogethercontinued:share");
                composer = composer.AddStaticText(shareString, CairoFont.WhiteSmallText(), textBounds = textBounds.BelowCopy(0, 9, 0, 0));

                return GuiComposerHelpers.AddSwitch(composer, OnShareSwitch, toggleBounds = toggleBounds.BelowCopy(0, 5, 0, 0).WithFixedWidth(200), shouldShareSwitchName);
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
