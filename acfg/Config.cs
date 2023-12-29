using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace acfg
{
    public abstract class Config
    {
        public Config(bool jsonIndent = true)
        {
            serializer = JsonSerializer.Create();
            serializer.Formatting = jsonIndent
                ? Formatting.Indented
                : Formatting.None;

            // Config 를 상속한 class 를 다시 상속한 후 serialize 하는 경우 "NullValue": null
            //  이 json 에 삽입되어 생성된다.(상속된 class member 들을 구분하기 위함 일 듯)
            //  이를 제거하기위해..
            serializer.ContractResolver = new NullValueExclusionResolver();
        }
        public string ToJson(Config? ignoreValues = null)
        { // ignoreValues 와 같은 값을 갖는 멤버는 저장하지 않음. null 이면 전체 저장
            if (ignoreValues == null)
            {
                return Serialize(this);
            }
            var objIgnore = JObject.FromObject(ignoreValues, serializer);
            var objTarget = JObject.FromObject(this, serializer);
            RemoveEqualProperties(objTarget, objIgnore);
            return objTarget.ToString();
        }

        public Config OverwriteFromJson(string jsonText)
        { // 이미 instance 위에 덮어쓰기(population) 위해
            serializer.Populate(new StringReader(jsonText), this);
            return this;
        }

        private readonly JsonSerializer serializer;

        #region helpers
        private string Serialize(object? obj)
        {
            var sw = new StringWriter();
            serializer.Serialize(sw, obj);
            return sw.ToString();
        }

        private static void RemoveEqualProperties(JObject dst, JObject src)
        {
            // https://github.com/JamesNK/Newtonsoft.Json/issues/2613
            // https://stackoverflow.com/questions/33022993

            var values = dst
                .OfType<JProperty>();
            var removables = new List<string>();
            foreach (var jprop in values)
            {
                var dstValue = jprop.Value;
                var srcValue = src[jprop.Name];
                // recursive 한 동작은 지원하지 않음.. object 내 일부 요솜소만 같을 경우 등..
                if (dstValue?.ToString() == srcValue?.ToString())
                {
                    removables.Add(jprop.Name);
                }
            }
            foreach (var propName in removables)
            {
                dst.Remove(propName);
            }
        }

        private class NullValueExclusionResolver
            : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
                if (property.PropertyName == "NullValue")
                {
                    property.ShouldSerialize = instance => false;
                }
                return property;
            }

        }
        #endregion
    }
}