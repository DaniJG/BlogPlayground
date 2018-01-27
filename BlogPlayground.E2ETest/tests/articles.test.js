'use strict';

const testArticle = {
    title: 'Testing with Nightwatch',
    abstract: 'This is an automated test',
    contents: 'Verifying articles can be added'
}

module.exports = {
    '@tags': ['articles-page'],
    
    'Articles can be opened with its url': function (browser) {
        browser
            // Open the articles list page
            .url(`${browser.launchUrl}/Articles`)
            .assert.title('Articles - BlogPlayground')
            .waitForElementVisible('body', browser.globals.navigation_timeout)
            // Verify at least the 2 default articles show up in the list
            .expect.element('.body-content .article-list li:nth-child(1)').to.be.present;
    },

    'New Articles can be added': function (browser) {
        browser
            // Go to the create page
            .url(`${browser.launchUrl}/Articles/Create`)
            .assert.title('Create - BlogPlayground')
            .waitForElementVisible('body', browser.globals.navigation_timeout)
            // Enter the details and submit
            .setValue('.body-content input[name=Title]', testArticle.title)
            .setValue('.body-content textarea[name=Abstract]', testArticle.abstract)
            .setValue('.body-content textarea[name=Contents]', testArticle.contents)
            .click('.body-content input[type=submit]')
            // Verify we go back to the articles list
            .pause(browser.globals.navigation_timeout)
            .assert.title('Articles - BlogPlayground');
    },

    'New Articles show in the Articles page': function (browser) {
        browser
            .assert.containsText('.body-content .article-list li:first-child', testArticle.title)
            .assert.containsText('.body-content .article-list li:first-child', testArticle.abstract)
            .assert.containsText('.body-content .article-list li:first-child .author-name', 'Tester');
    },

    'Articles can be read in their details page': function (browser) {
        browser
            // Open the article from the lisdt
            .click('.body-content .article-list li:first-child h4 a')
            // Verify navigation to the article details and the right contents are displayed
            .pause(browser.globals.navigation_timeout)
            .assert.title(`${testArticle.title} - BlogPlayground`)
            .assert.containsText('.body-content .article-summary', testArticle.title)
            .assert.containsText('.body-content .article-summary', testArticle.abstract)
            .assert.containsText('.body-content .article-summary .author-name', 'Tester')
            .assert.containsText('.body-content .markdown-contents', testArticle.contents);
    },

    'Articles can be removed': function (browser) {
        browser
            // Click remove on article details
            .click('.body-content .sidebar button.dropdown-toggle')
            .waitForElementVisible('.body-content .sidebar ul.dropdown-menu', browser.globals.navigation_timeout)
            .click('.body-content .sidebar ul.dropdown-menu li:last-child a')
            // Verify navigation to the confirmation page and click delete
            .pause(browser.globals.navigation_timeout)
            .assert.title('Delete - BlogPlayground')
            .click('.body-content input[type=submit]')
            // Verify navigation to articles list and that it disappeared from the list
            .pause(browser.globals.navigation_timeout)
            .assert.title('Articles - BlogPlayground')
            .assert.containsText('.body-content .article-list li:first-child', 'Test Article 2');
    }
};