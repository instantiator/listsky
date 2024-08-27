using Google.Protobuf.Collections;

namespace ListSky.Lib.DTO;

public class AllLists
{
    public IEnumerable<ListMetadata> Lists { get; set; } = new List<ListMetadata>();
}