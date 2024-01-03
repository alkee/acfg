using acfg;

namespace acfg_test;

[TestClass]
public class ComplexConfigSpec
{
    public class Member1Config
    {
        public int M1i { get; set; } = 11;
        public string M1s { get; set; } = "m1s";
    }
    public class Member2Config
    {
        public Member1Config M2m1 { get; set; } = new()
        {
            M1i = 22,
            M1s = "m2s",
        };
        public int[] M2is = [5, 4, 3, 2, 1];
    }

    public class TestConfig
        : Config
    {
        public Member1Config M1 { get; set; } = new();
        public Member2Config M2 { get; set; } = new();
        public Member1Config[] M1s { get; set; } = [
            new Member1Config { M1i = -1, M1s = "-111" },
            new Member1Config { M1i = -2, M1s = "-222" },
        ];
        public string Root { get; set; } = "root";
    }


    [TestMethod]
    public void Test()
    {
        var src = new TestConfig();
        var changed = new TestConfig
        {
            M1 = new Member1Config { M1i = 222 },
            M2 = new Member2Config
            {
                M2m1 = new Member1Config { M1i = 22, M1s = "111" },
                M2is = [4, 3, 2, 1]
            },
            M1s = [
                new Member1Config { M1i = -1, M1s = "-111" },
                new Member1Config { M1i = 222, M1s = "222" },
            ],
        };

        var srcJson = src.ToJson();
        var changedJson = changed.ToJson();
        var json = changed.ToJson(src);

        json
            .GetSubJson("M2").GetSubJson("M2m1")
            .ShouldNotHave("M1i");

    }

}