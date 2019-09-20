using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iot.Device.Pn532;
using UsbPcscReader;

namespace BankioskIoT.Models
{
    public class Pn532UsbScWrapper
    {
        Pn532 _nfc;
        SmartCard _reader;

        public Pn532UsbScWrapper()
        {
            _nfc = null;
            _reader = null;
        }

        public Pn532UsbScWrapper(Pn532 nfc)
        {
            _nfc = nfc;
        }

        public Pn532UsbScWrapper(SmartCard reader)
        {
            _reader = reader;
        }

        public Pn532 Pn532 => _nfc != null ? _nfc : throw new InvalidOperationException("No PN532 instance has been set");

        public SmartCard SmartCard => _reader != null ? _reader : throw new InvalidOperationException("No SmartCard reader instance has been set");

        public bool HasNfc => _nfc != null;

        public bool HasSmartCard => _reader != null;
    }
}
