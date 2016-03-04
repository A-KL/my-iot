// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http.Internal;
using System.Threading.Tasks;

namespace System.Net.Http.Handlers
{
    internal class ProgressWriteAsyncResult : AsyncResult
    {
        private readonly Stream _innerStream;
        private readonly ProgressStream _progressStream;
        private readonly int _count;

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is handled as part of IAsyncResult completion.")]
        public ProgressWriteAsyncResult(Stream innerStream, ProgressStream progressStream, byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            : base(callback, state)
        {
            Contract.Assert(innerStream != null);
            Contract.Assert(progressStream != null);
            Contract.Assert(buffer != null);

            _innerStream = innerStream;
            _progressStream = progressStream;
            _count = count;

            try
            {
                var result = innerStream.WriteAsync(buffer, offset, count).ContinueWith(WriteCompletedCallback);
                if (result.IsCompleted)
                {
                    WriteCompleted(result);
                }
            }
            catch (Exception e)
            {
                Complete(true, e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is handled as part of IAsyncResult completion.")]
        private static void WriteCompletedCallback(Task result)
        {
            if (result.IsCompleted)
            {
                return;
            }

            var thisPtr = (ProgressWriteAsyncResult)result.AsyncState;
            try
            {
                thisPtr.WriteCompleted(result);
            }
            catch (Exception e)
            {
                thisPtr.Complete(false, e);
            }
        }

        private void WriteCompleted(Task result)
        {
            result.Wait();
            _progressStream.ReportBytesSent(_count, AsyncState);
            Complete(result.IsCompleted);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<ProgressWriteAsyncResult>(result);
        }
    }
}
