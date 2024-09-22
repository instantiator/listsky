namespace ListSky.Lib.DTO;

public class ListEntry
{
    // required fields
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ListEntryType Type { get; set; } = ListEntryType.Individual;
    public string AccountName_BlueSky { get; set; } = null!;

    // optional extras
    public string? Server_Mastodon { get; set; }
    public string? AccountName_Mastodon { get; set; }
    public string? AccountName_Twitter { get; set; }
    public string? AccountName_LinkedIn { get; set; }
    public string? AccountName_YouTube { get; set; }
    public string? AccountName_GitHub { get; set; }

    public string? Url_RssFeed { get; set; }
    public string? Url_Website { get; set; }
    public string? Url_Blog { get; set; }

    public string? Providence { get; set; }

    public bool IsProbably(ListEntry alt)
    {
        return alt.Type == Type && (
            (AccountName_BlueSky != null && alt.AccountName_BlueSky == AccountName_BlueSky) ||
            (AccountName_GitHub != null && alt.AccountName_GitHub == AccountName_GitHub) ||
            (AccountName_LinkedIn != null && alt.AccountName_LinkedIn == AccountName_LinkedIn) ||
            (AccountName_Mastodon != null && alt.AccountName_Mastodon == AccountName_Mastodon && alt.Server_Mastodon == alt.Server_Mastodon) ||
            (AccountName_Twitter != null && alt.AccountName_Twitter == AccountName_Twitter) ||
            (AccountName_YouTube != null && alt.AccountName_YouTube == AccountName_YouTube) ||
            (Url_RssFeed != null && alt.Url_RssFeed == Url_RssFeed) ||
            (Url_Website != null && alt.Url_Website == Url_Website) ||
            (Url_Blog != null && alt.Url_Blog == Url_Blog));
    }

    public ListEntry UpdateFrom(ListEntry alt)
    {
        Name = !string.IsNullOrEmpty(alt.Name) ? alt.Name : Name;
        Description = !string.IsNullOrEmpty(alt.Description) ? alt.Description : Description;
        Type = alt.Type;

        AccountName_BlueSky = !string.IsNullOrEmpty(alt.AccountName_BlueSky) ? alt.AccountName_BlueSky : AccountName_BlueSky;
        Server_Mastodon = !string.IsNullOrEmpty(alt.Server_Mastodon) ? alt.Server_Mastodon : Server_Mastodon;
        AccountName_Mastodon = !string.IsNullOrEmpty(alt.AccountName_Mastodon) ? alt.AccountName_Mastodon : AccountName_Mastodon;
        AccountName_Twitter = !string.IsNullOrEmpty(alt.AccountName_Twitter) ? alt.AccountName_Twitter : AccountName_Twitter;
        AccountName_LinkedIn = !string.IsNullOrEmpty(alt.AccountName_LinkedIn) ? alt.AccountName_LinkedIn : AccountName_LinkedIn;
        AccountName_YouTube = !string.IsNullOrEmpty(alt.AccountName_YouTube) ? alt.AccountName_YouTube : AccountName_YouTube;
        AccountName_GitHub = !string.IsNullOrEmpty(alt.AccountName_GitHub) ? alt.AccountName_GitHub : AccountName_GitHub;

        Url_RssFeed = !string.IsNullOrEmpty(alt.Url_RssFeed) ? alt.Url_RssFeed : Url_RssFeed;
        Url_Website = !string.IsNullOrEmpty(alt.Url_Website) ? alt.Url_Website : Url_Website;
        Url_Blog = !string.IsNullOrEmpty(alt.Url_Blog) ? alt.Url_Blog : Url_Blog;

        Providence = alt.Providence;
        return this;
    }

    public ListEntry UpdateInto(ListEntry original) => original.UpdateFrom(this);
}
