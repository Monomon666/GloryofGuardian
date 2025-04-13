using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class PlanteraDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 72;
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
            count0 = 300;//默认发射间隔
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int count2 = 0;//常态攻击使用
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        public override void AI() {
            count++;
            count2++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //索敌与行动
            NPC target1 = Projectile.Center.InPosClosestNPC(800, true, true);
            if (target1 != null) {
                Attack(target1);
                Attack2();
            }

            foreach (NPC npc in Main.npc) {
                if (npc != null && npc.active && Vector2.Distance(Projectile.Center, npc.Center) < 2400) {
                    npc.AddBuff(BuffID.Poisoned, 60);
                    npc.AddBuff(BuffID.Venom, 60);

                    for (int j = 0; j < 1; j++) {
                        int num1 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Plantera_Pink, 0f, 0f, 10, Color.White, 1f);
                        Main.dust[num1].noGravity = true;
                        Main.dust[num1].velocity = new Vector2(0, -2);
                    }

                    if (count % 5 == 0) {
                        for (int j = 0; j < 1; j++) {
                            int num1 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Plantera_Green, 0f, 0f, 10, Color.White, 1f);
                            Main.dust[num1].noGravity = false;
                            Main.dust[num1].velocity *= 0.5f;
                        }
                    }
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
                        Projectile.Bottom = (droppos + new Vector2(0, y - 2) * 16);
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
            Vector2 tarpos = target1.Center + new Vector2(0, target1.height / 2);
            Vector2 projcen = Projectile.Center + new Vector2(0, -24);

            //发射参数计算
            float dx = tarpos.X - projcen.X;
            float dy = tarpos.Y - projcen.Y;
            //设置一个相对标准的下落加速度
            float G = 0.3f;
            //设置一个相对标准的初始垂直速度
            float vy = 16;

            //发射
            if (count >= Gcount) {
                //普通
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {

                    }
                }

                //过载
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        G *= Main.rand.NextFloat(0.8f, 1.2f);
                        vy *= Main.rand.NextFloat(0.9f, 1.1f);

                        //对速度进行一些观赏度调整
                        //vx *=
                        //vy *=
                        vy *= (dy >= 0 ? 0.75f : 1.2f);

                        float vx = dx / ((vy + (float)Math.Sqrt(vy * vy + 2 * G * dy)) / G);

                        Vector2 velfire = new Vector2(vx * Main.rand.NextFloat(0.9f, 1.1f), -vy * Main.rand.NextFloat(0.98f, 1.02f));//降低精度

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit9, Projectile.Center);
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, velfire, ModContent.ProjectileType<PlanteraCallProj>(), lastdamage, 4, Owner.whoAmI);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                //计时重置
                count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
            }
        }

        /// <summary>
        /// 尖刺攻击
        /// </summary>
        void Attack2() {
            if (count2 % (Gcount / 3) == 0) {
                for (int i = 0; i < 1; i++) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit9, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(0, 20), new Vector2(4, 0), ModContent.ProjectileType<PlanteraProj0>(), lastdamage, 1, Owner.whoAmI);
                    Projectile proj3 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(0, 20), new Vector2(-4, 0), ModContent.ProjectileType<PlanteraProj0>(), lastdamage, 1, Owner.whoAmI);
                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                        if (proj1.ModProjectile is GOGProj proj2) {
                            proj2.OrichalcumMarkProj = true;
                            proj2.OrichalcumMarkProjcount = 300;
                        }

                        if (proj3.ModProjectile is GOGProj proj4) {
                            proj4.OrichalcumMarkProj = true;
                            proj4.OrichalcumMarkProjcount = 300;
                        }
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

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PlanteraDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -26);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}