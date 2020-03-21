// Note this only includes basic configuration for development mode.
// For a more comprehensive configuration check:
// https://github.com/fable-compiler/webpack-config-template

var path = require("path");
var DotenvPlugin = require("dotenv-webpack");
var webpack = require("webpack");

const mode = "development";
module.exports = {
    mode: mode,
    entry: "./src/App.fsproj",
    output: {
        path: path.join(__dirname, "./public"),
        filename: "bundle.js",
    },
    devServer: {
        contentBase: "./public",
        port: 3000,
        hot: true,
        inline: true
    },
    plugins: mode === "development" ?
        // development mode plugins
        [
            new DotenvPlugin({
                path: path.join(__dirname, ".env"),
                silent: true,
                systemvars: true
            }),

            new webpack.HotModuleReplacementPlugin()
        ]
        :
        // production mode plugins
        [
            new DotenvPlugin({
                path: path.join(__dirname, ".env"),
                silent: true,
                systemvars: true
            })
        ],
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: [
                "fable-loader"
            ]
        }]
    }
}