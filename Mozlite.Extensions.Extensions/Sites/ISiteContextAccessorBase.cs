namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// ��ǰ��վ�����ķ�������
    /// </summary>
    public interface ISiteContextAccessorBase : ISingletonService
    {
        /// <summary>
        /// ��ȡ��ǰ��վ�����ġ�
        /// </summary>
        SiteContextBase SiteContext { get; }

        /// <summary>
        /// ���õ�ǰ������ʵ����
        /// </summary>
        /// <param name="domain">������ַ�����Ϊ�����HTTP�������еõ���</param>
        /// <returns>���ص�ǰ��վ������ʵ����</returns>
        SiteContextBase CreateSiteContext(string domain = null);
    }
}