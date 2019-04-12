namespace Mozlite.Extensions.Storages.Epub
{
    /// <summary>
    /// 都柏林核心元素集。
    /// </summary>
    public class DublinCore
    {
        /// <summary>
        /// 标题，赋予资源的名称。
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 创建者，创建资源内容的主要责任者。
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 主题，资源内容的主题描述。
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 描述，资源内容的解释。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 出版者，使资源成为可获得的责任实体。
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// 其他责任者，资源生存期中做出贡献的其他实体，除制作者/创作者之外的其他撰稿人和贡献者，如插图绘制者、编辑等。
        /// </summary>
        public string Contributor { get; set; }

        /// <summary>
        /// 日期，资源生存周期中的一些事件的相关时间。
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 类型,资源所属的类别，包括种类、体裁、作品级别等描述性术语。
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 格式,资源的物理或数字表现，可包括媒体类型或资源容量，可用于限定资源显示或操作所需要的软件、硬件或其他设备，容量表示数据所占的空间大小等。
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 标识符,资源的唯一标识，如URI（统一资源标识符）、URL（统一资源定位符）、DoI（数字对象标识符）、ISBN（国际标准书号）、ISSN（国际标准刊号）等。
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 语种,描述资源知识内容的语种。
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 来源，对当前资源来源的参照。
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 关联，与其他资源的索引关系，用标识系统来标引参考的相关资源。
        /// </summary>
        public string Relation { get; set; }

        /// <summary>
        /// 覆盖范围，资源应用的范围，包括空间位置（地名或地理坐标）、时代（年代、Et期或日期范围）或权限范围。
        /// </summary>
        public string Coverage { get; set; }

        /// <summary>
        /// 权限， 使用资源的权限信息，它包括知识产权、著作权和各种拥有权。如果没有此项，则表明放弃上述权力。
        /// </summary>
        public string Rights { get; set; }
    }
}