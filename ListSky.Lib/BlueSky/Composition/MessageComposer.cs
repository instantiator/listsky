using System.Text.Json;
using FishyFlip.Models;
using ListSky.Lib.BlueSky.Connectors;

namespace ListSky.Lib.BlueSky.Composition;

public enum Composition { AddedPersonToList };

public class MessageComposer(ATConnection connection)
{
    private ATFacetIdentifier identifier = new ATFacetIdentifier(connection);

    public async Task<List<Facet>> GetAutoFacetsAsync(string message)
    {
        var mentions = await identifier.MentionsFromAsync(message);
        var links = identifier.LinksFrom(message);
        return mentions.Concat(links).ToList();
    }

    public async Task<Tuple<string, IEnumerable<Facet>>> ComposeAsync(Composition composition, Dictionary<string,string> values)
    {
        var message = composition switch
        {
            Composition.AddedPersonToList => $"Added @{values["account"]} to list: {values["listName"]}",
            _ => JsonSerializer.Serialize(values)
        };

        var facets = await GetAutoFacetsAsync(message);

        // assume the list name appears at the end of the message after "list: ", and add a link to the list
        var listStartIndex = message.IndexOf("list: ") + 6;
        var listEndIndex = message.Length;
        var listUrlFacet = new Facet(new FacetIndex(listStartIndex, listEndIndex), FacetFeature.CreateLink(values["listUrl"]));
        facets.Add(listUrlFacet);

        return new Tuple<string, IEnumerable<Facet>>(message, facets);
    }
}