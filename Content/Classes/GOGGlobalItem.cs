using GloryofGuardian.Common;
using System.Collections.Generic;
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

            if (item.CountsAsClass<GuardianDamageClass>()) return GOGUtils.GetReworkedReforge(item, rand, storedPrefix);
            else return -1;
        }

        #endregion

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
                        line.Text = $"{critChance}% 过载频率";
                    } else {
                        // 如果提取失败，保留原始文本
                        line.Text = critText;
                    }
                }

                // 遍历所有TooltipLine
                // 但是遍历变量不能拿来修改
                for (int i = 0; i < tooltips.Count; i++) {
                    TooltipLine line = tooltips[i];

                    if (line.Name != "Damage") {
                        continue;
                    }

                    // 获取暴击率的文本（例如："暴击率: 13%"）
                    string DamageText = line.Text;

                    // 从文本中提取暴击率数值
                    if (TryExtractCritChance(DamageText, out int Damage)) {
                        // 替换“暴击率”为“过载频率”，并保留数值
                        line.Text = $"{Damage}% 戍卫能效";
                    } else {
                        // 如果提取失败，保留原始文本
                        line.Text = DamageText;
                    }
                }
            }
        }

        private bool TryExtractCritChance(string text, out int critChance) {
            // 使用正则表达式提取数值
            var match = System.Text.RegularExpressions.Regex.Match(text, @"\d+");
            if (match.Success && int.TryParse(match.Value, out critChance)) {
                return true;
            }

            // 如果提取失败，返回 false
            critChance = 0;
            return false;
        }
    }
}