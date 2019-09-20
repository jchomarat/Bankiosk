using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Linq;
using System.Threading.Tasks;
using BankioskIoT.Models;
using Iot.Device.Pn532;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using UsbPcscReader;

namespace BankioskIoT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            Pn532UsbScWrapper nfcWrapper = null;

            // Check if we have the environment variables
            bool noNfc = false;
            bool hasSmartCard = false;
            var noNfcEnv = Environment.GetEnvironmentVariable("NONFC");
            if (String.IsNullOrEmpty(noNfcEnv))
            {
                if (Configuration.GetSection("NfcSettings")["NoNfc"] == "true")
                {
                    noNfc = true;
                }
                else if (Configuration.GetSection("NfcSettings")["NoNfc"].ToLower() == "smartcard")
                {
                    noNfc = true;
                    hasSmartCard = true;
                }
            }
            else
            {
                if (noNfcEnv == "true")
                {
                    noNfc = true;
                }
                else if (noNfcEnv.ToLower() == "smartcard")
                {
                    noNfc = true;
                    hasSmartCard = true;
                }
            }

            if (!noNfc)
            {
                Pn532 nfc = null;
                var nfcMode = Environment.GetEnvironmentVariable("NFC_MODE");
                if (String.IsNullOrEmpty(nfcMode))
                {
                    nfcMode = Configuration.GetSection("NfcSettings")["Mode"];
                }
                if (Enum.TryParse<OperatingMode>(nfcMode, out OperatingMode nfcConfig))
                {
                    switch (nfcConfig)
                    {
                        case OperatingMode.HighSpeedUart:
                            var modeConfig = Environment.GetEnvironmentVariable("NFC_MODE_CONFIG");
                            if (String.IsNullOrEmpty(modeConfig))
                            {
                                modeConfig = Configuration.GetSection("NfcSettings")["ModeConfig"];
                            }
                            nfc = new Pn532(modeConfig);
                            break;
                        case OperatingMode.I2c:
                            I2cConnectionSettings connectionString = new I2cConnectionSettings(1, Pn532.I2cDefaultAddress);
                            var device = I2cDevice.Create(connectionString);
                            nfc = new Pn532(device);
                            break;
                        case OperatingMode.Spi:
                            var settings = new SpiConnectionSettings(0, 0)
                            {
                                ClockFrequency = 2_000_000,
                                Mode = SpiMode.Mode0,
                                ChipSelectLineActiveState = PinValue.Low,
                                //    DataFlow = DataFlow.LsbFirst
                            };
                            SpiDevice deviceI2c = SpiDevice.Create(settings);
                            nfc = new Pn532(deviceI2c);
                            break;
                        default:
                            nfc = new Pn532("/dev/ttyS0");
                            break;
                    }
                }
                nfcWrapper = new Pn532UsbScWrapper(nfc);
                services.AddSingleton(nfcWrapper);
            }
            else if (hasSmartCard)
            {
                nfcWrapper = new Pn532UsbScWrapper(new SmartCard());
                services.AddSingleton(nfcWrapper);
            }
            else
            {
                // When NFC is not available but we still want to use this webapi. Dummy data will be sent out
                nfcWrapper = new Pn532UsbScWrapper();
                services.AddSingleton(nfcWrapper);
            }


            // Add service and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddSingleton(new ApiSettings() { ApiUrl = Configuration.GetSection("BankioskApi")["ApiUrl"] });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("CorsPolicy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
