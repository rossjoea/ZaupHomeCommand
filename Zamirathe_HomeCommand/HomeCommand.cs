using System;
using System.Collections.Generic;
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
        public Dictionary<string, byte> WaitGroups = new Dictionary<string, byte>();
        public static HomeCommand Instance;

        protected override void Load()
        {
            HomeCommand.Instance = this;
            if (Loaded)
            {
                foreach (HomeGroup hg in this.Configuration.WaitGroups)
                {
                    WaitGroups.Add(hg.Id, hg.Wait);
                }
            }
        }
        // All we are doing here is checking the config to see if anything like restricted movement or time restriction is enforced.
        public static object[] CheckConfig(RocketPlayer player)
        {
            
            object[] returnv = { false, null, null };
            // First check if command is enabled.
            if (!HomeCommand.Instance.Configuration.Enabled)
            {
                // Command disabled.
                RocketChatManager.Say(player, String.Format(HomeCommand.Instance.Configuration.DisabledMsg, player.CharacterName));
                return returnv;
            }
            // It is enabled, but are they in a vehicle?
            if (player.Stance == EPlayerStance.DRIVING || player.Stance == EPlayerStance.SITTING)
            {
                // They are in a vehicle.
                RocketChatManager.Say(player, String.Format(HomeCommand.Instance.Configuration.NoVehicleMsg, player.CharacterName));
                return returnv;
            }
            // They aren't in a vehicle, so check if they have a bed.    
            Vector3 bedPos;
            byte bedRot;
            if (!BarricadeManager.tryGetBed(player.CSteamID, out bedPos, out bedRot))
            {
                // Bed not found.
                RocketChatManager.Say(player, String.Format(HomeCommand.Instance.Configuration.NoBedMsg, player.CharacterName));
                return returnv;
            }
            object[] returnv2 = { true, bedPos, bedRot };
            return returnv2;
        }
    }
}