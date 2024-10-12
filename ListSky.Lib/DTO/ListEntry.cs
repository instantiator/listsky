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
            (!string.IsNullOrWhiteSpace(AccountName_BlueSky) && alt.AccountName_BlueSky == AccountName_BlueSky) ||
            (!string.IsNullOrWhiteSpace(AccountName_GitHub) && alt.AccountName_GitHub == AccountName_GitHub) ||
            (!string.IsNullOrWhiteSpace(AccountName_LinkedIn) && alt.AccountName_LinkedIn == AccountName_LinkedIn) ||
            (!string.IsNullOrWhiteSpace(AccountName_Mastodon) && alt.AccountName_Mastodon == AccountName_Mastodon && alt.Server_Mastodon == alt.Server_Mastodon) ||
            (!string.IsNullOrWhiteSpace(AccountName_Twitter) && alt.AccountName_Twitter == AccountName_Twitter) ||
            (!string.IsNullOrWhiteSpace(AccountName_YouTube) && alt.AccountName_YouTube == AccountName_YouTube) ||
            (!string.IsNullOrWhiteSpace(Url_RssFeed) && alt.Url_RssFeed == Url_RssFeed) ||
            (!string.IsNullOrWhiteSpace(Url_Website) && alt.Url_Website == Url_Website) ||
            (!string.IsNullOrWhiteSpace(Url_Blog) && alt.Url_Blog == Url_Blog));
    }

    public bool DiffersTo(ListEntry alt)
    {
        return alt.Type != Type ||
            alt.Name != Name ||
            alt.Description != Description ||
            alt.AccountName_BlueSky != AccountName_BlueSky ||
            alt.AccountName_GitHub != AccountName_GitHub ||
            alt.AccountName_LinkedIn != AccountName_LinkedIn ||
            alt.AccountName_Mastodon != AccountName_Mastodon ||
            alt.AccountName_Twitter != AccountName_Twitter ||
            alt.AccountName_YouTube != AccountName_YouTube ||
            alt.Url_RssFeed != Url_RssFeed ||
            alt.Url_Website != Url_Website ||
            alt.Url_Blog != Url_Blog;
    }

    public ListEntry UpdateFrom(ListEntry alt)
    {
        Name = !string.IsNullOrEmpty(alt.Name) ? alt.Name : Name;
        Description = !string.IsNullOrEmpty(alt.Description) ? alt.Description : Description;
        Type = alt.Type;

        AccountName_BlueSky = !string.IsNullOrWhiteSpace(alt.AccountName_BlueSky) ? alt.AccountName_BlueSky : AccountName_BlueSky;
        Server_Mastodon = !string.IsNullOrWhiteSpace(alt.Server_Mastodon) ? alt.Server_Mastodon : Server_Mastodon;
        AccountName_Mastodon = !string.IsNullOrWhiteSpace(alt.AccountName_Mastodon) ? alt.AccountName_Mastodon : AccountName_Mastodon;
        AccountName_Twitter = !string.IsNullOrWhiteSpace(alt.AccountName_Twitter) ? alt.AccountName_Twitter : AccountName_Twitter;
        AccountName_LinkedIn = !string.IsNullOrWhiteSpace(alt.AccountName_LinkedIn) ? alt.AccountName_LinkedIn : AccountName_LinkedIn;
        AccountName_YouTube = !string.IsNullOrWhiteSpace(alt.AccountName_YouTube) ? alt.AccountName_YouTube : AccountName_YouTube;
        AccountName_GitHub = !string.IsNullOrWhiteSpace(alt.AccountName_GitHub) ? alt.AccountName_GitHub : AccountName_GitHub;

        Url_RssFeed = !string.IsNullOrWhiteSpace(alt.Url_RssFeed) ? alt.Url_RssFeed : Url_RssFeed;
        Url_Website = !string.IsNullOrWhiteSpace(alt.Url_Website) ? alt.Url_Website : Url_Website;
        Url_Blog = !string.IsNullOrWhiteSpace(alt.Url_Blog) ? alt.Url_Blog : Url_Blog;

        Providence = alt.Providence;
        return this;
    }

    public ListEntry UpdateInto(ListEntry original) => original.UpdateFrom(this);
}
