# Inside a Linux Box

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

## File edition

1. **Nano** - Easiest to learn, more limited than `'vi'` or `'emacs'`.
1. **Vi, vim** - Very powerful, it take some time to get used to it.
1. **Emacs** - Very powerful as well, different form of input from `'vi'`. Not installed by default.
1. **VS Code, sublime** - Full code editor, but you can only edit one file at a time.

Lets take a moment to create and edit a file.

1. `'touch'` allows you to create an empty file, it also changes the timestamp of the file to the actual instant when the command is run.

    ```bash
    touch file.txt
    ```

1. Edit the file with `'nano'` and write `'Hola señor guapo'`, use `'Control + o'` to save and `'Control + x'` to close it.

    ```bash
    nano file.txt
    ```

1. Use `'cat'` to get the content of the file and `'>'` operator to create a new file. Type the following commands one by one.

    ```bash
    cat file.txt > fileAux.txt
    cat file.txt
    cat fileAux.txt
    diff file.txt fileAux.txt
    ```

1. While `'>'` replaces the content of the file, `'>>'` operator concats the string to the file. You can alwas use `'diff'` to see how different files are.

    ```bash
    cat file.txt >> fileAux.txt
    cat fileAux.txt
    diff file.txt fileAux.txt
    diff fileAux.txt file.txt
    ```

1. Edit the file using `'vi'` and replace the word `'señor`' with `'mister'`. To edit a file first press `<ESC>` and then `'i'` and `<ENTER>`, you can use the arrow keys to move around the file and change the file. Once you are done you should press `<ESC>` key again and then `'w'` and `<ENTER>` to save your changes and finally `'q'` to exit the file. If you make an error press `<ESC>` a couple of times and type `'q!'` and `<ENTER>` this will leave the file untouched so you can start again. [Here is a quick reference on the basic commands inside 'vi'](https://kb.iu.edu/d/afdc).

## Forwarding a port to use VS Code

We can also use `'vscode'` to edit our files in our computer.

1. Close the current SSH connection to your VM using the command `'exit'` and add a rule to the `'~/.ssh/config'` file in your bash terminal in your computer.

    ```bash
    RemoteForward 52689 localhost:52689
    ```

**Note:** Sometimes you need to resize the Windows 10 bash window to see the instructions of nano.

1. (Putty users) Go to `'Connection' > 'SSH' > 'Tunnels'` and add a remote rule for port `'52689'`. Remember to save your configuration in the profile.

![alt text][tunnel]

1. SSH back into your VM and install `'rmate'`.

    ```bash
    ssh myVM
    wget https://raw.githubusercontent.com/aurora/rmate/master/rmate
    chmod a+x rmate
    sudo mv rmate /usr/local/bin/
    ```

1. Install `'Remote VSCode'` extension in VS Code `'Control + Shift + p'` and type `'install'`

1. Configure the user settings, again `'Control + Shift + p'` and start typing `'user settings'`. Don't forget to add a comma to the previous values. 

    ```Shell
    // Port number to use for connection.
    "remote.port": 52689,

    // Launch the server on start up.
    "remote.onstartup": true
    ```

1. And profit!

    ```bash
    rmate -p 52689 file.txt
    ```
Note*: Textmate and Sublime also support this capability.

## Following step

1. [Module 2](../Module2-AppDeployment/readme.md)

[fhs]: img/fhs.jpg "This is what the filesystem in Linux looks like"

[tunnel]: img/putty-tunnel.jpg "Make sure you mark it as remote"
