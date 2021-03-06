// Licensed to the Apache Software Foundation(ASF) under one
// or more contributor license agreements.See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied. See the License for the
// specific language governing permissions and limitations
// under the License.

// Copied and modified to have a Reset method from https://github.com/apache/thrift/blob/19baeefd8c38d62085891d7956349601f79448b3/lib/netcore/Thrift/Transports/Client/TMemoryBufferClientTransport.cs

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Thrift.Transports;

namespace Jaeger.Transport.Thrift.Transport.Internal
{
    internal class TMemoryBuffer : TClientTransport
    {
        private readonly MemoryStream _byteStream;
        private bool _isDisposed;

        public TMemoryBuffer()
        {
            _byteStream = new MemoryStream();
        }

        public override bool IsOpen => true;

        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override void Close()
        {
            // do nothing
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int length,
            CancellationToken cancellationToken)
        {
            return await _byteStream.ReadAsync(buffer, offset, length, cancellationToken);
        }

        public override async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            await _byteStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            await _byteStream.WriteAsync(buffer, offset, length, cancellationToken);
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public byte[] GetBuffer()
        {
            return _byteStream.ToArray();
        }

        public void Reset()
        {
            _byteStream.SetLength(0);
        }

        // IDisposable
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _byteStream?.Dispose();
                }
            }
            _isDisposed = true;
        }
    }
}