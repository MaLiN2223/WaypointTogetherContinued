
namespace WaypointTogetherContinued
{
    using HarmonyLib;
    using Vintagestory.API.Client;
    using Vintagestory.API.Config;
    using Vintagestory.API.MathTools;
    using Vintagestory.GameContent;

    [HarmonyPatch(typeof(GuiDialogAddWayPoint), "onSave")]
    class Patch_GuiDialogAddWayPoint_onSave
    {
        public static void Postfix(GuiDialogAddWayPoint __instance)
        {
            var instance = Traverse.Create(__instance);
            ICoreClientAPI capi = instance.Field("capi").GetValue<ICoreClientAPI>();
            GuiComposer guiComposer = __instance.SingleComposer;
            if (capi != null && guiComposer.GetSwitch(Settings.ShouldShareSwitchName).On)
            {
                Core mod = capi.ModLoader.GetModSystem<Core>();
                GuiElementTextInput nameInput = guiComposer.GetTextInput("nameInput");
                if (nameInput == null)
                {
                    capi.ShowChatMessage("Failed to parse nameInput");
                    return;
                }
                string icon = (string)instance.Field("curIcon").GetValue();
                int color = ColorUtil.Hex2Int((string)instance.Field("curColor").GetValue());
                Vec3d pos = (Vec3d)instance.Field("WorldPos").GetValue();

                string title = nameInput.GetText();

                var currentWaypoint = new Waypoint()
                {
                    Color = color,
                    Icon = icon,
                    Pinned = false,
                    Title = title,
                    Position = pos,
                    OwningPlayerUid = capi.World.Player.PlayerUID
                };
                mod.client.network.ShareWaypoint(currentWaypoint);
                string messageToTheUser = Lang.Get("waypointtogethercontinued:waypoint-shared", currentWaypoint.Title);
                capi.ShowChatMessage(messageToTheUser);
            }
        }
    }
}