## Install Docker Engine

Docker Engine is supported on Linux, Cloud, Windows, and OS X. To be consistent with the previous parts of the training, this guide will walk you through the installation steps for CentOS.
If you want to learn about how you can install Docker to different Linux distributions, you can check [this link](https://docs.docker.com/engine/installation/).

There are two ways to install Docker Engine. You can [install using the `yum`
package manager](centos.md#install-with-yum). Or you can use `curl` with the [`get.docker.com`
site](centos.md#install-with-the-script). This second method runs an installation script
which also installs via the `yum` package manager.


### Install with the script (the short way)

1. Log into your machine as a user with `sudo` or `root` privileges.

2.  Make sure your existing packages are up-to-date.

    ```bash
    $ sudo yum update
    ```

3.  Run the Docker installation script.

    ```bash
    $ curl -fsSL https://get.docker.com/ | sh
    ```

    This script adds the `docker.repo` repository and installs Docker.

4.  Enable the service.

    ```bash
    $ sudo systemctl enable docker.service
    ```

5.  Start the Docker daemon.

    ```bash
    $ sudo systemctl start docker
    ```

6.  Verify `docker` is installed correctly by running a test image in a container.

    ```bash
    $ sudo docker run hello-world
    ```

## Create a docker group

The `docker` daemon binds to a Unix socket instead of a TCP port. By default
that Unix socket is owned by the user `root` and other users can access it with
`sudo`. For this reason, `docker` daemon always runs as the `root` user.

To avoid having to use `sudo` when you use the `docker` command, create a Unix
group called `docker` and add users to it. When the `docker` daemon starts, it
makes the ownership of the Unix socket read/writable by the `docker` group.

>**Warning**: The `docker` group is equivalent to the `root` user; For details
>on how this impacts security in your system, see [*Docker Daemon Attack
>Surface*](../../security/security.md#docker-daemon-attack-surface) for details.

To create the `docker` group and add your user:

1. Log into your machine as a user with `sudo` or `root` privileges.

2.  Create the `docker` group.

    ```bash
    $ sudo groupadd docker
    ```

3.  Add your user to `docker` group.

    ```bash
    $ sudo usermod -aG docker your_username
    ```

4.  Log out and log back in.

    This ensures your user is running with the correct permissions.

5.  Verify that your user is in the docker group by running `docker` without `sudo`.

    ```bash
    $ docker run hello-world
    ```

## Start the docker daemon at boot

Configure the Docker daemon to start automatically when the host starts:

```bash
$ sudo systemctl enable docker
```


### Install with yum (the long way)

**NOTE:** If you already followed the previous steps and installed Docker, you don't need to follow the following steps. The remaining steps are just to provide information on installing Docker manually.

1. Log into your machine as a user with `sudo` or `root` privileges.

2.  Make sure your existing packages are up-to-date.

    ```bash
    $ sudo yum update
    ```

3.  Add the `yum` repo.

    ```bash
    $ sudo tee /etc/yum.repos.d/docker.repo <<-'EOF'
    [dockerrepo]
    name=Docker Repository
    baseurl=https://yum.dockerproject.org/repo/main/centos/7/
    enabled=1
    gpgcheck=1
    gpgkey=https://yum.dockerproject.org/gpg
    EOF
    ```

4.  Install the Docker package.

    ```bash
    $ sudo yum install docker-engine
    ```

5.  Enable the service.

    ```bash
    $ sudo systemctl enable docker.service
    ```

6.  Start the Docker daemon.

    ```bash
    $ sudo systemctl start docker
    ```

7. Verify `docker` is installed correctly by running a test image in a container.

        $ sudo docker run --rm hello-world

        Unable to find image 'hello-world:latest' locally
        latest: Pulling from library/hello-world
        c04b14da8d14: Pull complete
        Digest: sha256:0256e8a36e2070f7bf2d0b0763dbabdd67798512411de4cdcf9431a1feb60fd9
        Status: Downloaded newer image for hello-world:latest

        Hello from Docker!
        This message shows that your installation appears to be working correctly.

        To generate this message, Docker took the following steps:
         1. The Docker client contacted the Docker daemon.
         2. The Docker daemon pulled the "hello-world" image from the Docker Hub.
         3. The Docker daemon created a new container from that image which runs the
            executable that produces the output you are currently reading.
         4. The Docker daemon streamed that output to the Docker client, which sent it
            to your terminal.

        To try something more ambitious, you can run an Ubuntu container with:
         $ docker run -it ubuntu bash

        Share images, automate workflows, and more with a free Docker Hub account:
         https://hub.docker.com

        For more examples and ideas, visit:
         https://docs.docker.com/engine/userguide/