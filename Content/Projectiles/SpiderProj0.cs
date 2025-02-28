using DreamJourney.Content.Projectiles.Ranged;
using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;

namespace GloryofGuardian.Content.Projectiles
{
    public class SpiderProj0 : GOGProj
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 76;
            Projectile.height = 46;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 480;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 30;//默认发射间隔
            interval = 5;//多重攻击间隔长度
            //Projectile.velocity = new Vector2(0, 8);
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
        int atknum = 0;
        public override void AI() {
            count++;
            //可移动式炮台
            //摩擦力
            if (Projectile.velocity.X > 0 || Projectile.velocity.X < 0) {
                Projectile.velocity.X *= 0.98f;
                if (Projectile.velocity.X > -1 && Projectile.velocity.X < 1) {
                    Projectile.velocity.X = 0;
                }
            }

            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //索敌与行动
            NPC target1 = Projectile.Center.InPosClosestNPC(1600, false, true);
            if (target1 != null && Projectile.velocity.X == 0) {
                Attack(target1);
            }

            if (atknum > 12) Projectile.Kill();

            base.AI();
        }

        /// <summary>
        /// 坠落
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 0.5f;

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
            Gcount = count0;//攻击间隔因子重新提取
            //伤害修正
            int newDamage = (int)(Projectile.originalDamage);
            float rangedOffset = 1;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack(NPC target1) {
            Vector2 m = target1.Center;
            Vector2 projcen = Projectile.Center + new Vector2(0, 0);

            //发射
            if (count >= Gcount) {
                int firenum = Main.rand.Next(1, 3);
                atknum += 1;
                atknum += firenum;
                atknum += firenum;
                float fireveltor = Main.rand.NextFloat(0.07f, 0.09f) - firenum * 0.01f;
                Main.NewText(atknum);
                for (int i = -firenum; i <= firenum; i++) {
                    Vector2 vel = (target1.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12f;
                
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39);
                
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, vel.RotatedBy(i * fireveltor), ModContent.ProjectileType<SpiderProj>(), lastdamage, 0, Owner.whoAmI, 0, 0, 1);
                    
                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                        if (proj1.ModProjectile is GOGProj proj2) {
                            proj2.OrichalcumMarkProj = true;
                            proj2.OrichalcumMarkProjcount = 300;
                        }
                    }
                }



                //计时重置,通过更改这个值来重置攻击
                count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }

        public override bool? CanHitNPC(NPC target) {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
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
            //不同朝向时翻转贴图
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SpiderProj0").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, 6);
            if (Projectile.ai[0] == 0) Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}