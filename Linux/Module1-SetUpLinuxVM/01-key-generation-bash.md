# Create a pair of SSH keys using Bash (For Mac, Linux and Windows 10 with Bash installed)

1. Use *'ssh-keygen'* to create an RSA SSH key of 4096 bits pointing to our e-mail address.

    ```bash
    ssh-keygen -t rsa -b 4096 -C "your@email.com"
    ```

1. After this command you will be asked for a location to save your powerful key, you can give it a custom location but it's not really necessary unless you actually handle multiple keys.

    ```Shell
    Enter a file in which to save the key (/Users/you/.ssh/id_rsa): [Press enter]
    ```

1. Now you will be asked to enter a password to increase the level of security of your key. This password is optional, but if you decide to set it up you will need to enter it every time you interact with your SSH private key:

    ```Shell
    Enter passphrase (empty for no passphrase): [Type a passphrase]
    Enter same passphrase again: [Type passphrase again]
    ```

1. Now, after your key was created just *cat* the content of your public key (if you used a different location, make sure you use the correct file). This is the key you will be pasting in your azure cli to create the VM.

```Shell
cat ~/.ssh/id_rsa.pub
[This is an example of how and your pub key should look like:]
ssh-rsa 123456789/dTc6wJT+YCOUiLLS6F7Ge4WlCgmH7fW7UIUJpFcXwDv1bWVMQ3chBFFELWEhEjCqX7HAVoSjEF8oAwM0Ik5p6y66J420eeOGBLHkyV+nBiV0F5WVRKFS5Az1rZy8x/1usbMms/skMnS5Int9QcGIIA9g7Ws9xg28/2XA5IUPUZ0kIKbuSv7bAIqrHaH7WXzUeLeOjUIeW34d9WO52kNqiITjyW1D7kThXKtgS9Y5TEie5MuP8plzz+mBID59EFmdEhBK7QquuT6axXXXXXXXXXXXXXZ1rvoysOHxhDvzVWRuc623pV8PPjiBHiu1Y1T mymail@brus.ml
```
Note: Where "code" is, use your prefered text editor such as nano, vi, vim, emacs, etc. Also, copy the entire key to your keyboard for use in the vm creation process. This key should include "ssh-rsa" and the email address provided. It is strongly recommended that you DO NOT paste this to a notepad file.

## Following step

[Deploy VM with Azure-cli](02-deploy-vm.md)
