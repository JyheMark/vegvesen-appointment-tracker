# Statens Vegvesen appointment tracker
I made this small app because trying to snag an oppkjøring appointment at Statens Vegvesen was a real pain.

The app will pull down available appointments at traffic stations of your choosing and will send you an email with the updates.

Configure in `appsettings.json` 

Best used set up as a chronjob.

I made it to work with Gmail. Have not tested other providers.

## Setup
Open up the `appsettings.json` file and configure.

- `AuthenticationCookie`: This is the cookie attached to your session after you log in to Vegvesen. You can retrieve it using your browsers devtools.
- `TrafficStationIds`: An array of which traffic station IDs to monitor. Check `trafficstasjoner.json` file to find the relevant ones.
- `EmailSettings.FromAddress`: The account with which the application emails from
- `EmailSettings.ToAddress`: The account with which the application notifies
- `FromAccountPassword`: Self explanatory. For Gmail with 2FA, this requires a generated app password.
- `SmtpSettings`: The SMTP settings relevant to your provider.

Running the application once will fetch availabilities and email you. I set it up as a chronjob to run every hour.