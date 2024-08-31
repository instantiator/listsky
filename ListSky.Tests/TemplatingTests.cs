using HandlebarsDotNet;
using ListSky.Lib.DTO;
using ListSky.Lib.Templating;

namespace ListSky.Tests;

[TestClass]
public class TemplatingTests
{
    [TestMethod]
    public void TemplateFiles_Valid()
    {
        Assert.IsTrue(File.Exists(DocsGenerator.OVERVIEW_TEMPLATE_PATH), $"Overview template not found at: {DocsGenerator.OVERVIEW_TEMPLATE_PATH}");
        Assert.IsTrue(File.Exists(DocsGenerator.LIST_TEMPLATE_PATH), $"List template not found at: {DocsGenerator.OVERVIEW_TEMPLATE_PATH}");

        var overviewTemplate = Handlebars.Compile(File.ReadAllText(DocsGenerator.OVERVIEW_TEMPLATE_PATH));
        var listTemplate = Handlebars.Compile(File.ReadAllText(DocsGenerator.LIST_TEMPLATE_PATH));

        Assert.IsNotNull(overviewTemplate);
        Assert.IsNotNull(listTemplate);
    }

    [TestMethod]
    public void DocsGenerator_GeneratesListDocumentation()
    {
        var config = Config.FromEnv();
        var files = DocsGenerator.Render(config);
        Assert.AreEqual(config.AllListData.Lists.Count() + 1, files.Count());
    }

}

