TwitDJ
======

A fun project utilizing Twitter and iTunes APIs to create a Twitter at-reply driven party DJ to keep the party going

This project relies on TweetSharp; install with NuGet in Visual Studio

This project also relies on the iTunes COM interface and having iTunes installed. Right-click the "Reference" entry in the project, "Add Reference", and searc for iTunes under COM. Add that reference.

Expected tweet format: "@<accountname> DJ:Artist|Song|(Album)" seperate each part with pipes. Album is optional. Also acceptable is "@<accountname> DJ:SHUFFLE" (can also use RANDOM instead of SHUFFLE). This adds a random song from the iTunes library to the playlist
