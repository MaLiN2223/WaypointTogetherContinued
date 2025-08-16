namespace WaypointTogetherContinued
{
    using System;
    using System.Text.RegularExpressions;
    using Vintagestory.API.Config;
    using Vintagestory.API.MathTools;
    using Vintagestory.GameContent;

    internal static class WaypointCommon
    {
        //string.Format(
        //"/waypoint addati {0} ={1} ={2} ={3} {4} {5} {6}",
        //curIcon,
        //WorldPos.X.ToString(GlobalConstants.DefaultCultureInfo),
        //WorldPos.Y.ToString(GlobalConstants.DefaultCultureInfo),
        //WorldPos.Z.ToString(GlobalConstants.DefaultCultureInfo),
        //pinned,
        //curColor,
        //name)
        static readonly Regex addWaypointMessageRegex = new(@"/waypoint addati (.*) =(\d+\.\d+?) =(\d+\.\d+?) =(\d+\.\d+?) (.*) (.*) (.*)");



        public static Waypoint? ParseAddMessage(String message)
        {
            Match match = addWaypointMessageRegex.Match(message);
            if (match.Success)
            {
                Vec3d position = new Vec3d(
                    double.Parse(match.Groups[2].Value, GlobalConstants.DefaultCultureInfo),
                    double.Parse(match.Groups[3].Value, GlobalConstants.DefaultCultureInfo),
                    double.Parse(match.Groups[4].Value, GlobalConstants.DefaultCultureInfo)
                );

                int color = ColorUtil.Hex2Int(match.Groups[6].Value);
                string Name = match.Groups[7].Value;

                return new Waypoint
                {
                    Icon = match.Groups[1].Value,
                    Position = position,
                    Pinned = bool.Parse(match.Groups[5].Value),
                    Color = color,
                    Title = Name
                };
            }
            return null;
        }
    }
}
