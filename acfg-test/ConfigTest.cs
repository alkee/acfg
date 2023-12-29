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
        ShouldHave(fullJson, "SomeghingNull");
        ShouldHave(fullJson, "Value1");
        ShouldHave(fullJson, "Value2");
        ShouldHave(fullJson, "Value3");
    }

    [TestMethod]
    public void ToJson_ignored()
    {
        var sample1 = new TestConfig();
        var nothing = sample1.ToJson(sample1);
        ShouldBeEmptyObject(nothing);

        var sample2 = new TestConfig
        {
            Value2 = "differ", // Value2 만 다름
        };
        var ignoredJson = sample2.ToJson(new TestConfig());
        ShouldNotHave(ignoredJson, "Value1");
        ShouldHave(ignoredJson, "Value2");
        ShouldNotHave(ignoredJson, "Value3");
    }

    [TestMethod]
    public void Subclassing()
    {
        var subConfig = new TestSubConfig();
        var json = subConfig.ToJson();
        ShouldHave(json, "SubValue1"); // subclass value
        ShouldHave(json, "Value1"); // super class value
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
        ShouldHave(defaultconfigJson, "Value1");
        ShouldHave(defaultconfigJson, "Value2");
        // devConfig 와 같으면 저장되어있지 않아야 함
        ShouldNotHave(defaultconfigJson, "Value3");

        var userconfigJson = userConfig.ToJson(defaultConfig);
        ShouldHave(userconfigJson, "Value1");
        // defaultConfig 와 같으면 저장되어있지 않아야 함
        ShouldNotHave(userconfigJson, "Value2");
        ShouldNotHave(userconfigJson, "Value3");


        // load scenario
        var testUserConfig
            = new TestConfig() // devConfig
            .OverwriteFromJson(defaultconfigJson)
            .OverwriteFromJson(userconfigJson) as TestConfig;
        Assert.IsNotNull(testUserConfig);
        Assert.AreEqual(1000, testUserConfig.Value1);
        Assert.AreEqual("default", testUserConfig.Value2);
    }

    #region helpers
    private static string GetJsonProperty(string json, string propertyName)
    {
        var obj = JObject.Parse(json);
        var prop = obj.Property(propertyName);
        if (prop is null) return "";
        return prop.ToString();
    }

    private static void ShouldBeEmptyObject(string json)
    {
        var obj = JObject.Parse(json);
        Assert.IsNotNull(obj);
        Assert.IsFalse(obj.Properties().Any());
    }

    private static void ShouldNotHave(string json, string propertyName)
    {
        var obj = JObject.Parse(json);
        Assert.IsFalse(obj.ContainsKey(propertyName));
    }
    private static void ShouldHave(string json, string propertyName)
    {
        var obj = JObject.Parse(json);
        Assert.IsTrue(obj.ContainsKey(propertyName));
    }
    #endregion
}
