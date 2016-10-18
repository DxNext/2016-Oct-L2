# CentOS 101

## Where are my files?

When you first ssh into your machine you will find that you are located in the home directory of your user which is empty.
If you type the following command you will get your current location in the filesystem:

```Shell
pwd
```

Now, lets take a look at the root directory:

```Shell
ls -la /
```

CentOS follows the [Filesystem Hierarchy Standard(FHS)](http://www.pathname.com/fhs/) but just as an introduction lets name a couple of the most interesting folders.
![alt text][fhs]

## What process is my VM running?



## Users and permissions

[fhs]: ../../img/fhs.jpg "You can save your changes every time you want"