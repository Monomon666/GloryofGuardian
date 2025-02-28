using DreamJourney.Content.Items.Accessories;
using GloryofGuardian.Content.Buffs;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Class
{
    public class GOGModPlayer : ModPlayer
    {
        //Դ��ȡ
        public IEntitySource Source;

        //�Ҿ�����������Ҫʹ�õ�����ͨ�������ͳ�ﱣ��
        //������������ֽ׶ε�����˵�ܹ���Լ���װ���ȶԱ�����Ӱ��
        //��������ӵĶ�ȡ

        //����
        #region �����洢

        /// <summary>
        /// ��������
        /// </summary>
        public int Gslot = 0;

        /// <summary>
        /// ��������2
        /// </summary>
        public int Gslot2 = 0;

        /// <summary>
        /// �������������������
        /// </summary>
        public float GcountR = 1;

        /// <summary>
        /// �������������������
        /// </summary>
        public int GcountEx = 0;

        /// <summary>
        /// Ԥ������
        /// </summary>
        public int Todo;

        #endregion

        #region �䶯����1

        /// <summary>
        /// ������Ʒ�¶�����
        /// </summary>
        public bool residuallonelinessechoes = false;

        #endregion

        #region �䶯����2

        /// <summary>
        /// �ѽ�����
        /// </summary>
        public bool TitaniumShield = false;

        #endregion

        //player����
        #region ���б䶯

        //ȥ��ԭ�г�ʼ��Ʒ
        public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath) {
            //����ģʽ����ҳ�ʼװ������
            //Todo
            //������ģʽ����ҳ�ʼװ������
            //itemsByMod["Terraria"].RemoveAll(item => item.type == ItemID.CopperShortsword);
            //itemsByMod["Terraria"].RemoveAll(item => item.type == ItemID.CopperAxe);

            //itemsByMod["Terraria"].RemoveAll(item => item.type == ItemID.CopperPickaxe);
        }

        //��������µĳ�ʼ��Ʒ���Լ��к���Ҹ����������Ʒ
        //public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath) {
        //    if (mediumCoreDeath) {
        //        return Enumerable.Empty<Item>();
        //    } else {
        //        return new[] {
        //        new Item(ItemID.LifeCrystal,5),
        //        new Item(ItemID.ManaCrystal,4),
        //        new Item(ModContent.ItemType<WildCalling>(),1),//���ָ���Ұ������
        //        };
        //    }
        //}

        //Todo

        #endregion

        #region ������Ч

        //���ей�ʱ���������Դ���ĳЩ���ƣ������޸ĶԵз���λ��ɵ��κ��˺�
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
            //ְҵ���óͷ�
            //������̨������Ʒʱ�Լ��ٻ�����̨�ŻṤ��
            //Todo
            //����modifiers.SourceDamage,�����GcountΪ9999
        }

        //��ս�������ей�ʱ���������Դ���ĳЩ���ƣ������޸ĶԵз���λ��ɵ��˺������Ǹ�mod����ʲ�û�н�ս������
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers) {
        }

        //��Ļ���ей�ʱ���������Դ���ĳЩ���ƣ������޸ĶԵз���λ��ɵ��˺�
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers) {
            //ָ��õ��˵ĵ�λ����
            Vector2 totarget = (target.Center - Player.Center).SafeNormalize(Vector2.Zero);
            base.ModifyHitNPCWithProj(proj, target, ref modifiers);
        }

        #endregion

        #region �ܻ���Ч

        //�ܵ��˺�ʱ����
        public override void ModifyHurt(ref Player.HurtModifiers modifiers) {
            
        }
        //�ܵ��Ӵ��˺�ʱ����
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers) {

        }
        //�ܵ���Ļ�˺�ʱ����
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers) {

        }

        #endregion

        #region ���˷���

        public override bool FreeDodge(Player.HurtInfo info)//��������ǰ������,���ȴ���
        {
            //Player.immune = true;//�����޵�֡
            //Player.SetImmuneTimeForAllTypes(80);//�����޵�֡����
            //return true ����������
            if (TitaniumShield) {
                Player.immune = true;//�����޵�֡
                Player.SetImmuneTimeForAllTypes(90);//�����޵�֡����
                Player.ClearBuff(ModContent.BuffType<TitaniumShieldBuff>());
                Player.AddBuff(ModContent.BuffType<TitaniumReloadBuff>(), 45 * 60);
                return true;
            }

            return base.FreeDodge(info);
        }

        public override bool ConsumableDodge(Player.HurtInfo info)//�������ܺ�����ܣ���󴥷�
        {
            return base.ConsumableDodge(info);
        }

        #endregion

        #region ÿ֡����

        //ÿ֡����buff����ֵ����
        public override void UpdateBadLifeRegen() {
        }

        //ÿ֡����ֵ����
        public override void UpdateLifeRegen() {
            base.UpdateLifeRegen();
        }

        public override void PreUpdate() {
            //����ǰ���ø�ֵ
            Gslot = 0;
            GcountR = 1;
            GcountEx = 0;

            if (residuallonelinessechoes) {
                Gslot += 8;
            }
        }

        //ÿ֡�������
        public override void PostUpdate() {
            //���м�ʱ���������������ĳЩЧ�����г��ܻ���ȴ��ʱ
        }

        //ÿ֡����
        public override void ResetEffects() {
            //���ﲻ�ܹ�ֱ������ĳЩ���������õ��ֶ�,��Ȼ�ᵼ������,���Ǳ���������PreUpdate
            //��Ʒ�ж�����
            residuallonelinessechoes = false;//�¶�����

            //buff�ж�����
            TitaniumShield = false;
            base.ResetEffects();
        }

        #endregion
    }
}