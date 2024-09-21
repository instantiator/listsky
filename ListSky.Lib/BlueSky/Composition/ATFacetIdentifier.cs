using System.Text.RegularExpressions;
using FishyFlip.Models;
using ListSky.Lib.BlueSky.Connectors;

namespace ListSky.Lib.BlueSky.Composition;

public class ATFacetIdentifier(ATConnection connection)
{
    public const string RGX_MENTIONS = "(?<name>@[\\w\\d\\.\\-]+)";
    public const string RGX_LINKS = @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)";

    public IEnumerable<Facet> LinksFrom(string message)
    {
        var matches = Regex.Matches(message, RGX_LINKS);
        var facets = new List<Facet>();
        foreach (var match in matches.Cast<Match>())
        {
            facets.Add(new Facet(new FacetIndex(match.Index, match.Index + match.Value.Length), FacetFeature.CreateLink(match.Value)));
        }
        return facets;

    }

    public async Task<IEnumerable<Facet>> MentionsFromAsync(string message)
    {
        var matches = Regex.Matches(message, RGX_MENTIONS);
        var facets = new List<Facet>();
        foreach (var match in matches.Cast<Match>())
        {
            try
            {
                var person = await connection.FindPersonByHandleAsync(match.Value.Trim(['@']));
                if (person != null && person.Did != null)
                {
                    facets.Add(new Facet(new FacetIndex(match.Index, match.Index + match.Value.Length), FacetFeature.CreateMention(person.Did)));
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Person not found for handle: {match.Value}");
                Console.WriteLine(e);
            }
        }
        return facets;
    }

}