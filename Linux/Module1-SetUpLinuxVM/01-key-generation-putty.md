# Create a pair of SSH Keys using PuTTY (Windows)

1. After you have installed PuTTY, you will need to open PuTTYgen.  Make sure you use *'SSH-2 RSA'* and *'4096'* bits and click on *'Generate'*

    ![alt text][puttygen]

1. Start moving your cursor in the indicated blank area until you fill up the green bar.

    ![alt text][puttygen-mouse]

1. The program will take a moment to generate your keys

    ![alt text][puttygen-generate]

1. As a result you will obtain your public key, you can go ahead and copy that in your server's configuration.

    ![alt text][puttygen-done]

1. Now lets save (both) your public and private key. You can manage your private key in a safer manner by using a Key passphrase (this passphrase will be required every time you interact with your private key).

    ![alt text][puttygen-keys]

[puttygen]: img/puttygen.jpg "PuTTYgen is the tool that allows you to create your SSH access keys"
[puttygen-mouse]: img/puttygen-mouse.jpg "Move your mouse to generate the key"
[puttygen-generate]: img/puttygen-gen.jpg "The time it takes depends on how strong you chose the key to be"
[puttygen-done]: img/puttygen-done.jpg "You can also add comments to the key"
[puttygen-keys]: img/puttygen-keys.jpg "You can use txt as an extension for your public key if you prefer it"
