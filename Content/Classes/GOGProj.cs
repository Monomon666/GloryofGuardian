using GloryofGuardian.Content.Projectiles;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Class
{
    public abstract class GOGProj : ModProjectile
    {
        public override void OnSpawn(IEntitySource source) {
            base.OnSpawn(source);
        }

        //ɽͭǿ�����
        public bool OrichalcumMarkProj;
        public int OrichalcumMarkProjcount;
        public override void AI() {
            base.AI();
        }

        public override void PostAI() {
            if (OrichalcumMarkProjcount > 0) OrichalcumMarkProjcount -= 0;//����Ŀǰ���������ǿ��
            if (OrichalcumMarkProjcount == 0) OrichalcumMarkProj = false;

            //ɽͭǿ��������Ч
            if (OrichalcumMarkProj
                //�����������������������
                && Projectile.type != ModContent.ProjectileType<WildProj>()
                && Projectile.type != ModContent.ProjectileType<GarrisonProj>()
                && Projectile.type != ModContent.ProjectileType<MythrilProj2>()
                ) {
                for (int i = 0; i <= 1 && Projectile.width > 2 && Projectile.height > 2; i++) {
                    Dust dust1 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TeleportationPotion, 1f, 1f, 100, new Color(239, 113, 248), 1.2f);
                    dust1.velocity = Projectile.velocity;
                    dust1.noGravity = true;
                }
                for (int i = 0; i <= 1 && Projectile.width <= 2 && Projectile.height <= 2; i++) {
                    Dust dust1 = Dust.NewDustDirect(Projectile.position + new Vector2(-2, -2), 2, 2, DustID.TeleportationPotion, 1f, 1f, 100, new Color(239, 113, 248), 1.2f);
                    dust1.velocity = Projectile.velocity;
                    dust1.noGravity = true;
                }
            }
            base.PostAI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            //ɽͭǿ��׷���˺�
            if (OrichalcumMarkProj) {
                int Oriexdamage = (int)MathHelper.Max(1, ((int)(Projectile.originalDamage) * Main.player[Projectile.owner].GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f * 0.1f));
                if (target.life >= Oriexdamage) target.life -= Oriexdamage;
                if (target.life < Oriexdamage) target.life = 1;
                CombatText.NewText(target.Hitbox,//�������ɵľ��η�Χ
                                    new Color(239, 113 ,248),//���ֵ���ɫ
                                    Oriexdamage,//����������Ҫչʾ������
                                    false,//dramaticΪtrue����ʹ��������˸��
                                    false //dotΪtrue����ʹ��������С��������ʽҲ��ͬ(ԭ��debuff��Ѫ��ʽ)
                                    );
            }
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}