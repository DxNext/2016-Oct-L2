# Linux Platform as a Service (DEMO)

1. First create the App Service, you can follow the steps provided by Nazim Lala in this [guide](https://azure.microsoft.com/en-us/documentation/articles/app-service-linux-how-to-create-a-web-app/)

1. Register your bot [here](https://azure.microsoft.com/en-us/documentation/articles/app-service-linux-how-to-create-a-web-app/), don't forget to add the Pivacy and Service agreement url. The endpoint url is where your server will be listening in the demo case is  `'URL + /api/messages'`. Also, make sure you save both the Microsoft App ID and the Password.

1. Generate webchat keys

1. Set up Deployment options to work with github. This is the [sample](https://github.com/brusMX/supreme-boto-potato) we will be using, if you want you can fork it.

1. Type `'app.js'` as your `'Node.js Startup File'`

1. Inside of your app settings add the App Id you generated from the second step and add them as the following App Environment Variables: `'MICROSOFT_APP_ID'` and `'MICROSOFT_APP_PASSWORD'`.

    ![alt text][settings]

1. Finally go to the following URL replacing your `'YOUR_SECRET_HERE'`:

    ```Shell
    https://webchat.botframework.com/embed/dxtraining?s=YOUR_SECRET_HERE
    ```
1. Profit!


[settings]: img/app-settings.jpg "This is what the filesystem in Linux looks like"
