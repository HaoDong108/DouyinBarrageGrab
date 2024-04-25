using System.Collections.Generic;

namespace BarrageGrab.Modles
{
    /// <summary>
    /// 礼物信息
    /// </summary>
    public class WebCastGift
    {
        /// <summary>
        /// 图片信息
        /// </summary>
        public GiftImage image { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 是否通知
        /// </summary>
        public bool notify { get; set; }
        /// <summary>
        /// 持续时长
        /// </summary>
        public int duration { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 用于连麦
        /// </summary>
        public bool for_linkmic { get; set; }
        /// <summary>
        /// 涂鸦
        /// </summary>
        public bool doodle { get; set; }
        /// <summary>
        /// 用于粉丝团
        /// </summary>
        public bool for_fansclub { get; set; }
        /// <summary>
        /// 是否是连击礼物
        /// </summary>
        public bool combo { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 钻石数量
        /// </summary>
        public int diamond_count { get; set; }
        /// <summary>
        /// 是否显示在礼物面板上
        /// </summary>
        public bool is_displayed_on_panel { get; set; }
        /// <summary>
        /// 主要效果 ID
        /// </summary>
        public int primary_effect_id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string region { get; set; }
        /// <summary>
        /// 手动
        /// </summary>
        public string manual { get; set; }
        /// <summary>
        /// 是否为自定义礼物
        /// </summary>
        public bool for_custom { get; set; }
        /// <summary>
        /// 特殊效果
        /// </summary>
        public Dictionary<string, object> special_effects { get; set; }
        /// <summary>
        /// 图标信息
        /// </summary>
        public Icon icon { get; set; }
        /// <summary>
        /// 行为类型
        /// </summary>
        public int action_type { get; set; }

        /// <summary>
        /// 西瓜子数
        /// </summary>
        public int watermelon_seeds { get; set; }

        /// <summary>
        /// 金特效
        /// </summary>
        public string gold_effect { get; set; }

        /// <summary>
        /// 订阅列表
        /// </summary>
        public List<object> subs { get; set; }

        /// <summary>
        /// 金豆数
        /// </summary>
        public int golden_beans { get; set; }

        /// <summary>
        /// 荣誉等级
        /// </summary>
        public int honor_level { get; set; }

        /// <summary>
        /// 物品类型
        /// </summary>
        public int item_type { get; set; }

        /// <summary>
        /// 方案链接
        /// </summary>
        public string scheme_url { get; set; }

        /// <summary>
        /// 事件名称
        /// </summary>
        public string event_name { get; set; }

        /// <summary>
        /// 贵族等级
        /// </summary>
        public int noble_level { get; set; }

        /// <summary>
        /// 引导链接
        /// </summary>
        public string guide_url { get; set; }

        /// <summary>
        /// 惩罚药物
        /// </summary>
        public bool punish_medicine { get; set; }

        /// <summary>
        /// 是否为门户
        /// </summary>
        public bool for_portal { get; set; }

        /// <summary>
        /// 商业文本
        /// </summary>
        public string business_text { get; set; }

        /// <summary>
        /// 是否为CNY礼物
        /// </summary>
        public bool cny_gift { get; set; }

        /// <summary>
        /// App ID
        /// </summary>
        public int app_id { get; set; }

        /// <summary>
        /// VIP等级
        /// </summary>
        public int vip_level { get; set; }

        /// <summary>
        /// 是否灰度
        /// </summary>
        public bool is_gray { get; set; }

        /// <summary>
        /// 灰度方案链接
        /// </summary>
        public string gray_scheme_url { get; set; }

        /// <summary>
        /// 礼物场景
        /// </summary>
        public int gift_scene { get; set; }

        /// <summary>
        /// 触发词语列表
        /// </summary>
        public List<object> trigger_words { get; set; }

        /// <summary>
        /// 礼物Buff信息列表
        /// </summary>
        public List<object> gift_buff_infos { get; set; }

        /// <summary>
        /// 是否为首充礼物
        /// </summary>
        public bool for_first_recharge { get; set; }

        /// <summary>
        /// 发送后动作
        /// </summary>
        public int after_send_action { get; set; }

        /// <summary>
        /// 礼物下线时间
        /// </summary>
        public int gift_offline_time { get; set; }

        /// <summary>
        /// 顶部栏文本
        /// </summary>
        public string top_bar_text { get; set; }

        /// <summary>
        /// 横幅方案链接
        /// </summary>
        public string banner_scheme_url { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool is_locked { get; set; }

        /// <summary>
        /// 请求额外类型
        /// </summary>
        public int req_extra_type { get; set; }

        /// <summary>
        /// 资产ID列表
        /// </summary>
        public List<int> asset_ids { get; set; }

        /// <summary>
        /// 礼物预览信息
        /// </summary>
        public GiftPreviewInfo gift_preview_info { get; set; }

        /// <summary>
        /// 礼物提示信息
        /// </summary>
        public GiftTip gift_tip { get; set; }

        /// <summary>
        /// 需要扫光次数
        /// </summary>
        public int need_sweep_light_count { get; set; }

        /// <summary>
        /// 分组信息列表
        /// </summary>
        public List<GroupInfo> group_info { get; set; }

        /// <summary>
        /// 神秘商店状态
        /// </summary>
        public int mystery_shop_status { get; set; }

        /// <summary>
        /// 可选资产ID列表
        /// </summary>
        public List<object> optional_asset_ids { get; set; }

        /// <summary>
        /// 是否禁用心愿单
        /// </summary>
        public bool disable_wish_list { get; set; }

        /// <summary>
        /// 托盘动态图可翻转
        /// </summary>
        public bool tray_dynamic_img_flippable { get; set; }

        /// <summary>
        /// Pico展示动作
        /// </summary>
        public int pico_show_action { get; set; }

        /// <summary>
        /// 选择的动态效果
        /// </summary>
        public int selected_dynamic_effect { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public int biz_type { get; set; }

        /// <summary>
        /// WebP图片信息
        /// </summary>
        public WebpImage webp_image { get; set; }

        /// <summary>
        /// 礼物来源
        /// </summary>
        public int gift_source { get; set; }





        public class GiftImage
        {
            /// <summary>
            /// 图片 URL 列表
            /// </summary>
            public List<string> url_list { get; set; }
            /// <summary>
            /// 图片 URI
            /// </summary>
            public string uri { get; set; }
            /// <summary>
            /// 图片高度
            /// </summary>
            public int height { get; set; }
            /// <summary>
            /// 图片宽度
            /// </summary>
            public int width { get; set; }
            /// <summary>
            /// 平均颜色
            /// </summary>
            public string avg_color { get; set; }
            /// <summary>
            /// 图片类型
            /// </summary>
            public int image_type { get; set; }
            /// <summary>
            /// 开放的网页 URL
            /// </summary>
            public string open_web_url { get; set; }
            /// <summary>
            /// 是否是动画
            /// </summary>
            public bool is_animated { get; set; }
            /// <summary>
            /// 弹性设置列表
            /// </summary>
            public List<object> flex_setting_list { get; set; }
            /// <summary>
            /// 文本设置列表
            /// </summary>
            public List<object> text_setting_list { get; set; }
        }

        public class Icon
        {
            /// <summary>
            /// 图标 URL 列表
            /// </summary>
            public List<string> url_list { get; set; }
            /// <summary>
            /// 图标 URI
            /// </summary>
            public string uri { get; set; }
            /// <summary>
            /// 图标高度
            /// </summary>
            public int height { get; set; }
            /// <summary>
            /// 图标宽度
            /// </summary>
            public int width { get; set; }
            /// <summary>
            /// 平均颜色
            /// </summary>
            public string avg_color { get; set; }
            /// <summary>
            /// 图标类型
            /// </summary>
            public int image_type { get; set; }
            /// <summary>
            /// 开放的网页 URL
            /// </summary>
            public string open_web_url { get; set; }
            /// <summary>
            /// 是否是动画
            /// </summary>
            public bool is_animated { get; set; }
            /// <summary>
            /// 弹性设置列表
            /// </summary>
            public List<object> flex_setting_list { get; set; }
            /// <summary>
            /// 文本设置列表
            /// </summary>
            public List<object> text_setting_list { get; set; }
        }

        public class GiftTipDisplayTextKeyPiece
        {
            /// <summary>
            /// 键
            /// </summary>
            public string key { get; set; }
            /// <summary>
            /// 默认模式
            /// </summary>
            public string default_pattern { get; set; }
            /// <summary>
            /// 片段列表
            /// </summary>
            public List<object> pieces { get; set; }
            /// <summary>
            /// 模式信息
            /// </summary>
            public Dictionary<string, object> schema_infos { get; set; }
        }

        public class GiftTipDisplayText
        {
            /// <summary>
            /// 显示文本键
            /// </summary>
            public GiftTipDisplayTextKeyPiece display_text { get; set; }
        }

        public class GiftTip
        {
            /// <summary>
            /// 显示文本
            /// </summary>
            public GiftTipDisplayText display_text { get; set; }
            /// <summary>
            /// 背景颜色
            /// </summary>
            public string background_color { get; set; }
            /// <summary>
            /// 剩余时长
            /// </summary>
            public int remaining_duration { get; set; }
            /// <summary>
            /// 倒计时截止时间
            /// </summary>
            public int countdown_deadline_time { get; set; }
        }

        public class GiftPreviewInfo
        {
            /// <summary>
            /// 锁定状态
            /// </summary>
            public int lock_status { get; set; }
            /// <summary>
            /// 客户端阻止使用网页链接
            /// </summary>
            public bool client_block_use_scheme_url { get; set; }
            /// <summary>
            /// 阻止网页链接
            /// </summary>
            public string block_scheme_url { get; set; }
            /// <summary>
            /// 客户端检查剩余钻石
            /// </summary>
            public bool client_check_left_diamond { get; set; }
            /// <summary>
            /// 阻止提示
            /// </summary>
            public string block_toast { get; set; }
        }

        public class GroupInfo
        {
            /// <summary>
            /// 组数量
            /// </summary>
            public int group_count { get; set; }
            /// <summary>
            /// 组文本
            /// </summary>
            public string group_text { get; set; }
        }

        public class WebpImage
        {
            /// <summary>
            /// 图片 URL 列表
            /// </summary>
            public List<string> url_list { get; set; }
            /// <summary>
            /// 图片 URI
            /// </summary>
            public string uri { get; set; }
            /// <summary>
            /// 图片高度
            /// </summary>
            public int height { get; set; }
            /// <summary>
            /// 图片宽度
            /// </summary>
            public int width { get; set; }
            /// <summary>
            /// 平均颜色
            /// </summary>
            public string avg_color { get; set; }
            /// <summary>
            /// 图片类型
            /// </summary>
            public int image_type { get; set; }
            /// <summary>
            /// 开放的网页 URL
            /// </summary>
            public string open_web_url { get; set; }
            /// <summary>
            /// 是否是动画
            /// </summary>
            public bool is_animated { get; set; }
            /// <summary>
            /// 弹性设置列表
            /// </summary>
            public List<object> flex_setting_list { get; set; }
            /// <summary>
            /// 文本设置列表
            /// </summary>
            public List<object> text_setting_list { get; set; }
        }

    }

    /// <summary>
    /// 礼物组信息
    /// </summary>
    public class GiftsInfo
    {
        /// <summary>
        /// 新礼物ID
        /// </summary>
        public int new_gift_id { get; set; }

        /// <summary>
        /// 粉丝团礼物ID列表
        /// </summary>
        public List<int> fansclub_gift_ids { get; set; }

        /// <summary>
        /// 快速礼物ID
        /// </summary>
        public int speedy_gift_id { get; set; }

        /// <summary>
        /// 礼物词语
        /// </summary>
        public string gift_words { get; set; }

        /// <summary>
        /// 礼物组信息列表
        /// </summary>
        public List<GiftGroupInfo> gift_group_infos { get; set; }

        /// <summary>
        /// 免费礼物项列表
        /// </summary>
        public List<object> free_cell_items { get; set; }

        /// <summary>
        /// 荣耀礼物ID列表
        /// </summary>
        public List<int> honor_gift_ids { get; set; }

        /// <summary>
        /// 游戏礼物项列表
        /// </summary>
        public List<object> game_gift_items { get; set; }

        /// <summary>
        /// 贵族礼物ID列表
        /// </summary>
        public List<object> noble_gift_ids { get; set; }

        /// <summary>
        /// 是否隐藏充值入口
        /// </summary>
        public bool hide_recharge_entry { get; set; }

        /// <summary>
        /// 礼物入口图标信息
        /// </summary>
        public GiftEntranceIcon gift_entrance_icon { get; set; }

        /// <summary>
        /// VIP礼物ID列表
        /// </summary>
        public List<object> vip_gift_ids { get; set; }

        /// <summary>
        /// 礼物连击信息列表
        /// </summary>
        public List<GiftComboInfo> gift_combo_infos { get; set; }

        /// <summary>
        /// 快速礼物弹窗信息
        /// </summary>
        public SpeedyGiftPopupInfo speedy_gift_popup_info { get; set; }

        /// <summary>
        /// 首充快速礼物ID
        /// </summary>
        public int first_recharge_speedy_gift_id { get; set; }

        /// <summary>
        /// 消息处理过滤器
        /// </summary>
        public MsgProcessFilter msg_process_filter { get; set; }

        /// <summary>
        /// 额外参数
        /// </summary>
        public ExtraParams extra_params { get; set; }

        /// <summary>
        /// 连击信息
        /// </summary>
        public class GiftComboInfo
        {
            /// <summary>
            /// 连击次数
            /// </summary>
            public int combo_count { get; set; }

            /// <summary>
            /// 连击效果图片信息
            /// </summary>
            public ComboEffectImage combo_effect_img { get; set; }

            public class ComboEffectImage
            {
                /// <summary>
                /// 图片URL列表
                /// </summary>
                public List<string> url_list { get; set; }

                /// <summary>
                /// 图片URI
                /// </summary>
                public string uri { get; set; }

                /// <summary>
                /// 图片高度
                /// </summary>
                public int height { get; set; }

                /// <summary>
                /// 图片宽度
                /// </summary>
                public int width { get; set; }

                /// <summary>
                /// 平均颜色
                /// </summary>
                public string avg_color { get; set; }

                /// <summary>
                /// 图片类型
                /// </summary>
                public int image_type { get; set; }

                /// <summary>
                /// 打开网页链接
                /// </summary>
                public string open_web_url { get; set; }

                /// <summary>
                /// 是否为动画
                /// </summary>
                public bool is_animated { get; set; }

                /// <summary>
                /// 弹性设置列表
                /// </summary>
                public List<object> flex_setting_list { get; set; }

                /// <summary>
                /// 文本设置列表
                /// </summary>
                public List<object> text_setting_list { get; set; }
            }
        }

        public class GiftGroupInfo
        {
            /// <summary>
            /// 礼物组数量
            /// </summary>
            public int group_count { get; set; }

            /// <summary>
            /// 礼物组文本
            /// </summary>
            public string group_text { get; set; }
        }

        public class GiftEntranceIcon
        {
            /// <summary>
            /// 图片URL列表
            /// </summary>
            public List<string> url_list { get; set; }

            // ... (其他字段和类型同 ComboEffectImage)
        }

        public class SpeedyGiftPopupInfo
        {
            /// <summary>
            /// 工具栏图标方案URL
            /// </summary>
            public string toolbar_icon_scheme_url { get; set; }

            /// <summary>
            /// 弹出信息列表
            /// </summary>
            public List<object> pop_up_info { get; set; }

            /// <summary>
            /// 额外信息
            /// </summary>
            public object extra { get; set; }
        }



        public class MsgProcessFilter
        {
            // ... (这里可能还有其他字段，需要查看实际 JSON 结构)
        }

        public class ExtraParams
        {
            /// <summary>
            /// 预测字典
            /// </summary>
            public PredictDict predict_dict { get; set; }
        }

        public class PredictDict
        {
            // ... (这里可能还有其他字段，需要查看实际 JSON 结构)
        }
    }

    /// <summary>
    /// Response 包装器
    /// </summary>
    public class WebCastGiftPack
    {
        /// <summary>
        /// 礼物组
        /// </summary>
        public GiftsInfo gifts_info { get; set; }

        /// <summary>
        /// 礼物列表
        /// </summary>
        public List<WebCastGift> gifts { get; set; }
    }
}
