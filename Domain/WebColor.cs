namespace shunda.Domain;

/// <summary>
/// 网页安全色
/// </summary>
public class WebColor
{
    /// <summary>
    /// 颜色名称，如：turquoise
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 颜色中文名称, 如：青绿色
    /// </summary>
    public string CnName { get; set; }

    /// <summary>
    /// 颜色16进制，如：#40e0d0
    /// </summary>
    public string Hex { get; set; }

    private WebColor(string name, string cnName, string hex)
    {
        this.Name = name;
        this.CnName = cnName;
        this.Hex = hex;
    }

    public static IList<WebColor> GetColorList()
    {
        return colorList.ToList();
    }

    private static readonly IReadOnlyList<WebColor> colorList = new List<WebColor>() {
        new WebColor("black", "黑色", "#000000"),
        new WebColor("navy", "海军色", "#000080"),
        new WebColor("darkblue", "暗蓝色", "#00008b"),
        new WebColor("mediumblue", "中兰色", "#0000cd"),
        new WebColor("blue", "蓝色", "#0000ff"),
        new WebColor("darkgreen", "暗绿色", "#006400"),
        new WebColor("green", "绿色", "#008000"),
        new WebColor("teal", "水鸭色", "#008080"),
        new WebColor("darkcyan", "暗青色", "#008b8b"),
        new WebColor("deepskyblue", "深天蓝色", "#00bfff"),
        new WebColor("darkturquoise", "暗宝石绿", "#00ced1"),
        new WebColor("mediumspringgreen", "中春绿色", "#00fa9a"),
        new WebColor("lime", "酸橙色", "#00ff00"),
        new WebColor("springgreen", "春绿色", "#00ff7f"),
        new WebColor("aqua", "浅绿色", "#00ffff"),
        new WebColor("midnightblue", "中灰兰色", "#191970"),
        new WebColor("dodgerblue", "闪兰色", "#1e90ff"),
        new WebColor("lightseagreen", "亮海蓝色", "#20b2aa"),
        new WebColor("forestgreen", "森林绿", "#228b22"),
        new WebColor("seagreen", "海绿色", "#2e8b57"),
        new WebColor("darkslategray", "暗瓦灰色", "#2f4f4f"),
        new WebColor("limegreen", "橙绿色", "#32cd32"),
        new WebColor("mediumseagreen", "中海蓝", "#3cb371"),
        new WebColor("turquoise", "青绿色", "#40e0d0"),
        new WebColor("royalblue", "皇家蓝", "#4169e1"),
        new WebColor("steelblue", "钢兰色", "#4682b4"),
        new WebColor("darkslateblue", "暗灰蓝色", "#483d8b"),
        new WebColor("mediumturquoise", "中绿宝石", "#48d1cc"),
        new WebColor("indigo", "靛青色", "#4b0082"),
        new WebColor("darkolivegreen", "暗橄榄绿", "#556b2f"),
        new WebColor("cadetblue", "军兰色", "#5f9ea0"),
        new WebColor("cornflowerblue", "菊兰色", "#6495ed"),
        new WebColor("mediumaquamarine", "中绿色", "#66cdaa"),
        new WebColor("dimgray", "暗灰色", "#696969"),
        new WebColor("slateblue", "石蓝色", "#6a5acd"),
        new WebColor("olivedrab", "深绿褐色", "#6b8e23"),
        new WebColor("slategray", "灰石色", "#708090"),
        new WebColor("lightslategray", "亮蓝灰", "#778899"),
        new WebColor("mediumslateblue", "中暗蓝色", "#7b68ee"),
        new WebColor("lawngreen", "草绿色", "#7cfc00"),
        new WebColor("chartreuse", "黄绿色", "#7fff00"),
        new WebColor("aquamarine", "碧绿色", "#7fffd4"),
        new WebColor("maroon", "粟色", "#800000"),
        new WebColor("purple", "紫色", "#800080"),
        new WebColor("olive", "橄榄色", "#808000"),
        new WebColor("gray", "灰色", "#808080"),
        new WebColor("skyblue", "天蓝色", "#87ceeb"),
        new WebColor("lightskyblue", "亮天蓝色", "#87cefa"),
        new WebColor("blueviolet", "紫罗兰蓝色", "#8a2be2"),
        new WebColor("darkred", "暗红色", "#8b0000"),
        new WebColor("darkmagenta", "暗洋红", "#8b008b"),
        new WebColor("saddlebrown", "重褐色", "#8b4513"),
        new WebColor("darkseagreen", "暗海兰色", "#8fbc8f"),
        new WebColor("lightgreen", "亮绿色", "#90ee90"),
        new WebColor("mediumpurple", "中紫色", "#9370db"),
        new WebColor("darkviolet", "暗紫罗兰色", "#9400d3"),
        new WebColor("palegreen", "苍绿色", "#98fb98"),
        new WebColor("darkorchid", "暗紫色", "#9932cc"),
        new WebColor("sienna", "赭色", "#a0522d"),
        new WebColor("brown", "褐色", "#a52a2a"),
        new WebColor("darkgray", "暗灰色", "#a9a9a9"),
        new WebColor("lightblue", "亮蓝色", "#add8e6"),
        new WebColor("greenyellow", "黄绿色", "#adff2f"),
        new WebColor("paleturquoise", "苍宝石绿", "#afeeee"),
        new WebColor("lightsteelblue", "亮钢兰色", "#b0c4de"),
        new WebColor("powderblue", "粉蓝色", "#b0e0e6"),
        new WebColor("firebrick", "火砖色", "#b22222"),
        new WebColor("darkgoldenrod", "暗金黄色", "#b8860b"),
        new WebColor("mediumorchid", "中粉紫色", "#ba55d3"),
        new WebColor("rosybrown", "褐玫瑰红", "#bc8f8f"),
        new WebColor("darkkhaki", "暗黄褐色", "#bdb76b"),
        new WebColor("silver", "银色", "#c0c0c0"),
        new WebColor("mediumvioletred", "中紫罗兰色", "#c71585"),
        new WebColor("indianred", "印第安红", "#cd5c5c"),
        new WebColor("peru", "秘鲁色", "#cd853f"),
        new WebColor("chocolate", "巧可力色", "#d2691e"),
        new WebColor("tan", "茶色", "#d2b48c"),
        new WebColor("lightgray", "亮灰色", "#d3d3d3"),
        new WebColor("thistle", "蓟色", "#d8bfd8"),
        new WebColor("orchid", "淡紫色", "#da70d6"),
        new WebColor("goldenrod", "金麒麟色", "#daa520"),
        new WebColor("palevioletred", "苍紫罗兰色", "#db7093"),
        new WebColor("crimson", "暗深红色", "#dc143c"),
        new WebColor("gainsboro", "淡灰色", "#dcdcdc"),
        new WebColor("plum", "洋李色", "#dda0dd"),
        new WebColor("burlywood", "实木色", "#deb887"),
        new WebColor("lightcyan", "亮青色", "#e0ffff"),
        new WebColor("lavender", "淡紫色", "#e6e6fa"),
        new WebColor("darksalmon", "暗肉色", "#e9967a"),
        new WebColor("violet", "紫罗兰色", "#ee82ee"),
        new WebColor("palegoldenrod", "苍麒麟色", "#eee8aa"),
        new WebColor("lightcoral", "亮珊瑚色", "#f08080"),
        new WebColor("khaki", "黄褐色", "#f0e68c"),
        new WebColor("aliceblue", "艾利斯兰", "#f0f8ff"),
        new WebColor("honeydew", "蜜色", "#f0fff0"),
        new WebColor("azure", "天蓝色", "#f0ffff"),
        new WebColor("sandybrown", "沙褐色", "#f4a460"),
        new WebColor("wheat", "浅黄色", "#f5deb3"),
        new WebColor("beige", "米色", "#f5f5dc"),
        new WebColor("whitesmoke", "烟白色", "#f5f5f5"),
        new WebColor("mintcream", "薄荷色", "#f5fffa"),
        new WebColor("ghostwhite", "幽灵白", "#f8f8ff"),
        new WebColor("salmon", "鲜肉色", "#fa8072"),
        new WebColor("antiquewhite", "古董白", "#faebd7"),
        new WebColor("linen", "亚麻色", "#faf0e6"),
        new WebColor("lightgoldenrodyellow", "亮金黄色", "#fafad2"),
        new WebColor("oldlace", "老花色", "#fdf5e6"),
        new WebColor("red", "红色", "#ff0000"),
        new WebColor("fuchsia", "紫红色", "#ff00ff"),
        new WebColor("deeppink", "深粉红色", "#ff1493"),
        new WebColor("orangered", "红橙色", "#ff4500"),
        new WebColor("tomato", "西红柿色", "#ff6347"),
        new WebColor("hotpink", "热粉红色", "#ff69b4"),
        new WebColor("coral", "珊瑚色", "#ff7f50"),
        new WebColor("darkorange", "暗桔黄色", "#ff8c00"),
        new WebColor("lightsalmon", "亮肉色", "#ffa07a"),
        new WebColor("orange", "橙色", "#ffa500"),
        new WebColor("lightpink", "亮粉红色", "#ffb6c1"),
        new WebColor("pink", "粉红色", "#ffc0cb"),
        new WebColor("gold", "金色", "#ffd700"),
        new WebColor("peachpuff", "桃色", "#ffdab9"),
        new WebColor("navajowhite", "纳瓦白", "#ffdead"),
        new WebColor("moccasin", "鹿皮色", "#ffe4b5"),
        new WebColor("bisque", "桔黄色", "#ffe4c4"),
        new WebColor("mistyrose", "浅玫瑰色", "#ffe4e1"),
        new WebColor("blanchedalmond", "白杏色", "#ffebcd"),
        new WebColor("papayawhip", "番木色", "#ffefd5"),
        new WebColor("lavenderblush", "淡紫红", "#fff0f5"),
        new WebColor("seashell", "海贝色", "#fff5ee"),
        new WebColor("cornsilk", "米绸色", "#fff8dc"),
        new WebColor("lemonchiffon", "柠檬绸色", "#fffacd"),
        new WebColor("floralwhite", "花白色", "#fffaf0"),
        new WebColor("snow", "雪白色", "#fffafa"),
        new WebColor("yellow", "黄色", "#ffff00"),
        new WebColor("lightyellow", "亮黄色", "#ffffe0"),
        new WebColor("ivory", "象牙色", "#fffff0"),
        new WebColor("white", "白色", "#ffffff"),
    };
}