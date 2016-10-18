# Connect to your VM using putty

1. The next thing we need to do is open PuTTY and use our brand new generated key to connect to our server.

    ![alt text][putty]

1. On the left menu go to *'Connection'* > *'SSH'* > *'Auth'*. And click on *'Browse'* to select the *'.ppk'* file you just created.

    ![alt text][putty-select]

1. Go back to the *'Session'* Category in the menu, put down the URL/IP of your server, write down a custom name for your connection and click on *'Save'*. Now every time you want to connect to that server you can load that configuration.

    ![alt text][putty-save]

## Following step

1. [Inside your VM](04-inside-vm.md)

[putty]: img/putty.jpg ""
[putty-select]: img/putty-select.jpg ""
[putty-save]: img/putty-save.jpg "You can save your changes every time you want"