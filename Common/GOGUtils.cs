using System;
using System.Collections.Generic;
using Terraria.Chat;
using Terraria.GameContent.Prefixes;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;

namespace GloryofGuardian.Common
{
    //����һ��������,���ڴ�źܶೣ�õķ���
    public static class GOGUtils
    {
        #region System

        #endregion

        #region ����

        #endregion

        #region ����
        /// <summary>
        /// �������Ƿ���Ч���������
        /// </summary>
        /// <returns>���� true ��ʾ��Ծ������ false ��ʾΪ�ջ����Ѿ������ķǻ�Ծ״̬</returns>
        public static bool Alives(this Player player) {
            if (player == null) return false;
            return player.active && !player.dead;
        }

        /// <summary>
        /// ��˳Ӧ����е�����֮������
        /// </summary>
        /// <param name="vr1"></param>
        /// <param name="vr2"></param>
        /// <returns></returns>
        public static Vector2 To(this Vector2 vr1, Vector2 vr2) {
            return vr2 - vr1;
        }

        /// <summary>
        /// ����֮��ĵ�λ����
        /// </summary>
        /// <param name="vr1"></param>
        /// <param name="vr2"></param>
        /// <returns></returns>
        public static Vector2 Toz(this Vector2 vr1, Vector2 vr2) {
            return (vr2 - vr1).SafeNormalize(Vector2.Zero);
        }

        #endregion

        #region �ж�
        /// <summary>
        /// ���ָ������Ƿ���������,true���false�Ҽ�
        /// </summary>
        /// <param name="player">Ҫ�������</param>
        /// <param name="leftCed">�Ƿ�����������������������</param>
        /// <param name="netCed">�Ƿ��������ͬ�����</param>
        /// <returns>���������ָ�����������򷵻�true�����򷵻�false</returns>
        public static bool PressKey(this Player player, bool leftCed = true, bool netCed = true) {
            return (!netCed || Main.myPlayer == player.whoAmI) && (leftCed ? PlayerInput.Triggers.Current.MouseLeft : PlayerInput.Triggers.Current.MouseRight);
        }

        /// <summary>
        /// �ж���Ļ�Ƿ��ܱ�����赲
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="ignorePlatforms"></param>
        /// <param name="stickToEverything"></param>
        public static void StickToTiles(this Projectile projectile, bool ignorePlatforms, bool stickToEverything) {
            try {
                int xLeft = (int)(projectile.position.X / 16f) - 1;
                int xRight = (int)((projectile.position.X + projectile.width) / 16f) + 2;
                int yBottom = (int)(projectile.position.Y / 16f) - 1;
                int yTop = (int)((projectile.position.Y + projectile.height) / 16f) + 2;
                if (xLeft < 0) {
                    xLeft = 0;
                }
                if (xRight > Main.maxTilesX) {
                    xRight = Main.maxTilesX;
                }
                if (yBottom < 0) {
                    yBottom = 0;
                }
                if (yTop > Main.maxTilesY) {
                    yTop = Main.maxTilesY;
                }
                for (int x = xLeft; x < xRight; x++) {
                    for (int y = yBottom; y < yTop; y++) {
                        Tile tile = Main.tile[x, y];
                        bool platformCheck = true;
                        if (ignorePlatforms)
                            platformCheck = !TileID.Sets.Platforms[tile.TileType] && tile.TileType != TileID.PlanterBox;
                        bool tableCheck = false;
                        if (stickToEverything)
                            tableCheck = Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0;
                        if (tile != null && tile.HasUnactuatedTile && platformCheck && (Main.tileSolid[tile.TileType] || tableCheck)) {
                            Vector2 tileSize;
                            tileSize.X = x * 16;
                            tileSize.Y = y * 16;
                            if (projectile.position.X + projectile.width - 4f > tileSize.X && projectile.position.X + 4f < tileSize.X + 16f && projectile.position.Y + projectile.height - 4f > tileSize.Y && projectile.position.Y + 4f < tileSize.Y + 16f) {
                                projectile.velocity.X = 0f;
                                projectile.velocity.Y = -0.2f;
                            }
                        }
                    }
                }
            } catch {
            }
        }

        /// <summary>
        /// ��������������ʵ������ת�������
        /// </summary>
        /// <param name="wePos"></param>
        /// <returns></returns>
        public static Vector2 WEPosToTilePos(Vector2 wePos) {
            int tilePosX = (int)(wePos.X / 16f);
            int tilePosY = (int)(wePos.Y / 16f);
            Vector2 tilePos = new Vector2(tilePosX, tilePosY);
            tilePos = TileHelper.PTransgressionTile(tilePos);
            return tilePos;
        }
        #endregion

        #region ����
        /// <summary>
        /// Ѱ�Ҿ���ָ��λ�������NPC
        /// </summary>
        /// <param name="origin">��ʼ������λ��</param>
        /// <param name="maxDistanceToCheck">����NPC��������</param>
        /// <param name="ignoreTiles">�ڼ���ϰ���ʱ�Ƿ�������</param>
        /// <param name="bossPriority">�Ƿ�����ѡ��Boss</param>
        /// <param name="ignore">�Ƿ���Ը���Ŀ��</param>//������
        /// <returns>���������NPC��</returns>
        public static NPC InPosClosestNPC(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false, List<int> ignore = null) {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority) {
                bool bossFound = false;
                for (int index2 = 0; index2 < Main.npc.Length; index2++) {
                    if ((bossFound && !Main.npc[index2].boss && Main.npc[index2].type != NPCID.WallofFleshEye) || !Main.npc[index2].CanBeChasedBy()) {
                        continue;
                    }
                    if (ignore != null && ignore.Contains(Main.npc[index2].whoAmI)) {
                        continue;
                    }
                    float extraDistance2 = (Main.npc[index2].width / 2) + (Main.npc[index2].height / 2);
                    bool canHit2 = true;
                    if (extraDistance2 < distance && !ignoreTiles) {
                        canHit2 = Collision.CanHit(origin, 1, 1, Main.npc[index2].Center, 1, 1);
                    }
                    if (Vector2.Distance(origin, Main.npc[index2].Center) < distance + extraDistance2 && canHit2) {
                        if (Main.npc[index2].boss || Main.npc[index2].type == NPCID.WallofFleshEye) {
                            bossFound = true;
                        }
                        distance = Vector2.Distance(origin, Main.npc[index2].Center);
                        closestTarget = Main.npc[index2];
                    }
                }
            } else {
                for (int index = 0; index < Main.npc.Length; index++) {
                    if (Main.npc[index].CanBeChasedBy()) {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);
                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles) {
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);
                        }
                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance + extraDistance && canHit) {
                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            return closestTarget;
        }

        /// <summary>
        /// Ѱ�Ҿ���ָ��λ�ã���ָ�����ҷ����������NPC
        /// </summary>
        /// <param name="origin">��ʼ������λ��</param>
        /// <param name="maxDistanceToCheck">����NPC��������</param>
        /// <param name="ignoreTiles">�ڼ���ϰ���ʱ�Ƿ�������</param>
        /// <param name="bossPriority">�Ƿ�����ѡ��Boss</param>
        /// <param name="direction">���ĸ���������,-1Ϊ��,1Ϊ��</param>
        /// <param name="ignore">�Ƿ���Ը���Ŀ��</param>//������
        /// <returns>���������NPC��</returns>
        public static NPC InDirClosestNPC(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false, int direction = 0, List<int> ignore = null) {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority) {
                bool bossFound = false;
                for (int index2 = 0; index2 < Main.npc.Length; index2++) {
                    if ((bossFound && !Main.npc[index2].boss && Main.npc[index2].type != NPCID.WallofFleshEye) || !Main.npc[index2].CanBeChasedBy()) {
                        continue;
                    }
                    int dir = (Main.npc[index2].Center.X - origin.X) > 0 ? 1 : -1;
                    if (dir * direction == 1) {
                        continue;
                    }
                    if (ignore != null && ignore.Contains(Main.npc[index2].whoAmI)) {
                        continue;
                    }
                    float extraDistance2 = (Main.npc[index2].width / 2) + (Main.npc[index2].height / 2);
                    bool canHit2 = true;
                    if (extraDistance2 < distance && !ignoreTiles) {
                        canHit2 = Collision.CanHit(origin, 1, 1, Main.npc[index2].Center, 1, 1);
                    }
                    if (Vector2.Distance(origin, Main.npc[index2].Center) < distance + extraDistance2 && canHit2) {
                        if (Main.npc[index2].boss || Main.npc[index2].type == NPCID.WallofFleshEye) {
                            bossFound = true;
                        }
                        distance = Vector2.Distance(origin, Main.npc[index2].Center);
                        closestTarget = Main.npc[index2];
                    }
                }
            } else {
                for (int index = 0; index < Main.npc.Length; index++) {
                    if (Main.npc[index].CanBeChasedBy()) {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);
                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles) {
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);
                        }
                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance + extraDistance && canHit) {
                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            return closestTarget;
        }

        /// <summary>
        /// Ѱ�Ҿ���ָ��λ�ã���ָ�����·����������NPC
        /// </summary>
        /// <param name="origin">��ʼ������λ��</param>
        /// <param name="maxDistanceToCheck">����NPC��������</param>
        /// <param name="ignoreTiles">�ڼ���ϰ���ʱ�Ƿ�������</param>
        /// <param name="bossPriority">�Ƿ�����ѡ��Boss</param>
        /// <param name="direction">���ĸ���������,1Ϊ��,2Ϊ��</param>
        /// <param name="ignore">�Ƿ���Ը���Ŀ��</param>//������
        /// <returns>���������NPC��</returns>
        public static NPC InDir2ClosestNPC(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false, int direction = 0, List<int> ignore = null) {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority) {
                bool bossFound = false;
                for (int index2 = 0; index2 < Main.npc.Length; index2++) {
                    if ((bossFound && !Main.npc[index2].boss && Main.npc[index2].type != NPCID.WallofFleshEye) || !Main.npc[index2].CanBeChasedBy()) {
                        continue;
                    }
                    int dir = (Main.npc[index2].Center.Y - origin.Y) > 0 ? 1 : -1;
                    if (dir * direction == 1) {
                        continue;
                    }
                    if (ignore != null && ignore.Contains(Main.npc[index2].whoAmI)) {
                        continue;
                    }
                    float extraDistance2 = (Main.npc[index2].width / 2) + (Main.npc[index2].height / 2);
                    bool canHit2 = true;
                    if (extraDistance2 < distance && !ignoreTiles) {
                        canHit2 = Collision.CanHit(origin, 1, 1, Main.npc[index2].Center, 1, 1);
                    }
                    if (Vector2.Distance(origin, Main.npc[index2].Center) < distance + extraDistance2 && canHit2) {
                        if (Main.npc[index2].boss || Main.npc[index2].type == NPCID.WallofFleshEye) {
                            bossFound = true;
                        }
                        distance = Vector2.Distance(origin, Main.npc[index2].Center);
                        closestTarget = Main.npc[index2];
                    }
                }
            } else {
                for (int index = 0; index < Main.npc.Length; index++) {
                    if (Main.npc[index].CanBeChasedBy()) {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);
                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles) {
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);
                        }
                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance + extraDistance && canHit) {
                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            return closestTarget;
        }

        /// <summary>
        /// ����Բ�ε���ײ���
        /// </summary>
        /// <param name="centerPosition">���ĵ�</param>
        /// <param name="radius">�뾶</param>
        /// <param name="targetHitbox">��ײ���������ṹ</param>
        /// <returns></returns>
        public static bool CircularHitboxCollision(Vector2 centerPosition, float radius, Rectangle targetHitbox) {
            if (new Rectangle((int)centerPosition.X, (int)centerPosition.Y, 1, 1).Intersects(targetHitbox)) {
                return true;
            }

            float distanceToTopLeft = Vector2.Distance(centerPosition, targetHitbox.TopLeft());
            float distanceToTopRight = Vector2.Distance(centerPosition, targetHitbox.TopRight());
            float distanceToBottomLeft = Vector2.Distance(centerPosition, targetHitbox.BottomLeft());
            float distanceToBottomRight = Vector2.Distance(centerPosition, targetHitbox.BottomRight());
            float closestDistance = distanceToTopLeft;

            if (distanceToTopRight < closestDistance) {
                closestDistance = distanceToTopRight;
            }

            if (distanceToBottomLeft < closestDistance) {
                closestDistance = distanceToBottomLeft;
            }

            if (distanceToBottomRight < closestDistance) {
                closestDistance = distanceToBottomRight;
            }

            return closestDistance <= radius;
        }

        #endregion

        #region ��Ϊ

        /// <summary>
        /// ��ͨ��׷����Ϊ
        /// </summary>
        /// <param name="entity">��Ҫ���ݵ�ʵ��</param>
        /// <param name="TargetCenter">Ŀ��ص�</param>
        /// <param name="Speed">�ٶ�</param>
        /// <param name="ShutdownDistance">ͣ�ھ���</param>
        /// <returns></returns>
        public static Vector2 ChasingBehavior(this Entity entity, Vector2 TargetCenter, float Speed, float ShutdownDistance = 16) {
            if (entity == null) {
                return Vector2.Zero;
            }

            Vector2 ToTarget = TargetCenter - entity.Center;
            Vector2 ToTargetNormalize = ToTarget.SafeNormalize(Vector2.Zero);
            Vector2 speed = ToTargetNormalize * AsymptoticVelocity(entity.Center, TargetCenter, Speed, ShutdownDistance);
            entity.velocity = speed;
            return speed;
        }

        /// <summary>
        /// ����һ�������ٶ�ֵ
        /// </summary>
        /// <param name="thisCenter">����λ��</param>
        /// <param name="targetCenter">Ŀ��λ��</param>
        /// <param name="speed">�ٶ�</param>
        /// <param name="shutdownDistance">ͣ�ڷ�Χ</param>
        /// <returns></returns>
        public static float AsymptoticVelocity(Vector2 thisCenter, Vector2 targetCenter, float speed, float shutdownDistance) {
            Vector2 toMou = targetCenter - thisCenter;
            float thisSpeed = toMou.LengthSquared() > shutdownDistance * shutdownDistance ? speed : MathHelper.Min(speed, toMou.Length());
            return thisSpeed;
        }

        /// <summary>
        /// ʹ��ǰ�Ƕ�ת��Ŀ��Ƕ�,ÿ��ת��������һ��ֵ
        /// </summary>
        /// <param name="curAngle">���ڽǶ�</param>
        /// <param name="targetAngle">Ŀ��Ƕ�</param>
        /// <param name="maxChange">ת��ֵ</param>
        /// <returns></returns>
        public static float RotTowards(this float curAngle, float targetAngle, float maxChange) {
            curAngle = MathHelper.WrapAngle(curAngle);
            targetAngle = MathHelper.WrapAngle(targetAngle);
            if (curAngle < targetAngle) {
                if (targetAngle - curAngle > (float)Math.PI) {
                    curAngle += (float)Math.PI * 2f;
                }
            } else if (curAngle - targetAngle > (float)Math.PI) {
                curAngle -= (float)Math.PI * 2f;
            }

            curAngle += MathHelper.Clamp(targetAngle - curAngle, 0f - maxChange, maxChange);
            return MathHelper.WrapAngle(curAngle);
        }

        #endregion

        #region �µ�ǰ׺ѡȡ�߼�

        internal static int GetReworkedReforge(Item item, UnifiedRandom rand, int currentPrefix) {
            GloryofGuardianMod mod = GloryofGuardianMod.Instance;
            int GetCalPrefix(string name) {
                bool found = mod.TryFind(name, out ModPrefix ret);
                return found ? ret.Type : 0;
            }

            // Ĭ��ѡ��
            int prefix = -1;

            // ��Ʒ
            if (item.accessory) {
                int accRerolls = 0;
                int[][] accessoryReforgeTiers = new int[][]
                {
            /* 0 */ new int[] { PrefixID.Hard, PrefixID.Jagged, PrefixID.Brisk, PrefixID.Wild }, //, GetCalPrefix("Quiet") },
            /* 1 */ new int[] { PrefixID.Guarding, PrefixID.Spiked, PrefixID.Precise, PrefixID.Fleeting, PrefixID.Rash}, //, GetCalPrefix("Cloaked") },
            /* 2 */ new int[] { PrefixID.Armored, PrefixID.Angry, PrefixID.Hasty2, PrefixID.Intrepid, PrefixID.Arcane}, //, GetCalPrefix("Camouflaged") },
            /* 3 */ new int[] { PrefixID.Warding, PrefixID.Menacing, PrefixID.Lucky, PrefixID.Quick2, PrefixID.Violent}, //, GetCalPrefix("Silent") },
                };

                // �������������������������ͬ��ǰ׺
                do {
                    int newPrefix = IteratePrefix(rand, accessoryReforgeTiers, currentPrefix);
                    if (newPrefix != currentPrefix) {
                        prefix = newPrefix;
                        break;
                    }
                    accRerolls++;
                } while (accRerolls < 20);
            }

            //������ְҵ���޸��ݲ���Ч

            // ��ս �������ߺͱ���
            else if (item.CountsAsClass<MeleeDamageClass>() || item.CountsAsClass<SummonMeleeSpeedDamageClass>()) {
                if (PrefixLegacy.ItemSets.ItemsThatCanHaveLegendary2[item.type]) {
                    int[][] terrarianReforgeTiers = new int[][]
                    {
                /* 0 */ new int[] { PrefixID.Keen, PrefixID.Forceful, PrefixID.Strong },
                /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous },
                /* 2 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Godly },
                /* 3 */ new int[] { PrefixID.Legendary2 },
                    };
                    prefix = IteratePrefix(rand, terrarianReforgeTiers, currentPrefix);
                }
            
                // �����ޣ����ߣ�֧�ִ���ǰ׺��
                else if (PrefixLegacy.ItemSets.SwordsHammersAxesPicks[item.type] || (item.ModItem != null && item.ModItem.MeleePrefix())) {
                    int[][] meleeReforgeTiers = new int[][]
                    {
                /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Light, PrefixID.Heavy, PrefixID.Light, PrefixID.Forceful, PrefixID.Strong },
                /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Pointy, PrefixID.Bulky },
                /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Large, PrefixID.Dangerous, PrefixID.Sharp },
                /* 3 */ new int[] { PrefixID.Massive, PrefixID.Unpleasant, PrefixID.Savage, PrefixID.Superior },
                /* 4 */ new int[] { PrefixID.Demonic, PrefixID.Deadly2, PrefixID.Godly },
                /* 5 */ new int[] { PrefixID.Legendary } // for non-tools, Light is a mediocre low-tier reforge
                    };
                    int[][] toolReforgeTiers = new int[][]
                    {
                /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Heavy, PrefixID.Forceful, PrefixID.Strong },
                /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Pointy, PrefixID.Bulky },
                /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Large, PrefixID.Dangerous, PrefixID.Sharp },
                /* 3 */ new int[] { PrefixID.Massive, PrefixID.Unpleasant, PrefixID.Savage, PrefixID.Superior },
                /* 4 */ new int[] { PrefixID.Demonic, PrefixID.Deadly2, PrefixID.Godly },
                /* 5 */ new int[] { PrefixID.Legendary, PrefixID.Light } // for some tools, light is better than legendary. for others, it's equal
                    };
            
                    var tierListToUse = (item.pick > 0 || item.axe > 0 || item.hammer > 0) ? toolReforgeTiers : meleeReforgeTiers;
                    prefix = IteratePrefix(rand, tierListToUse, currentPrefix);
                }
            
                // ���������ϣ���ì��
                else {
                    int[][] meleeNoSpeedReforgeTiers = new int[][]
                    {
                /* 0 */ new int[] { PrefixID.Keen, PrefixID.Forceful, PrefixID.Strong },
                /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous },
                /* 2 */ new int[] { PrefixID.Superior, PrefixID.Demonic },
                /* 3 */ new int[] { PrefixID.Godly }
                    };
                    prefix = IteratePrefix(rand, meleeNoSpeedReforgeTiers, currentPrefix);
                }
            }
            
            // Զ��
            else if (item.CountsAsClass<RangedDamageClass>()) {
                int[][] rangedReforgeTiers = new int[][]
                {
            /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Powerful, PrefixID.Forceful, PrefixID.Strong },
            /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Intimidating },
            /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Hasty, PrefixID.Staunch, PrefixID.Unpleasant },
            /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Sighted },
            /* 4 */ new int[] { PrefixID.Godly, PrefixID.Rapid, /* ranged Deadly */ PrefixID.Deadly, /* universal Deadly */ PrefixID.Deadly2 },
            /* 5 */ new int[] { PrefixID.Unreal }
                };
                prefix = IteratePrefix(rand, rangedReforgeTiers, currentPrefix);
            }
            
            // ħ��
            else if (item.CountsAsClass<MagicDamageClass>() || item.CountsAsClass<MagicSummonHybridDamageClass>()) {
                int[][] magicReforgeTiers = new int[][]
                {
            /* 0 */ new int[] { PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Furious, PrefixID.Forceful, PrefixID.Strong },
            /* 1 */ new int[] { PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Taboo, PrefixID.Manic },
            /* 2 */ new int[] { PrefixID.Murderous, PrefixID.Agile, PrefixID.Adept, PrefixID.Celestial, PrefixID.Unpleasant },
            /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Mystic },
            /* 4 */ new int[] { PrefixID.Godly, PrefixID.Masterful, PrefixID.Deadly2 },
            /* 5 */ new int[] { PrefixID.Mythical }
                };
                prefix = IteratePrefix(rand, magicReforgeTiers, currentPrefix);
            }
            
            // �ٻ�(��������)
            else if (item.CountsAsClass<SummonDamageClass>()) {
                int[][] summonReforgeTiers = new int[][]
                {
            /* 0 */ new int[] { PrefixID.Nimble, PrefixID.Furious },
            /* 1 */ new int[] { PrefixID.Forceful, PrefixID.Strong, PrefixID.Quick, PrefixID.Taboo, PrefixID.Manic },
            /* 2 */ new int[] { PrefixID.Hurtful, PrefixID.Adept, PrefixID.Celestial },
            /* 3 */ new int[] { PrefixID.Superior, PrefixID.Demonic, PrefixID.Mystic, PrefixID.Deadly2 },
            /* 4 */ new int[] { PrefixID.Masterful, PrefixID.Godly },
            /* 5 */ new int[] { PrefixID.Mythical, PrefixID.Ruthless }
                };
                prefix = IteratePrefix(rand, summonReforgeTiers, currentPrefix);
            }

            // ����
            else if (item.CountsAsClass<GuardianDamageClass>()) {
                int[][] GuardianReforgeTiers = new int[][]
                {
            /* 0 */ new int[] { GetCalPrefix("Scrapped") , GetCalPrefix("Damaged") , GetCalPrefix("ShortCircuited") ,
                                GetCalPrefix("Silent") , GetCalPrefix("Blooey") ,
                                GetCalPrefix("Sensitive") , GetCalPrefix("Burdened") ,
                                GetCalPrefix("Precise") , GetCalPrefix("Overclocked") , GetCalPrefix("Stainless") ,
                                GetCalPrefix("Peerless") , GetCalPrefix("Excellent"), GetCalPrefix("Classic") },
                                

            // /* 0 */ new int[] { GetCalPrefix("Scrapped") , GetCalPrefix("Damaged") , GetCalPrefix("ShortCircuited") },
            // /* 1 */ new int[] { GetCalPrefix("Silent") , GetCalPrefix("Blooey") },
            // /* 2 */ new int[] { GetCalPrefix("Sensitive") , GetCalPrefix("Burdened") },
            // /* 3 */ new int[] { GetCalPrefix("Precise") , GetCalPrefix("Overclocked") , GetCalPrefix("Stainless") },
            // /* 4 */ new int[] { GetCalPrefix("Peerless") , GetCalPrefix("Excellent"), GetCalPrefix("Classic") },
            // /* 5 */ //
                };
                prefix = IteratePrefix(rand, GuardianReforgeTiers, currentPrefix);
            }

            return prefix;
        }

        private static int GetPrefixTier(int[][] tiers, int currentPrefix) {
            for (int checkingTier = 0; checkingTier < tiers.Length; ++checkingTier) {
                int[] tierList = tiers[checkingTier];
                for (int i = 0; i < tierList.Length; ++i)
                    if (tierList[i] == currentPrefix)
                        return checkingTier;
            }

            // ��⵽��Чǰ׺���ͷ��ʼѭ��
            return -1;
        }

        private static int IteratePrefix(UnifiedRandom rand, int[][] reforgeTiers, int currentPrefix) {
            int currentTier = GetPrefixTier(reforgeTiers, currentPrefix);

            // �Ȳ�����������
            int newTier = currentTier == reforgeTiers.Length - 1 ? currentTier : currentTier + 1;
            return rand.Next(reforgeTiers[newTier]);
        }

        #endregion

        #region ǰ׺ע��

        //�ڳ����µ���Ҫ֮ǰ������ֻ����ǰ׺
        public static int RandomGuardianPrefix() {
            Mod mod = ModContent.GetInstance<GloryofGuardianMod>();
            int GuardianPrefix = Utils.SelectRandom(Main.rand, new int[]
            {
                mod.Find<ModPrefix>("Peerless").Type,
                mod.Find<ModPrefix>("Excellent").Type,
                mod.Find<ModPrefix>("Classic").Type,

                mod.Find<ModPrefix>("Precise").Type,
                mod.Find<ModPrefix>("Overclocked").Type,
                mod.Find<ModPrefix>("Stainless").Type,

                mod.Find<ModPrefix>("Sensitive").Type,
                mod.Find<ModPrefix>("Burdened").Type,

                mod.Find<ModPrefix>("Silent").Type,
                mod.Find<ModPrefix>("Blooey").Type,

                mod.Find<ModPrefix>("Scrapped").Type,
                mod.Find<ModPrefix>("Damaged").Type,
                mod.Find<ModPrefix>("ShortCircuited").Type,

                //PrefixID.Annoying,//�������񼶺����ѵ�һϵ��ԭ��ͨ�ô�׺
            });
            return GuardianPrefix;
        }

        public static bool NegativeGuardianPrefix(int prefix) {
            //Mod mod = ModContent.GetInstance<GloryofGuardianMod>();
            //List<int> badPrefixes = new List<int>()
            //{
            //    mod.Find<ModPrefix>("Bad!").Type,
            //};
            //return badPrefixes.Contains(prefix);
            return false;
        }

        #endregion
    }
}