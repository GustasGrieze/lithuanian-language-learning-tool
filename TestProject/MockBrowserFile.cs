using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Threading;

namespace TestProject.Helpers
{
    public class MockBrowserFile : IBrowserFile
    {
        private readonly Stream _stream;

        public MockBrowserFile(string name, string contentType, string content)
        {
            Name = name;
            ContentType = contentType;
            _stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            Size = _stream.Length;
            LastModified = DateTimeOffset.Now;
        }

        public string Name { get; }
        public string ContentType { get; }
        public long Size { get; }
        public DateTimeOffset LastModified { get; }

        public Stream OpenReadStream(long maxAllowedSize = 1024 * 1024 * 15, CancellationToken cancellationToken = default)
        {
            return _stream;
        }
    }
}
