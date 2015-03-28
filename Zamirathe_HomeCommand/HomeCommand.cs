using System;
using Rocket.RocketAPI;
using SDG;
using UnityEngine;
using Steamworks;

namespace Zamirathe_HomeCommand
{
    public class HomeCommand : RocketPlugin<HomeCommandConfiguration>
    {
        public static bool running;
        public static DateTime start;
        public static HomeCommand Instance;

        protected override void Load()
        {
            HomeCommand.Instance = this;
        }
        // All we are doing here is checking the config to see if anything like restricted movement or time restriction is enforced.
        public static object[] CheckConfig(Player player, CSteamID playerid, HomePlayer homeplayer)
        {
            object[] returnv = { false, null, null };
            // First check if command is enabled.
            if (!HomeCommand.Instance.Configuration.Enabled)
            {
                // Command disabled.
                RocketChatManager.Say(playerid, String.Format(HomeCommand.Instance.Configuration.DisabledMsg, player.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName));
                return returnv;
            }
            // It is enabled, but are they in a vehicle?
            if (player.Stance.Stance == EPlayerStance.DRIVING || player.Stance.Stance == EPlayerStance.SITTING)
            {
                // They are in a vehicle.
                RocketChatManager.Say(playerid, String.Format(HomeCommand.Instance.Configuration.NoVehicleMsg, player.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName));
                return returnv;
            }
            // They aren't in a vehicle, so check if they have a bed.    
            Vector3 bedPos;
            byte bedRot;
            if (!BarricadeManager.tryGetBed(playerid, out bedPos, out bedRot))
            {
                // Bed not found.
                RocketChatManager.Say(playerid, String.Format(HomeCommand.Instance.Configuration.NoBedMsg, player.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName));
                return returnv;
            }
            object[] returnv2 = { true, bedPos, bedRot };
            return returnv2;
        }
    }
}