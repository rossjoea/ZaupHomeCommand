using System;
using Rocket.RocketAPI;
using SDG;
using UnityEngine;

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
        public static object[] CheckConfig(SteamPlayer player, HomePlayer homeplayer)
        {
            object[] returnv = { false, null, null };
            // First check if command is enabled.
            if (!HomeCommand.Instance.Configuration.Enabled)
            {
                // Command disabled.
                RocketChatManager.Say(player.SteamPlayerID.CSteamID, String.Format(HomeCommand.Instance.Configuration.DisabledMsg, player.SteamPlayerID.CharacterName));
                return returnv;
            }
            // It is enabled, but are they in a vehicle?
            if (player.Player.Stance.Stance == EPlayerStance.DRIVING || player.Player.Stance.Stance == EPlayerStance.SITTING)
            {
                // They are in a vehicle.
                RocketChatManager.Say(player.SteamPlayerID.CSteamID, String.Format(HomeCommand.Instance.Configuration.NoVehicleMsg, player.SteamPlayerID.CharacterName));
                return returnv;
            }
            // They aren't in a vehicle, so check if they have a bed.    
            Vector3 bedPos;
            byte bedRot;
            if (!BarricadeManager.tryGetBed(player.SteamPlayerID.CSteamID, out bedPos, out bedRot))
            {
                // Bed not found.
                RocketChatManager.Say(player.SteamPlayerID.CSteamID, String.Format(HomeCommand.Instance.Configuration.NoBedMsg, player.SteamPlayerID.CharacterName));
                return returnv;
            }
            object[] returnv2 = { true, bedPos, bedRot };
            return returnv2;
        }
    }
}