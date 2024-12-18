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
            //harmony.Patch(typeof(GuiDialogAddWayPoint).GetMethod("onSave", flags),
            //    postfix: typeof(WaypointLogic).GetMethod(nameof(PostOnAddSave)));
            //harmony.Patch(typeof(GuiDialogEditWayPoint).GetMethod("onSave", flags),
            //    postfix: typeof(WaypointLogic).GetMethod(nameof(PostOnAddSave)));
        }
    }

    static class BroadcastWaypointPatcher
    {
        public static readonly MethodInfo sendChatMessageMethod = AccessTools.Method(typeof(ICoreClientAPI), nameof(ICoreClientAPI.SendChatMessage), new Type[2] { typeof(string), typeof(string) });
        public static readonly MethodInfo toReplaceWith = AccessTools.Method(typeof(BroadcastWaypointPatcher), nameof(BroadcastWaypoint));
        public static void BroadcastWaypoint(ICoreClientAPI capi, string message, GuiDialogEditWayPoint instance)
        {
            if (capi != null)
            {
                if (instance.SingleComposer.GetSwitch(shouldShareSwitchName).Enabled)
                {
                    WaypointTogetherContinued.Core mod = capi.ModLoader.GetModSystem<WaypointTogetherContinued.Core>();
                    mod.client.network.ShareWaypoint(message, capi.World.Player.PlayerUID);
                    capi.SendChatMessage(message);
                    string messageToTheUser = Lang.Get("waypointtogethercontinued:waypoint-shared");
                    capi.ShowChatMessage(messageToTheUser);
                }
            }
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new();
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == sendChatMessageMethod)
                {
                    list.RemoveAt(list.Count - 1); // remove 'ldnull'
                    list.Add(new CodeInstruction(OpCodes.Ldarg_0)); // load 'this'
                    list.Add(new CodeInstruction(OpCodes.Call, toReplaceWith));
                }
                else
                {
                    list.Add(instruction);
                }
            }
            return list;
        }
    }

    [HarmonyPatch(typeof(GuiDialogEditWayPoint), "onSave")]
    class Patch_GuiDialogEditWayPoint_onSave
    {
        public static readonly MethodInfo sendChatMessageMethod = AccessTools.Method(typeof(ICoreClientAPI), nameof(ICoreClientAPI.SendChatMessage), new Type[2] { typeof(string), typeof(string) });
        public static readonly MethodInfo toReplaceWith = AccessTools.Method(typeof(Patch_GuiDialogEditWayPoint_onSave), nameof(BroadcastWaypoint));
        public static void BroadcastWaypoint(ICoreClientAPI capi, string message, GuiDialogEditWayPoint instance)
        {
            if (capi != null)
            {
                if (instance.SingleComposer.GetSwitch(shouldShareSwitchName).Enabled)
                {
                    WaypointTogetherContinued.Core mod = capi.ModLoader.GetModSystem<WaypointTogetherContinued.Core>();

                    var maplayers = capi.ModLoader.GetModSystem<WorldMapManager>().MapLayers;
                    var mapLayer = (maplayers.Find(x => x is WaypointMapLayer) as WaypointMapLayer);
                    var waypoint = Traverse.Create(instance).Field("waypoint").GetValue<Waypoint>();
                    mod.client.network.ShareWaypoint(message, waypoint.Guid);
                    capi.SendChatMessage(message);
                    string messageToTheUser = Lang.Get("waypointtogethercontinued:waypoint-shared");
                    capi.ShowChatMessage(messageToTheUser);
                }
            }
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new();
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == sendChatMessageMethod)
                {
                    list.RemoveAt(list.Count - 1); // remove 'ldnull'
                    list.Add(new CodeInstruction(OpCodes.Ldarg_0)); // load 'this'
                    list.Add(new CodeInstruction(OpCodes.Call, toReplaceWith));
                }
                else
                {
                    list.Add(instruction);
                }
            }
            return list;
        }
    }
    [HarmonyPatch(typeof(GuiDialogAddWayPoint), "onSave")]
    class Patch_GuiDialogAddWayPoint_onSave
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return BroadcastWaypointPatcher.Transpiler(instructions);
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
