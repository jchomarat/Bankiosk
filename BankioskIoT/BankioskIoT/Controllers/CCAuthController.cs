using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankioskIoT.Models;
using Iot.Device.Pn532;
using Iot.Device.Pn532.ListPassive;
using Iot.Device.Card.CreditCardProcessing;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UsbPcscReader;

namespace BankioskIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CCAuthController : ControllerBase
    {
        private const int TimeOutNfcAuthenticationMillisecond = 60_000;
        private string _apiUrl;
        private Pn532UsbScWrapper _nfcWrapper;
        private Pn532 _nfc;

        public CCAuthController(Pn532UsbScWrapper nfcWrapper, ApiSettings apiSettings)
        {
            if (nfcWrapper.HasNfc)
            {
                _nfc = nfcWrapper.Pn532;
            }
            _nfcWrapper = nfcWrapper;

            _apiUrl = apiSettings.ApiUrl;
        }

        /// <summary>
        /// Wait for the NFC reader to get the credit card data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult<JObject> GetNfcAuthentication(string id)
        {

            byte[] retData = null;
            var ccAuth = new CreditCardAuthenticated();
            dynamic resp = new JObject();
            CreditCard creditCard = null;
            try
            {
                if (_nfcWrapper.HasNfc)
                {
                    // This try/catch allow this method to work on any machine, even if come does not work.
                    // So if it does not work (i.e. error caught, then a dummy object is returned)
                    DateTimeOffset timeout = DateTimeOffset.Now.AddMilliseconds(TimeOutNfcAuthenticationMillisecond);
                    while ((timeout > DateTimeOffset.Now))
                    {
                        // PollingType.InnovisionJewel, PollingType.DepActive106kbps, PollingType.DepPassive106kbps,PollingType.Passive106kbpsISO144443_4A, PollingType.Passive106kbpsISO144443_4A, PollingType.GenericPassive106kbps, PollingType.MifareCard, PollingType.Passive106kbps
                        retData = _nfc.AutoPoll(5, 300, new PollingType[] { PollingType.Passive106kbpsISO144443_4B });
                        if (retData != null)
                            break;
                        // Give time to PN532 to process
                        Thread.Sleep(50);
                    }
                    if (retData == null)
                    {
                        resp.status = false;
                        resp.value = "Could not read the card";
                        return resp;
                    }
                    if (retData.Length < 3)
                    {
                        resp.status = false;
                        resp.value = "Could not read the card";
                        return resp;
                    }
                    //Check how many tags and the type
                    // Console.WriteLine($"Num tags: {retData[0]}, Type: {(PollingType)retData[1]}");
                    var decrypted = _nfc.TryDecodeData106kbpsTypeB(retData.AsSpan().Slice(3));
                    if (decrypted != null)
                    {
                        //Console.WriteLine($"{decrypted.TargetNumber}, Serial: {BitConverter.ToString(decrypted.NfcId)}, App Data: {BitConverter.ToString(decrypted.ApplicationData)}, " +
                        //    $"{decrypted.ApplicationType}, Bit Rates: {decrypted.BitRates}, CID {decrypted.CidSupported}, Command: {decrypted.Command}, FWT: {decrypted.FrameWaitingTime}, " +
                        //    $"ISO144443 compliance: {decrypted.ISO14443_4Compliance}, Max Frame size: {decrypted.MaxFrameSize}, NAD: {decrypted.NadSupported}");

                        creditCard = new CreditCard(_nfc, decrypted.TargetNumber);
                    }
                } else if(_nfcWrapper.HasSmartCard)
                {
                    DateTimeOffset timeout = DateTimeOffset.Now.AddMilliseconds(TimeOutNfcAuthenticationMillisecond);

                    while ((timeout > DateTimeOffset.Now))
                    {
                        if (_nfcWrapper.SmartCard.IsCardPResent)
                        {
                            creditCard = new CreditCard(_nfcWrapper.SmartCard, 0, 2);
                            break;
                        }
                    }

                }

                if (creditCard != null)
                {
                    creditCard.ReadCreditCardInformation();

                    // Console.WriteLine("All Tags for the Credit Card:");
                    // Find 0x5A = PAN = Credit Card number

                    var ccForAuth = new CreditCardForAuthentication();
                    var searchedTag = Tag.SearchTag(creditCard.Tags, 0x5A);
                    if (searchedTag.Count > 0)
                    {
                        TagDetails tg = new TagDetails(searchedTag.FirstOrDefault());
                        ccForAuth.CardNumber = CleanText(tg.ToString());
                        // 0x59 expiration date
                        searchedTag = Tag.SearchTag(creditCard.Tags, 0x59);
                        if (searchedTag.Count == 0)
                        {
                            // 0x59 expiration date
                            searchedTag = Tag.SearchTag(creditCard.Tags, 0x5F24);
                        }
                        if (searchedTag.Count > 0)
                        {
                            tg = new TagDetails(searchedTag.FirstOrDefault());
                            ccForAuth.Expiration = new DateTimeOffset(2000 + BcdToInt(tg.Data[0]), BcdToInt(tg.Data[1]), BcdToInt(tg.Data[2]), 0, 0, 0, TimeSpan.Zero);
                        }
                    }
                    if (String.IsNullOrEmpty(ccForAuth.CardNumber))
                    {
                        // Find 0x57 Track2 if no 5A
                        searchedTag = Tag.SearchTag(creditCard.Tags, 0x57);
                        // The Date is encoded with a separated car like XX XX XX XX XX XX XX XX DY YM MX
                        if (searchedTag.Count > 0)
                        {
                            var data = searchedTag.FirstOrDefault().Data;
                            ccForAuth.CardNumber = BitConverter.ToString(data, 0, 8);
                            ccForAuth.CardNumber = CleanText(ccForAuth.CardNumber);
                            // The Date is encoded with a separated car like XX XX XX XX XX XX XX XX DY YM MX
                            var year = 2000 + BcdToInt((byte)(data[8] << 8 + (data[9] >> 8)));
                            var month = BcdToInt((byte)(data[9] << 8 + (data[10] >> 8)));
                            ccForAuth.Expiration = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
                        }
                    }

                    if (String.IsNullOrEmpty(ccForAuth.CardNumber))
                    {
                        resp.status = false;
                        resp.value = "No card number found";
                        return resp;
                    }

                    // TODO: Call for the authentication
                    // http://bankioskapi.azurewebsites.net/api/customers/getbycard/5255591532318986

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(_apiUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.GetAsync($"customers/getbycard/{ccForAuth.CardNumber}").GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response body.
                        var customer = response.Content.ReadAsAsync<Customer>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll                    
                        Debug.WriteLine($"Found customer {customer?.RowKey} {customer?.FirstName} {customer?.Surname}");
                        if (customer == null)
                        {
                            resp.status = false;
                            resp.value = "No customer with this card found";
                            return resp;
                        }
                        // Do we really have someone?
                        if ((String.IsNullOrEmpty(customer.FirstName)) && (String.IsNullOrEmpty(customer.FirstName)))
                        {
                            resp.status = false;
                            resp.value = "No customer with this card found";
                            return resp;
                        }
                        // Is it the same ID?
                        if (customer.RowKey.ToLower() != id.ToLower())
                        {
                            resp.status = false;
                            resp.value = "Customer id mismatch";
                            return resp;
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"Error {response.StatusCode} ({response.ReasonPhrase})");
                        resp.status = false;
                        resp.value = "Could not find a customer with this id - or the service failed";
                        return resp;
                    }

                    // Let's assume we are authenticated
                    ccAuth.IsAuthenticated = true;
                    // Find all 0x9F36 tag Transaction count
                    searchedTag = Tag.SearchTag(creditCard.Tags, 0x9F36);
                    if (searchedTag.Count > 0)
                    {
                        ccAuth.NumberOperation = 0;
                        for (int i = 0; i < searchedTag.Count; i++)
                        {
                            ccAuth.NumberOperation = BinaryPrimitives.ReadUInt16BigEndian(searchedTag[i].Data);
                        }
                    }
                    // Find 0x9F17 PinTry Counter
                    searchedTag = Tag.SearchTag(creditCard.Tags, 0x9F17);
                    if (searchedTag.Count > 0)
                    {
                        ccAuth.PinRetryLeft = searchedTag.FirstOrDefault().Data[0];
                    }
                    ccAuth.ExpirationDate = ccForAuth.Expiration;
                    resp.status = true;
                    resp.value = JsonConvert.SerializeObject(ccAuth);
                    return resp;
                }
                else
                {
                    // No NFC => send dummy data
                    resp.status = true;
                    ccAuth.ExpirationDate = DateTimeOffset.Now.AddYears(2);
                    ccAuth.IsAuthenticated = true;
                    ccAuth.NumberOperation = 340;
                    ccAuth.PinRetryLeft = 2;
                    resp.value = JsonConvert.SerializeObject(ccAuth);
                    return resp;
                }
            }
            catch (Exception ex)
            {
                resp.status = false;
                resp.value = $"Exception: {ex.ToString()}";
                return resp;
            }
        }

        private string CleanText(string toClean)
        {
            string str = "";
            for (int i = 0; i < toClean.Length; i++)
            {
                if (!((toClean[i] == ' ') || (toClean[i] == '-')))
                    str += toClean[i];
            }
            return str;
        }

        private int BcdToInt(byte bcd) => BcdToInt(new byte[] { bcd });

        private int BcdToInt(byte[] bcds)
        {
            int result = 0;
            foreach (byte bcd in bcds)
            {
                result *= 100;
                result += (10 * (bcd >> 4));
                result += bcd & 0xf;
            }
            return result;
        }

    }
}