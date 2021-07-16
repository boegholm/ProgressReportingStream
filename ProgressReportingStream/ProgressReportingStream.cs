using System;
using System.IO;

namespace Utils
{
    public delegate void StreamUpdate(Stream sender, long progress, long totalProgress);
    public class ProgressReportingStream : Stream
    {
        public event StreamUpdate WriteProgress;
        public event StreamUpdate ReadProgress;
        long totalBytesRead;
        long totalBytesWritten;
        private void UpdateRead(long read)
        {
            totalBytesRead += read;
            ReadProgress?.Invoke(this, read, totalBytesRead);
        }
        private void UpdateWritten(long written)
        {
            totalBytesWritten += written;
            WriteProgress?.Invoke(this, written, totalBytesWritten);
        }

        Stream InnerStream { get; init; }

        public override bool CanRead => InnerStream.CanRead;

        public override bool CanSeek => InnerStream.CanSeek;

        public override bool CanWrite => InnerStream.CanWrite;

        public override long Length => InnerStream.Length;

        public override long Position { get => InnerStream.Position; set => InnerStream.Position = value; }

        public ProgressReportingStream(Stream s)
        {
            this.InnerStream = s;
        }

        public override void Flush()
        {
            InnerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = InnerStream.Read(buffer, offset, count);
            UpdateRead(result);
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return InnerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            InnerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            InnerStream.Write(buffer, offset, count);
            UpdateWritten(count);
        }
    }
}
