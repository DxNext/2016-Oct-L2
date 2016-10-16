
## Overview
The OMS Agent for Linux enables rich and real-time analytics for operational data (Syslog, Performance, Alerts, Inventory) from Linux servers, Docker Containers and monitoring tools like Nagios, Zabbix and System Center.

## Quick Install guide
Run the following commands to download the omsagent, validate the checksum, and install+onboard the agent. *Commands are for 64-bit*. The Workspace ID and Primary Key can be found inside the OMS Portal under Settings in the **connected sources** tab.

```

$> wget https://github.com/Microsoft/OMS-Agent-for-Linux/releases/download/OMSAgent_Ignite2016_v1.2.0-75/omsagent-1.2.0-75.universal.x64.sh

$> sha256sum ./omsagent-1.2.0-75.universal.x64.sh

$> sudo sh ./omsagent-1.2.0-75.universal.x64.sh --upgrade -w <YOUR OMS WORKSPACE ID> -s <YOUR OMS WORKSPACE PRIMARY KEY>

```

## Azure Install guide
If you are an Azure customer, we have an Azure VM Extension that allows you to onboard with a couple of clicks.

- [OMS Agent for Linux Azure VM Extension Documentation](https://github.com/Microsoft/OMS-Agent-for-Linux/blob/master/docs/VM-Extension.md)

- [Azure Video walkthrough](https://www.youtube.com/watch?v=mF1wtHPEzT0)


## [Download Latest OMS Agent for Linux (64-bit)](https://github.com/Microsoft/OMS-Agent-for-Linux/releases/download/OMSAgent_Ignite2016_v1.2.0-75/omsagent-1.2.0-75.universal.x64.sh)
## [Download Latest OMS Agent for Linux (32-bit)](https://github.com/Microsoft/OMS-Agent-for-Linux/releases/download/OMSAgent_Ignite2016_v1.2.0-75/omsagent-1.2.0-75.universal.x86.sh)
