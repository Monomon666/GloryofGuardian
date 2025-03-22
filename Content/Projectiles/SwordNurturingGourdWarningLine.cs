using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SwordNurturingGourdWarningLine : ModProjectile // 预警线
    {
        public override string Texture => GOGConstant.Projectiles + "WarningLine";
        public override void SetStaticDefaults() {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;//这一项代表弹幕超过屏幕外多少距离以内可以绘制
                                                                //用于长条形弹幕绘制
                                                                //激光弹幕建议4000左右
            base.SetStaticDefaults();
        }
        public override void SetDefaults() {
            Projectile.tileCollide = false; Projectile.ignoreWater = true;//穿墙并且不受水影响
            Projectile.width = Projectile.height = 4;//装饰性弹幕宽高随便写个小一点的数
            Projectile.timeLeft = 10;//预警线持续40帧，是BOSS完成蓄力的时间
        }
        Player Owner => Main.player[Projectile.owner];

        public NPC npc0 = null;
        public Vector2 npccen = new Vector2(0, 0);
        public override void AI() {
            if (drawcount == 0) {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<SwordNurturingGourdProj>(), Projectile.damage, 2, Owner.whoAmI, 2);
                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                    if (proj1.ModProjectile is GOGProj proj2) {
                        proj2.OrichalcumMarkProj = true;
                        proj2.OrichalcumMarkProjcount = 300;
                    }
                }
            }

            if (npc0 != null && npc0.active) {
                npc0.Center = npccen;
            }
            Projectile.Center -= Projectile.velocity;
        }

        public override bool ShouldUpdatePosition() {
            return false;//禁止速度影响弹幕位置
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        int drawcount = 0;
        public override bool PreDraw(ref Color lightColor) {
            if (drawcount > 10) return false;
            drawcount++;
            if (drawcount >= 10) drawcount = 0;
            float hue = (drawcount % 10) / 10f;
            Color rainbowColor = HsvToRgb(hue, 1f, 1f);

            float factor = (1 + (float)Math.Sin(Projectile.timeLeft / 3f));
            factor /= 2f;

            var tex = TextureAssets.MagicPixel.Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition,
                new Rectangle(0, 0, 2, 2),
                rainbowColor, Projectile.velocity.ToRotation()
                , Vector2.Zero, new Vector2(1500, 1)
                , SpriteEffects.None, 0);
            return false;
        }

        //生成渐变色
        Color HsvToRgb(float h, float s, float v) {
            h = h % 1f; // 确保色相在 [0, 1) 范围内
            if (h < 0) h += 1f;

            int hi = (int)(h * 6);
            float f = h * 6 - hi;
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);

            switch (hi) {
                case 0: return new Color(v, t, p);
                case 1: return new Color(q, v, p);
                case 2: return new Color(p, v, t);
                case 3: return new Color(p, q, v);
                case 4: return new Color(t, p, v);
                default: return new Color(v, p, q);
            }
        }
    }
}