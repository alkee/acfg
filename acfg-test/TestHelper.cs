using Newtonsoft.Json.Linq;

namespace acfg_test;

public static class TestHelper
{
    public static string GetSubJson(this string json, string propertyName)
    {
        var obj = JObject.Parse(json);
        var prop = obj.Property(propertyName);
        if (prop is null) return "";
        return prop.Value.ToString();
    }

    public static string ShouldBeEmptyObject(this string json)
    {
        var obj = JObject.Parse(json);
        Assert.IsNotNull(obj);
        Assert.IsFalse(obj.Properties().Any());
        return json;
    }

    public static string ShouldNotHave(this string json, string propertyName)
    {
        var obj = JObject.Parse(json);
        Assert.IsFalse(obj.ContainsKey(propertyName), $"{propertyName} found in {json}");
        return json;
    }
    public static string ShouldHave(this string json, string propertyName)
    {
        var obj = JObject.Parse(json);
        Assert.IsTrue(obj.ContainsKey(propertyName), $"{propertyName} not found in {json}");
        return json;
    }
}