# Linux training


1.	Setting up the environment – 1h
    1.	Generate SSH keys
        1. [Bash](content/01-set-up/01-key-generation-bash.md)
        1. [Putty](content/01-set-up/01-key-generation-putty.md)
    1.	[Deploy VM with Azure-cli](content/01-set-up/02-deploy-vm.md)
    1.	Connect to VM
        1.	[Bash](content/01-set-up/03-connect-to-vm-bash.md)
        1.  [Putty](content/01-set-up/03-connect-to-vm-putty.md)
    1.	[Get to know your CentOS distro](content/01-set-up/04-intro-centos)
2.	Install MEAN – 1.5h
    1.	scp, wget, curl, tar
    1.	permissions
    1.	explore files
    3.	Configure mongodb   
    1.	Import database
    1.	Git integration and deployment
    
3.	Go live – 1h
    1.	Network configuration, open ports, endpoints in azure (portal, cli)
    1.	Vnets
    1.  Rewrite rules
    1.	Custom script extensions
    1.	[Export VHD](https://github.com/brusMX/linux-training/blob/master/content/03-go-prod/capture-azure-vm.md)
    1.  Scheduling jobs
    
4.	Logs and (security) tips - 1.5h
    1.  [Install OMS Agent](https://github.com/brusMX/linux-training/blob/master/content/04-logs-security/install-OMS-Agent-for-Linux.md)
    1.	Check logs (access log, error log, php verbose mode)
    1.	Check processes and users running them (Identify possible threats)
    1.	Check for unauthorized access and shells running in the VM
    1.	Check open ports with nmap
    1.	Provide a cheat sheet of commands
    9.	Importing/Exporting a Linux VM VHD from AWS or VMware
    1.	WA Linux Agent

    
5.	Containers - 2h
    1.	Docker intro
    1.	Deploy MEAN
    1.	Stateful vs stateless
    1.	Devops 
        1.	Deploy git code in dev machine
        1.	Commit changes to code
        1.	Publish changes to production server (Release work)
        1.	Mount volume in personal computer
        
6.	ACS - 1h
    1.	Docker Swarm
    1.	DC/OS
    
7.	Linux AppServices (Tuxedo)
    1.	Intro
    1.	Demo deploying Wordpress from git

8.	Linux takeaways (MKTG) BRUNO OGUZ
    1.	Advantages of running linux on Azure
    2.	Keypoints where Azure is better than AWS
    3.	When to choose what Linux distro
    4.	When to use which container/orchestration technology
        1.	Open shift
        2.	Swarm
        3.	DC/OS
        4.	Marathon
        5.	Kubernetes
