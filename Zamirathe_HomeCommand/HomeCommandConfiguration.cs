using Rocket.RocketAPI;

namespace Zamirathe_HomeCommand
{
    public class HomeCommandConfiguration : RocketConfiguration
    {
        public bool Enabled;
        public string DisabledMsg;
        public string NoBedMsg;
        public string NoVehicleMsg;
        public bool TeleportWait;
        public byte TeleportWaitTime;
        public string TeleportMsg;
        public string FoundBedNowWaitMsg;
        public bool MovementRestriction;
        public string FoundBedWaitNoMoveMsg;
        public string UnableMoveSinceMoveMsg;
        public string NoTeleportDiedMsg;

        public RocketConfiguration DefaultConfiguration
        {
            get
            {
                return new HomeCommandConfiguration()
                {
                    Enabled = true,
                    DisabledMsg = "I'm sorry {0}, but the home command has been disabled.",
                    NoBedMsg = "I'm sorry {0}, but I could not find your bed.",
                    NoVehicleMsg = "I'm sorry {0}, but you can't be teleported while inside a vehicle.",
                    TeleportWait = false,
                    TeleportWaitTime = 5,
                    TeleportMsg = "You were sent back to your bed.",
                    FoundBedNowWaitMsg = "I have located your bed {0}, please wait for {1} seconds to be teleported.",
                    MovementRestriction = false,
                    FoundBedWaitNoMoveMsg = "I have located your bed {0}, now don't move for {1} seconds while I prepare you for teleport.",
                    UnableMoveSinceMoveMsg = "I'm sorry {0}, but you moved.  I am unable to teleport you.",
                    NoTeleportDiedMsg = "Sorry {0}, unable to finish home teleport as you died."
                };
            }
        }
    }
}