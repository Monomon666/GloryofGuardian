using GloryofGuardian.Common;
using Terraria.Utilities;

namespace GloryofGuardian.Content.Class
{
    public class GOGGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        // TODO //没弄明白,先放在这儿
        internal float GuardianStrikePrefixBonus;

        #region Reforge Mechanic Rework
        private static int storedPrefix = -1;
        public override void PreReforge(Item item) {
            storedPrefix = item.prefix;
        }

        // 对重铸词条的重做
        // 使用config拒绝重铸
        public override int ChoosePrefix(Item item, UnifiedRandom rand) {
            if (storedPrefix == -1 && item.CountsAsClass<GuardianDamageClass>() && (item.maxStack == 1 || item.AllowReforgeForStackableItem)) {
                // 原版设定沿用：制作时有75%概率附带词条
                // 负面修饰符有三分之二的概率被无效化, 热闹的修饰符被原版故意忽略
                int prefix = GOGUtils.RandomGuardianPrefix();
                bool keepPrefix = !GOGUtils.NegativeGuardianPrefix(prefix) || Main.rand.NextBool(3);
                return keepPrefix ? prefix : 0;
            }

            return GOGUtils.GetReworkedReforge(item, rand, storedPrefix);
        }

        #endregion
    }
}