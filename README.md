# acfg
application configuration by json files


## Getting started

### usage

```csharp

class MyConfig
    : acfg.Config
{
    public string Property1 { get; set; } = "default value";
    public int Property2 { get; set; } = -1;
    public float Property3 { get; set; } = 3.14f;
    public Dictionary<string, string> DicProperty { get; set; } = new () {
        ["key1"] = "value1",
        ["key2"] = "value2",
    };

    public void SampleUsage()
    {
        var devConfig = new MyConfig();
        // config overwriting cascade
        var config = devConfig // from constructor
            .OverwriteFromJson(DEFAULT_CONFIG_JSON)
            .OverwriteFromJson(USER_CONFIG_JSON);

        // saving to file only differences to defaultConfig
        System.IO.File.WriteAllText(
            config.ToJson(defaultConfig)
        );
    }
}
```
