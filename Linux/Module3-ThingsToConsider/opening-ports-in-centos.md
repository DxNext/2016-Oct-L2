The way to configure firewalls depends on the Linux distribution.

For CentOS 7.2, you can use the following commands to turn on the firewall and open a port.

1. Turn on the the firewall service
```
sudo systemctl start firewalld.service
```
2. Open port TCP 5000. Change the port number and protocol depending on your needs.

```
sudo firewall-cmd --zone=public --add-port=5000/tcp
```

You should see a message like the following:

```
[oguzp@oguzp-centos72 ~]$ sudo firewall-cmd --zone=public --add-port=5000/tcp
success
```
You have now successfuly opened port 5000 in the Linux firewall.

If you want to learn more about managing the firewall on CentOS 7.2, you may use the [tutorial here.](https://www.digitalocean.com/community/tutorials/how-to-set-up-a-firewall-using-firewalld-on-centos-7)
