using Rocket.Components;
using Rocket.RocketAPI;
using Rocket.Logging;
using SDG;
using Steamworks;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Zamirathe_HomeCommand
{
    public class HomePlayer : RocketPlayerComponent
    {
        private bool GoingHome;
        private DateTime LastCalledHomeCommand;
        private Vector3 LastCalledHomePos;
        private bool waitrestricted;
        private byte waittime;
        private bool movementrestricted;
        private bool cangohome;
        private Vector3 bedPos;
        private byte bedRot;
        private Player p;

        private void Load()
        {
            this.GoingHome = false;
            this.cangohome = false;
        }
        public void GoHome(Vector3 bedPos, byte bedRot, Player player)
        {
            this.waitrestricted = HomeCommand.Instance.Configuration.TeleportWait;
            this.movementrestricted = HomeCommand.Instance.Configuration.MovementRestriction;
            this.p = player;
            this.bedPos = Vector3.up + bedPos;
            this.bedRot = bedRot;

            if (this.waitrestricted)
            {
                // We have to wait to teleport now find out how long
                this.LastCalledHomeCommand = DateTime.Now;
                if (HomeCommand.Instance.WaitGroups.ContainsKey("all"))
                {
                    HomeCommand.Instance.WaitGroups.TryGetValue("all", out this.waittime);
                }
                else
                {
                    if (player.SteamChannel.SteamPlayer.IsAdmin && HomeCommand.Instance.WaitGroups.ContainsKey("admin"))
                    {
                        HomeCommand.Instance.WaitGroups.TryGetValue("admin", out this.waittime);
                    }
                    else
                    {
                        // Either not an admin or they don't get special wait restrictions.
                        List<Group> hg = player.GetGroups();
                        byte[] time2 = new byte[hg.Count];
                        for (byte g=0;g<hg.Count;g++)
                        {
                            Group hgr = hg[g];
                            HomeCommand.Instance.WaitGroups.TryGetValue(hgr.Id, out time2[g]);
                            if (time2[g] <= 0)
                            {
                                time2[g] = 60;
                            }
                        }
                        Array.Sort(time2);
                        // Take the lowest time.
                        this.waittime = time2[0];
                    }
                }
                if (this.movementrestricted)
                {
                    this.LastCalledHomePos = this.transform.position;
                    RocketChatManager.Say(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(HomeCommand.Instance.Configuration.FoundBedWaitNoMoveMsg, player.name, this.waittime));
                }
                else
                {
                    RocketChatManager.Say(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(HomeCommand.Instance.Configuration.FoundBedNowWaitMsg, player.name, this.waittime));
                }
            }
            else
            {
                this.cangohome = true;
            }
            this.GoingHome = true;
            this.DoGoHome();
        }
        private void DoGoHome()
        {
            if (!this.cangohome) return;
            RocketChatManager.Say(this.p.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(HomeCommand.Instance.Configuration.TeleportMsg, this.p.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName));
            this.p.sendTeleport(this.bedPos, this.bedRot);
            this.cangohome = false;
            this.GoingHome = false;
        }
        public void FixedUpdate()
        {
            if (!this.GoingHome) return;
            if (this.p.PlayerLife.Dead)
            {
                // Abort teleport, they died.
                RocketChatManager.Say(this.p.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(HomeCommand.Instance.Configuration.NoTeleportDiedMsg, this.p.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName));
                this.GoingHome = false;
                this.cangohome = false;
                return;
            }
            if (this.movementrestricted)
            {
                if (Vector3.Distance(this.p.transform.position, this.LastCalledHomePos) > 0.1)
                {
                    // Abort teleport, they moved.
                    RocketChatManager.Say(this.p.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(HomeCommand.Instance.Configuration.UnableMoveSinceMoveMsg, this.p.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName));
                    this.GoingHome = false;
                    this.cangohome = false;
                    return;
                }
            }
            if (this.waitrestricted)
            {
                if ((DateTime.Now - this.LastCalledHomeCommand).TotalSeconds < this.waittime) return;
            }
            // We made it this far, we can go home.
            this.cangohome = true;
            this.DoGoHome();
        }
    }
}
