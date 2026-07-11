using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Stealer.Utils
{
    public class InMemoryZip : IDisposable
    {
        private readonly ConcurrentDictionary<string, byte[]> _entries = new ConcurrentDictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        private bool _disposed;

        public int Count => _entries.Count;

        public void AddFile(string path, byte[] content)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(InMemoryZip));

            if (content == null || content.Length == 0)
                return;

            string normalizedPath = path.Replace('\\', '/').Trim('/');
            if (string.IsNullOrEmpty(normalizedPath))
                return;

            byte[] copy = new byte[content.Length];
            Buffer.BlockCopy(content, 0, copy, 0, content.Length);
            _entries.AddOrUpdate(normalizedPath, copy, (k, old) => copy);
        }

        public void AddTextFile(string path, string text)
        {
            if (!string.IsNullOrEmpty(text))
                AddFile(path, Encoding.UTF8.GetBytes(text));
        }

        public byte[] ToArray()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(InMemoryZip));

            using (var ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true, Encoding.UTF8))
                {
                    foreach (var entry in _entries)
                    {
                        var zipEntry = archive.CreateEntry(entry.Key, CompressionLevel.Fastest);
                        using (var stream = zipEntry.Open())
                        {
                            stream.Write(entry.Value, 0, entry.Value.Length);
                        }
                    }
                }
                return ms.ToArray();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _entries.Clear();
            }
        }
    }
}
