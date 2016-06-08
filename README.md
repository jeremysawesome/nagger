# README #

### What is this repository for? ###
Nagger is meant to be a time tracking tool. The tool asks you a simple question every 15 minutes, "What are you doing?". From here you can select a story/task you are working on. Nagger also remembers the choice you made last time and has that prepopulated. So if you are working on the same thing you can simply type "y" for Yes, push Enter, and move on.

Nagger uses an internal database to track tasks and time. This means that you can track your time (for known tasks) even when you are not connected to the internet. At the beginning of every day, Nagger will attempt to log your time in your chosen remote repository. If Nagger does not have an internet connection it will continue to try to log the time every 15 minutes (the same interval it asks questions).

Nagger currently supports two repositories, JIRA and a propietary time-tracking site called Meazure.

The idea behind Nagger is to make it as painless as possible to track time. By proactively asking you what you are working on it removes the problems caused by having to remember to stop/resume timers. It also makes it so you do not have to remember to insert time every day. Finally, by asking you at a given interval, it assures that you track time against those one off tasks that might get forgotten at the end of the day (or at the end of the month).

Why the name Nagger? Well... because that's what it does.

 (The 15 minute increment might be made configurable, that is TBD). 

How to Install
--------------------

There is no installer at the moment, so you are just going to have to download the ZIP and extract it somewhere on your harddrive. Then you run the "nagger.exe" and it does the rest.

In some cases you might need to "Unblock" the zip file before extracting the contents. Otherwise you'll find that the program may not work correctly. You can do this by Right-clicking the downloaded zip file, selecting properties, and pressing the `Unblock` button within the Security section of that window.

Note: Nagger makes use of `%localappdata%\Nagger`.
 
### How do I get set up? ###

Nagger is still in development. Setting it up should be as simple as downloading the repo and opening the solution in Visual Studio. Nagger makes use of NuGet for some packages, so package restore should be enabled.

### Nagger Commands ###

By default Nagger will attempt to log time to the remote repository (i.e. JIRA) at the beginning of every day. However, using the "-push" command line argument you can manually tell Nagger to push all un-logged time entries to your remote repository. Example, running "Nagger.exe -push" will push all unlogged entries to your chosen remote repository.

### Contribution guidelines ###

* Writing tests
Tests have not been written as of yet. This is a future goal!

### Who do I talk to? ###

* jeremysawesome (jeremy@jeremysawesome.com)
