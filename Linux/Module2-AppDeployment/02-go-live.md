# Make your app work

Now that we have installed the MEAN stack lets take it for a spin. We will get a fully functional app running in the VM.

1. Configure git
1. Deploy the app
1. MigrateDB

## Download and install sample app

We will be using the Northwind sample app created by [Bradley Braithwaite](https://github.com/bbraithwaite).

1. First of, lets fork his [Northwind repository](https://github.com/bbraithwaite/NorthwindNode).

    ![alt text][fork]

1. From your SSH session in the server lets create a new ssh key, just like we did in section 1 but this time make sure to keep the default location and and an empty passphrase. 

    ```bash
     ssh-keygen -t rsa -b 4096
     cat ~/.ssh/id_rsa.pub
     ```
1. Name your server

    ```bash
    git config --global user.name "Server name"
    git config --global user.email "my@server.rocks"
    ```

1. Add your server's key to the repo. Go to `'Settings' > 'Deploy Keys' > 'Add deploy key'`, and add the key from your server.

    ![alt text][deploy-key]

1. Clone your repo to your VM:

    ```bash
    cd ~
    git clone git@github.com:yourGithubUsername/NorthwindNode.git northwind

1. Lets create a git webhook in our server

    ```bash
    mkdir webhook
    cd webhook
    npm init
     ```

     Add the following lines to the `'index.js'` file:

     ```js
     var express = require('express'),
    http = require('http'),
    app = express();

    app.set('port', process.env.PORT || 3030);

    app.post('/push/', function (req, res) {
        var spawn = require('child_process').spawn,
            deploy = spawn('sh', [ './pull.sh' ]);

        deploy.stdout.on('data', function (data) {
            console.log(''+data);
        });

        deploy.on('close', function (code) {
            console.log('Child process exit code: ' + code);
        });
        res.json(200, {message: 'Received'})
    });

    http.createServer(app).listen(app.get('port'), function(){
    console.log('Listening on: ' + app.get('port'));
    });
    ```

    And create the file `'pull.sh'`:

    ```bash
    #!/bin/sh
    #First we remove all the changes in the local master, then we pull the changes
    cd ~/northwind
    git reset --hard origin/master
    git clean -f
    git pull
    ```

    We need to modify the `'package.json'` file to include the following line in the scripts section:

    ```json
    "start": "node index.js"
    ```

    It should look something like this:

    ```json
    {
        "name": "webhook",
        "version": "1.0.0",
        "description": "",
        "main": "index.js",
        "scripts": {
            "test": "echo \"Error: no test specified\" && exit 1",
            "start": "node index.js"
        },
        "author": "",
        "license": "ISC",
        "dependencies": {
            "express": "^4.14.0"
        }
    }
    ```

    We start the service:

    ```bash
    npm start
    ------------------------
    > webhook@1.0.0 start /home/bruno/webhook
    > node index.js

    Listening on: 3030
    ```

1. In github we go to `'Settings' > 'Webhook' > 'Add webhook'`. And we provide the url of our webhook which is `'http://<vm-url>:3030/push/'`.

    ![alt text][webhook]

    We will immediately see that our webhook will respond to that:

    ```Shell
    HEAD is now at 718026e move filter expression to controller

    Already up-to-date.

    Child process exit code: 0
     ```
1. Open a new SSH session to your VM and correct the dependecies in the package json file. Change `'"grunt-node-inspector": "~0.1.3",'` to `'"grunt-node-inspector": ">=0.1.3",'`

1. Install the dependencies (one by one)

    ```bash
    cd ~/northwind
    sudo npm install -g node-inspector
    sudo npm install -g node-gyp
    sudo npm install -g mocha
    sudo npm install -g migrate
    npm install grunt --save-dev
    npm install mocha --save-dev
    ```

    After you are done with the dependencies run:

    ```bash
    npm start
    ```

1. Test the app in the browser
1. Commit a file
1. profit!



## Following step

1. [Things to consider](../Module3-ThingsToConsider/readme.md)

[fork]: ../../img/fork.jpg "Fork it!"
[deploy-key]: ../../img/deploy-key.jpg "Add the whole key"
[webhook]: ../../img/webhook.jpg "3030 is the port"
