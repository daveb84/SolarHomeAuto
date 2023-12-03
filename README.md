# Solar Home Auto

Home project that automates turning on / off smart devices depending on solar power generated.  Currently implemented to integrate with:

- Solarman API for solar data collection
- Shelly smart devices

The application will periodicially monitor the Solarman API for solar usage data and can then automatically turn on / off smark devices (for example if there is excess solar power being generates, turn on a radiator).

## Solution
This is a .NET 7 solution, with a web application and a mobile app (written in .NET MAUI, only currently supported for Android).

The application can be setup to run in either of the following ways:

1. Web application only - login to the web application to interact with the app.

2. Mobile application only - host application on an Android device without any need for web hosting.  Useful, for using an old Android device as a server, which can then take advantage of Local Network connectivity. However, this may or maynot be advisable, depending on your device. The Android operating system does not like apps that run long-running tasks in the background and my own testing has shown that processing will slow down, which is beyond the control of the app.  This means that devices are can be left on when they should be turned off, and other such problems.

3. Web application + Mobile - the web application can be setup to do the processing, but can be controlled by the mobile application.


### 1. Web application
_SolarHomeAuto.WebServer_

See notes in [appsettings.Development.json](SolarHomeAuto.WebServer/appsettings.Development.json) to configure authentication and database connections.  Initial setup will require running [Entity Framework Migrations](SolarHomeAuto.Infrastructure.DataStore.SqlServer/README.md) to setup the SQL Server database.

### 2. Mobile Application
_SolarHomeAuto.MobileApp_

The .NET MAUI workload will need to be installed in Visual Studio.  Once this is setup, you should be able to connect your android device via USB and deploy using visual studio.

## Configuration

Configuration is done in the application, via the System page.  Here is a very brief explanation.  Feel free to reach out to me via Git Hub if you are interested in knowing more.

### Server connection (mobile app only)
Set the URL where you are hosting the Web Application, and API key (as configured in the appSettings.[Development / Production].json).  This allows syncing data between server and mobile app.

### Monitoring service mode
Configure whether or not the application should host the processing, or should just be used to remotely control the processing which is performed on another instance.

For example, set the web application to "Host" and the mobile application to "Remote".  The web application will perform the solar montoring and turn on / off devices.  The mobile applicaton will not perform any monitoring logic, but will be able to send commands to start/stop/turn on/off devices, which will be picked up by the web application an processed accordingly.

### Account credentials

Enter the credentials to connect to the Solarman API (you will need to contact Solarman to get API access - there's plenty on the internet about this).  Also, if you are using Shelly Cloud to control your devices, the credentials must also be entered here.

Example JSON:

```
{
  "ShellyCloud": {
    "ApiKey": "SHELLY API KEY",
    "BaseUrl": "https://shelly-67-eu.shelly.cloud"
  },
  "Solarman": {
    "BaseUrl": "https://globalapi.solarmanpv.com",
    "AppId": "SOLAR MAN APP ID",
    "AppSecret": "SOLAR MAN APP SECRET",
    "Email": "EMAIL ADDRESS USED TO LOGIN TO SOLARMAN APP",
    "Password": "SHA256 HASH OF PASSWORD USED TO LOGIN TO SOLARMAN APP",
    "DeviceSn": "DEVICE SN FROM SOLAR MAN APP",
    "CountryCode": "44" // not sure if this is needed, but the Solarman API supports it
  }
}
```

### Devices
This configures device connection details and schedules for controlling automation.  The following example has a single device configured, but any number can be added.

```
[
  {
    // Device ID - any unique value
    "DeviceId": "MyShellyDevice",

    // Friendly description of the device
    "Name": "Shelly switch for downstairs radiator",

    // Provider = ShellySwitch, to use the implement Shelly support
    "Provider": "ShellySwitch",

    "ProviderData": {
      // Shelly Device ID used when connecting via Shelly Cloud API
      "ShellyDeviceId": "1234567890ab",

      // Time in seconds. Throttle how often Shelly Cloud is called to retrieve the status of the
      // device.  This is to prevent violating the Shelly API fair use policy, which
      // will start returning errors if the API is called too often.
      "CloudRefreshStatusTime": "10",

      // LAN IP Address of device, when connecting via home network
      "ShellyLanIPAddress": "192.168.0.30",

      // MQTT settings - not used by default (see notes below)
      "ShellyMqttDeviceId": "shellypluspluguk-1234567890ab",

      // Time to wait to receive a response from an MQTT request
      "MqttTimeout": "5"
    },

    // Schedules: This section can be configured in the UI, or in JSON as below:
    "Schedules": [
      {
        // Always start schedule at 00:00:00
        "Time": "00:00:00",

        // Simple actions = TurnOn/TurnOff
        "Action": "TurnOff"
      },
      {
        "Time": "06:00:00",

        // Action = Conditional. Evaluate the configured conditions to determine 
        // to turn on / off the device
        "Action": "Conditional",

        // C# style boolean condition.  In this example:
        // - turn on when more than 300W of power is being fed back to the grid, and 
        //   the device is turned off, and this has been the case for more than 5 minutes
        // 
        // Explanation:
        //
        // Duration(300, [condition])
        // This means the [condition] must be true for at least 300 seconds (5 minutes)
        //
        // x => x.GridFeedIn > 300 && x.DeviceOff
        // This is the condition that must be true for at least 300 seconds. 
        // 
        // x.GridFeedIn > 300
        // Grid Feed In = amount of power (in Watts) that is being fed back to the grid
        //
        // x.DeviceOff
        // Boolean property indicating the device must be turned off.
        "TurnOnCondition": "Duration(300, x => x.GridFeedIn > 300 && x.DeviceOff)",

        // In this example, there is no Duration statement, so the device will turn off 
        // as soon as this is true:
        // - Power is being drawn from the grid
        // - OR Battery isn't full and Production is less than 300 W.
        "TurnOffCondition": "GridPurchase > 0 || (BatteryCapacity < 100 && Production < 300)",

        // The amount of history data that needs to be loaded to evaluate the condition
        // (set this to be equal/higher than what's used in the "Duration" expression)
        "ConditionRequiredDeviceHistorySeconds": 300,

        // How long to wait in seconds after the device has been turned on / off
        // before evaluating again.  Allows for ramp up / down time.
        "DelaySeconds": 180,

        // Order in which to evaluate this device relative to others when there
        // are multiple devices being evaluated in a cycle.
        "DeviceOrder": 3
      },
      {
        // Another schedule entry - force turn off after sun down.
        "Time": "19:00:00",
        "Action": "TurnOff"
      }
    ]
  }
]

```

### Solar data import schedule
Configures the time of day that solar data should be collected.

```
[
  {
    // Always start schedule at 00:00:00
    "Time": "00:00:00",

    // Stop = don't collect data
    "Action": "Stop"
  },
  {
    "Time": "06:00:00",

    // FetchData = start monitoring
    "Action": "FetchData"
  },
  {
    "Time": "21:00:00",

    // StopAndPurgeData = stop monitoring and delete that data that's been collected.
    "Action": "StopAndPurgeData"
  }
]
```

## MQTT
There is some integration with MQTT implemented.  It's possilble for both Web application and Mobile app to act as an MQTT server and client to communicate with Shelly devices via MQTT.  However, this seemed unreliable - the Shelly devices often seemed to drop the connection (perhaps due to poor WiFi Signal?) so I've not used it.  See the `Mqtt` section in the [appSettings.json](SolarHomeAuto.WebServer/appsettings.json) to turn support on, and there's also MQTT settings in the devices config, which are in example above.