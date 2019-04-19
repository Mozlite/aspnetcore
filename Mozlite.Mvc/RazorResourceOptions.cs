using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mozlite.Mvc
{
    /// <summary>
    /// Razor库资源选项。
    /// </summary>
    /// <typeparam name="TAssemblyResourceType">程序集资源类型。</typeparam>
    internal class RazorResourceOptions<TAssemblyResourceType> : IPostConfigureOptions<StaticFileOptions>
    {
        private readonly IHostingEnvironment Environment;
        public RazorResourceOptions(IHostingEnvironment environment)
        {
            Environment = environment;
        }

        /// <summary>Invoked to configure a TOptions instance.</summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configured.</param>
        public void PostConfigure(string name, StaticFileOptions options)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
            if (options.FileProvider == null && Environment.WebRootFileProvider == null)
                throw new InvalidOperationException("Missing FileProvider.");

            options.FileProvider = options.FileProvider ?? Environment.WebRootFileProvider;
            options.FileProvider = new CompositeFileProvider(options.FileProvider, new EmbeddedFileProvider());
        }

        internal class EmbeddedFileProvider : IFileProvider
        {
            private readonly Assembly _assembly;
            private readonly string _baseNamespace;
            private readonly DateTimeOffset _lastModified;
            private readonly ConcurrentDictionary<string, string> _manifestResourceNames;

            /// <summary>
            /// 初始化类<see cref="EmbeddedFileProvider"/>。
            /// </summary>
            /// <param name="root">资源根目录。</param>
            public EmbeddedFileProvider(string root = "wwwroot")
            {
                _assembly = typeof(TAssemblyResourceType).Assembly;
                root = $".{root}.";
                var resources = _assembly.GetManifestResourceNames()
                    .Where(x => x.IndexOf(root, StringComparison.OrdinalIgnoreCase) != -1)
                    .ToList();
                if (resources.Count > 0)
                {
                    var baseNamespace = resources.First();
                    var index = baseNamespace.IndexOf(root);
                    _baseNamespace = baseNamespace.Substring(0, index + root.Length);
                    var resourceNames = resources.ToDictionary(x => x.Substring(_baseNamespace.Length));
                    _manifestResourceNames = new ConcurrentDictionary<string, string>(resourceNames, StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    _baseNamespace = _assembly.GetName().Name + root;
                    _manifestResourceNames = new ConcurrentDictionary<string, string>();
                }
                _lastModified = DateTimeOffset.UtcNow;
                if (string.IsNullOrEmpty(_assembly.Location))
                    return;
                try
                {
                    _lastModified = File.GetLastWriteTimeUtc(_assembly.Location);
                }
                catch (PathTooLongException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            public IFileInfo GetFileInfo(string subpath)
            {
                if (string.IsNullOrEmpty(subpath))
                    return new NotFoundFileInfo(subpath);
                var builder = new StringBuilder();
                if (subpath.StartsWith("/", StringComparison.Ordinal))
                    builder.Append(subpath, 1, subpath.Length - 1);
                else
                    builder.Append(subpath);
                for (int length = 0; length < builder.Length; ++length)
                {
                    if (builder[length] == '/' || builder[length] == '\\')
                        builder[length] = '.';
                }

                var resourceName = builder.ToString();
                if (!_manifestResourceNames.TryGetValue(resourceName, out var path))//忽略大小写
                    return new NotFoundFileInfo(resourceName);
                string fileName = Path.GetFileName(subpath);
                if (_assembly.GetManifestResourceInfo(path) == null)
                    return new NotFoundFileInfo(fileName);
                return new EmbeddedResourceFileInfo(_assembly, path, fileName, _lastModified);
            }

            public IDirectoryContents GetDirectoryContents(string subpath)
            {
                if (subpath == null)
                    return NotFoundDirectoryContents.Singleton;
                if (subpath.Length != 0 && !string.Equals(subpath, "/", StringComparison.Ordinal))
                    return NotFoundDirectoryContents.Singleton;
                List<IFileInfo> fileInfoList = new List<IFileInfo>();
                foreach (var manifestResourceName in _manifestResourceNames)
                {
                    fileInfoList.Add(new EmbeddedResourceFileInfo(_assembly, manifestResourceName.Value, manifestResourceName.Key, _lastModified));
                }
                return new EnumerableDirectoryContents(fileInfoList);
            }

            public IChangeToken Watch(string pattern)
            {
                return NullChangeToken.Singleton;
            }
        }

        internal class EnumerableDirectoryContents : IDirectoryContents
        {
            private readonly IEnumerable<IFileInfo> _entries;

            public EnumerableDirectoryContents(IEnumerable<IFileInfo> entries)
            {
                _entries = entries ?? throw new ArgumentNullException(nameof(entries));
            }

            public bool Exists => true;

            public IEnumerator<IFileInfo> GetEnumerator()
            {
                return _entries.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _entries.GetEnumerator();
            }
        }
    }
}