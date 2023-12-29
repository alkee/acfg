using Newtonsoft.Json;

namespace acfg
{
    // public class Config { static Config Load(string jsonText) } 와 같이 구성하면
    //   상속받아 Load 사용하기 어려워보임

    // public abstract class Config
    // {
    // }

    // public class JsonConfigManager<T>
    //     where T : Config, new()
    // {
    //     // public readonly T DevConfig = new T();
    //     public T DevConfig { get; private set; }
    //     public T DefaultConfig { get; private set; }

    //     // public JsonConfigManager(Stream jsonStream)
    //     //     : this(new StreamReader(jsonStream).ReadToEnd())
    //     // {
    //     // }

    //     public JsonConfigManager(T defaultConfig, T devConfig = null)
    //     {
    //         DevConfig = devConfig ?? new T();
    //         DefaultConfig = defaultConfig;
    //     }

    //     public T LoadConfig(string jsonText)
    //     {
    //     }
    // }
}
