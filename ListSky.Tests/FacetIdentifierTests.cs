using System.Text.RegularExpressions;
using FishyFlip.Models;
using ListSky.Lib.Composition;
using ListSky.Lib.Connectors;
using ListSky.Lib.DTO;

namespace ListSky.Tests;

[TestClass]
public class FacetIdentifierTests
{
    private ATConnection connection = null!;
    private ATFacetIdentifier identifier = null!;
    private Session? session;

    [TestInitialize]
    public async Task TestInit()
    {
        var config = Config.FromEnv();
        connection = new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
        session = await connection.ConnectAsync();
        identifier = new ATFacetIdentifier(connection);
    }

    [TestMethod]
    public void FacetIdentifier_Created()
    {
        Assert.IsNotNull(identifier);
    }

    [TestMethod]
    public async Task FacetIdentifier_CanIdentify_Mentions()
    {
        var text = "Hello to @instantiator.bsky.social!";
        var matches = Regex.Matches(text, ATFacetIdentifier.RGX_MENTIONS);
        Assert.AreEqual(1, matches.Count());
        Assert.AreEqual("@instantiator.bsky.social", matches[0].Value);

        var facets = await identifier.MentionsFromAsync(text);
        Assert.AreEqual(1, facets.Count());
        Assert.AreEqual(9, facets.ElementAt(0).Index!.ByteStart);
        Assert.AreEqual(34, facets.ElementAt(0).Index!.ByteEnd);
        Assert.AreEqual(1, facets.ElementAt(0).Features!.Count());
    }

    [TestMethod]
    public void FacetIdentifier_CanIdentify_Links()
    {
        var text = "Hello to @instantiator.bsky.social at https://instantiator.dev";
        var matches = Regex.Matches(text, ATFacetIdentifier.RGX_LINKS);
        Assert.AreEqual(1, matches.Count());
        Assert.AreEqual("https://instantiator.dev", matches[0].Value);

        var facets = identifier.LinksFrom(text);
        Assert.AreEqual(1, facets.Count());
        Assert.AreEqual(38, facets.ElementAt(0).Index!.ByteStart);
        Assert.AreEqual(62, facets.ElementAt(0).Index!.ByteEnd);
        Assert.AreEqual(1, facets.ElementAt(0).Features!.Count());
    }

}