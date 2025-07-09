using System.Collections.Generic;
using GloryofGuardian.Common;
using Terraria.ID;
using Terraria.Utilities;

namespace GloryofGuardian.Content.Class {
    public class GOGGlobalItem : GlobalItem {
        public override bool InstancePerEntity => true;

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
                int prefix = GOGUtils.RandomGuardianPrefix();
                bool keepPrefix = !GOGUtils.NegativeGuardianPrefix(prefix) || Main.rand.NextBool(3);
                return keepPrefix ? prefix : 0;
            }

            if (item.CountsAsClass<GuardianDamageClass>()) return GOGUtils.GetReworkedReforge(item, rand, storedPrefix);
            else return -1;
        }

        #endregion

        public override void SetStaticDefaults() {
            //ItemID.Sets.ShimmerTransformToItem[ItemID.EmpressBlade] = ModContent.ItemType<SwordNurturingGourdCalling>();
            base.SetStaticDefaults();
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            if (item.DamageType == GuardianDamageClass.Instance) {
                // 遍历所有TooltipLine
                // 但是遍历变量不能拿来修改
                for (int i = 0; i < tooltips.Count; i++) {
                    TooltipLine line = tooltips[i];

                    if (line.Name != "CritChance") {
                        continue;
                    }

                    // 获取暴击率的文本（例如："暴击率: 13%"）
                    string critText = line.Text;

                    // 从文本中提取暴击率数值
                    if (TryExtractCritChance(critText, out int critChance)) {
                        // 替换“暴击率”为“过载频率”，并保留数值
                        line.Text = $"{critChance}% 超频率";
                    }
                    else {
                        // 如果提取失败，保留原始文本
                        line.Text = critText;
                    }
                }
            }
        }

        //原数值获取
        private bool TryExtractCritChance(string text, out int critChance) {
            var match = System.Text.RegularExpressions.Regex.Match(text, @"\d+");
            if (match.Success && int.TryParse(match.Value, out critChance)) {
                return true;
            }

            critChance = 0;
            return false;
        }
    }
}