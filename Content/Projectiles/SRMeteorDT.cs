using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SRMeteorDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 94;
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
            count0 = 90;//默认发射间隔
            firedamage = 33;//默认灼烧伤害
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int countfire = 0;
        int countdust = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        int firedamage = 0;//灼烧伤害
        //攻击
        List<int> ignore = new List<int>();
        List<int> whoistar = new List<int>();
        int num = 0;
        int num0 = 0;
        bool bosshere = false;
        public override void AI() {

            countfire++;
            countdust++;

            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();

            //粒子
            float breath = (float)Math.Sin(drawcount / 18f);

            if (countdust % 1 == 0) {
                for (int j = 0; j < 1; j++) {
                    int num1 = Dust.NewDust(Projectile.Center + new Vector2(-16, -16), 36, 24, DustID.Flare, 0f, 0f, 10, Color.White, 2f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity = new Vector2(0, -1).RotatedByRandom(1f) * Main.rand.NextFloat(2, 2.5f);
                }
            }

            if (countdust % 2 == 0) {
                for (int j = 0; j < 3; j++) {
                    int num1 = Dust.NewDust(Projectile.Center + new Vector2(-20, -40) + new Vector2(0, -4) * breath, 40, 20, DustID.Flare, 0f, 0f, 10, Color.White, 2f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity = new Vector2(0, -1).RotatedByRandom(2f) * Main.rand.NextFloat(2, 2.5f);
                }
            }

            if (count >= Gcount) {
                num0 = 2;
                //索敌与行动
                ignore.Clear();
                NPC target1 = null;

                //攻击人数预定
                ignore.Clear();
                for (int index = 0; index < Main.npc.Length; index++) {
                    target1 = (Projectile.Center).InPosClosestNPC(1200, true, true, ignore);

                    if (target1 != null && target1.active) {

                        if (num == num0) {
                            break;
                        }

                        ignore.Add(target1.whoAmI);//加入检索忽略目标

                        if (target1.HasBuff(BuffID.OnFire3)) {
                            //有狱火则再次攻击
                            num += 1;
                        }
                    }
                }

                ignore.Clear();
                for (int index = 0; index < Main.npc.Length; index++) {
                    target1 = Projectile.Center.InPosClosestNPC(1200, true, true, ignore);
                    if (target1 != null && target1.active) {

                        whoistar.Add(target1.whoAmI);//加入待打击目标
                        ignore.Add(target1.whoAmI);//加入检索忽略目标

                        num -= 1;
                        if (num == -1) {
                            break;
                        }
                    }
                }

                if (whoistar.Count > 0) {
                    if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                        foreach (int npc in whoistar) {
                            Attack(Main.npc[npc]);
                        }
                        for (int index = 0; index < Main.npc.Length; index++) {
                            NPC target2 = Main.npc[index];
                            if (target2 != null && target2.boss && target2.active && Vector2.Distance(Projectile.Center, target2.Center) < 800) {
                                Attack2(target2);
                            }
                        }
                    } else {
                        foreach (int npc in whoistar) {
                            Attack2(Main.npc[npc]);
                            whoistar.Clear();
                            break;
                        }
                        for (int index = 0; index < Main.npc.Length; index++) {
                            NPC target2 = Main.npc[index];
                            if (target2 != null && target2.boss && target2.active && Vector2.Distance(Projectile.Center, target2.Center) < 800) {
                                Attack2(target2);
                            }
                        }

                        for (int index = 0; index < Main.npc.Length; index++) {
                            NPC target2 = Main.npc[index];
                            if (target2 != null && target2.boss && target2.active && Vector2.Distance(Projectile.Center, target2.Center) < 800) {
                                Attack2(target2);
                            }
                        }
                    }


                    //计时重置
                    count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                    //攻击对象重置
                    whoistar.Clear();
                    //攻击计数重置
                    num = 0;
                }
            }

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
            //普通
            Vector2 tarpos = target1.Center + new Vector2(0, target1.height / 2);
            Vector2 projcen = Projectile.Center + new Vector2(0, -24);

            //发射参数计算
            float dx = tarpos.X - projcen.X;
            float dy = tarpos.Y - projcen.Y;
            //设置一个相对标准的下落加速度
            float G = 0.3f;
            //设置一个相对标准的初始垂直速度
            float vy = 16;

            for (int i = 0; i < 1; i++) {
                G *= Main.rand.NextFloat(0.8f, 1.2f);
                vy *= Main.rand.NextFloat(0.9f, 1.1f);

                //对速度进行一些观赏度调整
                //vx *=
                //vy *=
                vy *= (dy >= 0 ? 0.75f : 0.85f);

                float vx = dx / ((vy + (float)Math.Sqrt(vy * vy + 2 * G * dy)) / G);

                Vector2 velfire = new Vector2(vx, -vy);//降低精度 * Main.rand.NextFloat(0.9f, 1.1f)

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, velfire, ModContent.ProjectileType<SRMeteorProj>(), lastdamage, 8, Owner.whoAmI, 0, G);
                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    if (proj1.ModProjectile is GOGProj proj2) {
                        proj2.OrichalcumMarkProj = true;
                        proj2.OrichalcumMarkProjcount = 300;
                    }
                }
            }
        }

        /// <summary>
        /// 监测与头顶攻击
        /// </summary>
        void Attack2(NPC target1) {
            for (int i = 0; i < 3; i++) {
                Vector2 projcen = target1.Center + new Vector2(0, -1200);
                projcen += new Vector2(-300 + (300 * i), 0);

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, projcen.Toz(target1.Center + new Vector2(0, (Projectile.Center.Y - target1.Center.Y))) * 24f, ModContent.ProjectileType<SRMeteorProj>(), lastdamage, 8, Owner.whoAmI, 0);
                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    if (proj1.ModProjectile is GOGProj proj2) {
                        proj2.OrichalcumMarkProj = true;
                        proj2.OrichalcumMarkProjcount = 300;
                    }
                }
            }
            for (int i = 0; i < 2; i++) {
                Vector2 projcen = target1.Center + new Vector2(0, -1200);
                projcen += new Vector2(-350 + (700 * i), 0);

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, projcen.Toz(target1.Center + new Vector2(0, (Projectile.Center.Y - target1.Center.Y + 240))) * 24f, ModContent.ProjectileType<SRMeteorProj>(), lastdamage, 8, Owner.whoAmI, 0);
                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    if (proj1.ModProjectile is GOGProj proj2) {
                        proj2.OrichalcumMarkProj = true;
                        proj2.OrichalcumMarkProjcount = 300;
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

        int drawcount = 0;
        public override bool PreDraw(ref Color lightColor) {
            drawcount++;
            float breath = (float)Math.Sin(drawcount / 18f);

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SRMeteorDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SRMeteorDT2").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, 0);
            Vector2 drawPosition1 = Projectile.Center - Main.screenPosition + new Vector2(28, -48) + new Vector2(0, -4) * breath;

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPosition1, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}