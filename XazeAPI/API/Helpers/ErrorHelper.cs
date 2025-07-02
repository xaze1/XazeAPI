using Mirror;
using System;
using System.Linq;
using System.Text;
using LabApi.Loader.Features.Plugins;
using RueI.Extensions.HintBuilding;
using XazeAPI.API.Stats;

namespace XazeAPI.API.Helpers
{
    public class ErrorHelper
    {
        public static void ErrorLogStyling(Exception exception = null, string Text = "", Action handleError = null)
        {
            Logging.Error("\t\tTriggered Error");
            Logging.Error(Text);
            Logging.Error("-----------------------------------------------");
            Logging.Error($"Exception: {(exception != null ? exception.GetType() : "No Exception Given")}");
            Logging.Error($"Error: {(exception != null ? exception.Message : "No Exception Given")}");
            Logging.Error($"Source: {(exception != null ? exception.Source : "No Exception Given")}");
            Logging.Error($"TargetSite: {(exception != null ? exception.TargetSite : "No Exception Given")}");
            if (exception != null)
            {
                PluginStatistics.ExceptionCaught(false);
                foreach (var param in exception.TargetSite.GetParameters())
                {
                    Logging.Error($"Failed Parameters: {param}");
                }
            }
            Logging.Error($"-----------------------------------------------");
            Logging.Error(exception != null ? String.Format("{0}", exception) : "No Exception Given");

            if (NetworkServer.active && exception != null)
            {
                var sb = new StringBuilder()
                    .SetAlignment(HintBuilding.AlignStyle.Center)
                    .SetColor(System.Drawing.Color.Red)
                    .AppendLine("EXCEPTION CAUGHT")
                    .CloseColor()
                    .CloseAlign();

                if (Text != null && Text.Any())
                {
                    sb.AppendLine("Message: " + Text);
                }

                sb.AppendLine("Exception: " + exception.GetType().Name)
                    .AppendLine("Error: " + exception.Message)
                    .AppendLine("Source: " + exception.Source)
                    .AppendLine("TargetSite: " + exception.TargetSite)
                    .AppendLine("StackTrace:")
                    .SetSize(65, RueI.Parsing.Enums.MeasurementUnit.Percentage)
                    .AppendLine(exception.StackTrace);

                ServerRolesHelper.SendAdminChatMessage(sb.ToString(), "Plugin Exception caught");
            }

            if (handleError != null)
                handleError();
        }
    }
}
