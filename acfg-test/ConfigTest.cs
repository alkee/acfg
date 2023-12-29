using acfg;

namespace acfg_test;

[TestClass]
public class ConfigTest
{
    public class Custom
    {
        public int Member1 { get; set; } = -1;
        public string Member2 { get; set; } = "member2";
    }

    public class TestConfig
        : Config
    {
        public string? NullValue { get; set; } = null;
        public int Value1 { get; set; } = 1;
        public string Value2 { get; set; } = "dev";
        public Dictionary<string, string> Value3 { get; set; } = new()
        {
            ["key1"] = "value1",
            ["key2"] = "value2"
        };
        public Custom[] Value4 { get; set; } = [
            new Custom(),
            new Custom() { Member1 = -2 },
            new Custom() { Member2 = "xxx" }
        ];
    }

    public class TestSubConfig
        : TestConfig
    {
        public int SubValue1 { get; set; } = 100;
    }


    private readonly TestConfig sample1 = new();
    private readonly TestConfig devConfig = new();
    private readonly TestConfig defaultConfig = new()
    {
        Value2 = "default",
    };
    private readonly TestConfig userConfig = new()
    {
        Value1 = -1,
        Value2 = "user",
    };

    private readonly TestSubConfig subConfig = new();

    [TestMethod]
    public void OverwriteFromJson()
    {
        Assert.AreEqual(1, sample1.Value1);
        Assert.AreEqual("dev", sample1.Value2);
        sample1.OverwriteFromJson("{ \"Value1\": 2 }");
        Assert.AreEqual(2, sample1.Value1);
        Assert.AreEqual("dev", sample1.Value2);
    }

    [TestMethod]
    public void Subclassing()
    {
        var json = subConfig.ToJson();

    }

    [TestMethod]
    public void Scenario1()
    {
        var fullJson = sample1.ToJson();
        Console.WriteLine(fullJson);
        var simplifiedJson = defaultConfig.ToJson(devConfig);

    }
}
