using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class WildDT : GOGDT
    {
        //��ʹ����ͼ,��д����
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 32;
            Projectile.height = 26;
            Projectile.tileCollide = true;

            Projectile.sentry = false;//�ڱ����֤���������ǽ��½�һ��ְҵ�����������ص��ٻ��������ƻ��ƣ����Բ�����Ҫ����
            Projectile.friendly = true;
            Projectile.minion = true;//�ʹ����֤
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        //��ֹ�ƻ���ͼ����
        public override bool? CanCutTiles() {
            return false;
        }

        //����ʱ������׹
        public override void OnSpawn(IEntitySource source) {
            count0 = 120;//Ĭ�Ϸ�����
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        //����
        bool drop = true;
        //��̨ת��
        float wrotation = 0;
        float projRot = 0;
        //���ݶ�ȡ
        int Gcount = 0;
        int lastdamage = 0;
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//�γ��ж�
            Drop();
            Calculate();
            //�������ж�
            NPC target1 = Projectile.Center.InPosClosestNPC(800, false, true);
            if (target1 != null) {
                Attack(target1);
                Turn(target1);
            }

            base.AI();
        }

        /// <summary>
        /// ׹��
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 8f) {
                Projectile.velocity.Y = 8f;
            }

            Vector2 droppos = Projectile.Bottom;
            if (drop) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 16));
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 6) * 16);
                        break;
                    }
                }
                drop = false;
            }
        }

        /// <summary>
        /// ���¼���͸�ֵ����
        /// </summary>
        void Calculate() {
            Gcount = (int)(count0 * Owner.GetModPlayer<GOGModPlayer>().GcountR * Projectile.ai[0]);//�����������������ȡ
            //�˺�����
            int newDamage = (int)(Projectile.originalDamage);
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        /// <summary>
        /// ����빥��
        /// </summary>
        void Attack(NPC target1) {
            Vector2 m = target1.Center;
            Vector2 projcen = Projectile.Center + new Vector2(0, -20);

            //����
            if (count >= Gcount) {
                //��ͨ
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        float vel = Main.rand.NextFloat(0.9f, 1.15f) * 16f;
                        Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot);
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + nowvel * 20f, nowvel * vel, ModContent.ProjectileType<WildProj>(), lastdamage, 0, Owner.whoAmI, 0, 0, 1);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                //����
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        float vel = Main.rand.NextFloat(0.9f, 1.15f) * 16f;
                        Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot);
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + nowvel * 20f, nowvel * vel, ModContent.ProjectileType<WildProj>(), lastdamage, 1, Owner.whoAmI);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                //��ʱ����,ͨ���������ֵ�����ù���
                count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
            }
        }

        /// <summary>
        /// ��̨��ת
        /// </summary>
        void Turn(NPC target1) {
            Vector2 tarpos = target1.Center + new Vector2(0, target1.height / 2);
            Vector2 projcen = Projectile.Center + new Vector2(0, 16);

            Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
            float rot2 = vector2.ToRotation();
            float degree2 = (float)((180 / Math.PI) * rot2);
            float tarrot = MathHelper.ToRadians(projRot + degree2 * Projectile.spriteDirection);
            float rspeed = 0.04f;

            //תͷ
            if (wrotation != tarrot) {
                if (Math.Abs(wrotation - tarrot) % Math.PI <= rspeed) {//��������С�ڵ���ת���򱾴���ת������Ŀ��,ȡ�����Ҫ
                    wrotation = tarrot;//��ôֱ���÷����뵽Ŀ�귽���ֹ����
                                       //tarrot = wrotation;
                    return;
                } else {
                    Vector2 clockwise = (wrotation + rspeed).ToRotationVector2();//���Ǽ���NPC˳ʱ��ת����ĵ�λ��������
                    Vector2 anticlockwise = (wrotation - rspeed).ToRotationVector2();//���Ǽ���NPC��ʱ��ת����ĵ�λ��������
                                                                                     //��Ȼ��Ҫ�Ƚ����������ĸ���Ŀ��нǸ��������ǱȽ�������Ŀ�����������ĳ���
                    if ((clockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length() <= (anticlockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length())//���˳ʱ��Ĳ�ֵ��С
                    {
                        wrotation += rspeed;
                    } else {
                        wrotation -= rspeed;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }

        public override void OnKill(int timeLeft) {
            //��ը����
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            //��ͬ����ʱ��ת��ͼ
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "WildDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, 2);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "WildDT2").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, -20);
            Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, wrotation, texture.Size() * 0.5f + new Vector2(-16, 0), Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}