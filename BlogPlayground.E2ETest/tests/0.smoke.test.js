'use strict';

module.exports = {
    '@tags': ['smoke-test', 'home-page'],

    'can login with test user': function (browser) {        
        browser.login(browser.globals.user_info);        
    },

    'home page can be opened with default url': function (browser) {
        browser
            .url(browser.launchUrl)
            .assert.title('Home Page - BlogPlayground')
            .waitForElementVisible('body', browser.globals.navigation_timeout)
            .assert.containsText('.body-content #myCarousel .item:first-child', 'Learn how to build ASP.NET apps that can run anywhere.');
    },

    'can logout': function (browser) {
        browser
            .assert.containsText('.navbar form#logout-form', 'Hello Tester!')
            .click('.navbar form#logout-form button[type=submit]')
            .waitForElementVisible('.navbar .navbar-login', browser.globals.initial_load_timeout)
            .assert.containsText('.navbar .navbar-login', 'Log in')
            .assert.attributeContains('.navbar .navbar-login .login-link', 'href', browser.globals.login_url);
    }
};