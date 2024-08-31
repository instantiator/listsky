using HandlebarsDotNet;
using ListSky.Lib.DTO;

namespace ListSky.Lib.Templating;

public class DocsGenerator
{
    public const string OVERVIEW_TEMPLATE_PATH = "Templates/overview.template.html";
    public const string LIST_TEMPLATE_PATH = "Templates/list.template.html";

    // Templates are Handlebars.NET
    // https://github.com/Handlebars-Net/Handlebars.Net

    public static IEnumerable<DocFile> Render(Config config, string overviewTemplatePath = OVERVIEW_TEMPLATE_PATH, string listTemplatePath = LIST_TEMPLATE_PATH)
    {
        var listTemplateString = File.ReadAllText(listTemplatePath);
        var overviewTemplateString = File.ReadAllText(overviewTemplatePath);
        var listTemplate = Handlebars.Compile(listTemplateString);
        var overviewTemplate = Handlebars.Compile(overviewTemplateString);

        var docs = new List<DocFile>();
        var overviewModel = new OverviewModel()
        {
            AllLists = config.AllListData,
        };

        docs.Add(new DocFile()
        {
            Path = "index.html",
            Html = overviewTemplate(overviewModel),
        });

        docs.AddRange(config.AllListData.Lists.Select(list =>
        {
            var entries = config.ReadList(list.Path_CSV);
            var listModel = new ListModel()
            {
                Metadata = list,
                Entries = entries
            };
            return new DocFile()
            {
                Path = $"lists/{list.Slug}.html",
                Html = listTemplate(listModel),
            };
        }));

        return docs;
    }
}
