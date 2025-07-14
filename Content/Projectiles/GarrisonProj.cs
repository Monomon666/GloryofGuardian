using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class GarrisonProj : GOGProj {
        public override string Texture => GOGConstant.Projectiles + Name;

        //弩箭模板

        //下落
        int GravityDelayTimer = 0;//下落计时器
        int GravityDelay = 45;//下落前的飞行时间
        //钉入
        public bool IsStickingToTarget {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }
        //附着的敌人索引
        public int TargetWhoAmI {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private const int MaxStickingJavelin = 6; //该类标枪上限
        private readonly Point[] stickingJavelins = new Point[MaxStickingJavelin]; // 点数组
        int StickTimer = 0;//附着时间
        int StickTime = 60 * 5; // 附着15秒

        Vector2 ToTargetpos = new Vector2(0, 0);//相对位置
        int AlphaFadeInSpeed = 40; //淡出速度

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = 0;
            Projectile.penetrate = 2;

            Projectile.hide = true;//触发Drawbehind，来使贴图隐藏在某些图层之后

            GravityDelay = 45;

            // 旋转90度
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }

        public override void AI() {
            UpdateAlpha();
            if (IsStickingToTarget) {
                StickyAI();
            }
            else {
                NormalAI();
            }

            base.AI();
        }

        //普通模式
        private void NormalAI() {
            GravityDelayTimer++;

            if (Projectile.ai[2] == 0) {
                if (count % 2 == 0) {
                    for (int i = 0; i < 1; i++) {
                        int num1 = Dust.NewDust(Projectile.position, 8, 8, DustID.SilverCoin, 0f, 0f, 10, Color.White, 1f);
                        Main.dust[num1].noGravity = true;
                        Main.dust[num1].velocity = Projectile.velocity * Main.rand.NextFloat(1f);
                    }
                }
            }
            if (Projectile.ai[2] == 1) {
                if (count % 2 == 0) {
                    for (int i = 0; i < 2; i++) {
                        int num1 = Dust.NewDust(Projectile.position, 8, 8, DustID.GoldCoin, 0f, 0f, 10, Color.White, 1f);
                        Main.dust[num1].noGravity = true;
                        Main.dust[num1].velocity = Projectile.velocity * Main.rand.NextFloat(1f);
                    }
                }
            }

            //在一段时间内，标枪将以同样的速度运动，但在此之后，标枪的速度迅速下降。
            if (GravityDelayTimer >= GravityDelay) {
                GravityDelayTimer = GravityDelay;

                // 阻力
                Projectile.velocity.X *= 0.98f;
                // 重力
                Projectile.velocity.Y += 0.35f;
            }

            // 旋转90度
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }
        //钉入模式
        private void StickyAI() {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            StickTimer += 1;

            // 每30帧造成一次伤害效果
            bool hitEffect = StickTimer % 30f == 0f;
            int npcTarget = TargetWhoAmI;
            if (StickTimer >= StickTime || npcTarget < 0 || npcTarget >= 200) { // 检测
                Projectile.Kill();
            }
            else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) {
                // 检查宿主状态
                // 没有问题的话固定其位置
                Projectile.Center = Main.npc[npcTarget].Center + (Projectile.velocity.SafeNormalize(Vector2.Zero) * 6) + ToTargetpos
                    + new Vector2(-4, 0);

                //超频攻击粒子效果
                if (StickTimer % 12 == 0) {
                    for (int i = 0; i < 1; i++) {
                        int num2 = Dust.NewDust(Projectile.position, 8, 8, DustID.GoldCoin, 0f, 0f, 10, Color.White, 1f);
                        Main.dust[num2].noGravity = true;
                        Main.dust[num2].velocity *= 0.5f;
                    }
                }
                
                if (hitEffect) {
                    // 在这里模仿出被击中的效果,但是伤害实际通过debuff的dot实现
                    Main.npc[npcTarget].HitEffect(0, 1.0);
                }
            }
            else {
                Projectile.Kill();
            }
        }

        /// <summary>
        /// 淡出机制
        /// </summary>
        private void UpdateAlpha() {
            // 淡出
            if (Projectile.alpha > 0) {
                Projectile.alpha -= AlphaFadeInSpeed;
            }

            // 防止负数
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            IsStickingToTarget = true; // 击中目标确认
            pmode = 1;
            TargetWhoAmI = target.whoAmI; // 目标索引输入
            ToTargetpos = Projectile.Center - target.Center; //相对位置输入
            Projectile.velocity *= 0.01f; // 镶入
            Projectile.netUpdate = true; // 网络同步
            Projectile.damage = 0; // 移出伤害，之后通过debuff造成伤害

            // 添加debuff，该buff将会打开gnpc中的开关并造成伤害
            target.AddBuff(ModContent.BuffType<JavelinDebuff1>(), 30);

            // 启用该方法以设定一个上限，防止敌怪被扎了过多的弹幕
            // 使用该方法需要在ai槽中正确地填入(ai0为1,ai1为npc的index),想使用最好还是自己另写一个
            Projectile.KillOldestJavelin(Projectile.whoAmI, Type, target.whoAmI, stickingJavelins);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            //基本爆炸粒子
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Wraith, 0f, 0f, 50, Color.White, 0.8f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (count < 4) return false;

            for (int k = 0; k < Projectile.oldPos.Length; k++) {
                if (k != 0 && !IsStickingToTarget) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    //Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color * 0.3f, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                    //Main.spriteBatch.ResetBlendState();
                }

                if (k == 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    Main.EntitySpriteDraw(texture, drawPos, null, lightColor, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                }
            }

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            // 显示在正确的图层
            if (IsStickingToTarget) {
                int npcIndex = TargetWhoAmI;
                if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active) {
                    if (Main.npc[npcIndex].behindTiles) {
                        behindNPCsAndTiles.Add(index);
                    }
                    else {
                        behindNPCsAndTiles.Add(index);
                    }

                    return;
                }
            }
            behindNPCsAndTiles.Add(index);
        }
    }
}
