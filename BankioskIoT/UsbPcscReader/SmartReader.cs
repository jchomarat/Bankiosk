using System;
using System.Collections.Generic;
using System.IO;
using Iot.Device.Card;
using PCSC;
using PCSC.Exceptions;
using PCSC.Monitoring;
using PCSC.Utils;

namespace UsbPcscReader
{
    public class SmartCard : CardTransceiver, IDisposable
    {
        private SCardMonitor _monitor = null;
        private string[] _readerNames;
        private SCardContext _context;
        private int _readerSelected;
        private CardReader _reader;

        public bool IsCardPResent { get; set; }

        public SmartCard()
        {
            var contextFactory = ContextFactory.Instance;
            _context = (SCardContext)contextFactory.Establish(SCardScope.System);

            _readerNames = GetReaderNames();

            if (IsEmpty(_readerNames))
            {
                throw new IOException("Need to have at least a reader connected");
            }

            ReaderSelected = 0;

            _monitor = (SCardMonitor)MonitorFactory.Instance.Create(SCardScope.System);

            AttachToAllEvents(_monitor); // Remember to detach, if you use this in production!
            _monitor.Start(_readerNames);
        }

        public int ReaderSelected
        {
            get
            { return _readerSelected; }
            set
            {
                _readerSelected = value;

            }
        }

        private void AttachToAllEvents(ISCardMonitor monitor)
        {
            // Point the callback function(s) to the anonymous & static defined methods below.
            monitor.CardInserted += (sender, args) => CardInseted(args);
            monitor.CardRemoved += (sender, args) => CardRemoved(args);
            //monitor.Initialized += (sender, args) => CardInitialized(args);
            //monitor.StatusChanged += StatusChanged;
            //monitor.MonitorException += MonitorException;
        }

        private void CardRemoved(CardStatusEventArgs args)
        {
            _reader?.Dispose();
            _reader = null;
            IsCardPResent = false;
        }

        private void CardInseted(CardStatusEventArgs args)
        {
            _reader = (CardReader)_context.ConnectReader(_readerNames[_readerSelected], SCardShareMode.Shared, SCardProtocol.Any);
            IsCardPResent = true;
        }

        private string[] GetReaderNames()
        {
            using (var context = ContextFactory.Instance.Establish(SCardScope.System))
            {
                return context.GetReaders();
            }
        }

        private bool IsEmpty(ICollection<string> readerNames) => readerNames == null || readerNames.Count < 1;

        public override int Transceive(byte targetNumber, ReadOnlySpan<byte> dataToSend, Span<byte> dataFromCard)
        {
            byte[] received = new byte[dataFromCard.Length];
            var ret = _reader.Transmit(dataToSend.ToArray(), received);
            received.CopyTo(dataFromCard);
            // Adding a 0x00 at the end to be compliant with the NFC reader
            return ret;
        }

        public void Dispose()
        {
            if (_monitor.Monitoring)
            {
                _monitor.Cancel();
            }
        }
    }
}
