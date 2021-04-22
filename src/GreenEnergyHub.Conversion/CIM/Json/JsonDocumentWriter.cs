// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GreenEnergyHub.Conversion.CIM.Components;

namespace GreenEnergyHub.Conversion.CIM.Json
{
    public class JsonDocumentWriter : IAsyncDisposable, IDisposable
    {
        private readonly Stack<Action<Utf8JsonWriter>> _closeOperations;
        private readonly JsonPayloadWriter _payloadWriter;
        private Utf8JsonWriter? _writer;
        private bool _documentStartTagWritten;
        private bool _marketDocumentWritten;
        private bool _payloadStartTagWritten;

        public JsonDocumentWriter(Utf8JsonWriter writer, JsonPayloadWriter payloadWriter)
        {
            _writer = writer;
            _payloadWriter = payloadWriter;
            _closeOperations = new Stack<Action<Utf8JsonWriter>>(5);
        }

        public void WriteDocument(MarketDocument document)
        {
            if (_writer == null) throw new ObjectDisposedException(nameof(_writer), "Object has been disposed");
            if (_documentStartTagWritten == false)
            {
                _writer.WriteStartObject();
                AddCloseOperation(wrt => wrt.WriteEndObject());
            }

            if (_marketDocumentWritten) return;

            _documentStartTagWritten = true;
            _marketDocumentWritten = true;

            _writer.WriteString(PropertyNames.mRID, document.MRid);
            _writer.WriteString(PropertyNames.type, document.Type.Value);
            _writer.WriteDateTime(PropertyNames.createdDateTime, document.CreatedDateTime);

            _writer.WriteMarketParticipant(PropertyNames.SenderMarketParticipant, document.Sender);
            _writer.WriteMarketParticipant(PropertyNames.ReceiverMarketParticipant, document.Receiver);
        }

        public void WritePayload(MktActivityRecord record)
        {
            if (_writer == null) throw new ObjectDisposedException(nameof(_writer), "Object has been disposed");
            if (_payloadStartTagWritten == false) WritePayloadTag();

            _payloadWriter.WritePayload(_writer, record);
        }

        public void Close()
        {
            if (_writer == null) throw new ObjectDisposedException(nameof(_writer), "Object has been disposed");

            while (_closeOperations.TryPop(out var closeOperation))
            {
                closeOperation.Invoke(_writer);
            }

            Flush();
        }

        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            if (_writer == null) throw new ObjectDisposedException(nameof(_writer), "Object has been disposed");

            while (_closeOperations.TryPop(out var closeOperation))
            {
                closeOperation.Invoke(_writer);
            }

            return FlushAsync(cancellationToken);
        }

        public void Flush()
        {
            if (_writer == null) throw new ObjectDisposedException(nameof(_writer), "Object has been disposed");
            _writer.Flush();
        }

        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            if (_writer == null) throw new ObjectDisposedException(nameof(_writer), "Object has been disposed");
            return _writer.FlushAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal void AddCloseOperation(Action<Utf8JsonWriter> operation) => _closeOperations.Push(operation);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) _writer?.Dispose();
            _writer = null;
        }

        [SuppressMessage("Microsoft.VisualStudio.Threading.Analyzers", "VSTHRD200", Justification = "Follow recommandation from Microsoft with implementing IAsyncDispose")]
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_writer != null) await _writer.DisposeAsync().ConfigureAwait(false);
            _writer = null;
        }

        private void WritePayloadTag()
        {
            if (_writer == null) throw new ObjectDisposedException(nameof(_writer), "Object has been disposed");
            _payloadStartTagWritten = true;
            AddCloseOperation(wrt => wrt.WriteEndArray());
            _writer.WriteStartArray(PropertyNames.MktActivityRecord);
        }
    }
}
