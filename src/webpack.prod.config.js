const base = require('./webpack.config');
const merge = require('webpack-merge');
const webpack = require('webpack');
const path = require("path");
const ReplacePlugin = require('webpack-plugin-replace');
const uuidv4 = require('uuid/v4');

const generation = uuidv4();

module.exports = merge(base, {
    devtool: 'source-map',
    mode: 'production',
    output: {
        filename: () => `[name].${generation}.js`,
    },
    plugins: [
        new webpack.LoaderOptionsPlugin({
            minimize: true
        }),
        new ReplacePlugin({
            include: [
                /\.html/
            ],
            patterns: [
                {
                    regex: /main\.js/g,
                    value: `main.${generation}.js`
                },
                {
                    regex: /main\.css/g,
                    value: `main.${generation}.css`
                },
            ]
        })
    ],
});