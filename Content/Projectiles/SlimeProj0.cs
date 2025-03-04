using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SlimeProj0 : GOGProj
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 18;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 480;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 120;//默认发射间隔
            interval = 5;//多重攻击间隔长度
            Projectile.velocity = new Vector2(0, 8);
            if (Projectile.ai[0] == 1) Projectile.localNPCHitCooldown = 3;
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int interval = 0;
        //重力
        bool drop = true;
        //炮台转动
        float wrotation = 0;
        float projRot = 0;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        //重置判定
        bool firstatk = false;
        public override void AI() {
            count++;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();

            base.AI();
        }

        /// <summary>
        /// 坠落
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 64f;

            Vector2 droppos = Projectile.Bottom;
            if (drop) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 8));
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 6) * 8);
                        break;
                    }
                }
                drop = false;
            }
        }

        /// <summary>
        /// 重新计算和赋值参数
        /// </summary>
        void Calculate() {
            Gcount = (int)(count0 * Owner.GetModPlayer<GOGModPlayer>().GcountR * Projectile.ai[0]);//攻击间隔因子重新提取
            //伤害修正
            int newDamage = Projectile.originalDamage;
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (!target.boss && target.knockBackResist != 0) {
                target.velocity *= 0.05f;
            }
        }

        public override void OnKill(int timeLeft) {
            //爆炸
            for (int j = 0; j < 10; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0f, 0f, 10, Color.White, 1.7f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 3f;
                num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0f, 0f, 10);
                Main.dust[num2].velocity *= 3f;
            }

            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            //不同朝向时翻转贴图
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SlimeDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SlimeDT2").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, 0);
            if (Projectile.ai[0] == 0) Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            if (Projectile.ai[0] == 1) Main.EntitySpriteDraw(texture1, drawPosition0 + new Vector2(0, 0), null, lightColor, Projectile.rotation, texture1.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}