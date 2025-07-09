using Terraria.DataStructures;

namespace GloryofGuardian.Content.Classes {
    public class GOGModPlayer : ModPlayer {
        //源提取
        public IEntitySource Source;

        //把炮塔所要使用的数据通过玩家来统筹保存

        //变量
        #region 变量存储

        /// <summary>
        /// 固有炮塔栏位
        /// </summary>
        public int Gslot0 = 1;

        /// <summary>
        /// 当前炮塔栏位
        /// </summary>
        public int Gslot = 0;

        /// <summary>
        /// 炮塔攻击间隔乘算因子
        /// </summary>
        public float GcountR = 1;

        /// <summary>
        /// 炮塔攻击间隔加算因子
        /// </summary>
        public int GcountEx = 0;

        /// <summary>
        /// 预留变量
        /// </summary>
        public int Todo;

        #endregion

        //变量
        #region 饰品

        /// <summary>
        /// 测试饰品孤独残响
        /// </summary>
        public bool residuallonelinessechoes = false;

        #endregion

        public override void PreUpdate() {
            if (residuallonelinessechoes) {
                Gslot += 20;
            }
        }

        public override void PostUpdate() {

        }

        //每帧重置,虽然写在很靠前的位置,实际上在每帧最后作用
        public override void ResetEffects() {
            //这里不能够直接重置某些持续性作用的字段,不然会导致问题,它们被放置在了PreUpdate
            //饰品判定重置
            residuallonelinessechoes = false;//孤独残响
            base.ResetEffects();
        }


    }
}
