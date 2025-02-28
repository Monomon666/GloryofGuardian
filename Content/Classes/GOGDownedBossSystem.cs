using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace GloryofGuardian.Content.Classes
{
    public class GOGDownedBossSystem : ModSystem
    {
        // Bosses
        internal static bool _downedNightWatcher = false;

        #region Wrapper Properties for Lantern Nights
        public static bool downedNightWatcher {
            get => _downedNightWatcher;
            set {
                if (!value)
                    _downedNightWatcher = false;
                else
                    NPC.SetEventFlagCleared(ref _downedNightWatcher, -1);
            }
        }
        #endregion

        internal static void ResetAllFlags() {
            downedNightWatcher = false;
        }

        public override void OnWorldLoad() => ResetAllFlags();

        public override void OnWorldUnload() => ResetAllFlags();

        public override void SaveWorldData(TagCompound tag) {
            List<string> downed = new List<string>();

            if (downedNightWatcher)
                downed.Add("downedNightWatcher");

            tag["downedFlags"] = downed;
        }

        public override void LoadWorldData(TagCompound tag) {
            IList<string> downed = tag.GetList<string>("downedFlags");

            downedNightWatcher = downed.Contains("downedNightWatcher");
        }
    }
}
