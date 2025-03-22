using GloryofGuardian.Content.Items.Accessories;
using System.Linq;

namespace GloryofGuardian.Content.Class
{
    public abstract class GOGItem : ModItem
    {
        //���廥����Ʒ��
        //��������
        public static readonly int[] GuardiansBrand = new int[]
        {
            ModContent.ItemType<WildernessSeed>(),
            ModContent.ItemType<BloomingLife>(),
            ModContent.ItemType<HellFlare>(),

            ModContent.ItemType<HolyBond>(),
            ModContent.ItemType<Resolute>(),
            ModContent.ItemType<Syncore>()
        };

        //��������
        public static readonly int[] GuardiansErodedBrand = new int[]
        {
            ModContent.ItemType<Dirge>(),
            ModContent.ItemType<Elegy>(),
            ModContent.ItemType<Candor>(),
        };

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player) {
            // �����ǰ��Ʒ������Ʒ��������������
            if (GuardiansBrand.Contains(equippedItem.type) &&
                GuardiansBrand.Contains(incomingItem.type)) {
                return false; // ��ֹͬʱװ��
            }

            // �����ǰ��Ʒ������Ʒ��������ʴ����
            if (GuardiansErodedBrand.Contains(equippedItem.type) &&
                GuardiansErodedBrand.Contains(incomingItem.type)) {
                return false; // ��ֹͬʱװ��
            }

            return true; // ����ͬʱװ��
        }
    }
}