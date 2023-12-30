using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace TerminalTP
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private const string GUID = "me.kirbyquerby.lethalcompany.terminaltp";

        private const string NAME = "Terminal TP";

        private const string VERSION = "0.0.1";

        private readonly Harmony harmony = new Harmony("me.kirbyquerby.lethalcompany.terminaltp");

        private void Awake()
        {
            harmony.PatchAll();
        }
    }
}
namespace TerminalTP.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ParsePlayerSentence")]
        private static void HandleTP(ref Terminal __instance, ref TerminalNode __result)
        {
            string text = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            if (!(text.ToLower() == "tp"))
            {
                return;
            }

            ShipTeleporter[] teleporters = Object.FindObjectsOfType<ShipTeleporter>();
            ShipTeleporter foundTeleporter = null;

            foreach (ShipTeleporter t in teleporters)
            {
                if (!t.isInverseTeleporter)
                {
                    foundTeleporter = t;
                    break;
                }
            }

            __result = ScriptableObject.CreateInstance<TerminalNode>();
            if (foundTeleporter == null)
            {
                __result.displayText = "No teleporter found.";
                return;
            }
            
            if(foundTeleporter.buttonTrigger.interactable)
            {
                __result.displayText = "Teleporting...";
                foundTeleporter.PressTeleportButtonOnLocalClient();
                return;
            }

            __result.displayText = foundTeleporter.buttonTrigger.disabledHoverTip;
        }
    }
}
