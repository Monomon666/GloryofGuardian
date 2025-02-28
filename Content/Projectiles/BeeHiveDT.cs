using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class BeeHiveDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 28;
            Projectile.height = 32;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.scale *= 1f;

            //本体具有伤害的炮塔，需要设置无敌帧
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.light = 3f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 60;//默认发射间隔
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int Acount = 0;
        int lastdamage = 0;
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //常态下即攻击
            Attack();

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
            //辅助炮塔叠加计算
            Acount = 0;
            foreach (var proj in Main.projectile)//遍历所有弹幕
            {
                if (proj.active && proj.type == Projectile.type) {
                    Acount++;
                }
            }
            if (Acount > 4) Acount = 4;
            //最多叠加四个炮塔的计时,每个给予5秒正面buff时长,每个减少60秒负面buff中的10秒
        }

        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack() {
            Vector2 projcen = Projectile.Center + new Vector2(0, -12);

            //碰撞粒子展现
            //内部
            //for (int i = 0; i <= 1; i++) {
            //    Dust dust1 = Dust.NewDustDirect(projcen + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(120), 8, 8, DustID.Honey, 1f, 1f, 100, Color.White, 0.8f);
            //    dust1.velocity *= 0;
            //    dust1.noGravity = true;
            //}
            //环状
            for (int i = 0; i <= 5; i++) {
                Dust dust1 = Dust.NewDustDirect(projcen + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 120, 8, 8, DustID.Honey, 1f, 1f, 100, Color.White, 1f);
                dust1.velocity *= 0;
                dust1.noGravity = true;
            }

            //发射
            Vector2 velfire = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)).SafeNormalize(Vector2.Zero) * 16f;
            if (count >= Gcount) {
                //普通
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    foreach (Player player in Main.ActivePlayers){
                        if (Vector2.Distance(Projectile.Center, player.Center) <= 120) {
                            player.AddBuff(BuffID.Honey, 180);
                            player.AddBuff(BuffID.Regeneration, 180);
                            if (player.statLife < player.statLifeMax2) {
                                
                                if (!player.HasBuff<HoneyDebuff>()) {
                                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4);
                                    player.statLife += 40;
                                    player.AddBuff(ModContent.BuffType<HoneyDebuff>(), 3600 - Acount * 600);
                                    CombatText.NewText(player.Hitbox,//跳字生成的矩形范围
                                    Color.LightGreen,//跳字的颜色
                                    "40",//这里是你需要展示的文字
                                    false,//dramatic为true可以使得字体闪烁，
                                    false //dot为true可以使得字体略小，跳动方式也不同(原版debuff扣血格式)
                                    );
                                    for (int i = 0; i <= 5; i++) {
                                        Dust dust1 = Dust.NewDustDirect(player.Center, 0, 0, DustID.Honey, 1f, 1f, 100, Color.White, 1.5f);
                                        dust1.velocity *= 2;
                                        dust1.noGravity = true;
                                    }
                                }
                            }
                        }
                        if (Vector2.Distance(Projectile.Center, player.Center) <= 1200) {
                            if (player.HasBuff(BuffID.Poisoned)) {
                                player.ClearBuff(BuffID.Poisoned);
                                for (int i = 0; i <= 5; i++) {
                                    Dust dust1 = Dust.NewDustDirect(player.Center, 0, 0, DustID.Honey, 1f, 1f, 100, Color.White, 1.5f);
                                    dust1.velocity *= 10;
                                    dust1.noGravity = true;
                                }
                            }
                        }
                    }
                }

                //过载
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    foreach (Player player in Main.ActivePlayers) {
                        if (Vector2.Distance(Projectile.Center, player.Center) <= 120) {
                            player.AddBuff(BuffID.Honey, 180);
                            player.AddBuff(BuffID.Regeneration, 180);
                            if (player.statLife < player.statLifeMax2) {

                                if (!player.HasBuff<HoneyDebuff>()) {
                                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4);
                                    player.statLife += 60;
                                    player.AddBuff(ModContent.BuffType<HoneyDebuff>(), 3600 - Acount * 600);
                                    CombatText.NewText(player.Hitbox,//跳字生成的矩形范围
                                    Color.LightGreen,//跳字的颜色
                                    "60",//这里是你需要展示的文字
                                    false,//dramatic为true可以使得字体闪烁，
                                    false //dot为true可以使得字体略小，跳动方式也不同(原版debuff扣血格式)
                                    );
                                    for (int i = 0; i <= 5; i++) {
                                        Dust dust1 = Dust.NewDustDirect(player.Center, 0, 0, DustID.Honey, 1f, 1f, 100, Color.White, 1.5f);
                                        dust1.velocity *= 2;
                                        dust1.noGravity = true;
                                    }
                                }
                            }
                        }
                        if (Vector2.Distance(Projectile.Center, player.Center) <= 1200) {
                            if (player.HasBuff(BuffID.Poisoned)) {
                                player.ClearBuff(BuffID.Poisoned);
                                for (int i = 0; i <= 5; i++) {
                                    Dust dust1 = Dust.NewDustDirect(player.Center, 0, 0, DustID.Honey, 1f, 1f, 100, Color.White, 1.5f);
                                    dust1.velocity *= 10;
                                    dust1.noGravity = true;
                                }
                            }
                        }
                    }
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
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "BeeHiveDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -4);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}