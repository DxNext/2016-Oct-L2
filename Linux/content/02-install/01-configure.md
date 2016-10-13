# Install the MEAN Stack

Now that we are inside our machine we need to install these tools:

1. MongoDB
1. Node.js
1. Bower and Gulp
1. MEAN

## Package managers (.deb and .rpm)

There are many out in the wild but these are some of the most popular ones. The first one is the high-level package manager that provides a simple way to retrieve and install packages including dependency resolution and multiple sources. Then we have the low-level package manager that can only install, remove, provide info and build packages. The con is that it does not automatically download and install the package dependencies.

1. **YUM -> RPM** - Red Hat Enterprise Linux (RHEL), CentOS, Scientific Linux, Fedora, etc.
1. **Advanced Packaging Tool (APT-GET) * -> DPKG** - Ubuntu, Debian, KNOPPIX, etc.(Debian based distros)
1. **ZYPPER -> RPM** - Open SUSE, SUSE Linux Enterprise Server

### $ sudo yum install

1. First of all lets get the latest definitions for yum:

```Shell
sudo yum update
```

Confirm the update

```Shell
Transaction Summary
================================================
Install   1 Package
Upgrade  69 Packages

Total download size: 96 M
Is this ok [y/d/N]: y
```

1. While this is working lets open a new SSH connection to our server and run the following command

```Shell
ls -la /etc/yum.repos.d/
```

This folder contains all the information and repos that yum uses to maintain the packages installed in our OS. The listed files are the basic repos installed by default in our CentOS VM.

```Shell
cat /etc/yum.repos.d/CentOS-Sources.repo
```

For instance the source repo contains the definitions to the base, release, extra and extended functionality packages.

The update process should have ended by now, so lets close the connection and lets get back to our previous terminal.

```Shell
exit
```

In our main terminal we should see something like this:

```Shell
Verifying  : systemd-libs-219-19.el7_2.9.x86_64              139/139

Installed:
  kernel.x86_64 0:3.10.0-327.36.1.el7

Updated:
  NetworkManager.x86_64 1:1.0.6-31.el7_2                    NetworkManager-libnm.x86_64 1:1.0.6-31.el7_2              NetworkManager-team.x86_64 1:1.0.6-31.el7_2
  NetworkManager-tui.x86_64 1:1.0.6-31.el7_2                bash.x86_64 0:4.2.46-20.el7_2                             bind-libs.x86_64 32:9.9.4-29.el7_2.4

Complete!
```

1. Now we must install the a set of basic tools we need:

```Shell
sudo yum install gcc-c++ make git fontconfig bzip2 libpng-devel ruby ruby-devel
```

## MongoDB

1. First lets add the MongoDB repository to yum. To do that we need to add a file containing the MongoDB info to the `'/etc/yum.repos.d/'` folder. In this repo we can find that file in `'/materials/mongodb.org-3.2.repo'`.

1. We will accomplish this task using the command [`scp`](http://www.hypexr.org/linux_scp_help.php).

    For Linux, Mac or Windows 10 with bash open a new terminal and go to this repo using the `'cd'` command and then copy the file using scp:

    ```Shell
    cd /this-repo/
    scp materials/mongodb.org-3.2.repo myVM:/home/vmUsername
    ```

    For PuTTY we need to do the same in CMD or PowerShell but using the command `'pscp'`

    ```Shell
    cd /this-repo/
    pscp materials/mongodb.org-3.2.repo myVM:/home/vmUsername
    ```

    The Transaction should be over in a second:

    ```Shell
    mongodb.org-3.2.repo                           100%  204     0.2KB/s   00:00
    ```

    In our server shell we can verify its there by typing `'ls'` in our home directory

    ```Shell
    ls
    ...
    mongodb.org-3.2.repo
    ```

1. Using the `'mv'` command we can put it in its place.

    ```Shell
    sudo mv mongodb.org-3.2.repo /etc/yum.repos.d
    ```

1. We need to make sure it has the right permissions and ownership, `'ls -la'` allows us to see who owns the file and what permissions it has assigned.
    ```Shell
    ls -la /etc/yum.repos.d
    ```

    ```Shell
    total 52
    drwxr-xr-x.  2 root  root  4096 Oct 12 23:07 .
    drwxr-xr-x. 87 root  root  8192 Oct 12 21:58 ..
    -rw-r--r--.  1 root  root  1533 Jun 20 19:15 CentOS-Base.repo
    -rw-r--r--.  1 root  root  1309 Dec  9  2015 CentOS-CR.repo
    -rw-r--r--.  1 root  root   649 Dec  9  2015 CentOS-Debuginfo.repo
    -rw-r--r--.  1 root  root   290 Dec  9  2015 CentOS-fasttrack.repo
    -rw-r--r--.  1 root  root   630 Dec  9  2015 CentOS-Media.repo
    -rw-r--r--.  1 root  root  1331 Dec  9  2015 CentOS-Sources.repo
    -rw-r--r--.  1 root  root  1952 Dec  9  2015 CentOS-Vault.repo
    -rwxrwxr-x.  1 user  user   204 Oct 12 23:01 mongodb.org-3.2.repo
    -rw-r--r--.  1 root  root   282 Jun 20 19:15 OpenLogic.repo
    ```

    The first thing we noticed is tha the file is owned by `'user'`  from the group `'user'` (the user and group that created the file) while the rest of the files belong to `'root'`. To transfer the file to root we need to run the following command:

    ```Shell
    sudo chown root:root /etc/yum.repos.d/mongodb.org-3.2.repo
    ```

    The file now belongs to `'root'`, you can verify this with the `'ls -la'` command.

    ```Shell
    -rwxrwxr-x.  1 root root  204 Oct 12 23:01 mongodb.org-3.2.repo
    ```

    We can notice that the file has the following permissions:

    ```Shell
    - rwx rwx r-x.
    ```

    Permissions follow the next rule:

    ![alt text][permissions]
    There are multiple ways of fixing this file's permissions. Choose one of the followin commands to do so:

    Octal

    ```Shell
    sudo chmod 644 /etc/yum.repos.d/mongodb.org-3.2.repo
    ```
    Removing multiple permissions

    ```Shell
    sudo chmod a-x,g-w,o-w /etc/yum.repos.d/mongodb.org-3.2.repo
    ```
    Setting each specific permission
    ```Shell
    sudo chmod u=rw,g=r,o=r /etc/yum.repos.d/mongodb.org-3.2.repo
    ```
    Mixed mode
    ```Shell
    sudo chmod a=r,u+w /etc/yum.repos.d/mongodb.org-3.2.repo
    ```
    And if you were wondering about the sticky bit: When enabled, it prevents users in that directory from deleting files and folders that don't belong to them.
    Please verify that your file has the following permissions:
    ```Shell
    -rw-r--r--.  1 root root  204 Oct 12 23:01 mongodb.org-3.2.repo
    ```

1. Install MongoDB
    ```Shell
    sudo yum install mongodb-org
    ```
    Confirm the installation, we should see something like this:
    ```Shell
    Installed:
        mongodb-org.x86_64 0:3.2.10-1.el7

    Dependency Installed:
        mongodb-org-mongos.x86_64 0:3.2.10-1.el7   mongodb-org-server.x86_64 0:3.2.10-1.el7   mongodb-org-shell.x86_64 0:3.2.10-1.el7   mongodb-org-tools.x86_64 0:3.2.10-1.el7

    Complete!
    ```


1. Start the service:
    ```Shell
    sudo systemctl start mongod
    ```

    And make sure it's running
    ```Shell
    sudo systemctl status mongod
    ```

    ```Shell
    ● mongod.service - SYSV: Mongo is a scalable, document-oriented database.
    Loaded: loaded (/etc/rc.d/init.d/mongod)
    Active: active (running) since Thu 2016-10-13 01:39:07 UTC; 5s ago
        Docs: man:systemd-sysv-generator(8)
    Process: 48902 ExecStart=/etc/rc.d/init.d/mongod start (code=exited, status=0/SUCCESS)
    CGroup: /system.slice/mongod.service
            └─48913 /usr/bin/mongod -f /etc/mongod.conf

    Oct 13 01:39:07 chentos systemd[1]: Starting SYSV: Mongo is a scalable, document-oriented database....
    Oct 13 01:39:07 chentos runuser[48909]: pam_unix(runuser:session): session opened for user mongod by (uid=0)
    Oct 13 01:39:07 chentos runuser[48909]: pam_unix(runuser:session): session closed for user mongod
    Oct 13 01:39:07 chentos mongod[48902]: Starting mongod: [  OK  ]
    Oct 13 01:39:07 chentos systemd[1]: Started SYSV: Mongo is a scalable, document-oriented database..
    ```


## Node.js

1. First install the epel-release

    ```Shell
    sudo yum install epel-release
    ```
1. Install node
    ```Shell
    sudo yum install nodejs
    ```
1. Check the installed version
    ```Shell
    node --version
    v6.7.0
    ```

[permissions]: ../../img/permissions.png "You can save your changes every time you want"
