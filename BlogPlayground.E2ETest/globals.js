const childProcess = require('child_process');
const path = require('path');
const fs = require('fs');

let dotnetWebServer = null;
let dotnetWebServerStarted = false;

function startWebApplication(outputFolder, done) {
    const logFile = path.join(outputFolder, 'server.log');
    console.log(`Starting web application. Log found at: ${logFile}`);

    // Start web app in separated process.
    dotnetWebServer = childProcess.spawn("dotnet", ["run"]);

    // Fail test run startup if the server dies before it got properly started
    dotnetWebServer.on('close', (code) => {
        if (code !== 0 && !dotnetWebServerStarted) {
            console.error(`Could not start dotnet server. Exited with code ${code}. Check log at ${logFile}`);
            process.exit(-1);
        }
    });

    // Do not start the test until we see the "Application started" message from dotnet
    dotnetWebServer.stdout.on('data', (chunk) => {        
        if (chunk.toString().includes("Application started")) {
            dotnetWebServerStarted = true;
            done();
        }
    });

    // Redirect the standard output of the web application to a log file  
    const appLogPath = path.join(__dirname, logFile);
    const childServerLog = fs.createWriteStream(appLogPath);
    dotnetWebServer.stdout.pipe(childServerLog);
    dotnetWebServer.stderr.pipe(childServerLog);
}

module.exports = {
    // Specific initialization per browser, available in before/after methods as this.my_setting
    // 'default' : {
    //   my_setting : true,    
    // },
    // 'chrome' : {
    //   my_setting : true,
    // },

    // External before hook is ran at the beginning of the tests run, before creating the Selenium session
    before: function (done) {

        // run this only if we want to start the web app as part of the test run    
        if (this.start_app) {
            startWebApplication(this.output_folder, done);
        } else {
            done();
        }
    },

    // External after hook is ran at the very end of the tests run, after closing the Selenium session
    after: function (done) {
        // run this only if we start the app as part of the test run
        if (this.start_app) {
            // Kill the dotnet web application
            const os = process.platform;
            if (/^win/.test(os)) childProcess.spawn("taskkill", ["/pid", dotnetWebServer.pid, '/f', '/t']);
            else dotnetWebServer.kill('SIGINT');

            done();
        } else {
            done();
        }
    },

    // This will be run before each test suite is started
    beforeEach: function (browser, done) {
        // Set specific browser window size    
        browser
            .resizeWindow(browser.globals.window_size.width, browser.globals.window_size.height)
            .pause(500);

        // Every test will need to login with the test user (except in the smokeTest where login is part of the test itself)
        if ( !browser.currentTest.module.endsWith("smoke.test")) {
            browser.login(browser.globals.user_info);
        }

        //Once all steps are finished, signal we are done
        browser.perform(function () {
            done();
        });
    },

    // This will be run after each test suite is finished
    afterEach: function (browser, done) {
        //close the browser then signal we are done
        browser
            .end()
            .perform(function () {
                done();
            });
    }
};