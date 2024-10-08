﻿namespace ListSky.Lib.DTO;

public class ListEntry
{
    // required fields
    public string Name { get; set; } = null!;
    public string Description { get; set;} = null!;
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
}
