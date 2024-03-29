# Bankiosk UI Deployment

This project is the front user exeprience of Bankiosk. It is made in React, and it consumes API from BankioskIoT (which servers as API hub and hardward interface).

To deploy BankioskUI, follow the steps

First, clone the repository

```sh
/> git clone https://https://github.com/jchomarat/Bankiosk/bankioskUI
```

Build the docker image by running the following command

```sh
/> docker build . -t bankiosk/ui
```

For the following, I suppose that you have already 
* Added on your Azure subscription
    * An IoT Hub service, configures for the hardware you are deploying Bankiosk on
    * A Container Registry service to host your docker images.
* A hardware (such as [Raspberry PI 3](https://www.raspberrypi.org/))
    * Ideally 3B+, we haven't tested yet on the 4
    * Raspbian Stretch
    * IoT Edge installed (the guide can be [found](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-install-iot-edge-linux) here)

You can find more information also [here](https://github.com/Ellerbach/Raspberry-IoTEdge#pushing-the-docker-image-to-the-azure-container-registry-acr).

Let's assume that your container registry is called **bankioskcontainersregistry.azurecr.io**

Now, we need to tag our newly generated image

```sh
/> docker tag bankiosk/ui bankioskcontainersregistry.azurecr.io/bankiosk/ui:latest
```

Note that usually I use "latest" as a tag, you can of course set a number here!

Push your tagged image to your container registry on Azure

```sh
/> docker push bankioskcontainersregistry.azurecr.io/bankiosk/ui:latest
```

# Bankiosk UI configuration for IoT Edge

Once everything is deployed, you need to set a few properties on Iot Hub

For that, log onto your [Azure Portal](https://portal.azure.com), then navigate to your IoT Hub service.

From the left menu, click on the **IoT Edge** link

![IoT Edge link](./docs/ps1.png)

Select then your device you want to set the configuration to

![IoT Edge device](./docs/ps2.png)

Click on the button "Set modules" in the tool bar. You can see now the list of images to be pushed. Click on the configure button of the Bankiosk UI image

![Bankiosk UI image](./docs/ps3.png)

On the right, will load the configuration panel (after to print screen you will find all the settings so that you can simply copy and paste them)

![Bankiosk UI image settings](./docs/ps3.png)

* Name: the one you want
* Image URI: make sure it points to the correct image & version
* Environnement variable: add the following one:

```sh
REACT_APP_BASE_CORE_URL = URL of the BankioskIoT service (see other documentation for that)
```

* Container create option

```json
{
  "ExposedPorts": {
    "3000/tcp": {}
  },
  "HostConfig": {
    "Privileged": true,
    "PortBindings": {
      "3000/tcp": [
        {
          "HostPort": "3000"
        }
      ]
    }
  }
}
```
What is important above is the Privileged and the port (by default it is 3000, but you can off course set something else)


* Module twin's desired properties: leave at it is

# Booting the Raspberry Pi in kiosk mode

To have a maximezed experience, the RPI should boot in "kiosk" mode. The kiosk mode allows, once the RPI is loaded, to launch Chromium (the default browser) in full screen on the page you select. It creates an immersive experience.

## Remove the mouse cursor

First of all, it is best to hide the mouse cursor, to do so, edit the following file

```sh
> sudo nano /etc/lightdm/lightdm.conf
```

And look for the string **xserver-command**, uncomment it and add the following value

```sh
xserver-command=X -nocursor
```

## Set up the kiosk mode at boot time

To do so, follow the steps below (got from [here](http://blog.philippegarry.com/2018/08/19/faire-de-son-pi-une-borne-raspberry-pi-kiosk-mode-stretch-version/) in french)

Create the folder ~/.config/autostart if it does not exist (under your default account used at startup, by default *pi*)

```sh
> mkdir ~/.config/autostart
```

Create a file called *autoChromium.desktop* under this folder

```sh
> nano ~/.config/autostart/autoChromium.desktop
```

Paste the following content

```sh
[Desktop Entry]
Type=Application
Exec=/usr/bin/chromium-browser --noerrdialogs --incognito --disable-session-crashed-bubble --disable-infobars --kiosk $bankioskUI_URL
Hidden=false
X-GNOME-Autostart-enabled=true
Name[en_US]=AutoChromium
Name=AutoChromium
Comment=Start bankiosk in kiosk mode
```

Make sure to replace *$bankioskUI_URL* by the actual URL (by default, a React app URL is http://localhost:3000)

Reboot your RPI - and Bankiosk should be opened right away. Please note that it may takes some time to start the different containers, you may then receive an error message. Do no worry, it will refresh once ready!

## Set up the kiosk mode manually

Alternatively, you can start the kiosk mode manually. To achieve this, create a new file on the Desktop (that you can activate without a mouse or keyboard if the screen is tactlie)

```sh
> nano ~/Desktop/startKiosk.sh
```

Paste the following

```sh
/usr/bin/chromium-browser --noerrdialogs --incognito --disable-session-crashed-bubble --disable-infobars --kiosk $bankioskUI_URL 
```

Make sure to replace *$bankioskUI_URL* by the actual URL (by default, a React app URL is http://localhost:3000)

Save, and make this script executable

```sh
> chmod a+x ~/Desktop/startKiosk.sh
```