# Contributing Guidelines

So nice that you would like to help in this project!

Contributions to this project are licensed under a CC0 license. This project is
strictly educational, experimental and meant to be very basic.

## Submitting a Pull Request

1. [Fork](https://github.com/csplb/CatApi/fork) and clone the repository
2. Configure and install the dependencies: `dotnet restore`
3. Create a new branch: `git checkout -b my-branch-name`
4. Make your changes, along with tests if possible
5. Format your code! If your editor supports `.editorconfig` (Visual Studio
   does) that's fine. If not - please use `dotnet-format`!
6. Test if everything is working by using test suite `dotnet test
   test/CatApi.Test.csproj`
7. Push to your fork and [submit a pull
   request](https://github.com/csplb/CatApi/compare)
8. Pat your self on the back and wait for your pull request to be reviewed and
   merged.

Here are a few things you can do that will increase the likelihood of your pull
request being accepted:

- Follow the coding style used in this project.
- Follow REST api rules.
- Keep your change as focused as possible. If there are multiple changes you
  would like to make that are not dependent upon each other, consider submitting
  them as separate pull requests.
- Write [good commit
  messages](http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html).

Small tips to follow project structure:

1. When modifying `ConfigureServices()` in `Startup.cs`, 
   create an extension method in `StartupServices.cs` if code added is big enough.
2. New services should be injected in `InjectServices()` method located in `StartupServices.cs`.
3. Remember to write documentation for endpoints that you add!

## Resources

- [How to Contribute to Open
  Source](https://opensource.guide/how-to-contribute/)
- [Using Pull Requests](https://help.github.com/articles/about-pull-requests/)
- [GitHub Help](https://help.github.com)
