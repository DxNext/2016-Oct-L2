Compose is a tool for defining and running multi-container Docker applications. With Compose, you use a Compose file to configure your application’s services. Then, using a single command, you create and start all the services from your configuration.

## 1. Install Docker Compose
```
curl -L "https://github.com/docker/compose/releases/download/1.8.1/docker-compose-$(uname -s)-$(uname -m)" > /usr/local/bin/docker-compose
```

## 2. Apply executable permissions
```
chmod +x /usr/local/bin/docker-compose
```

## 3. Install command completion for bash
```
curl -L https://raw.githubusercontent.com/docker/compose/$(docker-compose version --short)/contrib/completion/bash/docker-compose > /etc/bash_completion.d/docker-compose
```

## 4. Test the installation
```
docker-compose --version
```

You should see something similar to the following after you run the command

docker-compose version: 1.8.1


Now you have Docker Compose installed, you may continue with the next exercise.