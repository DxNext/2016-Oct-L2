# Company & Product Overview 
  
Parts Unlimited is a company selling auto parts.
The competition is fierce and gaining market shares everyday through an efficient online presence and more agility in their marketing activities.  
The company’s new IT initiative, code name Project Phoenix, an e-commerce website, is critical to the future of Parts Unlimited, but the project is massively over budget and very late due to a lack of automation and quality assurance. 
  
## Project Phoenix’s Team
 
*	1 Product Owner (PO) 
*	1 Project Manager (PM) 
*	5 Developers 
*	2 Front-End 
*	3 Back-End 
*	1 Ops
*	1 Q.A / Tests 
 
## How is the project managed? 
 
We use the Scrum framework. Every week, the Product Owner will meet with the executive team and fill the backlog based on customer feedback and their own intuition about what needs to be done. 
The Project Manager will then choose features from the backlog and write the related User Stories (US). 
 
Every two weeks, the whole teams meet and assign points to each story, raise questions and discuss missing details. 
Since we don't have time to correctly analyze every US, we will often notice issues while working on them, creating a lot of emergency meetings, but that mostly works... 
 
 
## How is the product released? 
 
The executive team and the project manager meet every month to review the changes that have been implemented. They also check the report of the QA. Everyone have the power of veto and can for any reason stop the release of a version based on the business impact it has.  


# Ops

## Could you introduce yourself? 
  
Hi, my name is Yann, I have been working at Parts Unlimited for three years now and I am a System Engineer in charge of our infrastructure and our databases. 
 
## Can you give us a high level overview of the architecture and tools? 

Phoenix is hosted on Azure on an Ubuntu virtual machine.
On this VM, we have installed Tomcat to host the Java application as well as MongoDB.
    
## What is your responsibility on the Phoenix Project? 
  
I am in charge of the Integration and Production environments, no one else is allowed to make modification to this environments, especially not the developers! 
When the dev team releases a new version, I grab the packages (`.Jar` and `.War`) from a shared drive and deploy them first on Integration so that the QA team can do their stuff.
Once the QA team decides that everything is fine, I deploy the production environment.
  
## Did you automate the deployment? 
  
More or less, I have a shell script to help me install the dependencies using apt-get and other tools. But there is still a lot of manual steps to go through after that to be up and running.

## Did you automate the VM creation on Azure? 
  
No, actually we don't need it, we are using the portal but anyway we never destroy the existing VMs so we very rarely need to create new ones.
I tried to create documentation on how to reproduce this environment (Creation of the Vnet, VM, Storage, VIP, etc…). It’s somewhere on our shared drives, but I didn’t update it since a long time for various reasons.
  
## Can you tell me more on the deployment process of Phoenix? 
 
We have two packages:
*	Ordering
*	Web 
 
On each deployment, I have two main steps:  
*	Configure the packages for the environment:
*	Update the database endpoint 
*	Pre / Post scripts to run for the deployment (for example to update the database schema)
*	Login and password 
*	Tomcat port configuration 
*	Hostname configuration
*	Probably more stuff that I’m forgetting 
*	The deployment by itself 
*	`curl` the packages from the shared drive
*	Backup the database (just in case)
*	Kill the current Java instance 
*	Uninstall the old packages
*	Copy the new ones inside the tomcat webapps folder 
*	Restart tomcat 
   
I am working on a shell script to automate all that but for now it is taking me around 15 minutes for the configuration parts and 20 minutes to for the actual deployment.

I have to say, the configuration step is really not efficient, we have a lot of issues on that one due to a lack of communication with the dev teams. Usually it takes between 2 or 3 tries before the deployments actually works.
 
Currently I am the only one in the team who can deploy the application, I have to work on the documentation and the scripts. 
   
  
## What happened last time you release Phoenix in production? Any trouble? 
  
Oh boy… 
When QA is done, I have to do the deployment after the business hours within 2 days. 
But last time we had so much issues I actually couldn’t find time to do it before the next Friday (5 days later).
I had to stay in the office until 5 am because the deployment was a nightmare, too many things had changed that I didn’t knew about, I had to deploy 5 times before it actually worked…
   
## Are you running any tests on your side before / during / after your deployment? 
  
The only tests that I am doing on my side is to connect to the application with my browser and check each part on the website to make sure nothing is down, we call that a "Smoke Test"… It’s really just a 5-10 minutes sanity check. 
 
# Dev

## Profile 
Rodrigo is a Java developer at Parts Unlimited. He is working on the Phoenix project as team lead in a team of 5 developers. 
He is comfortable with Agile and Scrum processes having used them for quite a long time.  
 
## Can you tell us a little bit about the Phoenix project? What are the technology used? 
 
Phoenix is written in Java. As such our team is using Eclipse as IDE. The solution is built using Gradle. Unit tests are written using the JUnit framework (but we don't really write unit test anymore). 
The code is hosted on a Github private repository, this is also where bugs, missing feature etc are tracked. 
 
## What are your commitments as a team lead? 
 
Our customers are very demanding and the competition is fierce, so we have to ship a lot of stuff. 
Our sales team also will sell things that we don't have to close a deal... So we have to deliver features in a rush, which disrupts our plans. So very often everything is shipped hastily. 
That means unit and integrations tests aren't as complete as we would like them to be, but well, we have to ship. 
 
## How long does it takes you to implement new features? 
We have been doing scrum for a long time with 2 weeks’ sprints. We try to ship new version to the QA team at the end of every sprint.
 
## You are using Git then. What's your branching model? 
 
We follow the standart git-workflow, like pretty much everyone else I guess? 
Devs are supposed to be working on a feature branch. Once their work there is done and (sometimes) reviewed, they can merge on the develop branch. 
This is often pretty tricky and time consuming since all the devs tend to merge on develop at the same time towards the end of the sprint. 
When we feel like the develop branch is mature enough (or when we have to deliver a feature asap), the dev branch is merged into master.

## How do you hand off your work to the QA team? 
I build the solution on my machine, and drops the artifact on the shared drive. I shoot a mail to the Ops team telling them they can deploy the QA environment. When they are done they let the QA team know.
  
## You mentioned that you build the solution on your machine, don't you have a build server? 
 
Well, no.  We asked for one multiple times, but never got the budget so we have to build on our machine.
 
## How do you ensure the quality of what you ship? Do you have any kind of tests? 
 
When the project started we tried to have a good unit test coverage. That worked for some times. 
But then two things happened: The deadline was getting closer, so we had pressure to deliver faster, hence we pretty much stopped writing unit tests. 
And since we don't have any centralized build server, we don't know who broke a unit test, and of course everyone assumes that someone else did it, so no one takes time to fix it. 
Very quickly we had dozens of deprecated tests so we just stopped running them. We always say we will go back and fix them when we will have more time, but well.... 
 
We don't have any integration tests. 
So basically, we run some smoke tests on our machines and then let the QA team do it's magic. 


# QA

## Can you introduce yourself? 
 
My name is Michelle, I'm the only QA on Project Phoenix. 
 
## How do you know what you are testing? 
 
Yann, our Ops guy, sends me an email to let me know he deployed a new version
It also happens that he forget to send the email, and then I'll spend two days testing something different than what I thought I was testing... 
 
## Are you satisfied with the quality of the software? 
 
I'm not. It feels like I'm trying to dig myself out of a hole. 
The application is a single huge package. And the developers never tried to minimize the dependencies between the different components, they supposedly never had the time to do it. 
This means that a change in the code base, can introduce a bug anywhere in the application! 
So I have to check everything, for every single change... 
The worst part is that if I find a bug, when a developer push a new version containing the fix, I have to recheck everything, because that fix, could have caused a bug somewhere else! 
It's a problem with no end.... 
I have to do as much as 5 full QA round on every release before we are satisfied (meaning we don’t think we can do any better) with the quality of the application.
 
## How long does a full QA pass usually takes you? 
 
Around 5 days to test the full application. 
But I'm working on several projects, so when the QA environment is deployed I'm not always available to test it. It may take up to a week before I can actually start my QA. 
 

 
 
 
 
 


