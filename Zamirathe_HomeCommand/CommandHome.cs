using Rocket.RocketAPI;
using SDG;
using Steamworks;
using UnityEngine;
using System;

namespace Zamirathe_HomeCommand
{
    public class CommandHome : Command
    {
        public CommandHome()
        {
            this.commandName = "home";
            this.commandHelp = "Teleports you to your bed if you have one.";
            this.commandInfo = this.commandName + " - " + this.commandHelp;
        }

        protected override void execute(SteamPlayerID playerid, string bed)
        {
            if (!RocketCommand.IsPlayer(playerid)) return;
            SteamPlayer player = PlayerTool.getSteamPlayer(playerid.CSteamID);
            HomePlayer homeplayer = player.Player.transform.GetComponent<HomePlayer>();
            object[] cont = HomeCommand.CheckConfig(player, homeplayer);
            if (!(bool)cont[0]) return;
            // A bed was found, so let's run a few checks.
            if (HomeCommand.Instance.Configuration.TeleportWait)
            {
                // Admin want the player to have to wait before they can teleport.  Now do they have to stay still too?
                if (HomeCommand.Instance.Configuration.MovementRestriction)
                {
                    // Admin wants them not to move either.  So let's send the appropriate msg and start the timer.
                    RocketChatManager.Say(player.SteamPlayerID.CSteamID, String.Format(HomeCommand.Instance.Configuration.FoundBedWaitNoMoveMsg, player.Player.name, HomeCommand.Instance.Configuration.TeleportWaitTime));
                }
                else
                {
                    // Admin just wants them to wait but they can move.
                    RocketChatManager.Say(player.SteamPlayerID.CSteamID, String.Format(HomeCommand.Instance.Configuration.FoundBedNowWaitMsg, player.Player.name, HomeCommand.Instance.Configuration.TeleportWaitTime));
                }
            }
            homeplayer.GoHome((Vector3)cont[1], (byte)cont[2], player);
        }
    }
}
