exports.command = function (userInfo) {
    const browser = this;
    browser
        // go to login url
        .url(browser.globals.login_url)
        .waitForElementVisible('.form-login', browser.globals.initial_load_timeout)
        // fill login form      
        .setValue('input[name=Email]', userInfo.email)
        .setValue('input[name=Password]', userInfo.password)
        .click('.form-login button.btn-default')
        .pause(1000)
        // After login, we should land in the home page logged in as the test user
        .assert.title('Home Page - BlogPlayground')
        .assert.containsText('.navbar form#logout-form', 'Hello Tester!');        

    return this; // allows the command to be chained.
};