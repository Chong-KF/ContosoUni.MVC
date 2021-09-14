describe('Contoso Uni Department User View Test', function () {
    Cypress.Screenshot.defaults({ capture: 'runner' });
    let url = 'https://localhost:44313/';
    let firstrun = true;

    it('Department Admin View', function () {

        const username = 'admin@gmail.com';
        const password = 'W7TzX6ZJjUQ-X74';
        const role = 'Administrator'

        login(username, password, role);
        DepartmentCheck(role);
        DepartmentAdmistratorUndelete();
        Logout();
    });

    it('Department Admin Delete View', function () {

        const username = 'admin@gmail.com';
        const password = 'W7TzX6ZJjUQ-X74';
        const role = 'Administrator'

        login(username, password, role);
        DepartmentCheck(role);
        DepartmentAdmistratorUndelete();
        Logout();
    });

    it('Department Staff View', function () {

        const username = 'staff1@gmail.com';
        const password = 'Pass@1234';
        const role = 'Staff'

        login(username, password, role);
        DepartmentCheck(role);
        Logout();
    });

    it('Department Student View', function () {

        const username = 'student1@gmail.com';
        const password = 'Pass@1234';
        const role = 'Student'

        login(username, password, role);
        DepartmentCheck(role);
        Logout();
    });

    it('Department Public View', function () {

        const username = 'public1@gmail.com';
        const password = 'Pass@1234';
        const role = 'Public'

        login(username, password, role);
        DepartmentCheck(role);
        Logout();
    });

    function login(username, password, role) {
        cy.visit(url);
        if (firstrun)
        {
            cy.pause();
            firstrun = false;
        }
        
        //cy.screenshot(role + ' 1 HomePage before login');

        cy.get('a')
            .contains('Login')
            .click();

        cy.url().should('include', '/Identity/Account/Login');
        //cy.screenshot(role + ' 2 Login Page');

        cy.get('input[id="Input_Email"]')
            .type(username)

        cy.get('input[id="Input_Password"]')
            .type(password)

        //cy.screenshot(role + ' 3 Logging in');

        cy.get('button')
            .contains('Log in')
            .click();

        cy.get('h1').then(($h1) => {
            if ($h1.text().includes('Two-factor authenticatio')) {
                cy.get('h1').should('contain', 'Two-factor authentication');
                //cy.screenshot(role + ' 4 2FA Logging in');

                cy.get('a[id="recovery-code-login"]')
                    .click();

                cy.get('h1')
                    .should('contain', 'Recovery code verification');

                //acb09f60 cf23f01a
                cy.get('input[id="Input_RecoveryCode"]')
                    .type();

                //cy.screenshot(role + ' 5 2FA RecoveryCode Logging in');

                cy.get('button')                    
                    .contains('Log in')
                    .click();
            }
        });

        cy.get('a[title="Manage"]').should('contain', role);
        cy.wait(2000);

        //cy.screenshot(role + ' 6 HomePage after login');
    }

    function Logout()
    {
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

    function DepartmentCheck(role)
    {
        cy.screenshot(role + ' 1 Home Page');
        cy.get('ul[class="navbar-nav flex-grow-1"]').within(() => {
            cy.get('a[href="/Departments"]')
                .should('contain', 'Departments')
                .click();
        });
            
        cy.wait(1000);

        cy.screenshot(role + ' 2 Department Page');

        if (role == 'Administrator') {
            DepartmentAdmistrator();
            DepartmentAdmistratorUndelete();
        }
        else if (role == 'Staff') {
            DepartmentStaff();
        }
        else
        {
            DepartmentOther();
        }
            
    }

    function DepartmentAdmistrator()
    {
        cy.get('a[href="/Departments/Create"]')
            .should('exist');
        cy.get('a[title="Details"]')
            .should('exist');
        cy.get('a[title="Edit"]')
            .should('exist');
        cy.get('a[title="Delete"]')
            .should('exist');
        cy.get('input[id="customSwitch1"]')
            .should('exist')
            .click({ force: true });

        cy.wait(1000);
        cy.screenshot('Admistrator 3 Department Delete Page');
    }

    function DepartmentAdmistratorUndelete()
    {
        cy.get('a[href="/Departments/Create"]')
            .should('exist');
        cy.get('a[title="Details"]')
            .should('exist');
        cy.get('a[title="Edit"]')
            .should('exist');
        cy.get('a[title="Delete"]')
            .should('exist');
        cy.get('input[id="customSwitch1"]')
            .should('exist');
        cy.get('th')
            .contains('IsDeleted')
            .should('exist');
    }

    function DepartmentStaff() {
        cy.get('a[href="/Departments/Create"]')
            .should('exist');
        cy.get('a[title="Details"]')
            .should('exist');
        cy.get('a[title="Edit"]')
            .should('exist');
        cy.get('a[title="Delete"]')
            .should('exist');
        cy.get('input[id="customSwitch1"]')
            .should('not.exist');
    }

    function DepartmentOther() {
        cy.get('a[href="/Departments/Create"]')
            .should('not.exist');
        cy.get('a[title="Details"]')
            .should('not.exist');
        cy.get('a[title="Edit"]')
            .should('not.exist');
        cy.get('a[title="Delete"]')
            .should('not.exist');
        cy.get('input[id="customSwitch1"]')
            .should('not.exist');
    }
});