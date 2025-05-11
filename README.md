# Transfer GO Task
Notification service

# Local setup
## Prerequisites
- Docker
- docker-compose
- Twilio account (optional)

## Run the app

To run the app, start the docker containers, with the database, mock email and AWS Services clients using `docker compose up -d` when inside the `./TransferGOTask/NotificationService` directory

Then the app via `dotnet run` or using IDE

The documentation for testing should be available on `http://localhost:5088/swagger/index.html`.

### Twilio Setup

In the Twilio Developer Console go to **Notifications**->**SMS Notification**->**Start Building**

You will get the API key and a testing number. You can them as ENV vars by in this format:
```
"Twilio__AccountSid": "your-sid",
"Twilio__AuthToken": "your-token",
"Twilio__MessagingServiceSid": "your-sid",
"Twilio__FromNumber": "your-number"
```

You can export them by adding to the `launchSettings.json` or using `export Twilio__AccountSid="your_twilio_sid"` if in Linux or `$Env:Twilio__AccountSid = "yout_twilio_sid"` if in Windows

