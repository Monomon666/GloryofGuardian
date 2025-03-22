using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class ImmortalAltarDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 4;//动图帧数
        }

        public sealed override void SetDefaults() {
            Projectile.width = 23;
            Projectile.height = 32;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

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
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        public int countp = 0;
        public bool earliest = false;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        //状态机
        int allnum = 0;
        int mode = 0;

        int drawcircount = 0;
        //帧
        int frame1 = 0;
        int frame2 = 0;
        //位置标定
        Vector2 firepos = Vector2.Zero;
        public override void AI() {
            count++;
            countp = count;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();

            int dtnum = 0;
            int index = 0;//定义序号为0
            foreach (var proj in Main.projectile)//遍历所有弹幕
            {
                if (proj.active //活跃状态
                    && proj.ModProjectile is GOGDT gogdt1//遍历
                    && proj.type == ModContent.ProjectileType<ImmortalLotusDT>() //同类
                    && Vector2.Distance(Projectile.Center, proj.Center) < 3000
                    ) {

                    //编号
                    //序列号
                    if (gogdt1.globalcount < globalcount) {
                        //检测是否有比自己先生成的炮塔,如果有说明自己的序号要后移一位
                        index++;
                        earliest = false;
                    }

                    //调率同步


                    //总数
                    dtnum++;
                }
            }

            Main.NewText(dtnum);

            if (dtnum <= 2) mode = 0;
            if (dtnum > 2 && dtnum <= 5) mode = 1;
            if (dtnum > 5 && dtnum <= 9) mode = 2;

            //水晶位置标定
            firepos = Projectile.Center + new Vector2(-2, -96) + new Vector2(0, -8) * breath;

            if (mode >= 1) {
                for (int j = 0; j < 1; j++) {
                    int num1 = Dust.NewDust(Projectile.Center + new Vector2(-24, -72), 48, 48, DustID.SnowSpray, 0f, 0f, 10, Color.White, 0.4f);
                    Main.dust[num1].noGravity = false;
                    Main.dust[num1].velocity = new Vector2(0, -1f) * Main.rand.NextFloat(1, 2f);
                }
            }

            //索敌与行动
            NPC target1 = Projectile.Center.InPosClosestNPC(1200, true, true);
            if (target1 != null && target1.active) Attack(target1);

            //帧图
            Projectile.frameCounter++;
            frame2 = (Projectile.frameCounter / 8) % 4;//要手动填，不然会出错

            base.AI();
        }

        /// <summary>
        /// 坠落
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
        /// 重新计算和赋值参数
        /// </summary>
        void Calculate() {
            Gcount = (int)(count0 * Owner.GetModPlayer<GOGModPlayer>().GcountR * Projectile.ai[0]);//攻击间隔因子重新提取
            //伤害修正
            int newDamage = Projectile.originalDamage;
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack(NPC target1) {
            Vector2 projcen = firepos;
            Vector2 vel = projcen.Toz(target1.Center);

            //发射,不过载
            if (count >= Gcount) {
                for (int i = 0; i < 1; i++) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item26, Projectile.Center);

                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, Vector2.Zero, ModContent.ProjectileType<ImmortalLotusProj>(), lastdamage, 1, Owner.whoAmI);
                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                        if (proj1.ModProjectile is GOGProj proj2) {
                            proj2.OrichalcumMarkProj = true;
                            proj2.OrichalcumMarkProjcount = 300;
                        }
                    }

                    //int projectileID = ProjectileID.FairyQueenMagicItemShot; // 夜光弹幕的ID
                    //int projectileIndex = Projectile.NewProjectile(new EntitySource_Parent(Projectile), projcen, new Vector2(12, 0).RotatedBy(i * MathHelper.PiOver2), projectileID, lastdamage, 1, Owner.whoAmI);
                    //Projectile projectile = Main.projectile[projectileIndex];
                    //projectile.DamageType = GuardianDamageClass.Instance; // 修改伤害类型
                    //projectile.velocity *= 1.5f; // 提高弹幕速度
                    //projectile.scale = 1.2f; // 增大弹幕大小
                }

                //计时重置
                count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
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


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return false;
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

        public int drawcountp = 0;
        int drawcount = 0;
        float breath = 0;
        public override bool PreDraw(ref Color lightColor) {
            breath = (float)Math.Sin((int)Main.GameUpdateCount / 18f);

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalAltarDT").Value;
            Texture2D texture01 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalLotusDT21").Value;
            Texture2D texture02 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalLotusDT22").Value;
            Texture2D texture03 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalLotusDT23").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalAltarProjBegin").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalAltarProjMid").Value;
            Texture2D texture4 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalAltarProjEnd").Value;

            Vector2 drawPosition1 = Projectile.Center - Main.screenPosition + new Vector2(38, -8) + new Vector2(0, -8) * breath;
            int singleFrameY = texture0.Height / 4;
            Vector2 drawPos1 = Projectile.Center - Main.screenPosition + new Vector2(0, 16);

            Main.spriteBatch.Draw(
                texture0,
                drawPos1,
                new Rectangle(0, singleFrameY * frame2, texture0.Width, singleFrameY),//动图读帧
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                new Vector2(23, 62),
                Projectile.scale,
                SpriteEffects.None,
                0);

            Vector2 drawPos2 = Projectile.Center - Main.screenPosition + new Vector2(42, 64) + new Vector2(0, -8) * breath;

            Lighting.AddLight(firepos, 255 * 0.01f, 255 * 0.01f, 255 * 0.01f);

            Main.spriteBatch.Draw(
                texture01,
                drawPos2,
                null,
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                texture0.Size() / 2,
                Projectile.scale * 1.2f,
                SpriteEffects.None,
                0);
            Main.spriteBatch.Draw(
                texture03,
                drawPos2,
                null,
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                texture0.Size() / 2,
                Projectile.scale * 1.2f,
                SpriteEffects.None,
                0);
            Main.spriteBatch.Draw(
                texture03,
                drawPos2,
                null,
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                texture0.Size() / 2,
                Projectile.scale * 1.2f,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}