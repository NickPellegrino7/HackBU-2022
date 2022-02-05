﻿#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE

namespace HeathenEngineering.SteamAPI
{
    public enum ValveItemDefSchemaAttributes
    {
        name,
        description,
        display_type,
        promo,
        drop_start_time,
        price,
        price_category,
        background_color,
        name_color,
        icon_url,
        icon_url_large,
        marketable,
        tradable,
        tags,
        tag_generators,
        store_tags,
        store_images,
        hidden,
        store_hidden,
        use_drop_limit,
        drop_limit,
        drop_interval,
        use_drop_window,
        drop_window,
        drop_max_per_winidow,
        granted_manually,
        use_bundle_price,
        item_slot,
        item_quality,
        purchase_bundle_discount
    }
}
#endif