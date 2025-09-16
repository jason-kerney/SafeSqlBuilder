'use strict';

const childProcess = require('child_process');

childProcess.exec('booklisp ./readme.md ../README-tmp.md', function(error) {
    if(error) {
        console.log('An error occurred: ', error.message);
    } else {
        console.log('Compile complete');    
    }
});