using GloryofGuardian.Common;
using GloryofGuardian.Content.Items.Weapon;
using GloryofGuardian.Content.NPCs.Special;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SoulFerryLanternProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + "Wind";

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults() {
            Projectile.width = 20;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            //Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;

            Projectile.light = 1.5f;
            Projectile.scale *= 0.8f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;
        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        int mode = 0;

        // 虚影的位置
        private Vector2 shadowPosition;
        // 是否已经交换过一次位置
        private bool hasSwappedOnce = false;
        // 按住左键的时间
        private int leftClickTimer = 0;

        private const float MaxDistance = 500f; // 虚影与玩家的最大距离
        private const int SwapCooldown = 30; // 交换位置的冷却时间
        private const int ClearDuration = 120; // 清除虚影的持续时间（现在是1秒）

        private int swapCount = 0; // 交换位置的次数
        private int swapCooldownTimer = 0; // 交换位置的冷却计时器
        private int clearTimer = 0; // 清除虚影的计时器
        private bool isRightMouseDown = false; // 记录右键是否按下
        private bool isLeftMouseDown = false; // 记录左键是否按下

        Vector2 _currentScreenPos = Vector2.Zero; // 用于记录当前屏幕位置
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            if (count > 1200) Projectile.Kill();
            Projectile.velocity *= 0.1f;

            // 检测是否持有 SoulFerryLanternCalling
            if (Owner.HeldItem.type != ModContent.ItemType<SoulFerryLanternCalling>()) {
                Projectile.Kill();
                return;
            }

            // 检测右键是否按下
            bool isRightMousePressed = Owner.PressKey(false);
            bool isLeftMousePressed = Owner.PressKey(true);

            // 按住右键：视角平滑地拉回虚影处
            if (isRightMousePressed && !isLeftMouseDown) {
                Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().TargetScreenPos = Projectile.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            }

            // 检测右键和左键的组合操作
            if (isRightMousePressed && isLeftMousePressed && !isLeftMouseDown && swapCooldownTimer <= 0) {
                SwapPositionWithProjectile(Owner);
                swapCooldownTimer = SwapCooldown;
            }

            // 记录按键状态
            isRightMouseDown = isRightMousePressed;
            isLeftMouseDown = isLeftMousePressed;

            // 与虚影第二次交换位置后其会消失
            if (swapCount >= 2) {
                Projectile.Kill();
                return;
            }

            // 按住左键持续2秒：清除虚影
            if (isLeftMousePressed) {
                clearTimer += 2;
                if (clearTimer >= ClearDuration) {
                    Projectile.Kill();
                    return;
                }
            } else {
                clearTimer = 0;
            }

            // 更新交换位置的冷却计时器
            if (swapCooldownTimer > 0) {
                swapCooldownTimer--;
            }

            if (count % 1 == 0) {
                for (int i = 0; i < 6; i++) {
                    int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Wraith, 0f, 0f, 50, Color.White, 0.8f);
                    Main.dust[num].velocity *= 1f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (Projectile.ai[0] == 1 && count % 1 == 0) {
                for (int i = 0; i < 6; i++) {
                    int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Wraith, 0f, 0f, 50, Color.White, 0.8f);
                    Main.dust[num].velocity *= 1f;
                    Main.dust[num].noGravity = true;
                }
            }
        }

        void SwapPositionWithProjectile(Player player) {
            // 消耗生命值
            player.statLife -= 10; // 假设每次交换消耗10点生命值
            if (player.statLife <= 0) {
                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " sacrificed too much."), 1, 0);
            }

            // 交换位置
            Vector2 temp = player.position;
            player.position = Projectile.position;
            Projectile.position = temp;

            // 增加交换次数
            swapCount++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (Projectile.ai[0] == 1) return false;
            else return base.OnTileCollide(oldVelocity);
        }

        public override Color? GetAlpha(Color lightColor) {
            if (Projectile.ai[0] == 1) return new Color(225, 225, 100);
            else return Color.White;
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }

        Player clone;
        public override bool PreDraw(ref Color lightColor) {
            // 创建一个克隆玩家并设置其外观
            Player clone = new Player();
            clone.CopyVisuals(Owner);
            SetCloneAppearance(clone, Color.Gray, Color.Red); // 设置灰色外观和红色眼睛
            ApplyShadowDye(clone); // 应用暗影染料
            UpdateCloneState(clone); // 更新克隆玩家的状态

            // 设置克隆玩家的动作
            SetCloneAnimation(clone, Owner.bodyFrame);

            // 设置克隆玩家的方向
            clone.direction = Math.Sign(Projectile.DirectionTo(Owner.Center).X);

            // 绘制克隆玩家
            Main.PlayerRenderer.DrawPlayer(Main.Camera, clone, Projectile.position, 0f, clone.fullRotationOrigin, 0f, 1f);

            return false;

            // 设置克隆玩家的外观
            void SetCloneAppearance(Player clone, Color bodyColor, Color eyeColor) {
                clone.skinColor = bodyColor;
                clone.shirtColor = bodyColor;
                clone.underShirtColor = bodyColor;
                clone.pantsColor = bodyColor;
                clone.shoeColor = bodyColor;
                clone.hairColor = bodyColor;
                clone.eyeColor = eyeColor;
            }

            // 应用缥缈染料
            void ApplyShadowDye(Player clone) {
                for (int i = 0; i < clone.dye.Length; i++) {
                    if (clone.dye[i].type != ItemID.VoidDye) {
                        clone.dye[i].SetDefaults(ItemID.VoidDye);
                    }
                }
            }

            // 更新克隆玩家的状态
            void UpdateCloneState(Player clone) {
                clone.ResetEffects();
                clone.ResetVisibleAccessories();
                clone.DisplayDollUpdate();
                clone.UpdateSocialShadow();
                clone.UpdateDyes();
                clone.PlayerFrame();
            }

            // 设置克隆玩家的动画
            void SetCloneAnimation(Player clone, Rectangle bodyFrame) {
                clone.bodyFrame = bodyFrame;
                clone.legFrame.Y = 0; // 腿部保持静止
                if (clone.headFrame != new Rectangle(0, 0, 0, 0)) Main.NewText(clone.headFrame);

                clone.legFrame = new Rectangle(0, 280, 40, 56);//找到跳跃的一帧
            }

            //Vector2 drawPos = Projectile.Center;
            //DrawPlayer(Owner, drawPos);
            //
            //return false;
        }

        private void DrawPlayer(Player player, Vector2 drawPos) {
            // 保存玩家原始位置和旋转
            Vector2 oldPosition = player.position;
            float oldRotation = player.fullRotation;

            // 设置玩家的位置和旋转为弹幕的位置和旋转
            player.position = Projectile.Center - player.Size / 2f;
            player.fullRotation = Projectile.rotation;
            player.fullRotationOrigin = player.Size / 2f;

            // 调用玩家的绘制逻辑
            Main.PlayerRenderer.DrawPlayer(Main.Camera, player, drawPos, 0f, player.fullRotationOrigin, 0f);

            // 恢复玩家的原始位置和旋转
            player.position = oldPosition;
            player.fullRotation = oldRotation;
        }
    }
}
