using acfg;
using Newtonsoft.Json.Linq;

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
        public string? SomeghingNull { get; set; } = null;
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


    [TestMethod]
    public void OverwriteFromJson()
    {
        var sample1 = new TestConfig();
        Assert.AreEqual(1, sample1.Value1);
        Assert.AreEqual("dev", sample1.Value2);
        sample1.OverwriteFromJson("{ \"Value1\": 2 }");
        Assert.AreEqual(2, sample1.Value1);
        Assert.AreEqual("dev", sample1.Value2);
    }

    [TestMethod]
    public void ToJson()
    {
        var sample1 = new TestConfig();
        var fullJson = sample1.ToJson();
        fullJson
            .ShouldHave("SomeghingNull")
            .ShouldHave("Value1")
            .ShouldHave("Value2")
            .ShouldHave("Value3");
    }

    [TestMethod]
    public void ToJson_ignored()
    {
        var sample1 = new TestConfig();
        var nothing = sample1.ToJson(sample1);
        nothing
            .ShouldBeEmptyObject();

        var sample2 = new TestConfig
        {
            Value2 = "differ", // Value2 만 다름
        };
        var ignoredJson = sample2.ToJson(new TestConfig());
        ignoredJson
            .ShouldNotHave("Value1")
            .ShouldHave("Value2")
            .ShouldNotHave("Value3");
    }

    [TestMethod]
    public void Subclassing()
    {
        var subConfig = new TestSubConfig();
        var json = subConfig.ToJson();
        json
            .ShouldHave("SubValue1") // subclass value
            .ShouldHave("Value1"); // super class value
    }

    [TestMethod]
    public void Scenario_defaultconfig()
    {
        // userConfig 는 defaultConfig 로부터 user 에 의해 변경된
        //  정보만 file 로 저장. defaultConfig 역시 devConfig 로부터
        //  변경된 정보만 저장된 상태로 사용.
        var devConfig = new TestConfig();
        var defaultConfig = new TestConfig
        {
            Value1 = -1,
            Value2 = "default",
        };
        var userConfig = new TestConfig
        {
            Value1 = 1000,
            Value2 = "default",
        };

        // save scenario
        var defaultconfigJson = defaultConfig.ToJson(devConfig);

        defaultconfigJson
            .ShouldHave("Value1")
            .ShouldHave("Value2")
            // devConfig 와 같으면 저장되어있지 않아야 함
            .ShouldNotHave("Value3");

        var userconfigJson = userConfig.ToJson(defaultConfig);
        userconfigJson
            .ShouldHave("Value1")
            // defaultConfig 와 같으면 저장되어있지 않아야 함
            .ShouldNotHave("Value2")
            .ShouldNotHave("Value3");


        // load scenario
        var testUserConfig
            = new TestConfig() // devConfig
            .OverwriteFromJson(defaultconfigJson)
            .OverwriteFromJson(userconfigJson) as TestConfig;
        Assert.IsNotNull(testUserConfig);
        Assert.AreEqual(1000, testUserConfig.Value1);
        Assert.AreEqual("default", testUserConfig.Value2);
    }
}
