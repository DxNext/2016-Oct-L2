# Inside the Linux Box

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

## how to edit files

1. Vi, VIM
1. Nano
1. Emacs
1. 

[fhs]: ../../img/fhs.jpg "You can save your changes every time you want"