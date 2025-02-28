using GloryofGuardian.Common;
using Terraria.Utilities;

namespace GloryofGuardian.Content.Class
{
    public class GOGGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        // TODO //ûŪ����,�ȷ������
        internal float GuardianStrikePrefixBonus;

        #region Reforge Mechanic Rework
        private static int storedPrefix = -1;
        public override void PreReforge(Item item) {
            storedPrefix = item.prefix;
        }

        // ����������������
        // ʹ��config�ܾ�����
        public override int ChoosePrefix(Item item, UnifiedRandom rand) {
            if (storedPrefix == -1 && item.CountsAsClass<GuardianDamageClass>() && (item.maxStack == 1 || item.AllowReforgeForStackableItem)) {
                // ԭ���趨���ã�����ʱ��75%���ʸ�������
                // �������η�������֮���ĸ��ʱ���Ч��, ���ֵ����η���ԭ��������
                int prefix = GOGUtils.RandomGuardianPrefix();
                bool keepPrefix = !GOGUtils.NegativeGuardianPrefix(prefix) || Main.rand.NextBool(3);
                return keepPrefix ? prefix : 0;
            }

            return GOGUtils.GetReworkedReforge(item, rand, storedPrefix);
        }

        #endregion
    }
}