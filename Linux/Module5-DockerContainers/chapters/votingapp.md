## Run a multi-container app with Docker Compose
This portion of the tutorial will guide you through the creation and customization of a voting app. It's important that you follow the steps in order, and make sure to customize the portions that are customizable.

You'll need to have a [Docker Id](https://hub.docker.com/register/). Once you do run login from the commandline:

```
$ docker login
```

And follow the login directions. Now you can push images to Docker Hub.


### 3.1 Get the voting-app
You now know how to build your own Docker image, so let's take it to the next level and glue things together. For this app you have to run multiple containers and Docker Compose is the best way to do that.

Start by quickly reading the documentation [here](https://docs.docker.com/compose/overview/).

Clone the voting-app repository already available at [Github Repo](https://github.com/docker/example-voting-app.git).

```
git clone https://github.com/docker/example-voting-app.git
```

### 3.2 Customize the app

#### 3.2.1 Modify app.py

In the folder ```example-voting-app/vote``` you need to edit the app.py and change the two options for the programming languages you chose.

Edit the following lines:

```
option_a = os.getenv('OPTION_A', "Cats")
option_b = os.getenv('OPTION_B', "Dogs")
```

substituting two options of your choice. For instance:

```
option_a = os.getenv('OPTION_A', "Java")
option_b = os.getenv('OPTION_B', ".NET")
```
#### 3.2.2 Running your app
Now, run your application. To do that, we'll use [Docker Compose](https://docs.docker.com/compose). Docker Compose is a tool for defining and running multi-container Docker applications. With Compose, you define a `.yml` file that describes all the containers and volumes that you want, and the networks between them. In the example-voting-app directory, you'll see a `docker-compose.yml file`:

```yml
version: "2"

services:
  vote:
    build: ./vote
    command: python app.py
    volumes:
     - ./vote:/app
    ports:
      - "5000:80"
    networks:
      - front-tier
      - back-tier

  result:
    build: ./result
    command: nodemon --debug server.js
    volumes:
      - ./result:/app
    ports:
      - "5001:80"
      - "5858:5858"
    networks:
      - front-tier
      - back-tier

  worker:
    build: ./worker
    networks:
      - back-tier

  redis:
    image: redis:alpine
    container_name: redis
    ports: ["6379"]
    networks:
      - back-tier

  db:
    image: postgres:9.4
    container_name: db
    volumes:
      - "db-data:/var/lib/postgresql/data"
    networks:
      - back-tier

volumes:
  db-data:

networks:
  front-tier:
  back-tier:
```


Architecture
-----

![Architecture diagram](../images/architecture.png)

* A Python webapp which lets you vote between two options
* A Redis queue which collects new votes
* A .NET worker which consumes votes and stores them in…
* A Postgres database backed by a Docker volume
* A Node.js webapp which shows the results of the voting in real time

Note that three of the containers are built from Dockerfiles, while the other two are images on Docker Hub. To learn more about how they're built, you can examine each of the Dockerfiles in the three directories: `vote`, `result`, `worker`. 

The Compose file also defines two networks, front-tier and back-tier. Each container is placed on one or two networks. Once on those networks, they can access other services on that network in code just by using the name of the service. To learn more about networking check out the [Networking with Compose documentation](https://docs.docker.com/compose/networking/).

To launch your app navigate to the example-voting-app directory and run the following command:

```
$ docker-compose up -d
```

This tells Compose to start all the containers specified in the `docker-compose.yml` file. The `-d` tells it to run them in daemon mode, in the background. Navigate to `http://IP_ADDRESS_OF_YOUR_AZURE_VM:5000` in your browser, and you'll see the voting app running.

**TIP:** You can use the following Azure CLI command to get the IP address of your VM:
```
azure vm show <your-resource-group> <your-vm-name> |grep "Public IP address" | awk -F ":" '{print $3}'
```

**NOTE:** Since you will be running this example in Azure, you should make sure port 5000 and 5001 are publicly open and navigate to your DNS name instead of localhost.

You can use the guides for opening the ports using [Azure Resource Manager](https://github.com/DxNext/2016-Oct-L2/blob/master/Linux/Module3-ThingsToConsider/opening-ports-in-arm.md).

<img src="../images/vote.png" title="vote">

Click on one to vote. You can check the results at `http://IP_ADDRESS_OF_YOUR_AZURE_VM:5001`

#### 3.2.3 Build and tag images

You are all set now. Navigate to each of the directories where you have a Dockerfile to build and tag your images that you want to submit.

In order to build the images, make sure to replace `<YOUR_DOCKER_ID>` with your *Docker Hub username* in the following commands:

```
$ docker build --no-cache -t <YOUR_DOCKER_ID>/votingapp_voting-app .

$ docker build --no-cache -t <YOUR_DOCKER_ID>/votingapp_result-app .

$ docker build --no-cache -t <YOUR_DOCKER_ID>/votingapp_worker .
```

#### 3.2.4 Push images to Docker Hub

Push the images to Docker hub. Remember, you must have run `docker login` before you can push.

```
$ docker push <YOUR_DOCKER_ID>/votingapp_voting-app

$ docker push <YOUR_DOCKER_ID>/votingapp_result-app

$ docker push <YOUR_DOCKER_ID>/votingapp_worker
```

Now you can access these images anywhere by running

```
$ docker pull <YOUR_DOCKER_ID>/votingapp_voting-app
$ docker pull <YOUR_DOCKER_ID>/votingapp_result-app
$ docker pull <YOUR_DOCKER_ID>/votingapp_worker
```
