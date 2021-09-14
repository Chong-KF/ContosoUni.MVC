describe('Contoso Uni Login UI Test', function () {
    Cypress.Screenshot.defaults({ capture: 'runner' });
    let url = 'https://localhost:44313/';
    let firstrun = true;

    it('Login Admin', function () {

        const username = 'admin@gmail.com';
        const password = 'W7TzX6ZJjUQ-X74';
        const role = 'Administrator'

        login(username, password, role);
    });

    it('Login Staff', function () {

        const username = 'staff1@gmail.com';
        const password = 'Pass@1234';
        const role = 'Staff'

        login(username, password, role);
    });

    it('Login Student', function () {

        const username = 'student1@gmail.com';
        const password = 'Pass@1234';
        const role = 'Student'

        login(username, password, role);
    });

    it('Login Public', function () {

        const username = 'public1@gmail.com';
        const password = 'Pass@1234';
        const role = 'Public'

        login(username, password, role);
    });

    it('Login Errors Test', function () {

        //check login url
        cy.get('a')
            .contains('Login')
            .click();
        cy.url().should('include', '/Identity/Account/Login')

        // 1. check user name empty string
        cy.get('input[id="Input_Email"]')
            .type('{enter}');
        cy.get('span[id="Input_Email-error"]')
            .should('be.visible')
            .and('contain', 'The Email field is required.');

        // 2. check password empty string
        cy.get('input[id="Input_Password"]')
            .type('{enter}');
        cy.get('span[id="Input_Password-error"]')
            .should('be.visible')
            .and('contain', 'The Password field is required.');

        cy.screenshot('13 Login Page Error');

        //3. check invalid login
        cy.get('input[id="Input_Email"]')
            .type('admin@gmail.com{enter}');
        cy.get('input[id="Input_Password"]')
            .type('Password{enter}');
        cy.get('div[class="text-danger validation-summary-errors"]')
            .children('ul').children('li')
            .should('be.visible')
            .and('contain', 'Invalid login attempt.');

        cy.screenshot('14 Invalid login attempt');
    });

    function login(username, password, role) {
        cy.visit(url);
        if (firstrun) {
            cy.pause();
            firstrun = false;
        }

        cy.screenshot(role + ' 1 HomePage before login');

        cy.get('a')
            .contains('Login')
            .click();

        cy.url().should('include', '/Identity/Account/Login');
        cy.screenshot(role + ' 2 Login Page');

        cy.get('input[id="Input_Email"]')
            .type(username)

        cy.get('input[id="Input_Password"]')
            .type(password)

        cy.screenshot(role + ' 3 Logging in');

        cy.get('button')
            .contains('Log in')
            .click();

        if (role == 'Administrator')
        {
            cy.get('h1').then(($h1) => {
                if ($h1.text().includes('Two-factor authenticatio')) {
                    cy.get('h1').should('contain', 'Two-factor authentication');
                    cy.screenshot(role + ' 4 2FA Logging in');

                    cy.get('a[id="recovery-code-login"]')
                        .click();

                    cy.get('h1')
                        .should('contain', 'Recovery code verification');

                    //acb09f60 cf23f01a
                    cy.get('input[id="Input_RecoveryCode"]')
                        .type();

                    cy.screenshot(role + ' 5 2FA RecoveryCode Logging in');

                    cy.get('button')
                        .contains('Log in')
                        .click();
                }
            });
        }
        
        cy.get('a[title="Manage"]').should('contain', role);
        cy.wait(2000);

        cy.screenshot(role + ' 6 HomePage after login');

        Logout();
    }

    function Logout() {
        cy.get('button')
            .contains('Logout')
            .click();
        cy.url().should('eq', url);
        cy.get('a[href="/Identity/Account/Login"]').should('exist');
        cy.get('a[title="Manage"]').should('not.exist');
        cy.get('ul[class="navbar-nav flex-grow-1"]').within(() => {
            cy.get('a[href="/Departments"]')
                .should('not.exist');
            cy.get('a[href="/Subjects"]')
                .should('not.exist');
        });
    }
});