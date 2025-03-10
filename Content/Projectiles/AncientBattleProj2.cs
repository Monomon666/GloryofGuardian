using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class AncientBattleProj2 : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + "Wind";

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            //Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.extraUpdates = 0;
            Projectile.light = 1.5f;
            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;
        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
            windnum = 5;
        }

        int count = 0;
        float count0 = 0;
        int windnum = 0;
        int mode = 0;
        public override void AI() {
            count++;
            count0++;
            Drop();

            //吹飞
            // 更新所有计时器
            List<int> toRemove = new List<int>();
            foreach (var pair in moveTimers) {
                int npcID = pair.Key;
                int timer = pair.Value;

                // 如果计时器结束
                if (timer <= 0) {
                    NPC target = Main.npc[npcID];
                    if (target != null && target.active) {
                        // 将敌人的中心位置设置为弹幕的中心位置
                        target.Center = Vector2.Lerp(target.Center,
                            new Vector2(Projectile.Center.X + Projectile.velocity.X * 16, target.Center.Y), 0.2f);
                        target.velocity.X = Projectile.velocity.X;
                    }

                    // 标记需要移除的计时器
                    toRemove.Add(npcID);
                } else {
                    // 减少计时器
                    moveTimers[npcID]--;
                }
            }

            // 移除已完成的计时器
            foreach (int npcID in toRemove) {
                moveTimers.Remove(npcID);
            }

            if (windnum <= 14 && count % 10 == 0) {
                if (windnum == 14) {
                    count = 0;
                    windnum = 15;
                }
                windnum += 1;
            }

            if (windnum == 12 && Projectile.ai[1] == 1) Projectile.velocity.X += 1.6f;
            if (windnum == 12 && Projectile.ai[1] == -1) Projectile.velocity.X -= 1.6f;

            float lifeSpan = 900f;
            if (Projectile.soundDelay == 0) {
                Projectile.soundDelay = -1;
                SoundEngine.PlaySound(SoundID.Item122, Projectile.position);
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= lifeSpan) {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] >= 30f) {
                Projectile.damage = 0;
                if (Projectile.ai[0] < lifeSpan - 120f) {
                    float aiDecrement = Projectile.ai[0] % 60f;
                    Projectile.ai[0] = lifeSpan - 120f + aiDecrement;
                    Projectile.netUpdate = true;
                }
            }
            Point point8 = Projectile.Center.ToTileCoordinates();
            int sizeMod;
            int sizeMod2;
            Collision.ExpandVertically(point8.X, point8.Y, out sizeMod, out sizeMod2, windnum, windnum);
            sizeMod++;
            sizeMod2--;
            Vector2 sizeModVector = new Vector2(point8.X, sizeMod) * 16f + new Vector2(8f);
            Vector2 sizeModVector2 = new Vector2(point8.X, sizeMod2) * 16f + new Vector2(8f);
            Vector2 centering = Vector2.Lerp(sizeModVector, sizeModVector2, 0.5f);
            Vector2 sizeModPos = new Vector2(0f, sizeModVector2.Y - sizeModVector.Y);
            sizeModPos.X = sizeModPos.Y * 0.2f;
            Projectile.width = (int)(sizeModPos.X * 0.65f);
            Projectile.height = (int)sizeModPos.Y;
            Projectile.Center = centering;
            if (Projectile.owner == Main.myPlayer) {
                bool breakFlag = false;
                Vector2 playerCenter = Main.player[Projectile.owner].Center;
                Vector2 top = Main.player[Projectile.owner].Top;
                for (float i = 0f; i < 1f; i += 0.05f) {
                    Vector2 position2 = Vector2.Lerp(sizeModVector, sizeModVector2, i);
                    if (Collision.CanHitLine(position2, 0, 0, playerCenter, 0, 0) || Collision.CanHitLine(position2, 0, 0, top, 0, 0)) {
                        breakFlag = true;
                        break;
                    }
                }
                if (!breakFlag && Projectile.ai[0] < lifeSpan - 120f) {
                    float aiDecrement2 = Projectile.ai[0] % 60f;
                    Projectile.ai[0] = lifeSpan - 120f + aiDecrement2;
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.ai[0] < lifeSpan - 120f) {
                return;
            }
            return;
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
            int maxdropdis = 5000;
            for (int y = 0; y < maxdropdis; y++) {
                Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y + 1) * 16));
                if (tile0.HasTile) {
                    Projectile.Bottom = (droppos + new Vector2(0, y - 1) * 16);
                    break;
                }
            }
        }

        // 用于记录敌人及其被击中次数的字典
        private Dictionary<int, int> hitCounts = new Dictionary<int, int>();

        // 用于存储需要忽略的敌人的列表
        private List<int> ignore = new List<int>();

        // 用于记录需要移动的敌人及其计时器
        private Dictionary<int, int> moveTimers = new Dictionary<int, int>();

        int hitnum = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (target.knockBackResist > 0) {
                // 获取敌人的唯一标识符（whoAmI）
                int npcID = target.whoAmI;

                // 如果敌人已经在忽略列表中，直接返回
                if (ignore.Contains(npcID)) {
                    return;
                }

                // 如果敌人未被记录过，初始化击中次数为 0
                if (!hitCounts.ContainsKey(npcID)) {
                    hitCounts[npcID] = 0;
                }

                // 击中次数 +1
                hitCounts[npcID]++;


                // 如果击中次数达到 3 次，将敌人加入忽略列表
                if (hitCounts[npcID] >= 3) {
                    ignore.Add(npcID);

                } else {
                    // 如果敌人不在忽略列表中，启动计时器
                    moveTimers[npcID] = 3; // 3 帧后移动敌人
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
            // 如果敌人在忽略列表中，取消伤害
            if (ignore.Contains(target.whoAmI)) {
                modifiers.SetMaxDamage(0); // 将伤害设为 0
                modifiers.DisableCrit(); // 取消暴击
                modifiers.DisableKnockback(); // 取消击退
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.Gold;
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            float aiTracker = Projectile.ai[0];
            float trackerClamp = MathHelper.Clamp(aiTracker / 30f, 0f, 1f);
            if (aiTracker > 540f) {
                trackerClamp = MathHelper.Lerp(1f, 0f, (aiTracker - 540f) / 60f);
            }
            Point centerPoint = Projectile.Center.ToTileCoordinates();
            int sizeModding;
            int sizeModding2;
            Collision.ExpandVertically(centerPoint.X, centerPoint.Y, out sizeModding, out sizeModding2, windnum, windnum);
            sizeModding++;
            sizeModding2--;
            float vectorMult = 0.2f;
            Vector2 sizeModdingVector = new Vector2(centerPoint.X, sizeModding) * 16f + new Vector2(8f);
            Vector2 sizeModdingVector2 = new Vector2(centerPoint.X, sizeModding2) * 16f + new Vector2(8f);
            Vector2.Lerp(sizeModdingVector, sizeModdingVector2, 0.5f);
            Vector2 sizeModdingPos = new Vector2(0f, sizeModdingVector2.Y - sizeModdingVector.Y);
            sizeModdingPos.X = sizeModdingPos.Y * vectorMult;
            new Vector2(sizeModdingVector.X - sizeModdingPos.X / 2f, sizeModdingVector.Y);
            Texture2D texture2D23 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle drawRectangle = texture2D23.Frame(1, 1, 0, 0);
            Vector2 smallRect = drawRectangle.Size() / 2f;
            float aiTrackMult = -0.06283186f * aiTracker;
            Vector2 spinningpoint2 = Vector2.UnitY.RotatedBy((double)(aiTracker * 0.1f), default);
            float incrementStorage = 0f;
            float increment = 5.1f;
            Color sandYellow = new Color(225, 225, 100);
            for (float k = (int)sizeModdingVector2.Y; k > (int)sizeModdingVector.Y; k -= increment) {
                incrementStorage += increment;
                float colorChanger = incrementStorage / sizeModdingPos.Y;
                float incStorageMult = incrementStorage * 6.28318548f / -20f;
                float lowerColorChanger = colorChanger - 0.15f;
                Vector2 spinArea = spinningpoint2.RotatedBy((double)incStorageMult, default);
                Vector2 colorChangeVector = new Vector2(0f, colorChanger + 1f);
                colorChangeVector.X = colorChangeVector.Y * vectorMult;
                Color newSandYellow = Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.Transparent, sandYellow, colorChanger * 2f);
                if (colorChanger > 0.5f) {
                    newSandYellow = Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.Transparent, sandYellow, 2f - colorChanger * 2f);
                }
                newSandYellow.A = (byte)(newSandYellow.A * 0.5f);
                newSandYellow *= trackerClamp;
                spinArea *= colorChangeVector * 100f;
                spinArea.Y = 0f;
                spinArea.X = 0f;
                spinArea += new Vector2(sizeModdingVector2.X, k) - Main.screenPosition;
                Main.EntitySpriteDraw(texture2D23, spinArea, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), newSandYellow, aiTrackMult + incStorageMult, smallRect, 1f + lowerColorChanger, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
