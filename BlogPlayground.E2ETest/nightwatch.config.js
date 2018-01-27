const seleniumJar = require('selenium-server-standalone-jar');

// Nightwatch.js configuration.
// See the following url for a full list of settings: http://nightwatchjs.org/gettingstarted#settings-file
const settings = {
    // Nightwatch global settings
    src_folders: ['./tests'],
    output_folder: '.test-results/',

    // Nightwatch extension components (commands, pages, assertions, global hooks)
    globals_path: './globals.js',
    custom_commands_path: './commands',
    //'page_objects_path ': './pages',
    //custom_assertions_path: './assertions'
    
    // Selenium settings
    selenium: {
        start_process: true,
        server_path: seleniumJar.path,
        start_session: true,
        log_path: '.test-results/',
        port: process.env.SELENIUM_PORT || 4444,
        host: process.env.SELENIUM_HOST || '127.0.0.1',
        debug: true,
        cli_args: {
            'webdriver.edge.driver': './selenium-drivers/MicrosoftWebDriver.exe',
            'webdriver.chrome.driver': '',
            'webdriver.chrome.logfile': '.test-results/chromedriver.log',
            'webdriver.chrome.verboseLogging': false,
        }
    },

    test_settings: {
        default: {
            // Nightwatch test-specific settings
            launch_url: process.env.LAUNCH_URL || 'http://localhost:56745',
            selenium_port: process.env.SELENIUM_PORT || 4444,
            selenium_host: process.env.SELENIUM_HOST || 'localhost',
            silent: true,
            screenshots: {
                enabled: true,
                on_failure: true,
                on_error: true,
                path: '.test-results/screenshots'
            },
            // browser-related settings. To be defined on each specific browser section
            desiredCapabilities: {
            },
            // user defined settings
            globals: {
                window_size: {
                    width: 1280,
                    height: 1024
                },
                start_app: process.env.LAUNCH_URL ? false : true,
                login_url: (process.env.LAUNCH_URL || 'http://localhost:56745') + '/Account/Login',
                user_info: {
                    email: 'tester@test.com',
                    password: '!Covfefe123',
                },
                navigation_timeout: 5000,
                initial_load_timeout: 10000
            }
        },
        // Define an environment per each of the target browsers
        chrome: {
            desiredCapabilities: {
                browserName: 'chrome',
                javascriptEnabled: true,
                acceptSslCerts: true,
                chromeOptions: {
                    args: [
                        '--headless',
                        '--no-sandbox',
                        '--disable-popup-blocking',
                        '--disable-infobars',
                        '--enable-automation',
                    ],
                    //useAutomationExtension: true,
                    //prefs: {
                    //    credentials_enable_service: false,
                    //    profile: {
                    //        password_manager_enabled: false,
                    //    },
                    //},
                }
            },
        },
        edge: {
            desiredCapabilities: {
                browserName: 'MicrosoftEdge',
                javascriptEnabled: true,
                acceptSslCerts: true
            }
        }
    }
};

//make output folder available in code (so we can redirect the dotnet server output to a log file there)
settings.test_settings.default.globals.output_folder = settings.output_folder;

//Set path to chromedriver depending on host OS
var os = process.platform;
if (/^win/.test(os)) {
    settings.selenium.cli_args['webdriver.chrome.driver'] = './selenium-drivers/chromedriver-2.35-win.exe';
} else if (/^darwin/.test(os)) {
    settings.selenium.cli_args['webdriver.chrome.driver'] = './selenium-drivers/chromedriver-2.35-mac.exe';
} else {
    settings.selenium.cli_args['webdriver.chrome.driver'] = './selenium-drivers/chromedriver-2.35-linux.exe';
}

// Display effecctive settings on test run output
console.log(JSON.stringify(settings, null, 2));

module.exports = settings;