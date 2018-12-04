using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Mozlite.Extensions.Storages.Excels
{
    /// <summary>
    /// Excel数据。
    /// </summary>
    /// <typeparam name="TModel">实例类型。</typeparam>
    public class ExcelEnumerable<TModel> : IEnumerable<TModel>
        where TModel : class, new()
    {
        /// <summary>
        /// 工作表名称。
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 当前实体实例接口。
        /// </summary>
        public IEntityType EntityType { get; }

        private readonly List<ExcelColumnDescriptor> _descriptors = new List<ExcelColumnDescriptor>();
        /// <summary>
        /// 初始化类<see cref="ExcelEnumerable{TModel}"/>。
        /// </summary>
        public ExcelEnumerable()
        {
            SheetName = typeof(TModel).GetCustomAttribute<ExcelSheetAttribute>()?.SheetName ?? "sheet1";
            EntityType = typeof(TModel).GetEntityType();
            foreach (var property in EntityType.GetProperties())
            {
                if (property.PropertyInfo.IsDefined(typeof(ExcelColumnAttribute)) ||
                    property.PropertyInfo.IsDefined(typeof(ExcelAttribute)))
                {
                    _descriptors.Add(new ExcelColumnDescriptor(property));
                }
            }
            if (Columns == 0)
                throw new Exception($"当前类型“{typeof(TModel)}”没有任何属性有使用“{typeof(ExcelAttribute)}”或者“{typeof(ExcelColumnAttribute)}”标记。");
        }

        /// <summary>
        /// 初始化类<see cref="ExcelEnumerable{TModel}"/>。
        /// </summary>
        /// <param name="models">列表实例。</param>
        public ExcelEnumerable(IEnumerable<TModel> models)
            : this()
        {
            _models.AddRange(models);
        }

        /// <summary>
        /// 当前对象定义的描述列表。
        /// </summary>
        public List<ExcelColumnDescriptor> Descriptors => _descriptors;

        private readonly List<TModel> _models = new List<TModel>();

        /// <summary>
        /// 获取迭代器。
        /// </summary>
        /// <returns>返回当前迭代实例。</returns>
        public IEnumerator<TModel> GetEnumerator()
        {
            return _models.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 添加模型实例。
        /// </summary>
        /// <param name="model">当前模型实例。</param>
        public void Add(TModel model) => _models.Add(model);

        /// <summary>
        /// 行数。
        /// </summary>
        public int Rows => _models.Count;

        /// <summary>
        /// 列数。
        /// </summary>
        public int Columns => _descriptors.Count;
    }
}