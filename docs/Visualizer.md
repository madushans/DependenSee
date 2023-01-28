# Visualizer

DependenSee Visualizer is a web app that displays the output of DependenSee discovery results, and allows filtering and exploring of that dataset. This is written in React, however unlike typical react apps that are served over a web server, with multiple accompanying html, css and js files, DependenSee visualizer is a single self-contained html file.

This makes it easy for the visualizer to run, with close to no knowledge of how web apps work, and with no accompanying server. Users of the CLI also benefit from this design where they do not need any node or other infrastructure installed, or know any special instructions on running the visualizer. Since it is a single html file, it will run on any modern browser, just like any other html file.

# Integration

The CLI does not run the react project, rather it embeds the final html file in itself. When a discovery result is ready, it replaces known token(s) in the HTML file with the corresponding results and writes it to the specified location.

This has a few advantages

- CLI does not know or depend on any frontend technology.
- CLI does not have to run a node server or require any dependencies that require running node servers.


# Setup

Visualizer is based on the setup described in this [StackOverflow answer](https://stackoverflow.com/a/69594493/975887), inlined below in case it were to be removed. (Note that DependenSee uses `npm` instead of `yarn`)

---

> Previous answers don't work because html-webpack-inline-source-plugin is no longer supported by the author, replaced by official plugins html-inline-script-webpack-plugin and html-inline-css-webpack-plugin. The following method works with the latest React 17 as of October 2021. Steps:

> **Create a React app**

> `create-react-app my-app`

> `cd my-app`

> `yarn`

> `yarn start`

>Make sure it's working, you're happy with the output. Then, eject your config (I think this can be done without ejecting config, but I can't be bothered to look up how to configure CRA)

> `yarn eject`

> `rm -rf build node_modules`

> `yarn`
> `yarn build`

>Then, add the Webpack plugins (Assuming you want both CSS and scripts embedded here, just remove one if you only want the other)

>`yarn add html-inline-css-webpack-plugin -D`

>`yarn add html-inline-script-webpack-plugin -D`

>Finally, add these to the Webpack config config/webpack.config.js. Firstly declare them at the top of the file:

> `const HTMLInlineCSSWebpackPlugin = require('html-inline-css-webpack-plugin').default;`

>`const HtmlInlineScriptPlugin = require('html-inline-script-webpack-plugin');`

>Then add them to the plugins: [...] collection. Search for new InlineChunkHtmlPlugin and add them just after that:

>      isEnvProduction &&
>        shouldInlineRuntimeChunk &&
>        new HTMLInlineCSSWebpackPlugin(),
>      isEnvProduction &&
>        shouldInlineRuntimeChunk &&
>        new HtmlInlineScriptPlugin(),


>>Note: It is important to include the isEnvProduction / shouldInlineRuntimeChunk checks! If you skip these the hot refresh won't work when you run yarn start. You only want to embed the scripts in production mode, not during development.

> Now, if you run yarn build you'll find all the CSS and JS embedded in build/index.html. If you run yarn start it'll start a hot-reloading dev environment you can develop in.

>> You probably have to add this to your `HtmlWebpackPlugin` configuration inside your webpack.config.js. Look for the inject property and change it's value to body. This makes sure that your JavaScript is loaded after your HTML is done loading. The issue your probably having is that your script is loaded before your HTML is ready.

---

# Drawbacks

- Visualizer expects above mentioned webpack plugins to be available. They have changed in the past, so it is likely they may get deprecated or otherwise be unavailable in the future, where we will have to find replacements.

- Images are not inlined. Currently any and all images must be manually embedded via base64 encoding.
