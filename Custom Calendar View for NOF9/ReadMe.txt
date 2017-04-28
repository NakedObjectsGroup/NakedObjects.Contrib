This is a very crude implementation of a custom calendar view, which makes use of the ‘angular-calendar’ package
(rather than the JQuery calendar, that we used in NOF8).  

I suggest you experiment with this only after getting a much more basic custom view working.

The code two new components: custom-object and custom-calendar, 
each of which has the component, html, and css as usual for Angular components.  

You will need to add these in and make the necessary modifications (to e.g. app-module.ts & custom.service.ts), 
just as for the simple example of a custom view, that is explained in the NOF9 Developer Manual,
merging the code if you need both.  

You will also need to add a reference to the ‘angular-calendar’ (NPM) package into  packages.json i.e.:

"angular-calendar": "^0.14.0",

