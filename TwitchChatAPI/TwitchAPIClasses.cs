
using System.Collections.Generic;
using Newtonsoft.Json;

public class Links
{
    [JsonProperty("self")]
    public string Self { get; set; }

    [JsonProperty("channel")]
    public string Channel { get; set; }

    [JsonProperty("follows")]
    public string Follows { get; set; }

    [JsonProperty("commercial")]
    public string Commercial { get; set; }

    [JsonProperty("stream_key")]
    public string StreamKey { get; set; }

    [JsonProperty("chat")]
    public string Chat { get; set; }

    [JsonProperty("features")]
    public string Features { get; set; }

    [JsonProperty("subscriptions")]
    public string Subscriptions { get; set; }

    [JsonProperty("editors")]
    public string Editors { get; set; }

    [JsonProperty("teams")]
    public string Teams { get; set; }

    [JsonProperty("videos")]
    public string Videos { get; set; }

}

#region Chatters
public class Chatters
{

    [JsonProperty("_links")]
    public Links Links { get; set; }

    [JsonProperty("chatter_count")]
    public int? ChatterCount { get; set; }

    [JsonProperty("chatters")]
    public Chatter Chatter { get; set; }
}


public class Chatter
{
    public string[] Moderators { get; set; }
    public string[] Staff { get; set; }
    public string[] Admins { get; set; }
    public string[] Global_mods { get; set; }
    public string[] Viewers { get; set; }
}



#endregion

#region TwitchEmotic
public class Twitch
{
    [JsonProperty("emoticons")]
    public List<TwitchEmoticon> Emoticons { get; set; }
    public Links _links { get; set; }
}

public class TwitchEmoticon
{
    [JsonProperty("id")]
    public int? Id { get; set; }
    [JsonProperty("regex")]
    public string Regex { get; set; }
    [JsonProperty("images")]
    public List<TwitchImg> Images { get; set; }
}

public class TwitchImg : IEmotic
{

    [JsonProperty("emoticon_set")]
    public int? EmoticonSet { get; set; }

    public string Url { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }

    [JsonIgnore]
    public string ImageType { get; set; } = "bmp";
}




#endregion

#region BttvEmotic
public class BttvEmotic : IEmotic
{
    [JsonIgnore]
    private string m_url = "";

    [JsonProperty("regex")]
    public string Regex { get; set; }

    [JsonProperty("channel")]
    public string Channel { get; set; }

    [JsonProperty("emoticon_set")]
    public string EmoticonSet { get; set; }

    public string Url
    {
        get => m_url;
        set => m_url = "https:" + value;
    }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string ImageType { get; set; }

}

public class Bttv
{

    [JsonProperty("status")]
    public int? Status { get; set; }


    [JsonProperty("emotes")]
    public List<BttvEmotic> Emotes { get; set; }
}


#endregion

#region StreamInfo


public class Preview
{

    [JsonProperty("small")]
    public string Small { get; set; }

    [JsonProperty("medium")]
    public string Medium { get; set; }

    [JsonProperty("large")]
    public string Large { get; set; }

    [JsonProperty("template")]
    public string Template { get; set; }
}
public class Channel
{

    [JsonProperty("mature")]
    public bool Mature { get; set; }

    [JsonProperty("partner")]
    public bool Partner { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("broadcaster_language")]
    public string BroadcasterLanguage { get; set; }

    [JsonProperty("display_name")]
    public string DisplayName { get; set; }

    [JsonProperty("game")]
    public string Game { get; set; }

    [JsonProperty("language")]
    public string Language { get; set; }

    [JsonProperty("_id")]
    public int? Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public string UpdatedAt { get; set; }

    [JsonProperty("delay")]
    public object Delay { get; set; }

    [JsonProperty("logo")]
    public string Logo { get; set; }

    [JsonProperty("banner")]
    public object Banner { get; set; }

    [JsonProperty("video_banner")]
    public string VideoBanner { get; set; }

    [JsonProperty("background")]
    public object Background { get; set; }

    [JsonProperty("profile_banner")]
    public string ProfileBanner { get; set; }

    [JsonProperty("profile_banner_background_color")]
    public string ProfileBannerBackgroundColor { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("views")]
    public int? Views { get; set; }

    [JsonProperty("followers")]
    public int? Followers { get; set; }

    [JsonProperty("_links")]
    public Links Link { get; set; }
}
public class TwitchStream
{

    [JsonProperty("_id")]
    public long Id { get; set; }

    [JsonProperty("game")]
    public string Game { get; set; }

    [JsonProperty("viewers")]
    public int? Viewers { get; set; }

    [JsonProperty("video_height")]
    public int? VideoHeight { get; set; }

    [JsonProperty("average_fps")]
    public int? AverageFps { get; set; }

    [JsonProperty("delay")]
    public int? Delay { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }

    [JsonProperty("is_playlist")]
    public bool IsPlaylist { get; set; }

    [JsonProperty("stream_type")]
    public string StreamType { get; set; }

    [JsonProperty("preview")]
    public Preview Preview { get; set; }

    [JsonProperty("channel")]
    public Channel Channel { get; set; }

    [JsonProperty("_links")]
    public Links Link { get; set; }
}
public class TwitchStreamInfo
{

    [JsonProperty("stream")]
    public TwitchStream Stream { get; set; }

    [JsonProperty("_links")]
    public Links Link { get; set; }
}


#endregion

internal interface IEmotic
{
    [JsonProperty("url")]
    string Url { get; set; }
    [JsonProperty("width")]
    int? Width { get; set; }
    [JsonProperty("height")]
    int? Height { get; set; }

    [JsonProperty("imageType")]
    string ImageType { get; set; }
}