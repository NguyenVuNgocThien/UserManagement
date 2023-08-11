import { UserManagementTemplatePage } from './app.po';

describe('UserManagement App', function() {
  let page: UserManagementTemplatePage;

  beforeEach(() => {
    page = new UserManagementTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
