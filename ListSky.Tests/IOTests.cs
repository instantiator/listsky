using ListSky.Lib.IO;

namespace ListSky.Tests;

[TestClass]
[TestCategory("Unit")]
public class IOTests
{
    [TestMethod]
    public void CsvListIO_CanReadFile()
    {
        var list = CsvListIO.ReadFile("Data/list-trek.csv");
        Assert.IsNotNull(list);
        Assert.IsTrue(list.Count() > 0);
    }

    [TestMethod]
    public void CsvListIO_CanWriteFile()
    {
        var list = CsvListIO.ReadFile("Data/list-trek.csv");
        Assert.IsNotNull(list);
        Assert.IsTrue(list.Count() > 0);

        var path = "Data/list-trek-write.csv";
        CsvListIO.WriteFile(path, list);
        Assert.IsTrue(File.Exists(path));

        var list2 = CsvListIO.ReadFile(path);
        Assert.IsNotNull(list2);
        Assert.AreEqual(list.Count(), list2.Count());
    }

    [TestMethod]
    public async Task CsvListIO_CanReadUri()
    {
        var uri = new Uri("https://docs.google.com/spreadsheets/d/e/2PACX-1vSD7EtY6pg1Bk2OMmQzScWfgfKx7mJFkeLpHsN_DFd9qVSs6o6z5RGHAX_q1iW5b0O9rhOm5h4sxWEv/pub?gid=125969614&single=true&output=csv");
        var list = await CsvListIO.ReadUriAsync(uri);
        Assert.IsNotNull(list);
        Assert.IsTrue(list.Count() > 0);
    }
}