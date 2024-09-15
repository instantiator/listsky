using System.Text.RegularExpressions;
using ListSky.Lib.Composition;

namespace ListSky.Tests;

[TestClass]
public class RegexTests
{
    [TestMethod]
    public void RGX_IDENTIFIERS_CanIdentify_Identifiers()
    {
        var text = "Hello @world! @foo @bar.some.thing @baz.some-thing.com";
        var matches = Regex.Matches(text, ATFacetIdentifier.RGX_MENTIONS);
        Assert.AreEqual(4, matches.Count());

        Assert.AreEqual("@world", matches[0].Value);
        Assert.AreEqual("@foo", matches[1].Value);
        Assert.AreEqual("@bar.some.thing", matches[2].Value);
        Assert.AreEqual("@baz.some-thing.com", matches[3].Value);

        Assert.AreEqual(6, matches[0].Index);
        Assert.AreEqual(14, matches[1].Index);
        Assert.AreEqual(19, matches[2].Index);
        Assert.AreEqual(35, matches[3].Index);
   }
}