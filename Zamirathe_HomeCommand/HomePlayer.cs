using Rocket.Components;
using Rocket.RocketAPI;
using SDG;
using Steamworks;
using UnityEngine;
using System;

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
            this.waittime = HomeCommand.Instance.Configuration.TeleportWaitTime;
            this.movementrestricted = HomeCommand.Instance.Configuration.MovementRestriction;
            this.p = player;
            this.bedPos = Vector3.up + bedPos;
            this.bedRot = bedRot;

            if (this.waitrestricted)
            {
                // We have to wait to teleport.
                this.LastCalledHomeCommand = DateTime.Now;
                if (this.movementrestricted)
                {
                    this.LastCalledHomePos = this.transform.position;
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
