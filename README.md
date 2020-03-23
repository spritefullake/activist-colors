# Activist Colors

## Why?

All across the world, marginalized groups of people are subject to the cruelty of oppressors. And while we don't always have the power to stop injustice, **we can always start a movement to uphold justice**. 

I want to highlight some peoples who have unduly suffered from things like war, state violence, forced expulsion, flotillas and blockades, ethnic cleansing and occupation. In all cases, these have been human-created injustices which threaten the humanity of others. I hope this reminds those of us in safe refuges that **there are still refugees who need a home** and there is still so much that can be done to help.


---

Let's promote compassion, justice and empathy for all 🌐

🌎🌍🌏


#### Works in progress

This project is my first Fable project, so I am still learning how to improve it. Advice is welcome.

1. ##### Feature the following groups
      * Sudanese
      * Kashmiris
      * Uighurs
      * Rohingya
      * Palestinians
      * Yemenis
      * Syrians
      * Libyans
      * (To be continued)
2. ##### Allow images to be shared across social media
3. ##### Links to charities and resources

## Requirements

* [dotnet SDK](https://www.microsoft.com/net/download/core) 2.1 or higher
* [node.js](https://nodejs.org) with [npm](https://www.npmjs.com/)
* An F# editor like Visual Studio, Visual Studio Code with [Ionide](http://ionide.io/) or [JetBrains Rider](https://www.jetbrains.com/rider/).

## Building and running the app

* Install JS dependencies: `npm install`
* Start Webpack dev server: `npx webpack-dev-server` or `npm start`
* After the first compilation is finished, in your browser open: http://localhost:8080/

Any modification you do to the F# code will be reflected in the web page after saving.

## Project structure

### npm

JS dependencies are declared in `package.json`, while `package-lock.json` is a lock file automatically generated.

### Webpack

[Webpack](https://webpack.js.org) is a JS bundler with extensions, like a static dev server that enables hot reloading on code changes. Fable interacts with Webpack through the `fable-loader`. Configuration for Webpack is defined in the `webpack.config.js` file. Note this sample only includes basic Webpack configuration for development mode, if you want to see a more comprehensive configuration check the [Fable webpack-config-template](https://github.com/fable-compiler/webpack-config-template/blob/master/webpack.config.js).

### F#

The F# files are in the `src` folder.

### Web assets

The `index.html` file and other assets like an icon can be found in the `public` folder.
