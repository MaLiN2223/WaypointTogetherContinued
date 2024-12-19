namespace WaypointTogetherContinued
{
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Reflection;
    using Vintagestory.GameContent;
    using Vintagestory.API.Client;
    using Vintagestory.API.Config;

    [HarmonyPatch(typeof(GuiDialogEditWayPoint), "onSave")]
    class Patch_GuiDialogEditWayPoint_onSave
    {

        public static readonly MethodInfo toReplaceWith = AccessTools.Method(typeof(Patch_GuiDialogEditWayPoint_onSave), nameof(BroadcastWaypoint));
        public static void BroadcastWaypoint(ICoreClientAPI capi, string message, GuiDialogEditWayPoint instance)
        {
            if (capi != null)
            {
                if (instance.SingleComposer.GetSwitch(Settings.ShouldShareSwitchName).On)
                {
                    Core mod = capi.ModLoader.GetModSystem<Core>();
                    var maplayers = capi.ModLoader.GetModSystem<WorldMapManager>().MapLayers;
                    var mapLayer = (maplayers.Find(x => x is WaypointMapLayer) as WaypointMapLayer);
                    var waypoint = Traverse.Create(instance).Field("waypoint").GetValue<Waypoint>();
                    mod.client.network.ShareWaypoint(message, waypoint.Guid);
                    string messageToTheUser = Lang.Get("waypointtogethercontinued:waypoint-shared");
                    capi.ShowChatMessage(messageToTheUser);
                }
                capi.SendChatMessage(message);
            }
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new();
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == Settings.SendChatMessageMethod)
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
}
