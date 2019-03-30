import "./app";
require.context("../../output", true, /.*html$/);
require.context("../../output", false, /.*(xml|txt)$/);