using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.Projectiles;
using GloryofGuardian.Content.Projectiles.ProjNPC;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;
using Terraria.ID;

namespace GloryofGuardian.Content.Class
{
    public class GOGGlobalNPCs : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        //�����洢
        //Todo
        //����buff
        public bool JavelinDebuffEffect1;
        //Ӱ��buff
        public bool ShadowbladeDebuff;
        //����buff
        public bool OnfireMalice;
        //��������
        public int MythrilJavelin = 0;
        //�����Ƽ�
        public bool CobaltDebuff;
        public bool CobaltDebuff2;
        //��������
        public bool SirenDebuff;
        //���ױ��
        public bool NanoMarkDebuff1;
        public bool NanoMarkDebuff2;

        //ÿ֡buff���裬���ڶ�̬����ʱ��buff
        public override void ResetEffects(NPC npc) {
            JavelinDebuffEffect1 = false;
            ShadowbladeDebuff = false;
            OnfireMalice = false;
            MythrilJavelin = 0;
            CobaltDebuff = false;
            SirenDebuff = false;
            NanoMarkDebuff1 = false;
            NanoMarkDebuff2 = false;
        }

        public override void SetDefaults(NPC entity) {
            // ʹ���ǵĳ�ìdebuff�˺��ж���ԭ��Ĺ�ìһ��
            entity.buffImmune[ModContent.BuffType<JavelinDebuff1>()] = entity.buffImmune[BuffID.BoneJavelin];
        }

        int updatecount = 0;//buff��Ч��ʱ��
        int buffcount1 = 0;//���������ӳ���Ч
        int textcount = 0;
        public override void UpdateLifeRegen(NPC npc, ref int damage) {
            //buff�ܼ�ʱ��
            updatecount++;
            //�����ü�����
            textcount++;

            if (JavelinDebuffEffect1) {
                if (npc.lifeRegen > 0) {
                    npc.lifeRegen = 0;//���������ָ�
                }
                // �����˺�����
                int JavelinCount = 0;
                for (int i = 0; i < 1000; i++) {
                    Projectile p = Main.projectile[i];
                    if (p.active
                        && p.ai[0] == 1f
                        && p.ai[1] == npc.whoAmI) {
                        if (p.type == ModContent.ProjectileType<WildProj>()) {
                            JavelinCount += 2;
                        } else if (p.type == ModContent.ProjectileType<GarrisonProj>()) {
                            JavelinCount += 3;
                        } else if (p.type == ModContent.ProjectileType<MythrilProj2>()) {
                            MythrilJavelin += 1;
                            JavelinCount += 6;
                        }
                    }
                }

                if (MythrilJavelin < 3) {
                    buffcount1 = 0;
                }

                //��������
                if (MythrilJavelin >= 3) {
                    buffcount1 ++;

                    if (!npc.boss && npc.life <= npc.lifeMax * 0.15f) {
                        npc.life = 1;
                        CombatText.NewText(npc.Hitbox,//�������ɵľ��η�Χ
                            Color.Red,//���ֵ���ɫ
                            "9999",//����������Ҫչʾ������
                            false,//dramaticΪtrue����ʹ��������˸��
                            false //dotΪtrue����ʹ��������С��������ʽҲ��ͬ(ԭ��debuff��Ѫ��ʽ)
                            );
                        CombatText.NewText(npc.Hitbox,//�������ɵľ��η�Χ
                            Color.Red,//���ֵ���ɫ
                            "�г�",//����������Ҫչʾ������
                            true,//dramaticΪtrue����ʹ��������˸��
                            false //dotΪtrue����ʹ��������С��������ʽҲ��ͬ(ԭ��debuff��Ѫ��ʽ)
                            );
                        for (int i = 0; i < 1000; i++) {
                            Projectile p = Main.projectile[i];
                            if (p.active && p.ai[0] == 1f && p.ai[1] == npc.whoAmI) {
                                if (p.type == ModContent.ProjectileType<MythrilProj2>()) {
                                    p.Kill();
                                }
                            }
                        }
                    }


                    if (npc.boss && npc.life < npc.lifeMax * 0.05f) {
                        npc.life = 1;
                        CombatText.NewText(npc.Hitbox,//�������ɵľ��η�Χ
                            Color.Red,//���ֵ���ɫ
                            npc.lifeMax,//����������Ҫչʾ������
                            true,//dramaticΪtrue����ʹ��������˸��
                            false //dotΪtrue����ʹ��������С��������ʽҲ��ͬ(ԭ��debuff��Ѫ��ʽ)
                            );
                        CombatText.NewText(npc.Hitbox,//�������ɵľ��η�Χ
                            new Color(89, 194, 201),//���ֵ���ɫ
                            "����",//����������Ҫչʾ������
                            true,//dramaticΪtrue����ʹ��������˸��
                            false //dotΪtrue����ʹ��������С��������ʽҲ��ͬ(ԭ��debuff��Ѫ��ʽ)
                            );
                        Main.NewText(npc.FullName + "�ѱ�ʥ������!", Color.Red);
                        for (int i = 0; i < 1000; i++) {
                            Projectile p = Main.projectile[i];
                            if (p.active && p.ai[0] == 1f && p.ai[1] == npc.whoAmI) {
                                if (p.type == ModContent.ProjectileType<MythrilProj2>()) {
                                    p.Kill();
                                }
                            }
                        }
                    }
                }
                //һ����ӳ���Ч
                if (buffcount1 >=  60) {
                    if (npc.boss && npc.life > npc.lifeMax * 0.05f) {
                        npc.life -= Math.Max((int)(npc.life * 0.02f), 100);
                        CombatText.NewText(npc.Hitbox,//�������ɵľ��η�Χ
                            Color.Red,//���ֵ���ɫ
                            Math.Max((int)(npc.life * 0.02f), 100),//����������Ҫչʾ������
                            false,//dramaticΪtrue����ʹ��������˸��
                            false //dotΪtrue����ʹ��������С��������ʽҲ��ͬ(ԭ��debuff��Ѫ��ʽ)
                            );
                        CombatText.NewText(npc.Hitbox,//�������ɵľ��η�Χ
                            Color.Red,//���ֵ���ɫ
                            "���",//����������Ҫչʾ������
                            true,//dramaticΪtrue����ʹ��������˸��
                            false //dotΪtrue����ʹ��������С��������ʽҲ��ͬ(ԭ��debuff��Ѫ��ʽ)
                            );
                        for (int i = 0; i < 1000; i++) {
                            Projectile p = Main.projectile[i];
                            if (p.active && p.ai[0] == 1f && p.ai[1] == npc.whoAmI) {
                                if (p.type == ModContent.ProjectileType<MythrilProj2>()) {
                                    p.Kill();
                                }
                            }
                        }
                    }

                    buffcount1 = 0;
                }

                // �����������˺�����ȣ��������ŵĵ�λ��2��
                // ��ԭ����ʽ��ʾ������ʧ
                npc.lifeRegen -= JavelinCount * 2;
            }

            if (ShadowbladeDebuff) {
                for (int i = 0; i <= 1; i++) {
                    Dust dust1 = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Wraith, 1f, 1f, 100, Color.Black, 1f);
                    dust1.velocity *= 0.5f;
                    dust1.noGravity = true;
                }
            }

            if (OnfireMalice) {
                npc.lifeRegen -= 40;
            }

            if (SirenDebuff) {
            }

            if (CobaltDebuff) {
                npc.defense = (int)(npc.defDefense * 0.8f);
            }

            if (!CobaltDebuff && CobaltDebuff2) {
                npc.defense = npc.defDefense;
                CobaltDebuff2 = false;
            }

            if (NanoMarkDebuff1 || NanoMarkDebuff2) {
                Lighting.AddLight(npc.Center, 7 * 0.01f, 255 * 0.01f, 255 * 0.01f);//ս��״̬����
            }
        }

        //public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit) {
        //    base.OnHitNPC(npc, target, hit);
        //}

        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers) {
            //֩���ʵ�ַ���
            if (target.type == ModContent.NPCType<SpiderNPC>()) {
                int touchdamage = (int)MathHelper.Max(1, target.defense - (int)(npc.defense * 0.5f));
                if (npc.life <= touchdamage) npc.life = 0;
                else npc.life -= touchdamage;

                CombatText.NewText(npc.Hitbox,//�������ɵľ��η�Χ
                                Color.White,//���ֵ���ɫ
                                touchdamage,//����������Ҫչʾ������
                                false,//dramaticΪtrue����ʹ��������˸��
                                false //dotΪtrue����ʹ��������С��������ʽҲ��ͬ(ԭ��debuff��Ѫ��ʽ)
                                );
            }

            base.ModifyHitNPC(npc, target, ref modifiers);
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers) {
            base.ModifyHitByItem(npc, player, item, ref modifiers);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
            base.ModifyIncomingHit(npc, ref modifiers);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
            if (ShadowbladeDebuff && projectile.type == ModContent.ProjectileType<ShurikenProj>()) {
                modifiers.FinalDamage += npc.defense / 2;
                modifiers.SetMaxDamage(projectile.damage);
            }
            base.ModifyHitByProjectile(npc, projectile, ref modifiers);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) {
            base.OnHitByItem(npc, player, item, hit, damageDone);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) {
            base.OnHitByProjectile(npc, projectile, hit, damageDone);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            return base.PreDraw(npc, spriteBatch, screenPos, Color.Black);
        }

        int drawcount = 0;
        int nanomarkcount = 0;
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            drawcount++;
            if (NanoMarkDebuff1 || NanoMarkDebuff2) nanomarkcount++;
            if (NanoMarkDebuff2) nanomarkcount += 4;
            if (!NanoMarkDebuff1 && !NanoMarkDebuff2) nanomarkcount = 0;
            //��������
            {
                Texture2D texturemj = ModContent.Request<Texture2D>(GOGConstant.Buffs + "MJMark").Value;
                Texture2D texturemj1 = ModContent.Request<Texture2D>(GOGConstant.Buffs + "MJMark1").Value;
                Texture2D texturemj2 = ModContent.Request<Texture2D>(GOGConstant.Buffs + "MJMark2").Value;
                Texture2D texturemj3 = ModContent.Request<Texture2D>(GOGConstant.Buffs + "MJMark3").Value;
                if (MythrilJavelin == 1) texturemj = texturemj1;
                if (MythrilJavelin == 2) texturemj = texturemj2;
                if (MythrilJavelin >= 3) texturemj = texturemj3;

                float color0 = 0;
                if (MythrilJavelin == 1) color0 = 0.5f;
                if (MythrilJavelin == 2) color0 = 0.5f;
                if (MythrilJavelin >= 3) color0 = 0.4f;
                if (npc.boss) color0 *= 1.5f;

                float sca0 = 0;
                if (MythrilJavelin == 1) sca0 = 0.7f;
                if (MythrilJavelin == 2) sca0 = 0.8f;
                if (MythrilJavelin >= 3) sca0 = 1.1f;
                //��������
                float scabreath = (float)Math.Sin(drawcount / 10f) * 0.1f + 1.6f;
                //��������
                float sca1 = Math.Max(Math.Min((npc.width / 16f), (npc.height / 48f)) * 0.25f, 1);

                if (MythrilJavelin >= 1) {
                    //if (MythrilJavelin == 3) {
                    //    Main.spriteBatch.End();
                    //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    //}

                    Main.EntitySpriteDraw(
                    texturemj,
                    npc.Center - Main.screenPosition,
                    null,
                    Color.White * color0 * 1.5f,
                    0,
                    new Vector2(texturemj.Width, texturemj.Height) / 2,
                    sca0 * sca1 * scabreath,
                    SpriteEffects.None
                    );

                    //if (MythrilJavelin == 3) {
                    //    Main.spriteBatch.End();
                    //    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    //}
                }
            }

            //���ױ��
            {
                Texture2D texturenm = ModContent.Request<Texture2D>(GOGConstant.Buffs + "NanoMark").Value;
                Texture2D texturenm0 = ModContent.Request<Texture2D>(GOGConstant.Buffs + "NanoMark0").Value;
                Color color0 = Color.White;
                if (NanoMarkDebuff1 && NanoMarkDebuff2) {
                    NanoMarkDebuff1 = false;
                }
                if (NanoMarkDebuff1) color0 = new Color(7, 255, 255) * 0.8f;
                else if (NanoMarkDebuff2) color0 = Color.Red * 0.8f;
                float sca0 = 0.6f;
                if (NanoMarkDebuff1 && sca0 < 0.6f) sca0 += 0.01f;
                else if (NanoMarkDebuff2 && sca0 > 0.4f) sca0 -= 0.01f;
                if (NanoMarkDebuff1) sca0 *= MathHelper.Min(1, (nanomarkcount / 20f));
                if (NanoMarkDebuff1) sca0 *= 1;

                if (NanoMarkDebuff1 || NanoMarkDebuff2) { 
                    Main.EntitySpriteDraw(
                    texturenm,
                    npc.Center - Main.screenPosition,
                    null,
                    color0,
                    nanomarkcount * 0.01f,//��ת
                    new Vector2(texturenm.Width, texturenm.Height) / 2,
                    sca0 * 0.8f,
                    SpriteEffects.None
                    );
                }
            }

            base.DrawEffects(npc, ref drawColor);
        }
    }
}