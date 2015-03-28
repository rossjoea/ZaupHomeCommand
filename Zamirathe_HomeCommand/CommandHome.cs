using Rocket.RocketAPI;
using SDG;
using Steamworks;
using UnityEngine;
using System;

namespace Zamirathe_HomeCommand
{
    public class CommandHome : IRocketCommand
    {
        public bool RunFromConsole
        {
            get
            {
                return false;
            }
        }
        public string Name
        {
            get
            {
                return "home";
            }
        }
        public string Help
        {
            get
            {
                return "Teleports you to your bed if you have one.";
            }
        }
        public void Execute(CSteamID playerid, string bed)
        {
            Player player = PlayerTool.getPlayer(playerid);
            HomePlayer homeplayer = player.transform.GetComponent<HomePlayer>();
            object[] cont = HomeCommand.CheckConfig(player, playerid, homeplayer);
            if (!(bool)cont[0]) return;
            // A bed was found, so let's run a few checks.
            if (HomeCommand.Instance.Configuration.TeleportWait)
            {
                // Admin want the player to have to wait before they can teleport.  Now do they have to stay still too?
                if (HomeCommand.Instance.Configuration.MovementRestriction)
                {
                    // Admin wants them not to move either.  So let's send the appropriate msg and start the timer.
                    RocketChatManager.Say(playerid, String.Format(HomeCommand.Instance.Configuration.FoundBedWaitNoMoveMsg, player.name, HomeCommand.Instance.Configuration.TeleportWaitTime));
                }
                else
                {
                    // Admin just wants them to wait but they can move.
                    RocketChatManager.Say(playerid, String.Format(HomeCommand.Instance.Configuration.FoundBedNowWaitMsg, player.name, HomeCommand.Instance.Configuration.TeleportWaitTime));
                }
            }
            homeplayer.GoHome((Vector3)cont[1], (byte)cont[2], player);
        }
    }
}
