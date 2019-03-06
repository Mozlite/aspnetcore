const webpack = require('webpack');
const path = require('path');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const CopyPlugin = require('copy-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const commonCSS = new ExtractTextPlugin('css/[name].css');
const minCSS = new ExtractTextPlugin('css/[name].min.css');
const inputDir = path.join(__dirname, 'wwwroot', 'src');
const outputDir = path.join(__dirname, 'wwwroot');
const libDir = 'lib/';

module.exports = (env) => {
    const isDev = !(env && env.prod);
    return [{
        entry: {
            site: path.join(inputDir, 'site.js'),
            account: path.join(inputDir, 'account.js'),
            admin: path.join(inputDir, 'admin.js'),
            select2: path.join(inputDir, 'select2.js')
        },
        output: {
            filename: 'js/[name].min.js',
            path: outputDir,
            chunkFilename: 'js/[name].min.js',
            sourceMapFilename: 'js/[name].map',
            publicPath: '../'
        },
        resolve: {
            extensions: ['.js', '.json']
        },
        module: {
            rules: [{
                test: /\.js$/,
                include: inputDir,
                exclude: /(node_modules|bower_components)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['env'],
                        plugins: ['transform-runtime']
                    }
                }
            },
            {
                test: /\.(png|jpg|jpeg|gif|svg)$/,
                use: 'url-loader?limit=8192&name=images/[hash].[ext]'
            },
            {
                test: /\.(woff|woff2|eot|ttf|otf)$/,
                use: 'url-loader?limit=8192&name=fonts/[hash].[ext]'
            },
            {
                test: /\.s?css$/, //移到单独得文件
                use: isDev ?
                    ExtractTextPlugin.extract({ use: ['css-loader', 'sass-loader'] }) :
                    ExtractTextPlugin.extract({ use: ['css-loader?minimize', 'sass-loader?minimize'] })
            }
            ]
        },
        externals: {
            jquery: "jQuery",
            mozlite: "Mozlite"
        },
        plugins: [
            new webpack.ProvidePlugin({
                $: "jquery",
                jQuery: "jquery",
                "window.jQuery": "jquery",
                "Mozlite": "mozlite",
                "window.Mozlite": "mozlite"
            }),
            new webpack.IgnorePlugin(/\/bootstrap\//),
            new webpack.ContextReplacementPlugin(/moment[\/\\]locale$/, /zh-cn/),
            new webpack.optimize.OccurrenceOrderPlugin(true),
            new CopyPlugin([
                { from: 'node_modules/mozlite/dist', to: libDir + 'mozlite' },
                { from: 'node_modules/jquery/dist/', to: libDir + 'jquery' },
                { from: 'node_modules/bootstrap/dist/', to: libDir + 'bootstrap' },
                { from: 'node_modules/popper.js/dist/umd/', to: libDir + 'bootstrap/js' },
                { from: 'node_modules/font-awesome/css', to: libDir + 'font-awesome/css' },
                { from: 'node_modules/font-awesome/fonts', to: libDir + 'font-awesome/fonts' },
                { from: 'node_modules/d3/dist/', to: libDir + 'd3' },
                { from: inputDir + '/lib/highlight/highlight.pack.js', to: 'js' }
            ])
        ].concat(isDev ? [commonCSS] : [new UglifyJsPlugin(), minCSS])
    }];
};