# README #

### What is this repository for? ###
Nagger is meant to be a time tracking tool. The tool asks you a simple question every 15 minutes, "What are you doing?". From here you can select a story/task you are working on. Nagger also remembers the choice you made last time and has that prepopulated. So if you are working on the same thing you can simply push the "Confirmation" button and keep doing what you are doing. 

Nagger uses an internal database to track tasks and time. This means that you can track your time (for known tasks) even when you are not connected to the internet. At the beginning of every day, Nagger will attempt to log your time in your chosen remote repository. The only remote repository that is planned to be added at the moment is for JIRA. If Nagger does not have an internet connection it will continue to try to log the time every 15 minutes (the same interval it asks questions).

The idea behind Nagger is to make it as painless as possible to track time. By proactively asking you what you are working on it removes the problems caused by having to remember to stop/resume timers. It also makes it so you do not have to remember to insert time every day. Finally, by asking you at a given interval, it assures that you track time against those one off tasks that might get forgotten at the end of the day (or at the end of the month).

Why the name Nagger? Well... because that's what it does.

 (The 15 minute increment might be made configurable, that is TBD). 

### How do I get set up? ###

Nagger is still in development. Setting it up should be as simple as downloading the repo and opening the solution in Visual Studio. Nagger makes use of NuGet for some packages, so package restore should be enabled.

### Contribution guidelines ###

* Writing tests
Tests have not been written as of yet. This is a future goal!

### Who do I talk to? ###

* jeremysawesome (jeremy@jeremysawesome.com)