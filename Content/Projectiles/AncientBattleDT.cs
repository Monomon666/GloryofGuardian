using DreamJourney.Content.Projectiles;
using DreamJourney.Content.Projectiles.Ranged;
using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class AncientBattleDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 42;
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
            count0 = 45;//默认发射间隔
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        //符文
        int overatknum = 0;
        int runenul = 0;
        bool rune = false;
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //索敌与行动
            NPC target1 = Projectile.Center.InPosClosestNPC(800, true, true);
            if (target1 != null) {
                Attack(target1);
            }

            //符文充能
            if (overatknum >= 3) {
                overatknum = 0;
                runenul += 1;
            }
            if (runenul == 3) {
                rune = true;
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
            int newDamage = (int)(Projectile.originalDamage);
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack(NPC target1) {
            Vector2 tarpos = target1.Center;
            Vector2 projcen = Projectile.Center + new Vector2(0, -66);

            //发射

            //普通
            if (!rune) {
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    if (count >= Gcount) {
                        for (int i = 0; i < 1; i++) {
                            Vector2 velfire = (tarpos - projcen).SafeNormalize(Vector2.Zero);

                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item60);
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, velfire, ModContent.ProjectileType<AncientBattleProj>(), lastdamage, 1, Owner.whoAmI);
                            if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                if (proj1.ModProjectile is GOGProj proj2) {
                                    proj2.OrichalcumMarkProj = true;
                                    proj2.OrichalcumMarkProjcount = 300;
                                }
                            }
                        }

                        //计时重置
                        count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                    }
                }

                //过载
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    if (count >= Gcount) {
                        for (int i = 0; i < 1; i++) {
                            Vector2 velfire = (tarpos - projcen).SafeNormalize(Vector2.Zero);

                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item60);
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, velfire, ModContent.ProjectileType<AncientBattleProj>(), lastdamage, 1, Owner.whoAmI, 1);
                            if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                if (proj1.ModProjectile is GOGProj proj2) {
                                    proj2.OrichalcumMarkProj = true;
                                    proj2.OrichalcumMarkProjcount = 300;
                                }
                            }
                        }

                        overatknum += 1;
                        //计时重置
                        count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                    }
                }
            }

            //符文
            if (rune) {
                if (count >= Gcount) {
                    for (int i = 0; i < 1; i++) {
                        Vector2 velfire = (tarpos - projcen).SafeNormalize(Vector2.Zero);

                        int vec = Projectile.Center.X > target1.Center.X ? 1 : -1;

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item60);
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, velfire, ModContent.ProjectileType<AncientBattleProj2>(), lastdamage, 1, Owner.whoAmI, vec);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    runenul = 0;
                    rune = false;
                    //计时重置
                    count = Owner.GetModPlayer<GOGModPlayer>().GcountEx - 120;
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

        float glow1 = 0;
        float glow2 = 0;
        float glow3 = 0;
        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AncientBattleDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AncientBattleDT2").Value;
            Texture2D textureg1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AncientBattleDTGlow1").Value;
            Texture2D textureg2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AncientBattleDTGlow2").Value;
            Texture2D textureg3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AncientBattleDTGlow3").Value;

            if (runenul >= 1) glow1 += 0.05f;
            if (runenul >= 2) glow2 += 0.05f;
            if (runenul >= 3) glow3 += 0.05f;
            if (runenul == 0) {
                glow1 = 0;
                glow2 = 0;
                glow3 = 0;
            }

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture0, drawPosition0 + new Vector2(0, -24), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPosition0 + new Vector2(0, -42) + new Vector2(0, -24), null, lightColor, Projectile.rotation, texture1.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(textureg1, drawPosition0 + new Vector2(0, -24), null, lightColor * glow1, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(textureg2, drawPosition0 + new Vector2(0, -24), null, lightColor * glow2, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(textureg3, drawPosition0 + new Vector2(0, -24), null, lightColor * glow3, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}