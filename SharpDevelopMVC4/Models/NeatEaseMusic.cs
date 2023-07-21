using System.Collections.Generic;

namespace NeatEastMusicModel
{
	
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    
    public class AvatarDetail
    {
        public int userType { get; set; }
        public int identityLevel { get; set; }
        public string identityIconUrl { get; set; }
    }

    public class Creator
    {
        public bool defaultAvatar { get; set; }
        public int province { get; set; }
        public int authStatus { get; set; }
        public bool followed { get; set; }
        public string avatarUrl { get; set; }
        public int accountStatus { get; set; }
        public int gender { get; set; }
        public int city { get; set; }
        public int birthday { get; set; }
        public int userId { get; set; }
        public int userType { get; set; }
        public string nickname { get; set; }
        public string signature { get; set; }
        public string description { get; set; }
        public string detailDescription { get; set; }
        public long avatarImgId { get; set; }
        public long backgroundImgId { get; set; }
        public string backgroundUrl { get; set; }
        public int authority { get; set; }
        public bool mutual { get; set; }
        public object expertTags { get; set; }
        public object experts { get; set; }
        public int djStatus { get; set; }
        public int vipType { get; set; }
        public object remarkName { get; set; }
        public int authenticationTypes { get; set; }
        public AvatarDetail avatarDetail { get; set; }
        public bool anchor { get; set; }
        public string avatarImgIdStr { get; set; }
        public string backgroundImgIdStr { get; set; }
        public string avatarImgId_str { get; set; }
    }

    public class Ar
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<object> tns { get; set; }
        public List<object> alias { get; set; }
    }

    public class Al
    {
        public string id { get; set; }
        public string name { get; set; }
        public string picUrl { get; set; }
        public List<object> tns { get; set; }
        public string pic_str { get; set; }
        public object pic { get; set; }
    }

    public class H
    {
        public int br { get; set; }
        public int fid { get; set; }
        public int size { get; set; }
        public int vd { get; set; }
    }

    public class M
    {
        public int br { get; set; }
        public int fid { get; set; }
        public int size { get; set; }
        public int vd { get; set; }
    }

    public class L
    {
        public int br { get; set; }
        public int fid { get; set; }
        public int size { get; set; }
        public int vd { get; set; }
    }

    public class Artist
    {
        public int id { get; set; }
        public string name { get; set; }
        public string picUrl { get; set; }
        public string img1v1Url { get; set; }
    }

    public class AlbumMeta
    {
        public string id { get; set; }
        public string name { get; set; }
        public Artist artist { get; set; }
        public object publishTime { get; set; }
        public int size { get; set; }
        public int copyrightId { get; set; }
        public int status { get; set; }
        public object picId { get; set; }
        public int mark { get; set; }
    }

    public class OriginSongSimpleData
    {
        public string songId { get; set; }
        public string name { get; set; }
        public List<Artist> artists { get; set; }
        public AlbumMeta albumMeta { get; set; }
    }

    public class Track
    {
        public string name { get; set; }
        public string id { get; set; }
        public int pst { get; set; }
        public int t { get; set; }
        public List<Ar> ar { get; set; }
        public string audioType { get; set; }
        public List<string> alia { get; set; }
        public int pop { get; set; }
        public int st { get; set; }
        public string rt { get; set; }
        public int fee { get; set; }
        public int v { get; set; }
        public object crbt { get; set; }
        public string cf { get; set; }
        public Al al { get; set; }
        public int dt { get; set; }
        public H h { get; set; }
        public M m { get; set; }
        public L l { get; set; }
        public object a { get; set; }
        public string cd { get; set; }
        public int no { get; set; }
        public object rtUrl { get; set; }
        public int ftype { get; set; }
        public List<object> rtUrls { get; set; }
        public int djId { get; set; }
        public int copyright { get; set; }
        public int s_id { get; set; }
        public int mark { get; set; }
        public int originCoverType { get; set; }
        public OriginSongSimpleData originSongSimpleData { get; set; }
        public int single { get; set; }
        public object noCopyrightRcmd { get; set; }
        public int mst { get; set; }
        public int cp { get; set; }
        public int mv { get; set; }
        public int rtype { get; set; }
        public object rurl { get; set; }
        public object publishTime { get; set; }
        public List<string> tns { get; set; }
    }

    public class TrackId
    {
        public int id { get; set; }
        public int v { get; set; }
        public int t { get; set; }
        public object at { get; set; }
        public object alg { get; set; }
        public int uid { get; set; }
        public string rcmdReason { get; set; }
        public int? lr { get; set; }
    }

    public class Playlist
    {
        public int id { get; set; }
        public string name { get; set; }
        public long coverImgId { get; set; }
        public string coverImgUrl { get; set; }
        public string coverImgId_str { get; set; }
        public int adType { get; set; }
        public int userId { get; set; }
        public long createTime { get; set; }
        public int status { get; set; }
        public bool opRecommend { get; set; }
        public bool highQuality { get; set; }
        public bool newImported { get; set; }
        public long updateTime { get; set; }
        public int trackCount { get; set; }
        public int specialType { get; set; }
        public int privacy { get; set; }
        public long trackUpdateTime { get; set; }
        public string commentThreadId { get; set; }
        public long playCount { get; set; }
        public long trackNumberUpdateTime { get; set; }
        public int subscribedCount { get; set; }
        public int cloudTrackCount { get; set; }
        public bool ordered { get; set; }
        public string description { get; set; }
        public List<object> tags { get; set; }
        public object updateFrequency { get; set; }
        public int backgroundCoverId { get; set; }
        public object backgroundCoverUrl { get; set; }
        public int titleImage { get; set; }
        public object titleImageUrl { get; set; }
        public object englishTitle { get; set; }
        public object officialPlaylistType { get; set; }
        public List<object> subscribers { get; set; }
        public object subscribed { get; set; }
        public Creator creator { get; set; }
        public List<Track> tracks { get; set; }
        public object videoIds { get; set; }
        public object videos { get; set; }
        public List<TrackId> trackIds { get; set; }
        public int shareCount { get; set; }
        public int commentCount { get; set; }
        public object remixVideo { get; set; }
        public object sharedUsers { get; set; }
        public object historySharedUsers { get; set; }
        public string ToplistType { get; set; }
    }

    public class SearchSongItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<Artist> artists { get; set; }
        public List<Ar> ar { get; set; }
        public AlbumMeta album { get; set; }
        public Al al { get; set; }
        public string audioType { get; set; }
        public int duration { get; set; }
        public int copyrightId { get; set; }
        public int status { get; set; }
        public List<object> alias { get; set; }
        public int rtype { get; set; }
        public int ftype { get; set; }
        public int mvid { get; set; }
        public int fee { get; set; }
        public object rUrl { get; set; }
        public int mark { get; set; }
    }
        
    public class FreeTrialPrivilege
    {
        public bool resConsumable { get; set; }
        public bool userConsumable { get; set; }
    }

    public class ChargeInfoList
    {
        public int rate { get; set; }
        public object chargeUrl { get; set; }
        public object chargeMessage { get; set; }
        public int chargeType { get; set; }
    }

    public class Privilege
    {
        public int id { get; set; }
        public int fee { get; set; }
        public int payed { get; set; }
        public int realPayed { get; set; }
        public int st { get; set; }
        public int pl { get; set; }
        public int dl { get; set; }
        public int sp { get; set; }
        public int cp { get; set; }
        public int subp { get; set; }
        public bool cs { get; set; }
        public int maxbr { get; set; }
        public int fl { get; set; }
        public object pc { get; set; }
        public bool toast { get; set; }
        public int flag { get; set; }
        public bool paidBigBang { get; set; }
        public bool preSell { get; set; }
        public int playMaxbr { get; set; }
        public int downloadMaxbr { get; set; }
        public object rscl { get; set; }
        public FreeTrialPrivilege freeTrialPrivilege { get; set; }
        public List<ChargeInfoList> chargeInfoList { get; set; }
    }

    public class NeatEaseMusic
    {
        public int code { get; set; }
        //public object relatedVideos { get; set; }
        public Playlist playlist { get; set; }
        public List<SearchSongItem> songs { get; set; }
        //public object urls { get; set; }
        //public List<Privilege> privileges { get; set; }
        //public object sharedPrivilege { get; set; }
    }


}